//
// MarkStep.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
// (C) 2007 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mono.Linker.Steps {

	public class MarkStep : IStep {

		LinkContext _context;
		Queue _queue;

		public MarkStep ()
		{
			_queue = new Queue ();
		}

		public void Process (LinkContext context)
		{
			_context = context;

			Initialize ();
			Process ();
		}

		void Initialize ()
		{
			foreach (AssemblyDefinition assembly in _context.GetAssemblies ())
				InitializeAssembly (assembly);
		}

		void InitializeAssembly (AssemblyDefinition assembly)
		{
			MarkAssembly (assembly);
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				if (!Annotations.IsMarked (type))
					continue;

				InitializeType (type);
			}
		}

		void InitializeType (TypeDefinition type)
		{
			MarkType (type);

			InitializeFields (type);

			InitializeMethods (type.Methods);
			InitializeMethods (type.Constructors);
		}

		void InitializeFields (TypeDefinition type)
		{
			foreach (FieldDefinition field in type.Fields)
				if (Annotations.IsMarked (field))
					MarkField (field);
		}

		void InitializeMethods (ICollection methods)
		{
			foreach (MethodDefinition method in methods)
				if (Annotations.IsMarked (method))
					_queue.Enqueue (method);
		}

		void Process ()
		{
			if (QueueIsEmpty ())
				throw new InvalidOperationException ("No entry methods");

			while (!QueueIsEmpty ()) {
				MethodDefinition method = (MethodDefinition) _queue.Dequeue ();
				ProcessMethod (method);
			}
		}

		bool QueueIsEmpty ()
		{
			return _queue.Count == 0;
		}

		void MarkMethodBody (MethodBody body)
		{
			foreach (VariableDefinition var in body.Variables)
				MarkType (var.VariableType);

			foreach (ExceptionHandler eh in body.ExceptionHandlers)
				if (eh.Type == ExceptionHandlerType.Catch)
					MarkType (eh.CatchType);

			foreach (Instruction instruction in body.Instructions)
				MarkInstruction (instruction);
		}

		void MarkMarshalSpec (IHasMarshalSpec spec)
		{
			CustomMarshalerSpec marshaler = spec.MarshalSpec as CustomMarshalerSpec;
			if (marshaler == null)
				return;

			TypeDefinition type = _context.GetType (marshaler.ManagedType);
			MarkType (type);
		}

		void MarkCustomAttributes (ICustomAttributeProvider provider)
		{
			foreach (CustomAttribute ca in provider.CustomAttributes)
				MarkCustomAttribute (ca);
		}

		void MarkCustomAttribute (CustomAttribute ca)
		{
			MarkMethod (ca.Constructor);

			if (!ca.Resolved) {
				ca = ca.Clone ();
				ca.Resolve ();
			}

			if (!ca.Resolved)
				return;

			TypeDefinition type = _context.Resolver.Resolve (ca.Constructor.DeclaringType);
			MarkCustomAttributeParameters (ca);
			MarkCustomAttributeProperties (ca, type);
			MarkCustomAttributeFields (ca, type);
		}

		void MarkCustomAttributeProperties (CustomAttribute ca, TypeDefinition attribute)
		{
			foreach (DictionaryEntry de in ca.Properties) {
				string propertyname = (string) de.Key;

				PropertyDefinition [] properties = attribute.Properties.GetProperties (propertyname);

				if (properties != null && properties.Length != 0 && properties [0].SetMethod != null)
					MarkMethod (properties [0].SetMethod);

				TypeReference propType = ca.GetPropertyType (propertyname);
				MarkIfType (ca, propType, de.Value);
			}
		}

		void MarkCustomAttributeFields (CustomAttribute ca, TypeDefinition attribute)
		{
			foreach (DictionaryEntry de in ca.Fields) {
				string fieldname = (string) de.Key;

				FieldDefinition field = attribute.Fields.GetField (fieldname);
				if (field != null)
					MarkField (field);

				TypeReference fieldType = ca.GetFieldType (fieldname);
				MarkIfType (ca, fieldType, de.Value);
			}
		}

		void MarkCustomAttributeParameters (CustomAttribute ca)
		{
			for (int i = 0; i < ca.Constructor.Parameters.Count; i++) {
				ParameterDefinition param = ca.Constructor.Parameters [i];
				MarkIfType (ca, param.ParameterType, ca.ConstructorParameters [i]);
			}
		}

		void MarkIfType (CustomAttribute ca, TypeReference slotType, object value)
		{
			if (slotType.FullName != Constants.Type)
				return;

			TypeDefinition type = _context.GetType ((string) value);

			MarkType (type);
		}

		static bool CheckProcessed (IAnnotationProvider provider)
		{
			if (Annotations.IsProcessed (provider))
				return true;

			Annotations.Processed (provider);
			return false;
		}

		void MarkAssembly (AssemblyDefinition assembly)
		{
			if (CheckProcessed (assembly))
				return;

			MarkCustomAttributes (assembly);

			foreach (ModuleDefinition module in assembly.Modules)
				MarkCustomAttributes (module);
		}

		void MarkField (FieldReference field)
		{
			if (IgnoreScope (field.DeclaringType.Scope))
				return;

			FieldDefinition fd = ResolveFieldDefinition (field);

			if (CheckProcessed (fd))
				return;

			MarkType (fd.DeclaringType);
			MarkType (fd.FieldType);
			MarkCustomAttributes (fd);
			MarkMarshalSpec (fd);

			Annotations.Mark (fd);
		}

		bool IgnoreScope (IMetadataScope scope)
		{
			AssemblyDefinition assembly = ResolveAssembly (scope);
			return Annotations.GetAction (assembly) != AssemblyAction.Link;
		}

		FieldDefinition ResolveFieldDefinition (FieldReference field)
		{
			FieldDefinition fd = field as FieldDefinition;
			if (fd == null)
				fd = _context.Resolver.Resolve (field);

			return fd;
		}

		void MarkType (TypeReference type)
		{
			if (type == null)
				return;

			type = GetOriginalType (type);

			if (type is GenericParameter)
				return;

			if (IgnoreScope (type.Scope))
				return;

			TypeDefinition td = ResolveTypeDefinition (type);

			if (CheckProcessed (td))
				return;

			MarkType (td.BaseType);
			MarkType (td.DeclaringType);
			MarkCustomAttributes(td);

			if (IsMulticastDelegate (td))
				MarkMethodCollection (td.Constructors);

			if (IsSerializable(td)) {
				MarkMethodsIf (td.Constructors, new MethodPredicate (IsDefaultConstructor));
				MarkMethodsIf (td.Constructors, new MethodPredicate (IsSpecialSerializationConstructor));
			}

			MarkGenericParameters (td);

			if (td.IsValueType)
				MarkFields (td);

			foreach (TypeReference iface in td.Interfaces)
				MarkType (iface);

			MarkMethodsIf (td.Constructors, new MethodPredicate (IsStaticConstructor));

			MarkMethodsIf (td.Methods, new MethodPredicate (IsVirtual));

			Annotations.Mark (td);

			ApplyPreserveInfo (td);
		}

		static bool IsSpecialSerializationConstructor (MethodDefinition method)
		{
			if (!IsConstructor (method))
				return false;

			ParameterDefinitionCollection parameters = method.Parameters;
			if (parameters.Count != 2)
				return false;

			return parameters [0].ParameterType.Name == "SerializationInfo" &&
				parameters [1].ParameterType.Name == "StreamingContext";
		}

		delegate bool MethodPredicate (MethodDefinition method);

		void MarkMethodsIf (ICollection methods, MethodPredicate predicate)
		{
			foreach (MethodDefinition method in methods)
				if (predicate (method))
					MarkMethod (method);
		}

		static bool IsDefaultConstructor (MethodDefinition method)
		{
			return IsConstructor (method) && method.Parameters.Count == 0;
		}

		static bool IsConstructor (MethodDefinition method)
		{
			return method.Name == MethodDefinition.Ctor && method.IsSpecialName &&
				method.IsRuntimeSpecialName;
		}

		static bool IsVirtual (MethodDefinition method)
		{
			return method.IsVirtual;
		}

		static bool IsStaticConstructor (MethodDefinition method)
		{
			return method.Name == MethodDefinition.Cctor && method.IsSpecialName &&
				method.IsRuntimeSpecialName;
		}

		static bool IsSerializable (TypeDefinition td)
		{
			return (td.Attributes & TypeAttributes.Serializable) != 0;
		}

		static bool IsMulticastDelegate (TypeDefinition td)
		{
			return td.BaseType != null && td.BaseType.FullName == "System.MulticastDelegate";
		}

		TypeDefinition ResolveTypeDefinition (TypeReference type)
		{
			TypeDefinition td = type as TypeDefinition;
			if (td == null)
				td = _context.Resolver.Resolve (type);

			return td;
		}

		static TypeReference GetOriginalType (TypeReference type)
		{
			while (type is TypeSpecification)
				type = ((TypeSpecification) type).ElementType;

			return type;
		}

		void ApplyPreserveInfo (TypeDefinition type)
		{
			if (!Annotations.IsPreserved (type))
				return;

			switch (Annotations.GetPreserve (type)) {
			case TypePreserve.All:
				MarkFields (type);
				MarkMethods (type);
				break;
			case TypePreserve.Fields:
				MarkFields (type);
				break;
			case TypePreserve.Methods:
				MarkMethods (type);
				break;
			}
		}

		void MarkFields (TypeDefinition type)
		{
			foreach (FieldDefinition field in type.Fields)
				MarkField (field);
		}

		void MarkMethods (TypeDefinition type)
		{
			MarkMethodCollection (type.Methods);
			MarkMethodCollection (type.Constructors);
		}

		void MarkMethodCollection (IEnumerable methods)
		{
			foreach (MethodDefinition method in methods)
				MarkMethod (method);
		}

		void MarkMethod (MethodReference method)
		{
			method = GetOriginalMethod (method);

			if (method.DeclaringType is ArrayType)
				return;

			if (IgnoreScope (method.DeclaringType.Scope))
				return;

			MethodDefinition md = ResolveMethodDefinition (method);

			Annotations.SetAction (md, MethodAction.Parse);

			_queue.Enqueue (md);
		}

		AssemblyDefinition ResolveAssembly (IMetadataScope scope)
		{
			AssemblyDefinition assembly = _context.Resolve (scope);
			MarkAssembly (assembly);
			return assembly;
		}

		static MethodReference GetOriginalMethod (MethodReference method)
		{
			while (method is MethodSpecification)
				method = ((MethodSpecification) method).ElementMethod;

			return method;
		}

		MethodDefinition ResolveMethodDefinition (MethodReference method)
		{
			MethodDefinition md = method as MethodDefinition;
			if (md == null)
				md = _context.Resolver.Resolve (method);

			return md;
		}

		void ProcessMethod (MethodDefinition md)
		{
			if (CheckProcessed (md))
				return;

			MarkType (md.DeclaringType);
			MarkCustomAttributes (md);

			MarkGenericParameters (md);

			if (IsPropertyMethod (md))
				MarkProperty (GetProperty (md));
			else if (IsEventMethod (md))
				MarkEvent (GetEvent (md));

			foreach (ParameterDefinition pd in md.Parameters) {
				MarkType (pd.ParameterType);
				MarkCustomAttributes (pd);
				MarkMarshalSpec (pd);
			}

			foreach (MethodReference ov in md.Overrides)
				MarkMethod (ov);

			MarkType (md.ReturnType.ReturnType);
			MarkCustomAttributes (md.ReturnType);
			MarkMarshalSpec (md.ReturnType);

			if (ShouldParseMethodBody (md))
				MarkMethodBody (md.Body);

			Annotations.Mark (md);
		}

		void MarkGenericParameters (IGenericParameterProvider provider)
		{
			foreach (GenericParameter p in provider.GenericParameters)
				MarkCustomAttributes (p);
		}

		bool ShouldParseMethodBody (MethodDefinition method)
		{
			if (!method.HasBody)
				return false;

			AssemblyDefinition assembly = ResolveAssembly (method.DeclaringType.Scope);
			return (Annotations.GetAction (method) == MethodAction.ForceParse ||
				(Annotations.GetAction (assembly) == AssemblyAction.Link && Annotations.GetAction (method) == MethodAction.Parse));
		}

		static bool IsPropertyMethod (MethodDefinition md)
		{
			return (md.SemanticsAttributes & MethodSemanticsAttributes.Getter) != 0 ||
				(md.SemanticsAttributes & MethodSemanticsAttributes.Setter) != 0;
		}

		static bool IsEventMethod (MethodDefinition md)
		{
			return (md.SemanticsAttributes & MethodSemanticsAttributes.AddOn) != 0 ||
				(md.SemanticsAttributes & MethodSemanticsAttributes.Fire) != 0 ||
				(md.SemanticsAttributes & MethodSemanticsAttributes.RemoveOn) != 0;
		}

		static PropertyDefinition GetProperty (MethodDefinition md)
		{
			TypeDefinition declaringType = (TypeDefinition) md.DeclaringType;
			foreach (PropertyDefinition prop in declaringType.Properties)
				if (prop.GetMethod == md || prop.SetMethod == md)
					return prop;

			return null;
		}

		static EventDefinition GetEvent (MethodDefinition md)
		{
			TypeDefinition declaringType = (TypeDefinition) md.DeclaringType;
			foreach (EventDefinition evt in declaringType.Events)
				if (evt.AddMethod == md || evt.InvokeMethod == md || evt.RemoveMethod == md)
					return evt;

			return null;
		}

		void MarkProperty (PropertyDefinition prop)
		{
			MarkCustomAttributes (prop);
		}

		void MarkEvent (EventDefinition evt)
		{
			MarkCustomAttributes (evt);
			MarkMethodIfNotNull (evt.AddMethod);
			MarkMethodIfNotNull (evt.InvokeMethod);
			MarkMethodIfNotNull (evt.RemoveMethod);
		}

		void MarkMethodIfNotNull (MethodReference method)
		{
			if (method == null)
				return;

			MarkMethod (method);
		}

		void MarkInstruction (Instruction instruction)
		{
			switch (instruction.OpCode.OperandType) {
			case OperandType.InlineField:
				MarkField ((FieldReference) instruction.Operand);
				break;
			case OperandType.InlineMethod:
				MarkMethod ((MethodReference) instruction.Operand);
				break;
			case OperandType.InlineTok:
				object token = instruction.Operand;
				if (token is TypeReference)
					MarkType ((TypeReference) token);
				else if (token is MethodReference)
					MarkMethod ((MethodReference) token);
				else
					MarkField ((FieldReference) token);
				break;
			case OperandType.InlineType:
				MarkType ((TypeReference) instruction.Operand);
				break;
			default:
				break;
			}
		}
	}
}
