//
// StructureMerger.cs
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

using Mono.Cecil;

namespace Mono.Merge {

	public class StructureMerger : BaseStructureVisitor {

		MergeContext context;
		AssemblyDefinition target;
		AssemblyDefinition source;

		public StructureMerger (MergeContext context, AssemblyDefinition target, AssemblyDefinition source)
		{
			this.context = context;
			this.target = target;
			this.source = source;
		}

		bool IsAssemblyReferencesCollectionContains (AssemblyNameReferenceCollection col, AssemblyNameReference refer)
		{
			//skip merging
			foreach (AssemblyDefinition adef in context.MergedAssemblies) {
				if (adef.Name.Name == refer.Name)
					return true;
			}

			foreach (AssemblyNameReference aref in col) {
				if (aref.Name == refer.Name)
					return true;
			}
			return false;
		}

		void DeleteAssemblyReference (AssemblyNameReferenceCollection col, string refer)
		{
			foreach (AssemblyNameReference aref in col) {
				if (aref.Name == refer) {
					col.Remove (aref);
					return;
				}
			}
		}

		public override void VisitAssemblyNameReferenceCollection (AssemblyNameReferenceCollection names)
		{
			VisitCollection (names);
			DeleteAssemblyReference (target.MainModule.AssemblyReferences, source.Name.Name);
		}

		public override void VisitAssemblyNameReference (AssemblyNameReference name)
		{
			if (!IsAssemblyReferencesCollectionContains (target.MainModule.AssemblyReferences, name))
				target.MainModule.AssemblyReferences.Add (name);
		}

		public override void VisitResourceCollection (ResourceCollection resources)
		{
			VisitCollection (resources);
		}

		public override void VisitEmbeddedResource (EmbeddedResource res)
		{
			target.MainModule.Resources.Add (res);
		}

		public override void VisitLinkedResource (LinkedResource res)
		{
			target.MainModule.Resources.Add (res);
		}

		public override void VisitAssemblyLinkedResource (AssemblyLinkedResource res)
		{
			target.MainModule.Resources.Add (res);
		}

		public override void VisitModuleDefinitionCollection (ModuleDefinitionCollection modules)
		{
			VisitCollection (modules);
		}

		bool ModuleReferencesContains (ModuleReferenceCollection members, string name)
		{
			ModuleReference mr = GetModuleReference (members, name);
			return mr != null;
		}

		ModuleReference GetModuleReference (ModuleReferenceCollection members, string name)
		{
			foreach (ModuleReference mr in members) {
				if (mr.Name == name)
					return mr;
			}
			return null;
		}

		public override void VisitModuleReference (ModuleReference module)
		{
			string name = module.Name;
			name = name.ToLower ();
			if (!BaseAssemblyResolver.OnMono ()) {
				if (!name.EndsWith (".dll"))
					name += ".dll";
			}

			if (!ModuleReferencesContains (target.MainModule.ModuleReferences, name)) {
				module.Name = name;
				target.MainModule.ModuleReferences.Add (module);
			}
		}

		public override void VisitModuleReferenceCollection (ModuleReferenceCollection modules)
		{
			VisitCollection (modules);
		}

		public override void TerminateAssemblyDefinition (AssemblyDefinition asm)
		{
			foreach (ModuleDefinition mod in asm.Modules) {
				ReflectionMerger rm = new ReflectionMerger (context, target, source);
				mod.FullLoad ();
				rm.VisitModuleDefinition (mod);
				rm.VisitTypeDefinitionCollection (mod.Types); //this also loads bodies
				rm.VisitTypeReferenceCollection (mod.TypeReferences);
				rm.VisitMemberReferenceCollection (mod.MemberReferences);

				rm.TerminateModuleDefinition (mod);
			}
			context.MergedAssemblies.Add (asm);
		}
	}
}
