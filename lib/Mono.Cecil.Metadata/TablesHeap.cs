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

    [Heap("#~")]
    internal class TablesHeap : MetadataHeap {

        private uint m_reserved;
        private byte m_majorVersion;
        private byte m_minorVersion;
        private byte m_heapSizes;
        private byte m_reserved2;
        private long m_valid;
        private long m_sorted;

        private TableCollection m_tables;

        public uint Reserved {
            get { return m_reserved; }
            set { m_reserved = value; }
        }

        public byte MajorVersion {
            get { return m_majorVersion; }
            set { m_majorVersion = value; }
        }

        public byte MinorVersion {
            get { return m_minorVersion; }
            set { m_minorVersion = value; }
        }

        public byte HeapSizes {
            get { return m_heapSizes; }
            set { m_heapSizes = value; }
        }

        public byte Reserved2 {
            get { return m_reserved2; }
            set { m_reserved2 = value; }
        }

        public long Valid {
            get { return m_valid; }
            set { m_valid = value; }
        }

        public long Sorted {
            get { return m_sorted; }
            set { m_sorted = value; }
        }

        public TableCollection Tables {
            get { return m_tables; }
            set { m_tables = value; }
        }

        public IMetadataTable this[Type table] {
            get { return m_tables[GetTableId(table)] as IMetadataTable; }
            set { m_tables[GetTableId(table)] = value; }
        }

        public TablesHeap(MetadataStream stream) : base(stream) {}

        public bool HasTable(Type table) {
            return (m_valid & (1L << GetTableId(table))) != 0;
        }

        public override void Accept(IMetadataVisitor visitor) {
            visitor.Visit(this);
        }
        
        public static ushort GetTableId(Type table) {
            RIdAttribute[] id = table.GetCustomAttributes(
                typeof(RIdAttribute), false) as RIdAttribute[];
            if (id != null && id.Length == 1) {
                return (ushort)id[0].Id;
            } else {
                throw new ArgumentException("No RId attribute found on type");
            }
        }
    }
}
