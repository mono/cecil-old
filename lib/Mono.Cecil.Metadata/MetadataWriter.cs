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
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Implem;

	internal sealed class MetadataWriter : BaseMetadataVisitor {

		private MetadataRoot m_root;
		private TargetRuntime m_runtime;
		private ImageWriter m_imgWriter;
		private MetadataTableWriter m_tableWriter;
		private MemoryBinaryWriter m_binaryWriter;

		private IDictionary m_stringCache;
		private MemoryBinaryWriter m_stringWriter;

		private IDictionary m_guidCache;
		private MemoryBinaryWriter m_guidWriter;

		private IDictionary m_usCache;
		private MemoryBinaryWriter m_usWriter;

		private MemoryBinaryWriter m_blobWriter;

		private MemoryBinaryWriter m_tWriter;

		private MemoryBinaryWriter m_cilWriter;

		private MemoryBinaryWriter m_resWriter;

		private uint m_mdStart;
		private uint m_mdSize;

		private uint m_resStart;
		private uint m_resSize;

		private uint m_itStart;

		public MemoryBinaryWriter CilWriter {
			get { return m_cilWriter; }
		}

		public uint ItStartPos {
			get { return m_itStart; }
		}

		public MetadataWriter (MetadataRoot root, AssemblyKind kind, TargetRuntime rt, MemoryBinaryWriter writer)
		{
			m_root = root;
			m_runtime = rt;
			m_imgWriter = new ImageWriter (this, kind, writer);
			m_binaryWriter = m_imgWriter.GetTextWriter ();

			m_stringCache = new Hashtable ();
			m_stringWriter = new MemoryBinaryWriter (Encoding.UTF8);
			m_stringWriter.Write ('\0');

			m_guidCache = new Hashtable ();
			m_guidWriter = new MemoryBinaryWriter ();

			m_usCache = new Hashtable ();
			m_usWriter = new MemoryBinaryWriter (Encoding.Unicode);
			m_usWriter.Write ('\0');

			m_blobWriter = new MemoryBinaryWriter ();
			m_blobWriter.Write ((byte) 0);

			m_tWriter = new MemoryBinaryWriter ();
			m_tableWriter = new MetadataTableWriter (this, m_tWriter);

			m_cilWriter = new MemoryBinaryWriter ();

			m_resWriter = new MemoryBinaryWriter ();
		}

		public MetadataRoot GetMetadataRoot ()
		{
			return m_root;
		}

		public ImageWriter GetImageWriter ()
		{
			return m_imgWriter;
		}

		public MemoryBinaryWriter GetWriter ()
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
			foreach (char c in str)
				m_stringWriter.Write (c);
			m_stringWriter.Write ('\0');
			return pointer;
		}

		public uint AddBlob (byte [] data, bool withSize)
		{
			if (data == null || data.Length == 0)
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
			return pointer + 1;
		}

		public uint AddUserString (string str)
		{
			if (str == null || str.Length == 0)
				return 0;

			if (m_usCache.Contains (str))
				return (uint) m_usCache [str];

			uint pointer = (uint) Utilities.WriteCompressedInteger (
				m_usWriter, str.Length * 2 + 1);
			foreach (char c in str)
				m_usWriter.Write (c);
			m_usWriter.Write ('\0');
			return pointer;
		}

		private void CreateStream (string name)
		{
			MetadataStream stream = new MetadataStream ();
			stream.Header.Name = name;
			stream.Heap = MetadataHeap.HeapFactory (stream);
			m_root.Streams.Add (stream);
		}

		private void SetHeapSize (MetadataHeap heap, MemoryBinaryWriter data, byte flag)
		{
			if (data.BaseStream.Length > 65536) {
				m_root.Streams.TablesHeap.HeapSizes |= flag;
				heap.IndexSize = 4;
			} else
				heap.IndexSize = 2;
		}

		public uint AddResource (byte [] data)
		{
			uint offset = (uint) m_resWriter.BaseStream.Position;
			m_resWriter.Write (data);
			m_resWriter.QuadAlign ();
			return offset;
		}

		public override void Visit (MetadataRoot root)
		{
			WriteMemStream (m_cilWriter);
			m_resStart = (uint) m_binaryWriter.BaseStream.Position;
			WriteMemStream (m_resWriter);
			m_resSize = (uint) (m_binaryWriter.BaseStream.Position - m_resStart);
			// write strong name here

			m_mdStart = (uint) m_binaryWriter.BaseStream.Position;

			if (m_stringWriter.BaseStream.Length > 1) {
				CreateStream (MetadataStream.Strings);
				SetHeapSize (root.Streams.StringsHeap, m_stringWriter, 0x01);
				m_stringWriter.QuadAlign ();
			}

			if (m_guidWriter.BaseStream.Length > 0) {
				CreateStream (MetadataStream.GUID);
				SetHeapSize (root.Streams.GuidHeap, m_guidWriter, 0x02);
			}

			if (m_blobWriter.BaseStream.Length > 1) {
				CreateStream (MetadataStream.Blob);
				SetHeapSize (root.Streams.BlobHeap, m_blobWriter, 0x04);
				m_blobWriter.QuadAlign ();
			}

			if (m_usWriter.BaseStream.Length > 2) {
				CreateStream (MetadataStream.UserStrings);
				m_usWriter.QuadAlign ();
			}

			switch (m_runtime) {
			case TargetRuntime.NET_1_0 :
				m_root.Header.Version = "v1.0.3705";
				m_root.Header.MajorVersion = 1;
				m_root.Header.MinorVersion = 0;
				break;
			case TargetRuntime.NET_1_1 :
				m_root.Header.Version = "v1.1.4322";
				m_root.Header.MajorVersion = 1;
				m_root.Header.MinorVersion = 1;
				break;
			case TargetRuntime.NET_2_0 :
				throw new NotImplementedException (".net 2 assemblies are not supported");
			}

			m_root.Streams.TablesHeap.Tables.Accept (m_tableWriter);

			if (m_tWriter.BaseStream.Length == 0)
				m_root.Streams.Remove (m_root.Streams.TablesHeap.GetStream ());
		}

		public override void Visit (MetadataRoot.MetadataRootHeader header)
		{
			m_binaryWriter.Write (header.Signature);
			m_binaryWriter.Write (header.MajorVersion);
			m_binaryWriter.Write (header.MinorVersion);
			m_binaryWriter.Write (header.Reserved);
			m_binaryWriter.Write (header.Version.Length + 3 & (~3));
			foreach (char c in header.Version)
				m_binaryWriter.Write (c);
			m_binaryWriter.QuadAlign ();
			m_binaryWriter.Write (header.Flags);
			m_binaryWriter.Write ((ushort) m_root.Streams.Count);
		}

		public override void Visit (MetadataStreamCollection streams)
		{
			foreach (MetadataStream stream in streams) {
				MetadataStream.MetadataStreamHeader header = stream.Header;

				header.Offset = (uint) (m_binaryWriter.BaseStream.Position);
				m_binaryWriter.Write (header.Offset);
				MemoryBinaryWriter container;
				string name = header.Name;
				uint size = 0;
				switch (header.Name) {
				case MetadataStream.Tables :
					container = m_tWriter;
					size += 24; // header
					break;
				case MetadataStream.Strings :
					name += "\0\0\0\0";
					container = m_stringWriter;
					break;
				case MetadataStream.GUID :
					container = m_guidWriter;
					break;
				case MetadataStream.Blob :
					container = m_blobWriter;
					break;
				case MetadataStream.UserStrings :
					container = m_usWriter;
					break;
				default :
					throw new MetadataFormatException ("Unknown stream kind");
				}

				size += (uint) (container.BaseStream.Length + 3 & (~3));
				m_binaryWriter.Write (size);
				foreach (char c in name)
					m_binaryWriter.Write (c);
				m_binaryWriter.QuadAlign ();
			}
		}

		private void WriteMemStream (MemoryBinaryWriter writer)
		{
			m_binaryWriter.Write (writer);
			m_binaryWriter.QuadAlign ();
		}

		private void PatchStreamHeaderOffset (MetadataHeap heap)
		{
			long pos = m_binaryWriter.BaseStream.Position;
			m_binaryWriter.BaseStream.Position = heap.GetStream ().Header.Offset;
			m_binaryWriter.Write ((uint) (pos - m_mdStart));
			m_binaryWriter.BaseStream.Position = pos;
		}

		public override void Visit (GuidHeap heap)
		{
			PatchStreamHeaderOffset (heap);
			WriteMemStream (m_guidWriter);
		}

		public override void Visit (StringsHeap heap)
		{
			PatchStreamHeaderOffset (heap);
			WriteMemStream (m_stringWriter);
		}

		public override void Visit (TablesHeap heap)
		{
			PatchStreamHeaderOffset (heap);
			m_binaryWriter.Write (heap.Reserved);
			switch (m_runtime) {
			case TargetRuntime.NET_1_0 :
			case TargetRuntime.NET_1_1 :
				heap.MajorVersion = 1;
				heap.MinorVersion = 0;
				break;
			case TargetRuntime.NET_2_0 :
				throw new NotImplementedException (".net 2 assemblies are not supported");
			}
			m_binaryWriter.Write (heap.MajorVersion);
			m_binaryWriter.Write (heap.MinorVersion);
			m_binaryWriter.Write (heap.HeapSizes);
			m_binaryWriter.Write (heap.Reserved2);
			m_binaryWriter.Write (heap.Valid);
			m_binaryWriter.Write (heap.Sorted);
			WriteMemStream (m_tWriter);
		}

		public override void Visit (BlobHeap heap)
		{
			PatchStreamHeaderOffset (heap);
			WriteMemStream (m_blobWriter);
		}

		public override void Visit (UserStringsHeap heap)
		{
			PatchStreamHeaderOffset (heap);
			WriteMemStream (m_usWriter);
		}

		private void PatchHeader ()
		{
			Image img = m_imgWriter.GetImage ();

			if (m_mdSize > 0)
				img.CLIHeader.Metadata = new DataDirectory (
					img.TextSection.VirtualAddress + m_mdStart, m_itStart - m_mdStart);

			if (m_resSize > 0)
				img.CLIHeader.Resources = new DataDirectory (
					img.TextSection.VirtualAddress + m_resStart, m_resSize);
		}

		public override void Terminate (MetadataRoot root)
		{
			m_mdSize = (uint) (m_binaryWriter.BaseStream.Position - m_mdStart);
			m_itStart = (uint) m_binaryWriter.BaseStream.Position;
			m_binaryWriter.Write (new byte [0x60]); // imports
			m_imgWriter.Initialize ();
			PatchHeader ();
			root.GetImage ().Accept (m_imgWriter);
		}
	}
}
