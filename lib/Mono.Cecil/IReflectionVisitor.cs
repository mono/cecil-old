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

	public interface IReflectionVisitor {

		void VisitModuleDefinition (IModuleDefinition module);
		void VisitTypeDefinitionCollection (ITypeDefinitionCollection types);
		void VisitTypeDefinition (ITypeDefinition type);
		void VisitTypeReferenceCollection (ITypeReferenceCollection refs);
		void VisitTypeReference (ITypeReference type);
		void VisitMemberReferenceCollection (IMemberReferenceCollection members);
		void VisitMemberReference (IMemberReference member);
		void VisitInterfaceCollection (IInterfaceCollection interfaces);
		void VisitInterface (ITypeReference interf);
		void VisitExternTypeCollection (IExternTypeCollection externs);
		void VisitExternType (ITypeReference externType);
		void VisitOverrideCollection (IOverrideCollection meth);
		void VisitOverride (IMethodReference ov);
		void VisitNestedTypeCollection (INestedTypeCollection nestedTypes);
		void VisitNestedType (ITypeDefinition nestedType);
		void VisitParameterDefinitionCollection (IParameterDefinitionCollection parameters);
		void VisitParameterDefinition (IParameterDefinition parameter);
		void VisitMethodDefinitionCollection (IMethodDefinitionCollection methods);
		void VisitMethodDefinition (IMethodDefinition method);
		void VisitConstructorCollection (IConstructorCollection ctors);
		void VisitConstructor (IMethodDefinition ctor);
		void VisitPInvokeInfo (IPInvokeInfo pinvk);
		void VisitEventDefinitionCollection (IEventDefinitionCollection events);
		void VisitEventDefinition (IEventDefinition evt);
		void VisitFieldDefinitionCollection (IFieldDefinitionCollection fields);
		void VisitFieldDefinition (IFieldDefinition field);
		void VisitPropertyDefinitionCollection (IPropertyDefinitionCollection properties);
		void VisitPropertyDefinition (IPropertyDefinition property);
		void VisitSecurityDeclarationCollection (ISecurityDeclarationCollection secDecls);
		void VisitSecurityDeclaration (ISecurityDeclaration secDecl);
		void VisitCustomAttributeCollection (ICustomAttributeCollection customAttrs);
		void VisitCustomAttribute (ICustomAttribute customAttr);
		void VisitMarshalSpec (IMarshalSpec marshalSpec);

		void TerminateModuleDefinition (IModuleDefinition module);
	}
}
