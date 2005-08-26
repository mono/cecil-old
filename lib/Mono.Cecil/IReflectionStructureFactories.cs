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

	using System;
	using SR = System.Reflection;

	public interface IReflectionStructureFactories {

		IAssemblyReferenceFactory AssemblyReferenceFactory { get; }
		IResourceFactory ResourceFactory { get; }
		IModuleFactory ModuleFactory { get; }
	}

	public interface IAssemblyReferenceFactory {

		IAssemblyNameReference CreateAssemblyNameReference (string name);
		IAssemblyNameReference CreateAssemblyNameReference (string name, string culture);
		IAssemblyNameReference CreateAssemblyNameReference (string name, string culture, Version ver);
		IAssemblyNameReference CreateAssemblyNameReference (string name, string culture,
			Version ver, byte [] publicKeyToken);

		IAssemblyNameReference CreateFromAssembly (SR.Assembly asm);
		IAssemblyNameReference CreateFromFullyQualifiedName (string fqName);

	}

	public interface IResourceFactory {

		IEmbeddedResource CreateEmbeddedResource (string name,
			ManifestResourceAttributes attributes, byte [] data);

		ILinkedResource CreateLinkedResource (string name,
			ManifestResourceAttributes attributes, string file);

		IAssemblyLinkedResource CreateAssemblyLinkedResource (string name,
			ManifestResourceAttributes attributes, IAssemblyNameReference asm);
		IAssemblyLinkedResource CreateAssemblyLinkedResource (string name,
			ManifestResourceAttributes attributes, SR.Assembly asm);
	}

	public interface IModuleFactory {

		IModuleDefinition CreateModule (string name);

		IModuleReference CreateModuleReference (string name);
	}
}
