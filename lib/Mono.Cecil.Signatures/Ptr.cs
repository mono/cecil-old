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

    internal sealed class PTR : SigType {

        private CustomMod [] m_customMods;
        private SigType m_ptrType;
        private bool m_void;

        public CustomMod [] CustomMods {
            get { return m_customMods; }
            set { m_customMods = value; }
        }

        public SigType PtrType {
            get { return m_ptrType; }
            set { m_ptrType = value; }
        }

        public bool Void {
            get { return m_void; }
            set { m_void = value; }
        }

        public PTR () : base (ElementType.Ptr)
        {
        }
    }
}
