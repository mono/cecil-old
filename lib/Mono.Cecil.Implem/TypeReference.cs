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

namespace Mono.Cecil.Implem {

	using System;
	using System.Reflection;

	using Mono.Cecil;
	using Mono.Cecil.Metadata;

	internal class TypeReference : ITypeReference {

		private string m_name;
		private string m_namespace;
		private bool m_fullNameDiscarded;
		private string m_fullName;
		private bool m_isValueType;
		private ITypeReference m_decType;
		private IMetadataScope m_scope;
		private MetadataToken m_token;

		private CustomAttributeCollection m_customAttrs;

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

		public virtual ITypeReference DeclaringType {
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

		public IModuleDefinition Module {
			get { return m_module; }
		}

		public ModuleDefinition Mod {
			get { return m_module; }
			set { m_module = value; }
		}

		public MetadataToken MetadataToken {
			get { return m_token; }
			set { m_token = value; }
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
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

		public TypeReference (string name, string ns, IMetadataScope scope) : this (name, ns)
		{
			m_scope = scope;
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
