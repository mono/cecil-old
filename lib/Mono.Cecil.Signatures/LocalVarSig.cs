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

    internal sealed class LocalVarSig : Signature {

        private bool m_local;
        private int m_count;
        private LocalVariable [] m_lvTypes;

        public bool Local {
            get { return m_local; }
            set { m_local = value; }
        }

        public int Count {
            get { return m_count; }
            set { m_count = value; }
        }

        public LocalVariable [] LocalVariables {
            get { return m_lvTypes; }
            set { m_lvTypes = value; }
        }

        public LocalVarSig () : base ()
        {
        }

        public LocalVarSig (uint blobIndex) : base (blobIndex)
        {
        }

        public override void Accept (ISignatureVisitor visitor)
        {
            visitor.Visit (this);
        }

        public sealed class LocalVariable {

            private Constraint m_constraint;
            private bool m_byRef;
            private SigType m_type;

            public Constraint Constraint {
                get { return m_constraint; }
                set { m_constraint = value; }
            }

            public bool ByRef {
                get { return m_byRef; }
                set { m_byRef = value; }
            }

            public SigType Type {
                get { return m_type; }
                set { m_type = value; }
            }
        }
    }
}
