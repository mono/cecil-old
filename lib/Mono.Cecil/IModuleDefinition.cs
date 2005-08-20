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

		Guid Mvid { get; set; }
		bool Main { get; }

		IAssemblyNameReferenceCollection AssemblyReferences { get; }
		IAssemblyNameReference DefineAssemblyReference (string name);

		IModuleReferenceCollection ModuleReferences { get; }
		IModuleReference DefineModuleReference (string module);

		IResourceCollection Resources { get; }
		IEmbeddedResource DefineEmbeddedResource (string name, ManifestResourceAttributes attributes, byte [] data);
		ILinkedResource DefineLinkedResource (string name, ManifestResourceAttributes attributes, string file);
		IAssemblyLinkedResource DefineAssemblyLinkedResource (string name, ManifestResourceAttributes attributes, IAssemblyNameReference asm);

		ITypeDefinitionCollection Types { get; }
		ITypeDefinition DefineType (string name, string ns, TypeAttributes attributes);
		ITypeDefinition DefineType (string name, string ns, TypeAttributes attributes, ITypeReference baseType);
		ITypeDefinition DefineType (string name, string ns, TypeAttributes attributes, Type baseType);

		ITypeReference DefineTypeReference (string name, string ns, IAssemblyNameReference asm, bool valueType);
		IFieldReference DefineFieldReference (string name, ITypeReference declaringType, ITypeReference fieldType);
		IMethodReference DefineMethodReference (string name, ITypeReference declaringType,
			ITypeReference returnType, ITypeReference [] parametersTypes,
		bool hasThis, bool explicitThis, MethodCallingConvention conv);

		IExternTypeCollection ExternTypes { get; }

		IMetadataTokenProvider LookupByToken (MetadataToken token);
		IMetadataTokenProvider LookupByToken (TokenType table, int rid);
	}
}
