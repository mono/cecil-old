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

	public abstract class BaseStructureVisitor : IReflectionStructureVisitor {

		public virtual void VisitAssemblyDefinition (IAssemblyDefinition asm)
		{
		}

		public virtual void VisitAssemblyNameDefinition (IAssemblyNameDefinition name)
		{
		}

		public virtual void VisitAssemblyNameReferenceCollection (IAssemblyNameReferenceCollection names)
		{
		}

		public virtual void VisitAssemblyNameReference (IAssemblyNameReference name)
		{
		}

		public virtual void VisitResourceCollection (IResourceCollection resources)
		{
		}

		public virtual void VisitEmbeddedResource (IEmbeddedResource res)
		{
		}

		public virtual void VisitLinkedResource (ILinkedResource res)
		{
		}

		public virtual void VisitAssemblyLinkedResource (IAssemblyLinkedResource res)
		{
		}

		public virtual void VisitModuleDefinition (IModuleDefinition module)
		{
		}

		public virtual void VisitModuleDefinitionCollection (IModuleDefinitionCollection modules)
		{
		}

		public virtual void VisitModuleReference (IModuleReference module)
		{
		}

		public virtual void VisitModuleReferenceCollection (IModuleReferenceCollection modules)
		{
		}

		public virtual void TerminateAssemblyDefinition (IAssemblyDefinition asm)
		{
		}
	}
}
