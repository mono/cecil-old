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

    internal sealed class FieldDefinition : MemberDefinition, IFieldDefinition, IFieldLayoutInfo {

        private ITypeReference m_fieldType;
        private FieldAttributes m_attributes;

        private bool m_hasInfo;
        private uint m_offset;

        private object m_value;

        public IFieldLayoutInfo LayoutInfo {
            get { return this; }
        }

        public bool HasLayoutInfo {
            get { return m_hasInfo; }
        }

        public uint Offset {
            get { return m_offset; }
            set {
                m_hasInfo = true;
                m_offset = value;
            }
        }

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

        public FieldDefinition (string name, TypeDefinition decType, ITypeReference fieldType, FieldAttributes attrs) : base (name, decType)
        {
            m_hasInfo = false;
            m_fieldType = fieldType;
            m_attributes = attrs;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
