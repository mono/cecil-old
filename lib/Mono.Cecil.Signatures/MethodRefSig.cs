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

    internal sealed class MethodRefSig : MethodSig {

        private bool m_sentinel;
        private Param [] m_paramsBeyondSentinel;

        public bool Sentinel {
            get { return m_sentinel; }
            set { m_sentinel = value; }
        }

        public Param [] ParamsBeyondSentinel {
            get { return m_paramsBeyondSentinel; }
            set { m_paramsBeyondSentinel = value; }
        }

        public override void Accept (ISignatureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
