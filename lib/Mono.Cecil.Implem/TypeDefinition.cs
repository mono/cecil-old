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
				this.Module.Controller.Reader.ReadLayout (this);
				return this;
			}
		}

		public bool LayoutLoaded {
			get { return m_layoutLoaded; }
			set { m_layoutLoaded = value; }
		}

		public bool HasLayoutInfo {
			get { return m_hasInfo; }
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

		public IInterfaceCollection Interfaces {
			get {
				if (m_interfaces == null)
					m_interfaces = new InterfaceCollection (this, m_module.Controller);
				return m_interfaces;
			}
		}

		public INestedTypeCollection NestedTypes {
			get {
				if (m_nestedTypes == null)
					m_nestedTypes = new NestedTypeCollection (this);
				return m_nestedTypes;
			}
		}

		public IMethodDefinitionCollection Methods {
			get {
				if (m_methods == null)
					m_methods = new MethodDefinitionCollection (this, m_module.Controller);
				return m_methods;
			}
		}

		public IFieldDefinitionCollection Fields {
			get {
				if (m_fields == null)
					m_fields = new FieldDefinitionCollection (this, m_module.Controller);
				return m_fields;
			}
		}

		public IEventDefinitionCollection Events {
			get {
				if (m_events == null)
					m_events = new EventDefinitionCollection (this, m_module.Controller);
				return m_events;
			}
		}

		public IPropertyDefinitionCollection Properties {
			get {
				if (m_properties == null)
					m_properties = new PropertyDefinitionCollection (this, m_module.Controller);
				return m_properties;
			}
		}

		public ISecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					m_secDecls = new SecurityDeclarationCollection (this, m_module.Controller);
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

		public TypeDefinition (string name, string ns, TypeAttributes attrs, ModuleDefinition module) :
			base (name, ns, module, module.Assembly.Name)
		{
			m_hasInfo = false;
			m_attributes = attrs;
		}

		public ITypeDefinition DefineNestedType (string name, TypeAttributes attributes)
		{
			return DefineNestedType (name, attributes, typeof (object));
		}

		public ITypeDefinition DefineNestedType (string name, TypeAttributes attributes, ITypeReference baseType)
		{
			TypeDefinition type = new TypeDefinition (name, string.Empty, attributes, m_module);
			type.BaseType = baseType;
			type.DeclaringType = this;
			(this.NestedTypes as NestedTypeCollection) [name] = type;
			(m_module.Types as TypeDefinitionCollection) [type.FullName] = type;
			return type;
		}

		public ITypeDefinition DefineNestedType (string name, TypeAttributes attributes, Type baseType)
		{
			return DefineNestedType (name, attributes, m_module.Controller.Helper.RegisterType (baseType));
		}

		public IMethodDefinition DefineMethod (string name, MethodAttributes attributes)
		{
			MethodDefinition meth = new MethodDefinition (name, this, attributes);
			m_methods.Add (meth);
			return meth;
		}

		public IMethodDefinition DefineConstructor ()
		{
			return DefineConstructor (false);
		}

		public IMethodDefinition DefineConstructor (bool isstatic)
		{
			string name = null;
			MethodAttributes attrs = MethodAttributes.SpecialName;
			if (isstatic) {
				name = ".cctor";
				attrs |= MethodAttributes.Static;
			} else
				name = ".ctor";
			MethodDefinition meth = new MethodDefinition (name, this, attrs);

			(this.Methods as MethodDefinitionCollection).Add (meth);
			return meth;
		}

		public IFieldDefinition DefineField (string name, FieldAttributes attributes, ITypeReference fieldType)
		{
			FieldDefinition field = new FieldDefinition (name, this, fieldType, attributes);
			return field;
		}

		public IFieldDefinition DefineField (string name, FieldAttributes attributes, Type fieldType)
		{
			return DefineField (name, attributes, this.Module.Controller.Helper.RegisterType (fieldType));
		}

		public IEventDefinition DefineEvent (string name, EventAttributes attributes, ITypeReference eventType)
		{
			EventDefinition evt = new EventDefinition (name, this, eventType, attributes);
			(this.Events as EventDefinitionCollection) [name] = evt;
			return evt;
		}

		public IEventDefinition DefineEvent (string name, EventAttributes attributes, Type eventType)
		{
			return DefineEvent (name, attributes, this.Module.Controller.Helper.RegisterType (eventType));
		}

		public IPropertyDefinition DefineProperty (string name, PropertyAttributes attributes, ITypeReference propType)
		{
			PropertyDefinition prop = new PropertyDefinition (name, this, propType, attributes);
			(this.Properties as PropertyDefinitionCollection) [name] = prop;
			return prop;
		}

		public IPropertyDefinition DefineProperty (string name, PropertyAttributes attributes, Type propType)
		{
			return DefineProperty (name, attributes, this.Module.Controller.Helper.RegisterType (propType));
		}

		public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action)
		{
			SecurityDeclaration dec = new SecurityDeclaration (action);
			(this.SecurityDeclarations as SecurityDeclarationCollection).Add (dec);
			return dec;
		}

		public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action, byte [] declaration)
		{
			SecurityDeclaration dec = this.Module.Controller.Reader.BuildSecurityDeclaration (action, declaration);
			(this.SecurityDeclarations as SecurityDeclarationCollection).Add (dec);
			return dec;
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.Visit (this);

			m_interfaces.Accept (visitor);
			m_fields.Accept (visitor);
			m_properties.Accept (visitor);
			m_events.Accept (visitor);
			m_methods.Accept (visitor);
		}
	}
}
