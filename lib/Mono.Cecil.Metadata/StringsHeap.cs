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

    [Heap ("#Strings")]
    internal class StringsHeap : MetadataHeap {

        private readonly IDictionary m_strings;
        private int m_indexSize;

        public string this [uint index] {
            get { return m_strings [index] == null ? string.Empty : m_strings [index] as string; }
            set { m_strings [index] = value; }
        }

        public int IndexSize {
            get { return m_indexSize; }
            set { m_indexSize = value; }
        }

        public StringsHeap (MetadataStream stream) : base (stream)
        {
            m_strings = new SortedList ();
        }

        public override void Accept (IMetadataVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}

