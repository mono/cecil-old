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

    using System;

    internal abstract class Signature : ISignatureVisitable {

        private uint m_callingConvention;
        private uint m_blobIndex;

        public uint CallingConvention {
            get { return m_callingConvention; }
            set { m_callingConvention = value; }
        }

        public uint BlobIndex {
            get { return m_blobIndex; }
            set { m_blobIndex = value; }
        }

        public Signature (uint blobIndex)
        {
            m_blobIndex = blobIndex;
        }

        public Signature ()
        {
            m_blobIndex = 0;
        }

        public abstract void Accept (ISignatureVisitor visitor);
    }
}
