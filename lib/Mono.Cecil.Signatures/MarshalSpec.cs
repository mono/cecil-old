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

    internal class MarshalSpec {

        private NativeType m_natType;
        // array

        public NativeType NativeType {
            get { return m_natType; }
            set { m_natType = value; }
        }

        public MarshalSpec (NativeType nt)
        {
            m_natType = nt;
        }
    }
}
