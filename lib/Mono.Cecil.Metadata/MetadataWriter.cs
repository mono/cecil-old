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
	using System.IO;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Implem;

	internal sealed class MetadataWriter : IMetadataVisitor {

		private MetadataRoot m_root;
		private MetadataTableWriter m_tableWriter;
		private BinaryWriter m_binaryWriter;

		private IDictionary m_stringCache;
		private BinaryWriter m_stringWriter;

		private IDictionary m_guidCache;
		private BinaryWriter m_guidWriter;

		private IDictionary m_usCache;
		private BinaryWriter m_usWriter;

		private BinaryWriter m_blobWriter;

		private BinaryWriter m_tWriter;

		public MetadataWriter (ReflectionWriter reflectionWriter, MetadataRoot root)
		{
			m_root = root;
			m_binaryWriter = reflectionWriter.GetWriter ();

			m_stringCache = new Hashtable ();
			m_stringWriter = new BinaryWriter (new MemoryStream (), Encoding.UTF8);
			m_stringWriter.Write ('\0');

			m_guidCache = new Hashtable ();
			m_guidWriter = new BinaryWriter (new MemoryStream ());

			m_usCache = new Hashtable ();
			m_usWriter = new BinaryWriter (new MemoryStream (), Encoding.Unicode);
			m_usWriter.Write ('\0');

			m_blobWriter = new BinaryWriter (new MemoryStream ());

			m_tWriter = new BinaryWriter (new MemoryStream ());
			m_tableWriter = new MetadataTableWriter (this, m_tWriter);
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
			if (str == null || str.Length == 0)
				return 0;

			if (m_stringCache.Contains (str))
				return (uint) m_stringCache [str];

			uint pointer = (uint) m_stringWriter.BaseStream.Position;
			m_stringWriter.Write (str);
			m_stringWriter.Write ('\0');
			m_root.Streams.StringsHeap [pointer] = str;
			return pointer;
		}

		public uint AddBlob (byte [] data, bool withSize)
		{
			if (data.Length == 0)
				return 0;

			uint pointer = withSize ? (uint) Utilities.WriteCompressedInteger (
				m_blobWriter, data.Length) : (uint) m_blobWriter.BaseStream.Position;
			m_blobWriter.Write (data);
			return pointer;
		}

		public uint AddGuid (Guid g)
		{
			if (m_guidCache.Contains (g))
				return (uint) m_guidCache [g];

			uint pointer = (uint) m_guidWriter.BaseStream.Position;
			m_guidWriter.Write (g.ToByteArray ());
			m_root.Streams.GuidHeap [pointer] = g;
			return pointer;
		}

		public uint AddUserString (string str)
		{
			if (str == null || str.Length == 0)
				return 0;

			if (m_usCache.Contains (str))
				return (uint) m_usCache [str];

			uint pointer = (uint) Utilities.WriteCompressedInteger (
				m_usWriter, str.Length * 2 + 1);
			m_usWriter.Write (str);
			m_usWriter.Write ('\0');
			m_root.Streams.UserStringsHeap [pointer] = str;
			return pointer;
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
