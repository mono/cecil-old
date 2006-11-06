//
// FixReflectionAfterMerge.cs
//
// Author:
//	 Alex Prudkiy (prudkiy@gmail.com)
//
// (C) 2006 Alex Prudkiy
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

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;

namespace Mono.Merge {

	public class FixReflectionAfterMerge : BaseMergeReflectionVisitor, ICodeVisitor {

		public FixReflectionAfterMerge (MergeContext context, AssemblyDefinition target, AssemblyDefinition source)
			: base (context, target, source)
		{
			module = target.MainModule;
		}

		ModuleDefinition module;

		public void Process ()
		{
			VisitTypeReferenceCollection (module.TypeReferences);
			VisitTypeDefinitionCollection (module.Types);
			VisitMemberReferenceCollection (module.MemberReferences);

			TerminateModuleDefinition (module);
		}

		public override void VisitTypeDefinitionCollection (TypeDefinitionCollection types)
		{
			VisitCollection (types);
		}

		public override void VisitTypeDefinition (TypeDefinition type)
		{
			//delete unneeded TypeRefs, MemberRefs
			if (module.TypeReferences.Contains (type)) {
				TypeReference tr = module.TypeReferences [type.FullName];
				int i = 0;
				while (i < module.MemberReferences.Count) {
					MemberReference mr = module.MemberReferences [i];
					if (mr.DeclaringType != null && mr.DeclaringType.FullName == tr.FullName)
						module.MemberReferences.RemoveAt (i);
					else
						i++;
				}
				module.TypeReferences.Remove (tr);
			}

			if (type.BaseType != null)
				type.BaseType = GetTypeReference (type.BaseType);
		}

		public override void VisitMemberReferenceCollection (MemberReferenceCollection members)
		{
			foreach (MemberReference mr in members)
				VisitMemberReference (mr);

			//drop duplicates
			for (int i = 0; i < members.Count; i++) {
				MemberReference mr = members [i];
				for (int j = i + 1; j < members.Count; j++) {
					if (members [j].ToString () == mr.ToString ())
						members.RemoveAt (j);
				}
			}
		}

		bool MemberReferencesContains (MemberReferenceCollection members, MemberReference member)
		{
			MemberReference mr = GetMemberReference (members, member);
			return mr != null;
		}

		public override void VisitMemberReference (MemberReference member)
		{
			if (member is MethodReference) {
				MethodReference mr = member as MethodReference;
				if (mr.DeclaringType != null)
					mr.DeclaringType = GetTypeReference (mr.DeclaringType);
				mr.GenericParameters.Accept (this);
				mr.Parameters.Accept (this);
				mr.ReturnType.ReturnType = GetTypeReference (mr.ReturnType.ReturnType);
			} else if (member is FieldReference) {
				FieldReference fr = member as FieldReference;
				if (fr.DeclaringType != null)
					fr.DeclaringType = GetTypeReference (fr.DeclaringType);
				fr.FieldType = GetTypeReference (fr.FieldType);
			} else
				throw new ArgumentException ("Can't found member:" + member.DeclaringType.FullName + "::" + member.Name);
		}

		public override void VisitInterfaceCollection (InterfaceCollection interfaces)
		{
			for (int i = 0; i < interfaces.Count; i++)
				interfaces [i] = GetTypeReference (interfaces [i]);

			foreach (TypeReference interf in interfaces)
				VisitInterface (interf);
		}

		public override void VisitInterface (TypeReference interf)
		{
			if (interf.DeclaringType != null)
				interf.DeclaringType = GetTypeReference (interf.DeclaringType);
		}

		public override void VisitOverrideCollection (OverrideCollection meth)
		{
			VisitCollection (meth);
			for (int i = 0; i < meth.Count; i++)
				meth [i] = GetMethodReference (meth [i]);
		}

		public override void VisitNestedTypeCollection (NestedTypeCollection nestedTypes)
		{
			VisitCollection (nestedTypes);
		}

		public override void VisitParameterDefinitionCollection (ParameterDefinitionCollection parameters)
		{
			VisitCollection (parameters);
		}

		public override void VisitParameterDefinition (ParameterDefinition parameter)
		{
			parameter.ParameterType = GetTypeReference (parameter.ParameterType);
		}

		public override void VisitMethodDefinitionCollection (MethodDefinitionCollection methods)
		{
			VisitCollection (methods);
		}

		ModuleReference GetModuleReference (ModuleReferenceCollection members, ModuleReference module)
		{
			string name = module.Name;
			name = name.ToLower ();
			if (!BaseAssemblyResolver.OnMono ()) {
				if (!name.EndsWith (".dll"))
					name += ".dll";
			}

			foreach (ModuleReference mr in members) {
				if (mr.Name == name)
					return mr;
			}
			return null;
		}

		public override void VisitMethodDefinition (MethodDefinition method)
		{
			if (method.Body != null)
				method.Body.Accept (this);

			if (method.PInvokeInfo != null)
				method.PInvokeInfo =
					new PInvokeInfo (
						method, method.PInvokeInfo.Attributes, method.PInvokeInfo.EntryPoint,
						GetModuleReference (module.ModuleReferences, method.PInvokeInfo.Module));

			method.ReturnType.ReturnType = GetTypeReference (method.ReturnType.ReturnType);
		}

		public override void VisitConstructorCollection (ConstructorCollection ctors)
		{
			VisitCollection (ctors);
		}

		public override void VisitConstructor (MethodDefinition ctor)
		{
			if (ctor.Body != null)
				ctor.Body.Accept (this);
		}

		public override void VisitEventDefinitionCollection (EventDefinitionCollection events)
		{
			VisitCollection (events);
		}

		public override void VisitEventDefinition (EventDefinition evt)
		{
			evt.DeclaringType = GetTypeReference (evt.DeclaringType);

			evt.EventType = GetTypeReference (evt.EventType);

			if (evt.AddMethod != null)
				evt.AddMethod = (MethodDefinition) GetMethodReference (evt.AddMethod);

			if (evt.InvokeMethod != null)
				evt.InvokeMethod = (MethodDefinition) GetMethodReference (evt.InvokeMethod);

			if (evt.RemoveMethod != null)
				evt.RemoveMethod = (MethodDefinition) GetMethodReference (evt.RemoveMethod);
		}

		public override void VisitFieldDefinitionCollection (FieldDefinitionCollection fields)
		{
			VisitCollection (fields);
		}

		public override void VisitFieldDefinition (FieldDefinition field)
		{
			field.DeclaringType = GetTypeReference (field.DeclaringType);
			field.FieldType = GetTypeReference (field.FieldType);
		}

		public override void VisitPropertyDefinitionCollection (PropertyDefinitionCollection properties)
		{
			VisitCollection (properties);
		}

		public override void VisitPropertyDefinition (PropertyDefinition property)
		{
			property.DeclaringType = GetTypeReference (property.DeclaringType);
			property.PropertyType = GetTypeReference (property.PropertyType);
			VisitCollection (property.Parameters);
			if (property.GetMethod != null)
				property.GetMethod = (MethodDefinition) GetMethodReference (property.GetMethod);

			if (property.SetMethod != null)
				property.SetMethod = (MethodDefinition) GetMethodReference (property.SetMethod);
		}

		public override void VisitCustomAttributeCollection (CustomAttributeCollection customAttrs)
		{
			VisitCollection (customAttrs);
		}

		public override void VisitCustomAttribute (CustomAttribute customAttr)
		{
			customAttr.Constructor = GetMethodReference (customAttr.Constructor);

			Hashtable dict = new Hashtable (customAttr.Fields);
			foreach (DictionaryEntry entry in dict)
				customAttr.SetFieldType (
					(string) entry.Key, GetTypeReference (customAttr.GetFieldType ((string) entry.Key)));

			dict = new Hashtable (customAttr.Properties);
			foreach (DictionaryEntry entry in dict)
				customAttr.SetPropertyType (
					(string) entry.Key, GetTypeReference (customAttr.GetPropertyType ((string) entry.Key)));
		}

		public override void VisitGenericParameterCollection (GenericParameterCollection genparams)
		{
			VisitCollection (genparams);
		}

		public override void VisitMarshalSpec (MarshalSpec marshalSpec)
		{
			if (marshalSpec.Container is FieldDefinition) {
				FieldReference fr = (FieldReference) marshalSpec.Container;
				TypeReference tr = GetTypeReference (fr.DeclaringType);
				if (tr is TypeDefinition)
					marshalSpec.Container = (tr as TypeDefinition).Fields.GetField (fr.Name);
				else
					throw new ArgumentException ("Can't found field:" + fr.DeclaringType.FullName + "::" + fr.Name);
			}
		}

		public void VisitMethodBody (MethodBody body)
		{
		}

		public void VisitInstructionCollection (InstructionCollection instructions)
		{
			foreach (Instruction instr in instructions)
				instr.Accept (this);
		}

		public MethodReference GetMethodReferenceForType (TypeReference type, MethodReference method)
		{
			MethodReference res;
			if ((type is TypeDefinition) && (method is MethodDefinition)) {
				MethodDefinition md = method as MethodDefinition;
				TypeDefinition td = type as TypeDefinition;
				if (md.IsConstructor)
					res = td.Constructors.GetConstructor (md.IsStatic, md.Parameters);
				else
					res = td.Methods.GetMethod (md.Name, md.Parameters);
			} else if (type is TypeDefinition) {
				TypeDefinition td = type as TypeDefinition;
				if (method.Name == MethodDefinition.Ctor || method.Name == MethodDefinition.Cctor)
					res = td.Constructors.GetConstructor (!method.HasThis, method.Parameters);
				else
					res = td.Methods.GetMethod (method.Name, method.Parameters);
			} else {
				res = (MethodReference) GetMemberReference (Target.MainModule.MemberReferences, method);
			}

			if (res == null)
				throw new ArgumentException ("Can't found method:" + method.DeclaringType.FullName + "::" + method.Name);

			return res;
		}

		SortedDictionary<string, MethodReference> methodsrefs_cache = new SortedDictionary<string, MethodReference> ();

		MethodReference GetMethodReference (MethodReference source)
		{
			string name = source.ToString ();
			MethodReference result;
			if (!methodsrefs_cache.TryGetValue (name, out result)) {
				TypeReference tr = GetTypeReference (source.DeclaringType, false);
				if (source is GenericInstanceMethod) {
					GenericInstanceMethod mr = source as GenericInstanceMethod;
					mr.ElementMethod = GetMethodReference (mr.ElementMethod);
					VisitParameterDefinitionCollection (mr.Parameters);
					result = mr;
				} else
					result = GetMethodReferenceForType (tr, source);

				methodsrefs_cache.Add (name, result);
			}
			return result;
		}

		public void VisitInstruction (Instruction instr)
		{
			Instruction instruction = instr;
			if (instruction.Operand is FieldReference) {
				FieldReference fr = (FieldReference) instruction.Operand;
				TypeReference tr = GetTypeReference (fr.DeclaringType);
				if (tr is TypeDefinition)
					instruction.Operand = (tr as TypeDefinition).Fields.GetField (fr.Name);
				else
					instruction.Operand = GetMemberReference (Target.MainModule.MemberReferences, fr);

				fr = (FieldReference) instruction.Operand;
				fr.FieldType = GetTypeReference (fr.FieldType);
			} else if (instruction.Operand is MethodReference) {
				MethodReference mr = (MethodReference) instruction.Operand;
				instruction.Operand = GetMethodReference (mr);
			} else if (instruction.Operand is TypeReference) {
				TypeReference tr = (TypeReference) instruction.Operand;
				instruction.Operand = GetTypeReference (tr);
			} else if (instruction.Operand is EventReference) {
				EventReference et = (EventReference) instruction.Operand;
				instruction.Operand = GetEventReference (et);
			}
		}

		public EventReference GetEventReference (TypeReference type, EventReference eventa)
		{
			EventReference res = null;
			if (type is TypeDefinition) {
				EventDefinition md = eventa as EventDefinition;
				TypeDefinition td = type as TypeDefinition;
				res = td.Events.GetEvent (eventa.Name);
			}

			if (res == null)
				throw new ArgumentException ("Can't found event:" + eventa.DeclaringType.FullName + "::" + eventa.Name);

			return res;
		}

		EventReference GetEventReference (EventReference source)
		{
			EventReference mr = source;
			TypeReference tr = GetTypeReference (mr.DeclaringType, false);
			EventReference result = GetEventReference (tr, mr);
			return result;
		}

		public void VisitExceptionHandlerCollection (ExceptionHandlerCollection seh)
		{
			foreach (ExceptionHandler eh in seh)
				eh.Accept (this);
		}

		public void VisitExceptionHandler (ExceptionHandler eh)
		{
			if (eh.Type == ExceptionHandlerType.Catch)
				eh.CatchType = GetTypeReference (eh.CatchType);
		}

		public void VisitVariableDefinitionCollection (VariableDefinitionCollection variables)
		{
			foreach (VariableDefinition vd in variables)
				vd.Accept (this);
		}

		public void VisitVariableDefinition (VariableDefinition var)
		{
			TypeReference tr = var.VariableType;
			var.VariableType = GetTypeReference (tr);
		}

		public void VisitScopeCollection (ScopeCollection scopes)
		{
			VisitCollection (scopes);
		}

		public void VisitScope (Scope s)
		{
		}

		public void TerminateMethodBody (MethodBody body)
		{
		}
	}
}
