/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using Mono.Cecil;
	using Mono.Cecil.Binary;

	internal sealed class StructureWriter : IReflectionStructureVisitor {

		private AssemblyDefinition m_asm;

		public StructureWriter (AssemblyDefinition asm)
		{
			m_asm = asm;
		}

		public void Visit (IAssemblyDefinition asm)
		{
			// TODO
		}

		public void Visit (IAssemblyNameDefinition name)
		{
			// TODO
		}

		public void Visit (IAssemblyNameReferenceCollection names)
		{
			// TODO
		}

		public void Visit (IAssemblyNameReference name)
		{
			// TODO
		}

		public void Visit (IResourceCollection resources)
		{
			// TODO
		}

		public void Visit (IEmbeddedResource res)
		{
			// TODO
		}

		public void Visit (ILinkedResource res)
		{
			// TODO
		}

		public void Visit (IAssemblyLinkedResource res)
		{
			// TODO
		}

		public void Visit (IModuleDefinition module)
		{
			ModuleDefinition mod = module as ModuleDefinition;
			if (mod.Image.CLIHeader.Metadata.VirtualAddress != RVA.Zero)
				mod.Image = Image.CreateImage ();
		}

		public void Visit (IModuleDefinitionCollection modules)
		{
			// TODO
		}

		public void Visit (IModuleReference module)
		{
			// TODO
		}

		public void Visit (IModuleReferenceCollection modules)
		{
			// TODO
		}
	}
}
