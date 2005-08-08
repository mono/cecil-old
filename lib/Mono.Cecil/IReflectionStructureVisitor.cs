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

namespace Mono.Cecil {

	public interface IReflectionStructureVisitor {

		void Visit (IAssemblyDefinition asm);
		void Visit (IAssemblyNameDefinition name);
		void Visit (IAssemblyNameReferenceCollection names);
		void Visit (IAssemblyNameReference name);
		void Visit (IResourceCollection resources);
		void Visit (IEmbeddedResource res);
		void Visit (ILinkedResource res);
		void Visit (IAssemblyLinkedResource res);
		void Visit (IModuleDefinition module);
		void Visit (IModuleDefinitionCollection modules);
		void Visit (IModuleReference module);
		void Visit (IModuleReferenceCollection modules);

		void Terminate (IAssemblyDefinition asm);
	}
}

