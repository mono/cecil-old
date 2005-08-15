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
		bool IsValueType { get; }
		bool IsEnum { get; }

		IInterfaceCollection Interfaces { get; }
		void DefineInterface (Type interf);
		void DefineInterface (ITypeReference interf);

		INestedTypeCollection NestedTypes { get; }
		ITypeDefinition DefineNestedType (string name, TypeAttributes attributes);
		ITypeDefinition DefineNestedType (string name, TypeAttributes attributes, ITypeReference baseType);
		ITypeDefinition DefineNestedType (string name, TypeAttributes attributes, Type baseType);

		IMethodDefinitionCollection Methods { get; }
		IMethodDefinition DefineMethod (string name, MethodAttributes attributes, ITypeReference retType);
		IMethodDefinition DefineMethod (string name, MethodAttributes attributes, Type retType);

		IMethodDefinitionCollection Constructors { get; }
		IMethodDefinition DefineConstructor ();
		IMethodDefinition DefineConstructor (bool isstatic);

		IFieldDefinitionCollection Fields { get; }
		IFieldDefinition DefineField (string name, FieldAttributes attributes, ITypeReference fieldType);
		IFieldDefinition DefineField (string name, FieldAttributes attributes, Type fieldType);

		IEventDefinitionCollection Events { get; }
		IEventDefinition DefineEvent (string name, EventAttributes attributes, ITypeReference eventType);
		IEventDefinition DefineEvent (string name, EventAttributes attributes, Type eventType);

		IPropertyDefinitionCollection Properties { get; }
		IPropertyDefinition DefineProperty (string name, PropertyAttributes attributes, ITypeReference propType);
		IPropertyDefinition DefineProperty (string name, PropertyAttributes attributes, Type propType);
	}
}
