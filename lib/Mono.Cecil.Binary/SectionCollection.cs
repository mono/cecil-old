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

namespace Mono.Cecil.Binary {

    using System;
    using System.Collections;

    internal class SectionCollection : ICollection, IBinaryVisitable {

        private IList m_items;

        public Section this[int index] {
            get { return m_items[index] as Section; }
            set { m_items[index] = value; }
        }

        public Section this[string name] {
            get {
                foreach (Section sect in this) {
                    if (sect.Name == name) {
                        return sect;
                    }
                }
                return null;
            }
            set {
                int pos = -1;
                for (int i = 0 ; i < m_items.Count ; i++) {
                    if (this[i].Name == name) {
                        pos = i;
                    }
                }
                this[pos] = value;
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

        public SectionCollection() {
            m_items = new ArrayList(4);
        }

        public void Add(Section value) {
            m_items.Add(value);
        }

        public void Clear() {
            m_items.Clear();
        }

        public bool Contains(Section value) {
            return m_items.Contains(value);
        }

        public int IndexOf(Section value) {
            return m_items.IndexOf(value);
        }

        public void Insert(int index, Section value) {
            m_items.Insert(index, value);
        }

        public void Remove(Section value) {
            m_items.Remove(value);
        }

        public void RemoveAt(int index) {
            m_items.Remove(index);
        }

        public void CopyTo(Array ary, int index) {
            m_items.CopyTo(ary, index);
        }

        public IEnumerator GetEnumerator() {
            return m_items.GetEnumerator();
        }

        public void Accept(IBinaryVisitor visitor) {
            visitor.Visit(this);

            for (int i = 0 ; i < m_items.Count ; i++) {
                this[i].Accept(visitor);
            }
        }
    }
}
