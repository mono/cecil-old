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

    internal class MetadataStream : IMetadataVisitable {

        private MetadataStreamHeader m_header;
        private MetadataHeap m_heap;

        private MetadataRoot m_root;

        public MetadataStreamHeader Header {
            get { return m_header; }
            set { m_header = value; }
        }

        public MetadataHeap Heap {
            get { return m_heap; }
            set { m_heap = value; }
        }

        public MetadataStream(MetadataRoot root) {
            m_root = root;
        }

        public void Accept(IMetadataVisitor visitor) {
            visitor.Visit(this);

            m_header.Accept(visitor);
            if (m_heap != null) {
                m_heap.Accept(visitor);
            }
        }
        
        internal class MetadataStreamHeader : IMetadataVisitable {
    
            private uint m_offset;
            private uint m_size;
            private string m_name;
    
            private MetadataStream m_stream;

            public uint Offset {
                get { return m_offset; }
                set { m_offset = value; }
            }

            public uint Size {
                get { return m_size; }
                set { m_size = value; }
            }

            public string Name {
                get { return m_name; }
                set { m_name = value; }
            }

            public MetadataStream Stream {
                get { return m_stream; }
            }

            public MetadataStreamHeader(MetadataStream stream) {
                m_stream = stream;
            }

            public void Accept(IMetadataVisitor visitor) {
                visitor.Visit(this);
            }
        }
    }
}
