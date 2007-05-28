//
// SweepStep.cs
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

using System.Collections;

using Mono.Cecil;

namespace Mono.Linker {

	public class SweepStep : IStep {

		public void Process (LinkContext context)
		{
			foreach (AssemblyDefinition assembly in context.GetAssemblies ())
				SweepAssembly (assembly, context);
		}

		static void SweepAssembly (AssemblyDefinition assembly, LinkContext context)
		{
			if (Annotations.GetAction (assembly) != AssemblyAction.Link)
				return;

			foreach (TypeDefinition type in Clone (assembly.MainModule.Types)) {
				if (Annotations.IsMarked (type)) {
					SweepType (type);
					continue;
				}

				assembly.MainModule.Types.Remove (type);
				SweepTypeReferences (assembly, type, context);
			}
		}

		static ICollection Clone (ICollection collection)
		{
			return new ArrayList (collection);
		}

		static void SweepTypeReferences (AssemblyDefinition assembly, TypeDefinition type, LinkContext context)
		{
			foreach (AssemblyDefinition asm in context.GetAssemblies ()) {
				ModuleDefinition module = asm.MainModule;
				if (!module.TypeReferences.Contains (type))
					continue;

				TypeReference typeRef = module.TypeReferences [type.FullName];
				if (AssemblyMatch (assembly, typeRef))
					module.TypeReferences.Remove (typeRef);
			}
		}

		static bool AssemblyMatch (AssemblyDefinition assembly, TypeReference type)
		{
			AssemblyNameReference reference = type.Scope as AssemblyNameReference;
			if (reference == null)
				return false;

			return assembly.Name.FullName == reference.FullName;
		}

		static void SweepType (TypeDefinition type)
		{
			SweepMethods (type);
			SweepFields (type);
		}

		static void SweepFields (TypeDefinition type)
		{
			foreach (FieldDefinition field in Clone (type.Fields))
				if (!Annotations.IsMarked (field))
					type.Fields.Remove (field);
		}

		static void SweepMethods (TypeDefinition type)
		{
			foreach (MethodDefinition meth in Clone (type.Methods))
				if (!Annotations.IsMarked (meth))
					type.Methods.Remove (meth);

			foreach (MethodDefinition ctor in Clone (type.Constructors))
				if (!Annotations.IsMarked (ctor))
					type.Constructors.Remove (ctor);
		}
	}
}
