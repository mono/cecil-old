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

namespace Mono.Cecil.Implem {

    using System;

    using Mono.Cecil;
    using Mono.Cecil.Signatures;

    internal sealed class PinnedType : TypeReference, IPinnedType {

        private ITypeReference m_pinnedType;

        public override string Name {
            get { return m_pinnedType.Name; }
            set { m_pinnedType.Name = value; }
        }

        public override string Namespace {
            get { return m_pinnedType.Namespace; }
            set { m_pinnedType.Namespace = value; }
        }

        public ITypeReference Type {
            get { return m_pinnedType; }
            set { m_pinnedType = value; }
        }

        public override string FullName {
            get { return m_pinnedType.FullName; }
        }

        public PinnedType (ITypeReference pType) : base (string.Empty, string.Empty)
        {
            m_pinnedType = pType;
        }
    }
}
