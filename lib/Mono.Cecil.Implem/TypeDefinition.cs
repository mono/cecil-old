/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

    using System;

    using Mono.Cecil;

    internal sealed class TypeDefinition : ITypeDefinition {

        private string m_name;
        private string m_namespace;
        private TypeAttributes m_attributes;
        private IType m_baseType;

        private NestedTypesCollection m_nestedTypes;
        private InterfaceCollection m_interfaces;
        private MethodDefinitionCollection m_methods;

        private IType m_declaringType;

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public string Namespace {
            get { return m_namespace; }
            set { m_namespace = value; }
        }

        public TypeAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public IType BaseType {
            get { return m_baseType; }
            set { m_baseType = value; }
        }

        public string FullName {
            get {
                if (m_declaringType != null)
                    return string.Format ("{0}/{1}", m_declaringType.FullName, m_name);

                if (m_namespace == null || m_namespace.Length == 0)
                    return m_name;

                return string.Format ("{0}.{1}", m_namespace, m_name);
            }
        }

        public IType DeclaringType {
            get { return m_declaringType; }
            set { m_declaringType = value; }
        }

        public INestedTypesCollection NestedTypes {
            get { return m_nestedTypes; }
        }

        public IInterfaceCollection Interfaces {
            get { return m_interfaces; }
        }

        public IMethodDefinitionCollection Methods {
            get { return m_methods; }
        }

        public TypeDefinition (string name, string ns, TypeAttributes attrs)
        {
            m_name = name;
            m_namespace = ns;
            m_attributes = attrs;

            m_nestedTypes = new NestedTypesCollection (this);
            m_interfaces = new InterfaceCollection (this);
            m_methods = new MethodDefinitionCollection (this);
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);

            m_interfaces.Accept (visitor);
            m_methods.Accept (visitor);
        }
    }
}
