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

	public class MetadataStreamCollection : ICollection, IMetadataVisitable {

		private IList m_items;

		private BlobHeap m_blobHeap;
		private GuidHeap m_guidHeap;
		private StringsHeap m_stringsHeap;
		private UserStringsHeap m_usHeap;
		private TablesHeap m_tablesHeap;

		public MetadataStream this [int index] {
			get { return m_items [index] as MetadataStream; }
			set { m_items [index] = value; }
		}

		public int Count {
			get { return m_items.Count; }
		}

		public bool IsSynchronized {
			get { return false; }
		}

		public object SyncRoot {
			get { return this; }
		}

		public BlobHeap BlobHeap {
			get {
				if (m_blobHeap == null)
					m_blobHeap = GetHeap ("#Blob") as BlobHeap;
				return m_blobHeap;
			}
		}

		public GuidHeap GuidHeap {
			get {
				if (m_guidHeap == null)
					m_guidHeap = GetHeap ("#GUID") as GuidHeap;
				return m_guidHeap;
			}
		}

		public StringsHeap StringsHeap {
			get {
				if (m_stringsHeap == null)
					m_stringsHeap = GetHeap ("#Strings") as StringsHeap;
				return m_stringsHeap;
			}
		}

		public TablesHeap TablesHeap {
			get {
				if (m_tablesHeap == null)
					m_tablesHeap = GetHeap ("#~") as TablesHeap;
				return m_tablesHeap;
			}
		}

		public UserStringsHeap UserStringsHeap {
			get {
				if (m_usHeap == null)
					m_usHeap = GetHeap ("#US") as UserStringsHeap;
				return m_usHeap;
			}
		}

		public MetadataStreamCollection ()
		{
			m_items = new ArrayList (5);
		}

		private MetadataHeap GetHeap (string name)
		{
			MetadataHeap heap = null;
			for (int i = 0; i < m_items.Count; i++) {
				MetadataStream stream = m_items [i] as MetadataStream;
				if (stream.Heap.Name == name) {
					heap = stream.Heap;
					break;
				}
			}
			return heap;
		}

		internal void Add (MetadataStream value)
		{
			m_items.Add (value);
		}

		internal void Clear ()
		{
			m_items.Clear ();
		}

		public bool Contains (MetadataStream value)
		{
			return m_items.Contains (value);
		}

		public int IndexOf (MetadataStream value)
		{
			return m_items.IndexOf (value);
		}

		internal void Insert (int index, MetadataStream value)
		{
			m_items.Insert (index, value);
		}

		internal void Remove (MetadataStream value)
		{
			m_items.Remove (value);
		}

		internal void RemoveAt (int index)
		{
			m_items.Remove (index);
		}

		public void CopyTo (Array ary, int index)
		{
			m_items.CopyTo (ary, index);
		}

		public IEnumerator GetEnumerator ()
		{
			return m_items.GetEnumerator ();
		}

		public void Accept (IMetadataVisitor visitor)
		{
			visitor.Visit (this);

			for (int i = 0; i < m_items.Count; i++)
				this [i].Accept (visitor);

			visitor.Terminate (this);
		}
	}
}
