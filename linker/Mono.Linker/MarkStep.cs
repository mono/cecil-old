//
// MarkStep.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.Linker {

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

			InitializeQueue ();
			Process ();
		}

		void InitializeQueue ()
		{
			foreach (AssemblyMarker ac in _context.GetAssemblies()) {
				foreach (TypeMarker type in ac.GetTypes ()) {
					MarkType (type.Type);

					foreach (FieldMarker field in type.GetFields ())
						MarkField (field.Field);

					foreach (MethodMarker meth in type.GetMethods ())
						_queue.Enqueue (meth);
				}
			}
		}

		void Process ()
		{
			if (_queue.Count == 0)
				throw new InvalidOperationException ("No entry methods");

			while (_queue.Count > 0) {
				MethodMarker method = (MethodMarker) _queue.Dequeue ();
				ProcessMethod (method);
			}
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

		void MarkCustomAttributes (ICustomAttributeProvider provider)
		{
			foreach (CustomAttribute ca in provider.CustomAttributes)
				MarkMethod (ca.Constructor);
		}

		void MarkAssembly (AssemblyMarker am)
		{
			if (am.Processed)
				return;

			am.Processed = true;

			MarkCustomAttributes (am.Assembly);

			foreach (ModuleDefinition module in am.Assembly.Modules) {
				MarkCustomAttributes (module);
				MarkModule (module);
			}
		}

		void MarkModule (ModuleDefinition module)
		{
			MarkCustomAttributes (module);
		}

		void MarkField (FieldReference field)
		{
			AssemblyMarker am = _context.Resolve (field.DeclaringType.Scope);
			MarkAssembly (am);
			if (am.Action != AssemblyAction.Link)
				return;

			FieldDefinition fd = field as FieldDefinition;
			if (fd == null)
				fd = am.Resolve (field);

			TypeMarker tm = am.Mark ((TypeDefinition) fd.DeclaringType);
			FieldMarker fm = tm.Mark (fd);
			if (fm.Processed)
				return;

			fm.Processed = true;

			MarkType (fd.DeclaringType);
			MarkType (fd.FieldType);
			MarkCustomAttributes (fd);

			AnnotateMarker (fd, fm);
		}

		void MarkType (TypeReference type)
		{
			if (type == null)
				return;

			while (type is TypeSpecification)
				type = ((TypeSpecification) type).ElementType;

			if (type is GenericParameter)
				return;

			AssemblyMarker am = _context.Resolve (type.Scope);
			MarkAssembly (am);
			if (am.Action != AssemblyAction.Link)
				return;

			TypeDefinition td = type as TypeDefinition;
			if (td == null)
				td = am.Resolve (type);

			TypeMarker tm = am.Mark (td);
			if (tm.Processed)
				return;

			tm.Processed = true;

			MarkType (td.BaseType);
			if (td.DeclaringType != null)
				MarkType (td.DeclaringType);
			MarkCustomAttributes(td);

			if (td.BaseType != null && td.BaseType.FullName == "System.MulticastDelegate")
				foreach (MethodDefinition ctor in td.Constructors)
					MarkMethod (ctor);

			foreach (GenericParameter p in td.GenericParameters)
				MarkCustomAttributes (p);

			if (td.IsValueType)
				foreach (FieldDefinition field in td.Fields)
					MarkField (field);

			foreach (TypeReference iface in td.Interfaces)
				MarkType (iface);

			foreach (MethodDefinition ctor in td.Constructors)
				if (ctor.Name == MethodDefinition.Cctor)
					MarkMethod (ctor);

			foreach (MethodDefinition meth in td.Methods)
				if (meth.IsVirtual)
					MarkMethod (meth);

			am.Mark (td);

			ApplyPreserveInfo (am, tm);

			AnnotateMarker (td, tm);
		}

		static void AnnotateMarker (IAnnotationProvider provider, Marker marker)
		{
			provider.Annotations [Marker.MarkerKey] = marker;
		}

		void ApplyPreserveInfo (AssemblyMarker am, TypeMarker tm)
		{
			if (!am.HasPreserveInfo (tm))
				return;

			switch (am.GetPreserveInfo (tm)) {
			case TypePreserve.All:
				MarkFields (tm);
				MarkMethods (tm);
				break;
			case TypePreserve.Fields:
				MarkFields (tm);
				break;
			case TypePreserve.Methods:
				MarkMethods (tm);
				break;
			}
		}

		void MarkFields (TypeMarker tm)
		{
			foreach (FieldDefinition field in tm.Type.Fields)
				MarkField (field);
		}

		void MarkMethods(TypeMarker tm)
		{
			MarkMethodCollection (tm.Type.Methods);
			MarkMethodCollection (tm.Type.Constructors);
		}

		void MarkMethodCollection (IEnumerable methods)
		{
			foreach (MethodDefinition method in methods)
				MarkMethod (method);
		}

		void MarkMethod (MethodReference method)
		{
			while (method is MethodSpecification)
				method = ((MethodSpecification) method).ElementMethod;

			if (method.DeclaringType is ArrayType)
				return;

			AssemblyMarker am = _context.Resolve (method.DeclaringType.Scope);
			MarkAssembly (am);
			if (am.Action != AssemblyAction.Link)
				return;

			MethodDefinition md = method as MethodDefinition;
			if (md == null)
				md = am.Resolve (method);

			TypeMarker tm = am.Mark ((TypeDefinition) md.DeclaringType);
			MethodMarker mm = tm.Mark (md);

			_queue.Enqueue (mm);
		}

		void ProcessMethod (MethodMarker mm)
		{
			MethodDefinition md = mm.Method;
			AssemblyMarker am = _context.Resolve (md.DeclaringType.Scope);

			if (mm.Processed)
				return;

			mm.Processed = true;

			MarkType (md.DeclaringType);
			MarkCustomAttributes (md);

			foreach (GenericParameter p in md.GenericParameters)
				MarkCustomAttributes (p);

			if (IsPropertyMethod (md))
				MarkProperty (GetProperty (md));
			else if (IsEventMethod (md))
				MarkEvent (GetEvent (md));

			foreach (ParameterDefinition pd in md.Parameters) {
				MarkType (pd.ParameterType);
				MarkCustomAttributes (pd);
			}

			foreach (MethodReference ov in md.Overrides)
				MarkMethod (ov);

			MarkType (md.ReturnType.ReturnType);
			MarkCustomAttributes (md.ReturnType);

			if (md.HasBody && ShouldParseMethodBody (am, mm))
				MarkMethodBody (md.Body);

			AnnotateMarker (md, mm);
		}

		static bool ShouldParseMethodBody (AssemblyMarker am, MethodMarker mm)
		{
			return (mm.Action == MethodAction.ForceParse ||
			        (am.Action == AssemblyAction.Link && mm.Action == MethodAction.Parse));
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
