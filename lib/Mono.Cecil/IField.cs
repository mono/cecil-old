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

	public interface IFieldReference : IMemberReference, IReflectionVisitable {

		ITypeReference FieldType { get; set; }
	}

	public interface IFieldDefinition : IMemberDefinition, IFieldReference, ICustomAttributeProvider, IHasMarshalSpec, IReflectionVisitable {

		FieldAttributes Attributes { get; set; }
		IFieldLayoutInfo LayoutInfo { get; }
		object Constant { get;  set; }

		bool IsLiteral { get; set; }
		bool IsReadOnly { get; set; }
		bool IsRuntimeSpecialName { get; set; }
		bool IsSpecialName { get; set; }
		bool IsStatic { get; set; }
	}
}
