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

    internal class TablesHeap : MetadataHeap {

        public uint Reserved;
        public byte MajorVersion;
        public byte MinorVersion;
        public byte HeapSizes;
        public byte Reserved2;
        public long Valid;
        public long Sorted;

        private TableCollection m_tables;

        private static IDictionary m_tidCache;

        public TableCollection Tables {
            get { return m_tables; }
            set { m_tables = value; }
        }

        public IMetadataTable this [Type table]
        {
            get { return m_tables [GetTableId (table)] as IMetadataTable; }
            set { m_tables [GetTableId (table)] = value; }
        }

        public TablesHeap (MetadataStream stream) : base(stream)
        {
            m_tidCache = new Hashtable (46);
        }

        public bool HasTable (Type table)
        {
            return (Valid & (1L << GetTableId (table))) != 0;
        }

        public override void Accept (IMetadataVisitor visitor)
        {
            visitor.Visit (this);
        }

        public static ushort GetTableId (Type table)
        {
            object id = m_tidCache [table];
            if (id != null)
                return (ushort) id;

            RIdAttribute [] rid = table.GetCustomAttributes (
                typeof(RIdAttribute), false) as RIdAttribute [];

            if (rid != null && rid.Length == 1) {
                m_tidCache [table] = (ushort) rid [0].Id;
                return (ushort) rid [0].Id;
            }

            throw new ArgumentException ("No RId attribute found on type");

        }
    }
}
