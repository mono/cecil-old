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

        private int m_sentinel;

        public int Sentinel {
            get { return m_sentinel; }
            set { m_sentinel = value; }
        }

        public MethodRefSig () : this (0)
        {
        }

        public MethodRefSig (uint blobIndex) : base (blobIndex)
        {
            m_sentinel = -1;
        }

        public override void Accept (ISignatureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
