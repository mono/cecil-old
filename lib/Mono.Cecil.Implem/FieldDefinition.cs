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

    internal sealed class FieldDefinition : MemberDefinition, IFieldDefinition {

        private ITypeReference m_fieldType;
        private FieldAttributes m_attributes;

        private object m_value;

        public ITypeReference FieldType {
            get { return m_fieldType; }
            set { m_fieldType = value; }
        }

        public FieldAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public object Value {
            get { return m_value; }
            set { m_value = value; }
        }

        public FieldDefinition (string name, TypeDefinition decType, ITypeReference fieldType, FieldAttributes attrs)
        {
            this.Name = name;
            m_fieldType = fieldType;
            m_attributes = attrs;
            SetDeclaringType (decType);
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
