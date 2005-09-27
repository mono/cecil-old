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
 * Generated by /CodeGen/cecil-gen.rb do not edit
 * Sun Aug 28 03:29:05 CEST 2005
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	internal class ConstructorCollection : IConstructorCollection {

		private IList m_items;
		private TypeDefinition m_container;

		public event ConstructorEventHandler OnConstructorAdded;
		public event ConstructorEventHandler OnConstructorRemoved;

		public IMethodDefinition this [int index] {
			get { return m_items [index] as IMethodDefinition; }
			set { m_items [index] = value; }
		}

		public ITypeDefinition Container {
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

		public ConstructorCollection (TypeDefinition container)
		{
			m_container = container;
			m_items = new ArrayList ();
		}

		public void Add (IMethodDefinition value)
		{
			if (OnConstructorAdded != null && !this.Contains (value))
				OnConstructorAdded (this, new ConstructorEventArgs (value));
			m_items.Add (value);
		}

		public void Clear ()
		{
			if (OnConstructorRemoved != null)
				foreach (IMethodDefinition item in this)
					OnConstructorRemoved (this, new ConstructorEventArgs (item));
			m_items.Clear ();
		}

		public bool Contains (IMethodDefinition value)
		{
			return m_items.Contains (value);
		}

		public int IndexOf (IMethodDefinition value)
		{
			return m_items.IndexOf (value);
		}

		public void Insert (int index, IMethodDefinition value)
		{
			if (OnConstructorAdded != null && !this.Contains (value))
				OnConstructorAdded (this, new ConstructorEventArgs (value));
			m_items.Insert (index, value);
		}

		public void Remove (IMethodDefinition value)
		{
			if (OnConstructorRemoved != null && this.Contains (value))
				OnConstructorRemoved (this, new ConstructorEventArgs (value));
			m_items.Remove (value);
		}

		public void RemoveAt (int index)
		{
			if (OnConstructorRemoved != null)
				OnConstructorRemoved (this, new ConstructorEventArgs (this [index]));
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

		public IMethodDefinition GetConstructor (bool isStatic, Type [] parameters)
		{
			foreach (IMethodDefinition ctor in this)
				if (ctor.IsStatic == isStatic && ctor.Parameters.Count == parameters.Length)
					for (int i = 0; i < parameters.Length; i++)
						if (ctor.Parameters [i].ParameterType.FullName ==  ReflectionHelper.GetTypeSignature (parameters [i]))
							return ctor;

			return null;
		}

		public IMethodDefinition GetConstructor (bool isStatic, ITypeReference [] parameters)
		{
			foreach (IMethodDefinition ctor in this)
				if (ctor.IsStatic == isStatic && ctor.Parameters.Count == parameters.Length)
					for (int i = 0; i < parameters.Length; i++)
						if (ctor.Parameters [i].ParameterType.FullName == parameters [i].FullName)
							return ctor;

			return null;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitConstructorCollection (this);
		}
	}
}