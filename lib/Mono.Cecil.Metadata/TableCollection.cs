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

	public class TableCollection : ICollection, IMetadataTableVisitable {

		private IList m_items;
		private Hashtable m_index;

		private TablesHeap m_heap;

		public IMetadataTable this [int index] {
			get { return m_items [index] as IMetadataTable; }
			set { m_items [index] = value; }
		}

		public IMetadataTable this [ushort id]
		{
			get {
				if (m_index.ContainsKey (id))
					return m_items [(int) m_index [id]] as IMetadataTable;

				for (int i = 0; i < m_items.Count; i++) {
					IMetadataTable table = m_items [i] as IMetadataTable;
					if (TablesHeap.GetTableId (table.GetType ()) == id) {
						m_index [id] = i;
						return table;
					}
				}
				return null;
			}
			set {
				int index = IndexOf (value);
				if (index > -1) {
					m_items [index] = value;
					m_index [id] = index;
				}
			}
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

		public TablesHeap Heap {
			get { return m_heap; }
		}

		internal TableCollection (TablesHeap heap)
		{
			m_heap = heap;
			m_items = new ArrayList ();
			m_index = new Hashtable ();
		}

		internal void Add (IMetadataTable value)
		{
			m_items.Add (value);
		}

		internal void Clear ()
		{
			m_items.Clear ();
		}

		public bool Contains (IMetadataTable value)
		{
			return m_items.Contains (value);
		}

		public int IndexOf (IMetadataTable value)
		{
			return m_items.IndexOf (value);
		}

		internal void Insert (int index, IMetadataTable value)
		{
			m_items.Insert (index, value);
		}

		internal void Remove (IMetadataTable value)
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

		public void Accept (IMetadataTableVisitor visitor)
		{
			visitor.Visit (this);

			for (int i = 0; i < m_items.Count; i++)
				this [i].Accept (visitor);

			visitor.Terminate (this);
		}
	}
}
