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
	using System.IO;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;

	internal sealed class MetadataInitializer : BaseMetadataVisitor {

		private MetadataRoot m_root;

		public MetadataInitializer (ImageInitializer init)
		{
			m_root = init.Image.MetadataRoot;
		}

		public override void VisitMetadataRoot (MetadataRoot root)
		{
			root.Header = new MetadataRoot.MetadataRootHeader ();
			root.Streams = new MetadataStreamCollection ();
		}

		public override void VisitMetadataRootHeader (MetadataRoot.MetadataRootHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void VisitMetadataStreamCollection (MetadataStreamCollection coll)
		{
			MetadataStream tables = new MetadataStream ();
			tables.Header.Name = MetadataStream.Tables;
			tables.Heap = MetadataHeap.HeapFactory (tables);
			TablesHeap th = tables.Heap as TablesHeap;
			th.Tables = new TableCollection (th);
			m_root.Streams.Add (tables);
		}

		public override void VisitTablesHeap (TablesHeap th)
		{
			th.Reserved = 0;
			th.MajorVersion = 1;
			th.MinorVersion = 0;
			th.Reserved2 = 1;
			th.Sorted = 0x2003301fa00;
		}
	}
}
