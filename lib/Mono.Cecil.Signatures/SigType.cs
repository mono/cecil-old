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

    internal class SigType {

        private ElementType m_elementType;

        public ElementType ElementType {
            get { return m_elementType; }
            set { m_elementType = value; }
        }

        public SigType (ElementType elem)
        {
            m_elementType = elem;
        }
    }
}
