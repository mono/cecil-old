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

		public ModuleDefinition Module {
			get { return m_module; }
		}

		public MetadataToken MetadataToken {
			get { return m_token; }
			set { m_token = value; }
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null && m_module != null)
					m_customAttrs = new CustomAttributeCollection (this, m_module.Controller);
				else if (m_customAttrs == null && m_module == null)
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

		public TypeReference (string name, string ns)
		{
			m_name = name;
			m_namespace = ns;
			m_fullNameDiscarded = false;
		}

		public TypeReference (string name, string ns, ModuleDefinition module, IMetadataScope scope) : this (name, ns)
		{
			m_module = module;
			m_scope = scope;
		}

		public override bool Equals (object obj)
		{
			TypeDefinition td = obj as TypeDefinition;
			if (td != null)
				return td.FullName == this.FullName;

			return false;
		}

		public override int GetHashCode()
		{
			return this.FullName.GetHashCode ();
		}

		public virtual ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
		{
			CustomAttribute ca = new CustomAttribute(ctor);
			m_customAttrs.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
		{
			return DefineCustomAttribute (m_module.Controller.Helper.RegisterConstructor(ctor));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor, byte [] data)
		{
			CustomAttribute ca = m_module.Controller.Reader.GetCustomAttribute (ctor, data);
			m_customAttrs.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor, byte [] data)
		{
			return DefineCustomAttribute (
				m_module.Controller.Helper.RegisterConstructor(ctor), data);
		}

		public virtual void Accept (IReflectionVisitor visitor)
		{
			visitor.Visit (this);
		}

		public override string ToString ()
		{
			return this.FullName;
		}
	}
}
