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

namespace Mono.Cecil.Signatures {

    using Mono.Cecil;

    internal class TypeSpec {

        private SigType m_type;

        public SigType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public TypeSpec (SigType type)
        {
            switch (type.ElementType) {
            case ElementType.Ptr :
            case ElementType.FnPtr :
            case ElementType.Array :
            case ElementType.SzArray :
                m_type = type;
                return;
            default :
                throw new ReflectionException ("Non valid TypeSpec");
            }
        }
    }
}
