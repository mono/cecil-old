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

	internal sealed class MetadataInitializer : IMetadataVisitor {

		private MetadataRoot m_root;

		public MetadataInitializer (ImageInitializer init)
		{
			m_root = init.Image.MetadataRoot;
		}

		public void Visit (MetadataRoot root)
		{
			root.Header = new MetadataRoot.MetadataRootHeader ();
			root.Streams = new MetadataStreamCollection ();
		}

		public void Visit (MetadataRoot.MetadataRootHeader header)
		{
			header.SetDefaultValues ();
		}

		public void Visit (MetadataStreamCollection coll)
		{
			MetadataStream tables = new MetadataStream ();
			tables.Header = new MetadataStream.MetadataStreamHeader (tables);
			tables.Header.Name = "#~";
			tables.Heap = MetadataHeap.HeapFactory (tables);
			m_root.Streams.Add (tables);
		}

		public void Visit (MetadataStream stream)
		{
		}

		public void Visit (MetadataStream.MetadataStreamHeader header)
		{
		}

		public void Visit (GuidHeap heap)
		{
		}

		public void Visit (StringsHeap heap)
		{
		}

		public void Visit (TablesHeap heap)
		{
		}

		public void Visit (BlobHeap heap)
		{
		}

		public void Visit (UserStringsHeap heap)
		{
		}

		public void Terminate (MetadataStreamCollection coll)
		{
		}

		public void Terminate (MetadataRoot root)
		{
		}
	}
}
