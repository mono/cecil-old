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

    internal sealed class ARRAY : SigType {

        private SigType m_type;
        private ArrayShape m_aryShape;

        public SigType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public ArrayShape Shape {
            get { return m_aryShape; }
            set { m_aryShape = value; }
        }

        public ARRAY () : base (ElementType.Array)
        {
        }
    }
}
