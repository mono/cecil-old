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

    internal sealed class SZARRAY : SigType {

        private CustomMod [] m_customMods;
        private SigType m_type;

        public CustomMod [] CustomMods {
            get { return m_customMods; }
            set { m_customMods = value; }
        }

        public SigType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public SZARRAY () : base (ElementType.SzArray)
        {
        }
    }
}
