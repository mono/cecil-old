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

	using Mono.Cecil.Metadata;

	public interface IModuleDefinition : ICustomAttributeProvider, IMetadataScope,
		IReflectionStructureVisitable, IReflectionVisitable {

		IAssemblyDefinition Assembly { get; }

		Guid Mvid { get; set; }
		bool Main { get; }

		IAssemblyNameReferenceCollection AssemblyReferences { get; }
		IModuleReferenceCollection ModuleReferences { get; }
		IResourceCollection Resources { get; }
		ITypeDefinitionCollection Types { get; }
		IExternTypeCollection ExternTypes { get; }
		ITypeReferenceCollection TypeReferences { get; }
		IMemberReferenceCollection MemberReferences { get; }

		IReflectionFactories Factories { get; }

		IMetadataTokenProvider LookupByToken (MetadataToken token);
		IMetadataTokenProvider LookupByToken (TokenType table, int rid);
	}
}
