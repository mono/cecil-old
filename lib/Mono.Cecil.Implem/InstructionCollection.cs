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

namespace Mono.Cecil.Implem {

    using System;
    using System.Collections;

    using Mono.Cecil.Cil;

    internal class InstructionCollection : IInstructionCollection, ILazyLoadableCollection, ICodeVisitable {

        private IList m_items;
        private MethodBody m_container;
        private bool m_loaded;

        public IInstruction this [int index]
        {
            get {
                ((TypeDefinition) m_container.Method.DeclaringType).Module.Loader.CodeReader.Visit (this);
                return m_items [index] as IInstruction;
            }
            set { m_items [index] = value; }
        }

        public int Count {
            get {
                ((TypeDefinition) m_container.Method.DeclaringType).Module.Loader.CodeReader.Visit (this);
                return m_items.Count;
            }
        }

        public bool IsSynchronized {
            get { return false; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public IMethodBody Container {
            get { return m_container; }
        }

        public bool Loaded {
            get { return m_loaded; }
            set { m_loaded = value; }
        }

        public InstructionCollection (MethodBody container)
        {
            m_container =  container;
            m_items = new ArrayList ();
        }

        public void Add (IInstruction value)
        {
            m_items.Add (value);
        }

        public void Clear ()
        {
            m_items.Clear ();
        }

        public bool Contains (IInstruction value)
        {
            ((TypeDefinition) m_container.Method.DeclaringType).Module.Loader.CodeReader.Visit (this);
            return m_items.Contains (value);
        }

        public int IndexOf (IInstruction value)
        {
            ((TypeDefinition) m_container.Method.DeclaringType).Module.Loader.CodeReader.Visit (this);
            return m_items.IndexOf (value);
        }

        public void Insert (int index, IInstruction value)
        {
            m_items.Insert (index, value);
        }

        public void Remove (IInstruction value)
        {
            m_items.Remove (value);
        }

        public void RemoveAt (int index)
        {
            m_items.Remove (index);
        }

        public void CopyTo (Array ary, int index)
        {
            ((TypeDefinition) m_container.Method.DeclaringType).Module.Loader.CodeReader.Visit (this);
            m_items.CopyTo (ary, index);
        }

        public IEnumerator GetEnumerator ()
        {
            ((TypeDefinition) m_container.Method.DeclaringType).Module.Loader.CodeReader.Visit (this);
            return m_items.GetEnumerator ();
        }

        public void Accept (ICodeVisitor visitor)
        {
            visitor.Visit (this);

            for (int i = 0; i < m_items.Count; i++)
                this [i].Accept (visitor);
        }
    }
}
