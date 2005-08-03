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

	public abstract class BaseReflectionVisitor : IReflectionVisitor {

		public virtual void Visit (ITypeDefinitionCollection types)
		{
		}

		public virtual void Visit (ITypeDefinition type)
		{
		}

		public virtual void Visit (ITypeReferenceCollection refs)
		{
		}

		public virtual void Visit (ITypeReference type)
		{
		}

		public virtual void Visit (IInterfaceCollection interfaces)
		{
		}

		public virtual void Visit (IExternTypeCollection externs)
		{
		}

		public virtual void Visit (IOverrideCollection meth)
		{
		}

		public virtual void Visit (INestedTypeCollection nestedTypes)
		{
		}

		public virtual void Visit (IParameterDefinitionCollection parameters)
		{
		}

		public virtual void Visit (IParameterDefinition parameter)
		{
		}

		public virtual void Visit (IMethodDefinitionCollection methods)
		{
		}


		public virtual void Visit (IMethodDefinition method)
		{
		}

		public virtual void Visit (IPInvokeInfo pinvk)
		{
		}

		public virtual void Visit (IEventDefinitionCollection events)
		{
		}

		public virtual void Visit (IEventDefinition evt)
		{
		}

		public virtual void Visit (IFieldDefinitionCollection fields)
		{
		}

		public virtual void Visit (IFieldDefinition field)
		{
		}

		public virtual void Visit (IPropertyDefinitionCollection properties)
		{
		}

		public virtual void Visit (IPropertyDefinition property)
		{
		}

		public virtual void Visit (ISecurityDeclarationCollection secDecls)
		{
		}

		public virtual void Visit (ISecurityDeclaration secDecl)
		{
		}

		public virtual void Visit (ICustomAttributeCollection customAttrs)
		{
		}

		public virtual void Visit (ICustomAttribute customAttr)
		{
		}

		public virtual void Visit (IMarshalSpec marshalSpec)
		{
		}
	}
}
