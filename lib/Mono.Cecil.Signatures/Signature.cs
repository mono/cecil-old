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

        public uint CallingConvention {
            get { return m_callingConvention; }
            set { m_callingConvention = value; }
        }

        public abstract void Accept (ISignatureVisitor visitor);
    }
}
