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

    using Mono.Cecil;

    internal sealed class FieldReference : MemberReference, IFieldReference {

        private ITypeReference m_fieldType;

        public ITypeReference FieldType {
            get { return m_fieldType; }
            set { m_fieldType = value; }
        }

        public FieldReference (string name, ITypeReference decType, ITypeReference fieldType) : base (name, decType)
        {
            m_fieldType = fieldType;
        }

        public void Accept (IReflectionVisitor visitor)
        {
        }

        public override string ToString ()
        {
            return string.Concat (m_fieldType.FullName, " ", base.ToString ());
        }
    }
}
