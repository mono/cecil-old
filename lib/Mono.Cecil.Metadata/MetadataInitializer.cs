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

		public override void Visit (MetadataRoot root)
		{
			root.Header = new MetadataRoot.MetadataRootHeader ();
			root.Streams = new MetadataStreamCollection ();
		}

		public override void Visit (MetadataRoot.MetadataRootHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (MetadataStreamCollection coll)
		{
			MetadataStream tables = new MetadataStream ();
			tables.Header.Name = "#~";
			tables.Heap = MetadataHeap.HeapFactory (tables);
			m_root.Streams.Add (tables);
		}
	}
}
