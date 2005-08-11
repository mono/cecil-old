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

	public interface IAssemblyNameReference : IMetadataScope, IReflectionStructureVisitable {

		string Name { get; set; }
		string Culture { get; set; }
		string FullName { get; }
		Version Version { get; set; }
		AssemblyFlags Flags { get; set; }
		byte [] PublicKey { get; set; }
		byte [] PublicKeyToken { get; set; }
		AssemblyHashAlgorithm HashAlgorithm { get; set; }
		byte [] Hash { get; }
	}

	public interface IAssemblyNameDefinition : IAssemblyNameReference {}
}
