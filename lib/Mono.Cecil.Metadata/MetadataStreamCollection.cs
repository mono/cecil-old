/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Metadata {

    using System;
    using System.Collections;

    internal class MetadataStreamCollection : ICollection, IMetadataVisitable {

        private IList m_items;

        private MetadataRoot m_root;

        public MetadataStream this [int index] {
            get { return m_items [index] as MetadataStream; }
            set { m_items [index] = value; }
        }

        public MetadataStream this [string name] {
            get {
                foreach (MetadataStream ms in m_items) {
                    if (ms.Header.Name == name)
                        return ms;
                }
                return null;
            }
            set {
                if (!(this.IndexOf (value) > -1)) {
                    m_items.Add (value);
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

        public BlobHeap BlobHeap {
            get { return this ["#Blob"].Heap as BlobHeap; }
        }

        public GuidHeap GuidHeap {
            get { return this ["#GUID"].Heap as GuidHeap; }
        }

        public StringsHeap StringsHeap {
            get { return this ["#Strings"].Heap as StringsHeap; }
        }

        public TablesHeap TablesHeap {
            get { return this ["#~"].Heap as TablesHeap; }
        }

        public MetadataStreamCollection (MetadataRoot root)
        {
            m_root = root;
            m_items = new ArrayList (5);
        }

        public void Add (MetadataStream value)
        {
            m_items.Add (value);
        }

        public void Clear ()
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

        public void Insert (int index, MetadataStream value)
        {
            m_items.Insert (index, value);
        }

        public void Remove (MetadataStream value)
        {
            m_items.Remove (value);
        }

        public void RemoveAt (int index)
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
