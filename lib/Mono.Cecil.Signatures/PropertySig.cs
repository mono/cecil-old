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

    internal sealed class PropertySig : Signature {

        private bool m_property;
        private uint m_paramCount;
        private SigType m_type;
        private Param [] m_parameters;

        public bool Property {
            get { return m_property; }
            set { m_property = value; }
        }

        public uint ParamCount {
            get { return m_paramCount; }
            set { m_paramCount = value; }
        }

        public SigType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public Param [] Parameters {
            get { return m_parameters; }
            set { m_parameters = value; }
        }

        public override void Accept (ISignatureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
