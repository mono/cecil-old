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

    internal sealed class TypeDefinition : TypeReference, ITypeDefinition, IClassLayoutInfo {

        private TypeAttributes m_attributes;
        private ITypeReference m_baseType;

        private bool m_hasInfo;
        private ushort m_packingSize;
        private uint m_classSize;

        private ModuleDefinition m_module;

        private InterfaceCollection m_interfaces;
        private MethodDefinitionCollection m_methods;
        private FieldDefinitionCollection m_fields;
        private EventDefinitionCollection m_events;
        private PropertyDefinitionCollection m_properties;
        private SecurityDeclarationCollection m_secDecls;

        public TypeAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public ITypeReference BaseType {
            get { return m_baseType; }
            set { m_baseType = value; }
        }

        public IClassLayoutInfo LayoutInfo {
            get { return this; }
        }

        public bool HasLayoutInfo {
            get { return m_hasInfo; }
        }

        public ushort PackingSize {
            get { return m_packingSize; }
            set {
                m_hasInfo = true;
                m_packingSize = value;
            }
        }

        public uint ClassSize {
            get { return m_classSize; }
            set {
                m_hasInfo = true;
                m_classSize = value;
            }
        }

        public IInterfaceCollection Interfaces {
            get {
                if (m_interfaces == null)
                    m_interfaces = new InterfaceCollection (this);
                return m_interfaces;
            }
        }

        public IMethodDefinitionCollection Methods {
            get {
                if (m_methods == null)
                    m_methods = new MethodDefinitionCollection (this);
                return m_methods;
            }
        }

        public IFieldDefinitionCollection Fields {
            get {
                if (m_fields == null)
                    m_fields = new FieldDefinitionCollection (this);
                return m_fields;
            }
        }

        public IEventDefinitionCollection Events {
            get {
                if (m_events == null)
                    m_events = new EventDefinitionCollection (this);
                return m_events;
            }
        }

        public IPropertyDefinitionCollection Properties {
            get {
                if (m_properties == null)
                    m_properties = new PropertyDefinitionCollection (this);
                return m_properties;
            }
        }

        public ISecurityDeclarationCollection SecurityDeclarations {
            get {
                if (m_secDecls == null)
                    m_secDecls = new SecurityDeclarationCollection (this);
                return m_secDecls;
            }
        }

        public ModuleDefinition Module {
            get { return m_module; }
        }

        public TypeDefinition (string name, string ns, TypeAttributes attrs, ModuleDefinition module) : base (name, ns)
        {
            m_hasInfo = false;
            m_attributes = attrs;
            m_module = module;
        }

        public override void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);

            m_interfaces.Accept (visitor);
            m_fields.Accept (visitor);
            m_properties.Accept (visitor);
            m_events.Accept (visitor);
            m_methods.Accept (visitor);
        }
    }
}
