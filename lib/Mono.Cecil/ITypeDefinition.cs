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

	public interface ITypeDefinition : ITypeReference, IMemberDefinition, IHasSecurity {

		TypeAttributes Attributes { get; set; }
		ITypeReference BaseType { get; set; }
		IClassLayoutInfo LayoutInfo { get; }

		bool IsAbstract { get; set; }
		bool IsBeforeFieldInit { get; set; }
		bool IsInterface { get; set; }
		bool IsRuntimeSpecialName { get; set; }
		bool IsSealed { get; set; }
		bool IsSpecialName { get; set; }
		bool IsEnum { get; }

		IInterfaceCollection Interfaces { get; }
		INestedTypeCollection NestedTypes { get; }
		IMethodDefinitionCollection Methods { get; }
		IConstructorCollection Constructors { get; }
		IFieldDefinitionCollection Fields { get; }
		IEventDefinitionCollection Events { get; }
		IPropertyDefinitionCollection Properties { get; }
	}
}
