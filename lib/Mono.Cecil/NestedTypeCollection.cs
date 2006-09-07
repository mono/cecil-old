//
// NestedTypeCollection.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Generated by /CodeGen/cecil-gen.rb do not edit
// Wed Sep 06 13:53:07 CEST 2006
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

namespace Mono.Cecil {

	using System;
	using System.Collections;

	using Mono.Cecil.Cil;

	public sealed class NestedTypeCollection : IIndexedCollection, IReflectionVisitable {

		IList m_items;
		TypeDefinition m_container;

		public TypeDefinition this [int index] {
			get { return m_items [index] as TypeDefinition; }
			set { m_items [index] = value; }
		}

		object IIndexedCollection.this [int index] {
			get { return m_items [index]; }
		}

		public TypeDefinition Container {
			get { return m_container; }
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

		public NestedTypeCollection (TypeDefinition container)
		{
			m_container = container;
			m_items = new ArrayList ();
		}

		public void Add (TypeDefinition value)
		{
			if (!this.Contains (value))
				Attach (value);

			m_items.Add (value);
		}

		public void Clear ()
		{
			foreach (TypeDefinition item in this)
				Detach (item);

			m_items.Clear ();
		}

		public bool Contains (TypeDefinition value)
		{
			return m_items.Contains (value);
		}

		public int IndexOf (TypeDefinition value)
		{
			return m_items.IndexOf (value);
		}

		public void Insert (int index, TypeDefinition value)
		{
			if (!this.Contains (value))
				Attach (value);

			m_items.Insert (index, value);
		}

		public void Remove (TypeDefinition value)
		{
			if (this.Contains (value))
				Detach (value);

			m_items.Remove (value);
		}

		public void RemoveAt (int index)
		{
			Detach (this [index]);

			m_items.RemoveAt (index);
		}

		public void CopyTo (Array array, int index)
		{
			m_items.CopyTo (array, index);
		}

		public IEnumerator GetEnumerator ()
		{
			return m_items.GetEnumerator ();
		}

		void Attach (MemberReference member)
		{
			if (member.DeclaringType != null)
				throw new ReflectionException ("Member already attached, clone it instead");

			member.DeclaringType = m_container;
		}

		void Detach (MemberReference member)
		{
			member.DeclaringType = null;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitNestedTypeCollection (this);
		}
	}
}
