//
// TableCollection.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil.Metadata {

	using System;
	using System.Collections;

	public class TableCollection : ICollection, IMetadataTableVisitable {

		ArrayList m_items;
		Hashtable m_index;

		TablesHeap m_heap;

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

		public void Sort ()
		{
			m_index.Clear ();
			m_items.Sort (TableComparer.Instance);
		}

		public IEnumerator GetEnumerator ()
		{
			return m_items.GetEnumerator ();
		}

		public void Accept (IMetadataTableVisitor visitor)
		{
			visitor.VisitTableCollection (this);

			for (int i = 0; i < m_items.Count; i++)
				this [i].Accept (visitor);

			visitor.TerminateTableCollection (this);
		}

		class TableComparer : IComparer {

			public static readonly TableComparer Instance = new TableComparer ();

			public int Compare (object x, object y)
			{
				return Comparer.Default.Compare (
					TablesHeap.GetTableId (x.GetType ()),
					TablesHeap.GetTableId (y.GetType ()));
			}
		}
	}
}
