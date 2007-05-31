//
// CustomResolver.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
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

namespace Mono.Linker {

	public class CustomResolver : DefaultAssemblyResolver {

		public void CacheAssembly (AssemblyDefinition assembly)
		{
			RegisterAssembly (assembly);
		}

		public TypeDefinition Resolve (TypeReference type)
		{
			if (type is TypeDefinition)
				return (TypeDefinition) type;

			AssemblyNameReference reference = type.Scope as AssemblyNameReference;
			if (reference != null) {
				AssemblyDefinition assembly = Resolve (reference);
				return assembly.MainModule.Types [type.FullName];
			}

			throw new NotImplementedException ();
		}

		public FieldDefinition Resolve (FieldReference field)
		{
			TypeDefinition type = Resolve (GetDeclaringType (field.DeclaringType));
			return GetField (type.Fields, field);
		}

		static FieldDefinition GetField (ICollection collection, FieldReference reference)
		{
			foreach (FieldDefinition field in collection) {
				if (field.Name != reference.Name)
					continue;

				if (!AreSame (field.FieldType, reference.FieldType))
					continue;

				return field;
			}

			return null;
		}

		public MethodDefinition Resolve (MethodReference method)
		{
			TypeDefinition type = Resolve (GetDeclaringType (method.DeclaringType));
			if (method.Name == MethodDefinition.Cctor || method.Name == MethodDefinition.Ctor)
				return GetMethod (type.Constructors, method);
			else
				return GetMethod (type.Methods, method);
		}

		static TypeReference GetDeclaringType (TypeReference type)
		{
			TypeReference t = type;
			while (t is TypeSpecification)
				t = ((TypeSpecification) t).ElementType;
			return t;
		}

		static MethodDefinition GetMethod (ICollection collection, MethodReference reference)
		{
			foreach (MethodDefinition meth in collection) {
				if (meth.Name != reference.Name)
					continue;

				if (!AreSame (meth.ReturnType.ReturnType, reference.ReturnType.ReturnType))
					continue;

				if (!AreSame (meth.Parameters, reference.Parameters))
					continue;

				return meth;
			}

			return null;
		}

		static bool AreSame (ParameterDefinitionCollection a, ParameterDefinitionCollection b)
		{
			if (a.Count != b.Count)
				return false;

			if (a.Count == 0)
				return true;

			for (int i = 0; i < a.Count; i++)
				if (!AreSame (a [i].ParameterType, b [i].ParameterType))
					return false;

			return true;
		}

		static bool AreSame (TypeReference a, TypeReference b)
		{
			while (a is TypeSpecification || b is TypeSpecification) {
				if (a.GetType () != b.GetType ())
					return false;

				a = ((TypeSpecification) a).ElementType;
				b = ((TypeSpecification) b).ElementType;
			}

			if (a is GenericParameter || b is GenericParameter) {
				if (a.GetType() != b.GetType())
					return false;

				GenericParameter pa = (GenericParameter) a;
				GenericParameter pb = (GenericParameter) b;

				return pa.Position == pb.Position;
			}

			return a.FullName == b.FullName;
		}
	}
}
