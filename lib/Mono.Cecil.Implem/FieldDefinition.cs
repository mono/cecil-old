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

	using Mono.Cecil;
	using Mono.Cecil.Binary;

	internal sealed class FieldDefinition : MemberDefinition, IFieldDefinition, IFieldLayoutInfo {

		private ITypeReference m_fieldType;
		private FieldAttributes m_attributes;

		private CustomAttributeCollection m_customAttrs;

		private bool m_layoutLoaded;
		private bool m_hasInfo;
		private uint m_offset;

		private bool m_initValLoaded;
		private RVA m_rva;
		private byte [] m_initVal;

		private bool m_constLoaded;
		private bool m_hasConstant;
		private object m_const;

		private bool m_marshalLoaded;
		private MarshalDesc m_marshalDesc;

		public IFieldLayoutInfo LayoutInfo {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_layoutLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadLayout (this);

				return this;
			}
		}

		public bool LayoutLoaded {
			get { return m_layoutLoaded; }
			set { m_layoutLoaded = value; }
		}

		public bool HasLayoutInfo {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_layoutLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadLayout (this);

				return m_hasInfo;
			}
		}

		public uint Offset {
			get { return m_offset; }
			set {
				m_hasInfo = true;
				m_offset = value;
			}
		}

		public bool InitialValueLoaded {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_initValLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadInitialValue (this);

				return m_initValLoaded;
			}
			set { m_initValLoaded = value; }
		}

		public RVA RVA {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_initValLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadInitialValue (this);

				return m_rva;
			}
			set { m_rva = value; }
		}

		public byte [] InitialValue {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_initValLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadInitialValue (this);

				return m_initVal;
			}
			set { m_initVal = value; }
		}

		public ITypeReference FieldType {
			get { return m_fieldType; }
			set { m_fieldType = value; }
		}

		public FieldAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public bool ConstantLoaded {
			get { return m_constLoaded; }
			set { m_constLoaded = value; }
		}

		public bool HasConstant {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_constLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadConstant (this);

				return m_hasConstant;
			}
		}

		public object Constant {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_constLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadConstant (this);

				return m_const;
			}
			set {
				m_hasConstant = true;
				m_const = value;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this, this.DecTypeDef.Module.Controller);

				if (!this.DecTypeDef.Module.IsNew && !m_customAttrs.Loaded)
					m_customAttrs.Load ();

				return m_customAttrs;
			}
		}

		public bool MarshalSpecLoaded {
			get { return m_marshalLoaded; }
			set { m_marshalLoaded = value; }
		}

		public IMarshalSpec MarshalSpec {
			get {
				if (!this.DecTypeDef.Module.IsNew && !m_marshalLoaded)
					this.DecTypeDef.Module.Controller.Reader.ReadMarshalSpec (this);

				return m_marshalDesc;
			}
			set { m_marshalDesc = value as MarshalDesc; }
		}

		public bool IsLiteral {
			get { return (m_attributes & FieldAttributes.Literal) != 0; }
			set { m_attributes |= value ? FieldAttributes.Literal : 0; }
		}

		public bool IsReadOnly {
			get { return (m_attributes & FieldAttributes.InitOnly) != 0; }
			set { m_attributes |= value ? FieldAttributes.InitOnly : 0; }
		}

		public bool IsRuntimeSpecialName {
			get { return (m_attributes & FieldAttributes.RTSpecialName) != 0; }
			set { m_attributes |= value ? FieldAttributes.RTSpecialName : 0; }
		}

		public bool IsSpecialName {
			get { return (m_attributes & FieldAttributes.SpecialName) != 0; }
			set { m_attributes |= value ? FieldAttributes.SpecialName : 0; }
		}

		public bool IsStatic {
			get { return (m_attributes & FieldAttributes.Static) != 0; }
			set { m_attributes |= value ? FieldAttributes.Static : 0; }
		}

		public FieldDefinition (string name, TypeDefinition decType, ITypeReference fieldType, FieldAttributes attrs) : base (name, decType)
		{
			m_hasInfo = false;
			m_fieldType = fieldType;
			m_attributes = attrs;
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
		{
			CustomAttribute ca = new CustomAttribute(ctor);
			this.CustomAttributes.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
		{
			return DefineCustomAttribute (this.DecTypeDef.Module.Controller.Helper.RegisterConstructor(ctor));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor, byte [] data)
		{
			CustomAttribute ca = this.DecTypeDef.Module.Controller.Reader.GetCustomAttribute (ctor, data);
			this.CustomAttributes.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor, byte [] data)
		{
			return DefineCustomAttribute (
				this.DecTypeDef.Module.Controller.Helper.RegisterConstructor(ctor), data);
		}

		public override string ToString ()
		{
			return string.Concat (m_fieldType.ToString (), ' ',
				this.DeclaringType.ToString (), "::", this.Name);
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitFieldDefinition (this);

			if (this.MarshalSpec != null)
				this.MarshalSpec.Accept (visitor);

			this.CustomAttributes.Accept (visitor);
		}
	}
}
