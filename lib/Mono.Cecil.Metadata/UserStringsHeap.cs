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

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Text;

    internal class UserStringsHeap : MetadataHeap {

        private readonly IDictionary m_strings;

        public IDictionary Strings {
            get { return m_strings; }
        }

        public string this [int offset] {
            get {
                string us = m_strings [offset] as string;
                if (us == null) {
                    us = ReadStringAt (offset);
                    m_strings [offset] = us;
                }
                return us;
            }
            set { m_strings [offset] = value; }
        }

        public UserStringsHeap (MetadataStream stream) : base(stream)
        {
            m_strings = new HybridDictionary ();
        }

        private string ReadStringAt (int offset)
        {
            int length = Utilities.ReadCompressedInteger (this.Data, offset, out offset);
            if (length == 0)
                return string.Empty;
            return Encoding.Unicode.GetString (this.Data, offset, length);
        }

        public override void Accept (IMetadataVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
