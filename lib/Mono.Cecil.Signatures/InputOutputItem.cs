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

    internal abstract class InputOutputItem {

        private CustomMod [] m_customMods;
        private bool m_byRef;
        private SigType m_type;
        private bool m_typedByRef;

        public CustomMod [] CustomMods {
            get { return m_customMods; }
            set { m_customMods = value; }
        }

        public bool ByRef {
            get { return m_byRef; }
            set { m_byRef = value; }
        }

        public SigType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public bool TypedByRef {
            get { return m_typedByRef; }
            set { m_typedByRef = value; }
        }
    }
}
