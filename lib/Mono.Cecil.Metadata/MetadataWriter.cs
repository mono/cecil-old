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
	using Mono.Cecil.Implem;

	internal sealed class MetadataWriter : IMetadataVisitor {

		private MetadataRoot m_root;
		private MetadataTableWriter m_tableWriter;
		private BinaryWriter m_binaryWriter;

		public MetadataWriter (ReflectionWriter reflectionWriter, MetadataRoot root)
		{
			m_root = root;
			m_tableWriter = new MetadataTableWriter (this);
			m_binaryWriter = reflectionWriter.GetWriter ();
		}

		public MetadataRoot GetMetadataRoot ()
		{
			return m_root;
		}

		public BinaryWriter GetWriter ()
		{
			return m_binaryWriter;
		}

		public MetadataTableWriter GetTableVisitor ()
		{
			return m_tableWriter;
		}

		public uint AddString (string str)
		{
			return 0;
		}

		public uint AddBlob (byte [] data)
		{
			return 0;
		}

		public uint AddGuid (Guid g)
		{
			return 0;
		}

		public uint AddUserString (string str)
		{
			return 0;
		}

		public void Visit (MetadataRoot root)
		{
			// TODO
		}

		public void Visit (MetadataRoot.MetadataRootHeader header)
		{
			// TODO
		}

		public void Visit (MetadataStreamCollection streams)
		{
			// TODO
		}

		public void Visit (MetadataStream stream)
		{
			// TODO
		}

		public void Visit (MetadataStream.MetadataStreamHeader header)
		{
			// TODO
		}

		public void Visit (GuidHeap heap)
		{
			// TODO
		}

		public void Visit (StringsHeap heap)
		{
			// TODO
		}

		public void Visit (TablesHeap heap)
		{
			// TODO
		}

		public void Visit (BlobHeap heap)
		{
			// TODO
		}

		public void Visit (UserStringsHeap heap)
		{
			// TODO
		}

		public void Terminate (MetadataStreamCollection streams)
		{
			// TODO
		}

		public void Terminate (MetadataRoot root)
		{
			// TODO
		}
	}
}
