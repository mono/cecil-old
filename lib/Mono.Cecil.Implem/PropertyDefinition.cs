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

    internal sealed class PropertyDefinition : MemberDefinition, IPropertyDefinition {

        private ITypeReference m_propertyType;
        private ParameterDefinitionCollection m_parameters;
        private PropertyAttributes m_attributes;

        private IMethodDefinition m_getMeth;
        private IMethodDefinition m_setMeth;

        private bool m_readed = false;

        public ITypeReference PropertyType {
            get { return m_propertyType; }
            set { m_propertyType = value; }
        }

        public bool Readed {
            get { return m_readed; }
            set { m_readed = value; }
        }

        public IParameterDefinitionCollection Parameters {
            get {
                if (m_parameters == null) {
                    if (this.GetMethod != null)
                        m_parameters = this.GetMethod.Parameters as ParameterDefinitionCollection;
                    else
                        m_parameters = new ParameterDefinitionCollection (this);
                }
                return m_parameters;
            }
        }

        public PropertyAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public IMethodDefinition GetMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.SemanticReader.ReadMethods (this);
                return m_getMeth;
            }
            set { m_getMeth = value; }
        }

        public IMethodDefinition SetMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.SemanticReader.ReadMethods (this);
                return m_setMeth;
            }
            set { m_setMeth = value; }
        }

        public PropertyDefinition (string name, TypeDefinition decType, ITypeReference propType, PropertyAttributes attrs) : base (name, decType)
        {
            m_propertyType = propType;
            m_attributes = attrs;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
