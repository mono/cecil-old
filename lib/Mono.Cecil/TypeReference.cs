//
// TypeReference.cs
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

namespace Mono.Cecil {

	using System;
	using System.Reflection;

	using Mono.Cecil.Metadata;

	public class TypeReference : ITypeReference {

		string m_name;
		string m_namespace;
		bool m_fullNameDiscarded;
		string m_fullName;
		protected bool m_isValueType;
		TypeReference m_decType;
		protected IMetadataScope m_scope;
		MetadataToken m_token;

		CustomAttributeCollection m_customAttrs;
		GenericParameterCollection m_genparams;

		protected ModuleDefinition m_module;

		public virtual string Name {
			get { return m_name; }
			set {
				m_name = value;
				m_fullNameDiscarded = true;
			}
		}

		public virtual string Namespace {
			get { return m_namespace; }
			set {
				m_namespace = value;
				m_fullNameDiscarded = true;
			}
		}

		public virtual TypeReference DeclaringType {
			get { return m_decType; }
			set {
				m_decType = value;
				m_fullNameDiscarded = true;
			}
		}

		public virtual bool IsValueType {
			get { return m_isValueType; }
			set { m_isValueType = value; }
		}

		public ModuleDefinition Module {
			get { return m_module; }
			set { m_module = value; }
		}

		public MetadataToken MetadataToken {
			get { return m_token; }
			set { m_token = value; }
		}

		public CustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public GenericParameterCollection GenericParameters {
			get {
				if (m_genparams == null) {
					m_genparams = new GenericParameterCollection (this);
					m_genparams.OnGenericParameterAdded += new GenericParameterEventHandler (OnGenericParameterAdded);
				}

				return m_genparams;
			}
		}

		public virtual IMetadataScope Scope {
			get {
				if (m_decType != null)
					return m_decType.Scope;

				return m_scope;
			}
		}

		public virtual string FullName {
			get {
				if (m_fullName != null && !m_fullNameDiscarded)
					return m_fullName;

				if (m_decType != null)
					return string.Concat (m_decType.FullName, "/", m_name);

				if (m_namespace == null || m_namespace.Length == 0)
					return m_name;

				m_fullName = string.Concat (m_namespace, ".", m_name);
				m_fullNameDiscarded = false;
				return m_fullName;
			}
		}

		protected TypeReference (string name, string ns)
		{
			m_name = name;
			m_namespace = ns;
			m_fullNameDiscarded = false;
		}

		internal TypeReference (string name, string ns, IMetadataScope scope) : this (name, ns)
		{
			m_scope = scope;
		}

		public TypeReference (string name, string ns, IMetadataScope scope, bool valueType) :
			this (name, ns, scope)
		{
			this.IsValueType = valueType;
		}

		internal void AttachToScope (IMetadataScope scope)
		{
			m_scope = scope;
		}

		void OnGenericParameterAdded (object sender, GenericParameterEventArgs ea)
		{
			ea.GenericParameter.Position = m_genparams.Count + 1;
		}

		public virtual void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitTypeReference (this);
		}

		public override string ToString ()
		{
			return this.FullName;
		}
	}
}
