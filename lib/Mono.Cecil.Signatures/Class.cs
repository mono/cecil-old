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

    using Mono.Cecil.Metadata;

    internal sealed class Class : SigType {

        private MetadataToken m_type;

        public MetadataToken Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public Class () : base (ElementType.Class)
        {
        }
    }
}
