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

    internal sealed class FieldSig : Signature {

        private bool m_field;
        private CustomMod [] m_customMods;
        private SigType m_type;

        public bool Field {
            get { return m_field; }
            set { m_field = value; }
        }

        public CustomMod [] CustomMods {
            get { return m_customMods; }
            set { m_customMods = value; }
        }

        public SigType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public override void Accept (ISignatureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
