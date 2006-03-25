//
// IModuleDefinition.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
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
//

namespace Mono.Cecil {

	using System;
	using SR = System.Reflection;

	using Mono.Cecil.Metadata;

	public interface IModuleDefinition : ICustomAttributeProvider, IMetadataScope,
		IReflectionStructureVisitable, IReflectionVisitable {

		AssemblyDefinition Assembly { get; }

		Guid Mvid { get; set; }
		bool Main { get; }

		AssemblyNameReferenceCollection AssemblyReferences { get; }
		ModuleReferenceCollection ModuleReferences { get; }
		ResourceCollection Resources { get; }
		TypeDefinitionCollection Types { get; }
		ExternTypeCollection ExternTypes { get; }
		TypeReferenceCollection TypeReferences { get; }
		MemberReferenceCollection MemberReferences { get; }

		IMetadataTokenProvider LookupByToken (MetadataToken token);
		IMetadataTokenProvider LookupByToken (TokenType table, int rid);

		TypeReference Import (Type type);
		MethodReference Import (SR.MethodBase meth);
		FieldReference Import (SR.FieldInfo field);

		TypeReference Import (TypeReference type);
		MethodReference Import (MethodReference meth);
		FieldReference Import (FieldReference field);

		TypeDefinition Inject (TypeDefinition type);
		MethodDefinition Inject (MethodDefinition meth);
		FieldDefinition Inject (FieldDefinition field);

		void FullLoad ();

		byte [] GetAsByteArray (CustomAttribute ca);
		byte [] GetAsByteArray (SecurityDeclaration dec);

		CustomAttribute FromByteArray (MethodReference ctor, byte [] data);
		SecurityDeclaration FromByteArray (SecurityAction action, byte [] data);
	}
}
