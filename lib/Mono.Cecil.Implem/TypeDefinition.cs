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

		private const string m_cctor = ".cctor";
		private const string m_ctor = ".ctor";

		private TypeAttributes m_attributes;
		private ITypeReference m_baseType;

		private bool m_layoutLoaded;
		private bool m_hasInfo;
		private ushort m_packingSize;
		private uint m_classSize;

		private InterfaceCollection m_interfaces;
		private NestedTypeCollection m_nestedTypes;
		private MethodDefinitionCollection m_methods;
		private MethodDefinitionCollection m_ctors;
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
				if (!m_module.IsNew && !m_layoutLoaded)
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
				if (!m_module.IsNew && !m_layoutLoaded)
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

		public IInterfaceCollection Interfaces {
			get {
				if (m_interfaces == null)
					m_interfaces = new InterfaceCollection (this, m_module.Controller);

				if (!m_module.IsNew && !m_interfaces.Loaded)
					m_interfaces.Load ();

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

				if (!m_module.IsNew && !m_methods.Loaded)
					m_methods.Load ();

				return m_methods;
			}
		}

		public IMethodDefinitionCollection Constructors {
			get {
				if (m_ctors  == null) {
					m_ctors = new MethodDefinitionCollection (this);
					foreach (MethodDefinition meth in this.Methods)
						if ((meth.Name == m_cctor || meth.Name == m_ctor) && meth.IsSpecialName)
							m_ctors.Add (meth);
				}

				return m_ctors;
			}
		}

		public IFieldDefinitionCollection Fields {
			get {
				if (m_fields == null)
					m_fields = new FieldDefinitionCollection (this, m_module.Controller);

				if (!m_module.IsNew && !m_fields.Loaded)
					m_fields.Load ();

				return m_fields;
			}
		}

		public IEventDefinitionCollection Events {
			get {
				if (m_events == null)
					m_events = new EventDefinitionCollection (this, m_module.Controller);

				if (!m_module.IsNew && !m_events.Loaded)
					m_events.Load ();

				return m_events;
			}
		}

		public IPropertyDefinitionCollection Properties {
			get {
				if (m_properties == null)
					m_properties = new PropertyDefinitionCollection (this, m_module.Controller);

				if (!m_module.IsNew && !m_properties.Loaded)
					m_properties.Load ();

				return m_properties;
			}
		}

		public ISecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					m_secDecls = new SecurityDeclarationCollection (this, m_module.Controller);

				if (!m_module.IsNew && !m_secDecls.Loaded)
					m_secDecls.Load ();

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
			get { return m_baseType != null &&
					m_module.Controller.Writer.IsCoreType (m_baseType, Constants.Enum); }
		}

		public bool IsValueType {
			get {
				return m_baseType != null && (
					this.IsEnum || m_module.Controller.Writer.IsCoreType (
						m_baseType, Constants.ValueType));
			}
		}

		public TypeDefinition (string name, string ns, TypeAttributes attrs, ModuleDefinition module) :
			base (name, ns, module, module.Assembly.Name)
		{
			m_hasInfo = false;
			m_attributes = attrs;
		}

		public void DefineInterface (ITypeReference type)
		{
			this.Interfaces.Add (type);
		}

		public void DefineInterface (Type type)
		{
			if (!type.IsInterface)
				throw new ArgumentException ("The type is not an interface");
			DefineInterface (m_module.Controller.Helper.RegisterType (type));
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
			this.NestedTypes.Add (type);
			m_module.Types.Add (type);
			return type;
		}

		public ITypeDefinition DefineNestedType (string name, TypeAttributes attributes, Type baseType)
		{
			return DefineNestedType (name, attributes, m_module.Controller.Helper.RegisterType (baseType));
		}

		public IMethodDefinition DefineMethod (string name, MethodAttributes attributes, ITypeReference retType)
		{
			MethodDefinition meth = new MethodDefinition (name, this, attributes);
			meth.HasThis = !meth.IsStatic;
			meth.ReturnType.ReturnType = retType;
			this.Methods.Add (meth);
			return meth;
		}

		public IMethodDefinition DefineMethod (string name, MethodAttributes attributes, Type retType)
		{
			return DefineMethod (name, attributes, m_module.Controller.Helper.RegisterType (retType));
		}

		public IMethodDefinition DefineConstructor ()
		{
			return DefineConstructor (false);
		}

		public IMethodDefinition DefineConstructor (bool isstatic)
		{
			string name = null;
			MethodAttributes attrs = MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
			if (isstatic) {
				name = m_cctor;
				attrs |= MethodAttributes.Static;
			} else
				name = m_ctor;

			MethodDefinition meth = new MethodDefinition (name, this, attrs);
			meth.HasThis = !isstatic;
			meth.ReturnType.ReturnType = m_module.Controller.Writer.GetCoreType (Constants.Void);

			this.Methods.Add (meth);
			this.Constructors.Add (meth);

			return meth;
		}

		public IFieldDefinition DefineField (string name, FieldAttributes attributes, ITypeReference fieldType)
		{
			FieldDefinition field = new FieldDefinition (name, this, fieldType, attributes);
			field.ConstantLoaded = field.LayoutLoaded = field.MarshalSpecLoaded = true;
			this.Fields.Add (field);
			return field;
		}

		public IFieldDefinition DefineField (string name, FieldAttributes attributes, Type fieldType)
		{
			return DefineField (name, attributes, this.Module.Controller.Helper.RegisterType (fieldType));
		}

		public IEventDefinition DefineEvent (string name, EventAttributes attributes, ITypeReference eventType)
		{
			EventDefinition evt = new EventDefinition (name, this, eventType, attributes);
			this.Events.Add (evt);
			return evt;
		}

		public IEventDefinition DefineEvent (string name, EventAttributes attributes, Type eventType)
		{
			return DefineEvent (name, attributes, this.Module.Controller.Helper.RegisterType (eventType));
		}

		public IPropertyDefinition DefineProperty (string name, PropertyAttributes attributes, ITypeReference propType)
		{
			PropertyDefinition prop = new PropertyDefinition (name, this, propType, attributes);
			this.Properties.Add (prop);
			return prop;
		}

		public IPropertyDefinition DefineProperty (string name, PropertyAttributes attributes, Type propType)
		{
			return DefineProperty (name, attributes, this.Module.Controller.Helper.RegisterType (propType));
		}

		public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action)
		{
			SecurityDeclaration dec = new SecurityDeclaration (action);
			this.SecurityDeclarations.Add (dec);
			return dec;
		}

		public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action, byte [] declaration)
		{
			SecurityDeclaration dec = this.Module.Controller.Reader.BuildSecurityDeclaration (action, declaration);
			this.SecurityDeclarations.Add (dec);
			return dec;
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitTypeDefinition (this);

			this.CustomAttributes.Accept (visitor);
			this.SecurityDeclarations.Accept (visitor);
			this.Interfaces.Accept (visitor);
			this.Fields.Accept (visitor);
			this.Methods.Accept (visitor);
			this.Properties.Accept (visitor);
			this.Events.Accept (visitor);
			this.NestedTypes.Accept (visitor);
		}
	}
}
