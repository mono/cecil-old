//
// TypeDefinition.cs
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

	public sealed class TypeDefinition : TypeReference, ITypeDefinition, IClassLayoutInfo, ICloneable {

		TypeAttributes m_attributes;
		TypeReference m_baseType;

		bool m_layoutLoaded;
		bool m_hasInfo;
		ushort m_packingSize;
		uint m_classSize;

		InterfaceCollection m_interfaces;
		NestedTypeCollection m_nestedTypes;
		MethodDefinitionCollection m_methods;
		ConstructorCollection m_ctors;
		FieldDefinitionCollection m_fields;
		EventDefinitionCollection m_events;
		PropertyDefinitionCollection m_properties;
		SecurityDeclarationCollection m_secDecls;

		public TypeAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public TypeReference BaseType {
			get { return m_baseType; }
			set { m_baseType = value; }
		}

		public IClassLayoutInfo LayoutInfo {
			get { return this; }
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

		public InterfaceCollection Interfaces {
			get {
				if (m_interfaces == null)
					m_interfaces = new InterfaceCollection (this);

				return m_interfaces;
			}
		}

		public NestedTypeCollection NestedTypes {
			get {
				if (m_nestedTypes == null) {
					m_nestedTypes = new NestedTypeCollection (this);
					m_nestedTypes.OnNestedTypeAdded += new NestedTypeEventHandler (OnNestedTypeAdded);
					m_nestedTypes.OnNestedTypeRemoved += new NestedTypeEventHandler (OnNestedTypeRemoved);
				}

				return m_nestedTypes;
			}
		}

		public MethodDefinitionCollection Methods {
			get {
				if (m_methods == null) {
					m_methods = new MethodDefinitionCollection (this);
					m_methods.OnMethodDefinitionAdded += new MethodDefinitionEventHandler (OnMethodAdded);
					m_methods.OnMethodDefinitionRemoved += new MethodDefinitionEventHandler (OnMethodRemoved);
				}

				return m_methods;
			}
		}

		public ConstructorCollection Constructors {
			get {
				if (m_ctors == null) {
					m_ctors = new ConstructorCollection (this);
					m_ctors.OnConstructorAdded += new ConstructorEventHandler (OnCtorAdded);
					m_ctors.OnConstructorRemoved += new ConstructorEventHandler (OnCtorRemoved);
				}

				return m_ctors;
			}
		}

		public FieldDefinitionCollection Fields {
			get {
				if (m_fields == null) {
					m_fields = new FieldDefinitionCollection (this);
					m_fields.OnFieldDefinitionAdded += new FieldDefinitionEventHandler (OnFieldAdded);
					m_fields.OnFieldDefinitionRemoved += new FieldDefinitionEventHandler (OnFieldRemoved);
				}

				return m_fields;
			}
		}

		public EventDefinitionCollection Events {
			get {
				if (m_events == null) {
					m_events = new EventDefinitionCollection (this);
					m_events.OnEventDefinitionAdded += new EventDefinitionEventHandler (OnEventAdded);
					m_events.OnEventDefinitionRemoved += new EventDefinitionEventHandler (OnEventRemoved);
				}

				return m_events;
			}
		}

		public PropertyDefinitionCollection Properties {
			get {
				if (m_properties == null) {
					m_properties = new PropertyDefinitionCollection (this);
					m_properties.OnPropertyDefinitionAdded += new PropertyDefinitionEventHandler (OnPropertyAdded);
					m_properties.OnPropertyDefinitionRemoved += new PropertyDefinitionEventHandler (OnPropertyRemoved);
				}

				return m_properties;
			}
		}

		public SecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
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

		internal TypeDefinition (string name, string ns, TypeAttributes attrs, ModuleDefinition module) :
			base (name, ns)
		{
			m_hasInfo = false;
			m_attributes = attrs;
			m_scope = module;
		}

		public TypeDefinition (string name, string ns,
			TypeAttributes attributes, ModuleDefinition module, TypeReference baseType) :
			this (name, ns, attributes, module)
		{
			if (baseType.Module != module)
				baseType = module.Import (baseType);

			this.BaseType = baseType;
		}

		public TypeDefinition (string name, string ns,
			TypeAttributes attributes, ModuleDefinition module, Type baseType) :
		this (name, ns, attributes, module)
		{
			this.BaseType = module.Import (baseType);
		}

		void OnMethodAdded (object sender, MethodDefinitionEventArgs ea)
		{
			AttachMember (ea.MethodDefinition);
		}

		void OnMethodRemoved (object sender, MethodDefinitionEventArgs ea)
		{
			DetachMember (ea.MethodDefinition);
		}

		void OnFieldAdded (object sender, FieldDefinitionEventArgs ea)
		{
			AttachMember (ea.FieldDefinition);
		}

		void OnFieldRemoved (object sender, FieldDefinitionEventArgs ea)
		{
			DetachMember (ea.FieldDefinition);
		}

		void OnCtorAdded (object sender, ConstructorEventArgs ea)
		{
			AttachMember (ea.Constructor);
		}

		void OnCtorRemoved (object sender, ConstructorEventArgs ea)
		{
			DetachMember (ea.Constructor);
		}

		void OnEventAdded (object sender, EventDefinitionEventArgs ea)
		{
			AttachMember (ea.EventDefinition);
		}

		void OnEventRemoved (object sender, EventDefinitionEventArgs ea)
		{
			DetachMember (ea.EventDefinition);
		}

		void OnPropertyAdded (object sender, PropertyDefinitionEventArgs ea)
		{
			AttachMember (ea.PropertyDefinition);
		}

		void OnPropertyRemoved (object sender, PropertyDefinitionEventArgs ea)
		{
			DetachMember (ea.PropertyDefinition);
		}

		void OnNestedTypeAdded (object sender, NestedTypeEventArgs ea)
		{
			AttachType (ea.NestedType);
		}

		void OnNestedTypeRemoved (object sender, NestedTypeEventArgs ea)
		{
			DetachType (ea.NestedType);
		}

		void AttachType (TypeReference t)
		{
			if (t.DeclaringType != null)
				throw new ReflectionException ("Member already attached, clone it instead");

			t.DeclaringType = this;
		}

		void DetachType (TypeReference t)
		{
			t.DeclaringType = null;
		}

		void AttachMember (MemberReference member)
		{
			if (member.DeclaringType != null)
				throw new ReflectionException ("Member already attached, clone it instead");

			member.DeclaringType = this;
		}

		void DetachMember (MemberReference member)
		{
			member.DeclaringType = null;
		}

		object ICloneable.Clone ()
		{
			return this.Clone ();
		}

		public TypeDefinition Clone ()
		{
			return Clone (this, null);
		}

		internal static TypeDefinition Clone (TypeDefinition type, ReflectionHelper helper)
		{
			TypeDefinition nt = new TypeDefinition (
				type.Name,
				type.Namespace,
				type.Attributes,
				type.Module);

			nt.BaseType = helper == null ? type.BaseType : helper.ImportTypeReference (type.BaseType);

			if (type.LayoutInfo.HasLayoutInfo) {
				nt.LayoutInfo.ClassSize = type.LayoutInfo.ClassSize;
				nt.LayoutInfo.PackingSize = type.LayoutInfo.PackingSize;
			}

			foreach (FieldDefinition field in type.Fields)
				nt.Fields.Add (FieldDefinition.Clone (field, helper));
			foreach (MethodDefinition ctor in type.Constructors)
				nt.Constructors.Add (MethodDefinition.Clone (ctor, helper));
			foreach (MethodDefinition meth in type.Methods)
				nt.Methods.Add (MethodDefinition.Clone (meth, helper));
			foreach (EventDefinition evt in type.Events)
				nt.Events.Add (EventDefinition.Clone (evt, helper));
			foreach (PropertyDefinition prop in type.Properties)
				nt.Properties.Add (PropertyDefinition.Clone (prop, helper));
			foreach (TypeReference intf in type.Interfaces)
				nt.Interfaces.Add (helper == null ? intf : helper.ImportTypeReference (intf));
			foreach (TypeDefinition nested in type.NestedTypes)
				nt.NestedTypes.Add (Clone (nested, helper));
			foreach (CustomAttribute ca in type.CustomAttributes)
				nt.CustomAttributes.Add (CustomAttribute.Clone (ca, helper));
			foreach (SecurityDeclaration dec in type.SecurityDeclarations)
				nt.SecurityDeclarations.Add (SecurityDeclaration.Clone (dec));

			return nt;
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitTypeDefinition (this);

			this.GenericParameters.Accept (visitor);
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
