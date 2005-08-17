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

	using Mono.Cecil.Binary;

	public interface IFieldReference : IMemberReference, IReflectionVisitable {

		ITypeReference FieldType { get; set; }
	}

	public interface IFieldDefinition : IMemberDefinition, IFieldReference,
		ICustomAttributeProvider, IHasMarshalSpec, IHasConstant, IReflectionVisitable {

		FieldAttributes Attributes { get; set; }
		IFieldLayoutInfo LayoutInfo { get; }

		RVA RVA { get; set; }
		byte [] InitialValue { get; set; }

		bool IsLiteral { get; set; }
		bool IsReadOnly { get; set; }
		bool IsRuntimeSpecialName { get; set; }
		bool IsSpecialName { get; set; }
		bool IsStatic { get; set; }
	}
}
