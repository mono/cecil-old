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

		void VisitAssemblyDefinition (IAssemblyDefinition asm);
		void VisitAssemblyNameDefinition (IAssemblyNameDefinition name);
		void VisitAssemblyNameReferenceCollection (IAssemblyNameReferenceCollection names);
		void VisitAssemblyNameReference (IAssemblyNameReference name);
		void VisitResourceCollection (IResourceCollection resources);
		void VisitEmbeddedResource (IEmbeddedResource res);
		void VisitLinkedResource (ILinkedResource res);
		void VisitAssemblyLinkedResource (IAssemblyLinkedResource res);
		void VisitModuleDefinition (IModuleDefinition module);
		void VisitModuleDefinitionCollection (IModuleDefinitionCollection modules);
		void VisitModuleReference (IModuleReference module);
		void VisitModuleReferenceCollection (IModuleReferenceCollection modules);

		void TerminateAssemblyDefinition (IAssemblyDefinition asm);
	}
}

