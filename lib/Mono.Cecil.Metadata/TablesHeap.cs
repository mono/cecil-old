/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

	using System;
	using System.Collections;

	public class TablesHeap : MetadataHeap {

		public uint Reserved;
		public byte MajorVersion;
		public byte MinorVersion;
		public byte HeapSizes;
		public byte Reserved2;
		public long Valid;
		public long Sorted;

		private TableCollection m_tables;

		private static IDictionary m_tidCache = new Hashtable (46);

		public TableCollection Tables {
			get { return m_tables; }
			set { m_tables = value; }
		}

		public IMetadataTable this [Type table]
		{
			get { return m_tables [GetTableId (table)] as IMetadataTable; }
			set { m_tables [GetTableId (table)] = value; }
		}

		internal TablesHeap (MetadataStream stream) : base(stream, MetadataStream.Tables)
		{
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
