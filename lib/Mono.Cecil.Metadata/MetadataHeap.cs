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

    internal abstract class MetadataHeap : IMetadataVisitable  {

        private MetadataStream m_stream;
        private byte[] m_data;

        public byte[] Data {
            get { return m_data; }
            set { m_data = value; }
        }

        protected MetadataHeap(MetadataStream stream) {
            m_stream = stream;
        }

        public static MetadataHeap HeapFactory(MetadataStream stream) {
            switch (stream.Header.Name) {
                case "#~" :
                    return new TablesHeap(stream);
                case "#-" :
                    throw new MetadataFormatException("Non standard #- heap found");
                case "#GUID" :
                    return new GuidHeap(stream);
                case "#Strings" :
                    return new StringsHeap(stream);
                case "#US" :
                    return new UserStringsHeap(stream);
                case "#Blob" :
                    return new BlobHeap(stream);
                default :
                    return null;
            }
        }

        public MetadataStream GetStream() {
            return m_stream;
        }

        protected virtual byte[] ReadBytesFromStream(uint pos) {
            int length = 0, start = (int)pos;
            if ((m_data[pos] & 0x80) == 0) {
                length = m_data[pos];
                start++;
            } else if ((m_data[pos] & 0x40) == 0) {
                length = (m_data[start] & ~0x80) << 8;
                length |= m_data[pos + 1];
                start += 2;
            } else {
                length = (m_data[start] & ~0xc0) << 24;
                length |= m_data[pos + 1] << 16;
                length |= m_data[pos + 2] << 8;
                length |= m_data[pos + 3];
                start += 4;
            }
            byte[] buffer = new byte[length];
            Buffer.BlockCopy(m_data, start, buffer, 0, length);
            return buffer;
        }

        public abstract void Accept(IMetadataVisitor visitor);
    }
}
