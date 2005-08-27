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

	using Mono.Cecil;

	internal sealed class TypeDefinition : TypeReference, ITypeDefinition, IClassLayoutInfo {

		private TypeAttributes m_attributes;
		private ITypeReference m_baseType;

		private bool m_layoutLoaded;
		private bool m_hasInfo;
		private ushort m_packingSize;
		private uint m_classSize;

		private InterfaceCollection m_interfaces;
		private NestedTypeCollection m_nestedTypes;
		private MethodDefinitionCollection m_methods;
		private ConstructorCollection m_ctors;
		private FieldDefinitionCollection m_fields;
		private EventDefinitionCollection m_events;
		private PropertyDefinitionCollection m_properties;
		private SecurityDeclarationCollection m_secDecls;

		public TypeAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public ITypeReference BaseType {
			get { return m_baseType; }
			set { m_baseType = value; }
		}

		public IClassLayoutInfo LayoutInfo {
			get {
				if (IsLazyLoadable && !m_layoutLoaded)
					m_module.Controller.Reader.ReadLayout (this);

				return this;
			}
		}

		public bool LayoutLoaded {
			get { return m_layoutLoaded; }
			set { m_layoutLoaded = value; }
		}

		public bool HasLayoutInfo {
			get {
				if (IsLazyLoadable && !m_layoutLoaded)
					m_module.Controller.Reader.ReadLayout (this);

				return m_hasInfo;
			}
		}

		public ushort PackingSize {
			get { return m_packingSize; }
			set {
				m_hasInfo = true;
				m_packingSize = value;
			}
		}

		public uint ClassSize {
			get { return m_classSize; }
			set {
				m_hasInfo = true;
				m_classSize = value;
			}
		}

		public bool IsLazyLoadable {
			get { return m_module != null && !m_module.IsNew; }
		}

		public IInterfaceCollection Interfaces {
			get {
				if (m_interfaces == null) {
					if (this.IsLazyLoadable) {
						m_interfaces = new InterfaceCollection (this, m_module.Controller);
						m_interfaces.Load ();
					} else
						m_interfaces = new InterfaceCollection (this);
				}

				return m_interfaces;
			}
		}

		public INestedTypeCollection NestedTypes {
			get {
				if (m_nestedTypes == null) {
					m_nestedTypes = new NestedTypeCollection (this);
				}

				return m_nestedTypes;
			}
		}

		public IMethodDefinitionCollection Methods {
			get {
				if (m_methods == null) {
					if (this.IsLazyLoadable) {
						m_methods = new MethodDefinitionCollection (this, m_module.Controller);
						m_methods.Load ();
					} else
						m_methods = new MethodDefinitionCollection (this);

					m_methods.OnMethodDefinitionAdded += new MethodDefinitionEventHandler (OnMethodAdded);
					m_methods.OnMethodDefinitionRemoved += new MethodDefinitionEventHandler (OnMethodRemoved);
				}

				return m_methods;
			}
		}

		public IConstructorCollection Constructors {
			get {
				if (m_ctors == null) {
					if (this.IsLazyLoadable) {
						m_ctors = new ConstructorCollection (this, m_module.Controller);
						m_ctors.Load ();
					} else
						m_ctors = new ConstructorCollection (this);

					m_ctors.OnConstructorAdded += new ConstructorEventHandler (OnCtorAdded);
					m_ctors.OnConstructorRemoved += new ConstructorEventHandler (OnCtorRemoved);
				}

				return m_ctors;
			}
		}

		public IFieldDefinitionCollection Fields {
			get {
				if (m_fields == null) {
					if (this.IsLazyLoadable) {
						m_fields = new FieldDefinitionCollection (this, m_module.Controller);
						m_fields.Load ();
					} else
						m_fields = new FieldDefinitionCollection (this);

					m_fields.OnFieldDefinitionAdded += new FieldDefinitionEventHandler (OnFieldAdded);
					m_fields.OnFieldDefinitionRemoved += new FieldDefinitionEventHandler (OnFieldRemoved);
				}

				return m_fields;
			}
		}

		public IEventDefinitionCollection Events {
			get {
				if (m_events == null) {
					if (this.IsLazyLoadable) {
						m_events = new EventDefinitionCollection (this, m_module.Controller);
						m_events.Load ();
					} else
						m_events = new EventDefinitionCollection (this);

					m_events.OnEventDefinitionAdded += new EventDefinitionEventHandler (OnEventAdded);
					m_events.OnEventDefinitionRemoved += new EventDefinitionEventHandler (OnEventRemoved);
				}

				return m_events;
			}
		}

		public IPropertyDefinitionCollection Properties {
			get {
				if (m_properties == null) {
					if (this.IsLazyLoadable) {
						m_properties = new PropertyDefinitionCollection (this, m_module.Controller);
						m_properties.Load ();
					} else
						m_properties = new PropertyDefinitionCollection (this);

					m_properties.OnPropertyDefinitionAdded += new PropertyDefinitionEventHandler (OnPropertyAdded);
					m_properties.OnPropertyDefinitionRemoved += new PropertyDefinitionEventHandler (OnPropertyRemoved);
				}

				return m_properties;
			}
		}

		public ISecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					if (this.IsLazyLoadable) {
						m_secDecls = new SecurityDeclarationCollection (this, m_module.Controller);
						m_secDecls.Load ();
					} else
						m_secDecls = new SecurityDeclarationCollection (this);

				return m_secDecls;
			}
		}

		public bool IsAbstract {
			get { return (m_attributes & TypeAttributes.Abstract) != 0; }
			set { m_attributes |= value ? TypeAttributes.Abstract : 0; }
		}

		public bool IsBeforeFieldInit {
			get { return (m_attributes & TypeAttributes.BeforeFieldInit) != 0; }
			set { m_attributes |= value ? TypeAttributes.BeforeFieldInit : 0; }
		}

		public bool IsInterface {
			get { return (m_attributes & TypeAttributes.ClassSemanticMask) == TypeAttributes.Interface; }
			set { m_attributes |= value ? (TypeAttributes.ClassSemanticMask & TypeAttributes.Interface) : 0; }
		}

		public bool IsRuntimeSpecialName {
			get { return (m_attributes & TypeAttributes.RTSpecialName) != 0; }
			set { m_attributes |= value ? TypeAttributes.RTSpecialName : 0; }
		}

		public bool IsSealed {
			get { return (m_attributes & TypeAttributes.Sealed) != 0; }
			set { m_attributes |= value ? TypeAttributes.Sealed : 0; }
		}

		public bool IsSpecialName {
			get { return (m_attributes & TypeAttributes.SpecialName) != 0; }
			set { m_attributes |= value ? TypeAttributes.SpecialName : 0; }
		}

		public bool IsEnum {
			get { return m_baseType != null && m_baseType.FullName == Constants.Enum; }
		}

		public override bool IsValueType {
			get {
				return m_baseType != null && (
					this.IsEnum || m_baseType.FullName == Constants.ValueType);
			}
		}

		public TypeDefinition (string name, string ns, TypeAttributes attrs) :
			base (name, ns)
		{
			m_hasInfo = false;
			m_attributes = attrs;
		}

		private void OnMethodAdded (object sender, MethodDefinitionEventArgs ea)
		{
			AttachMember (ea.MethodDefinition as MemberReference);
		}

		private void OnMethodRemoved (object sender, MethodDefinitionEventArgs ea)
		{
			DetachMember (ea.MethodDefinition as MemberReference);
		}

		private void OnFieldAdded (object sender, FieldDefinitionEventArgs ea)
		{
			AttachMember (ea.FieldDefinition as MemberReference);
		}

		private void OnFieldRemoved (object sender, FieldDefinitionEventArgs ea)
		{
			DetachMember (ea.FieldDefinition as MemberReference);
		}

		private void OnCtorAdded (object sender, ConstructorEventArgs ea)
		{
			AttachMember (ea.Constructor as MemberReference);
		}

		private void OnCtorRemoved (object sender, ConstructorEventArgs ea)
		{
			DetachMember (ea.Constructor as MemberReference);
		}

		private void OnEventAdded (object sender, EventDefinitionEventArgs ea)
		{
			AttachMember (ea.EventDefinition as MemberReference);
		}

		private void OnEventRemoved (object sender, EventDefinitionEventArgs ea)
		{
			DetachMember (ea.EventDefinition as MemberReference);
		}

		private void OnPropertyAdded (object sender, PropertyDefinitionEventArgs ea)
		{
			AttachMember (ea.PropertyDefinition as MemberReference);
		}

		private void OnPropertyRemoved (object sender, PropertyDefinitionEventArgs ea)
		{
			DetachMember (ea.PropertyDefinition as MemberReference);
		}

		private void AttachMember (MemberReference member)
		{
			if (member.DeclaringType != null)
				throw new ReflectionException ("Member already attached, clone it instead");

			member.DeclaringType = this;
		}

		private void DetachMember (MemberReference member)
		{
			member.DeclaringType = null;
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitTypeDefinition (this);

			this.Interfaces.Accept (visitor);
			this.Constructors.Accept (visitor);
			this.Methods.Accept (visitor);
			this.Fields.Accept (visitor);
			this.Properties.Accept (visitor);
			this.Events.Accept (visitor);
			this.NestedTypes.Accept (visitor);
			this.CustomAttributes.Accept (visitor);
			this.SecurityDeclarations.Accept (visitor);
		}
	}
}
