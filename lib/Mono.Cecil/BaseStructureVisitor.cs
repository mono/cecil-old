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

		public virtual void Visit (IAssemblyDefinition asm)
		{
		}

		public virtual void Visit (IAssemblyNameDefinition name)
		{
		}

		public virtual void Visit (IAssemblyNameReferenceCollection names)
		{
		}

		public virtual void Visit (IAssemblyNameReference name)
		{
		}

		public virtual void Visit (IResourceCollection resources)
		{
		}

		public virtual void Visit (IEmbeddedResource res)
		{
		}

		public virtual void Visit (ILinkedResource res)
		{
		}

		public virtual void Visit (IAssemblyLinkedResource res)
		{
		}

		public virtual void Visit (IModuleDefinition module)
		{
		}

		public virtual void Visit (IModuleDefinitionCollection modules)
		{
		}

		public virtual void Visit (IModuleReference module)
		{
		}

		public virtual void Visit (IModuleReferenceCollection modules)
		{
		}
	}
}
