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

    using System.IO;

    [Heap("#Blob")]
    internal class BlobHeap : MetadataHeap {

        private int m_indexSize;

        public int IndexSize {
            get { return m_indexSize; }
            set { m_indexSize = value; }
        }

        public byte[] Read(uint index) {
            return ReadBytesFromStream(index);
        }

        public BinaryReader GetReader(uint index) {
            return new BinaryReader(new MemoryStream(Read(index)));
        }

        public BlobHeap(MetadataStream stream) : base(stream) {}

        public override void Accept(IMetadataVisitor visitor) {
            visitor.Visit(this);
        }
    }
}
