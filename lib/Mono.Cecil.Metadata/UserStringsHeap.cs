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

namespace Mono.Cecil.Metadata {

    using System.Collections;

    [Heap("#US")]
    internal class UserStringsHeap : MetadataHeap {

        private readonly IList m_strings;

        public IList Strings {
            get { return m_strings; }
        }

        public UserStringsHeap(MetadataStream stream) : base(stream) {
            m_strings = new ArrayList();
        }

        public override void Accept(IMetadataVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
