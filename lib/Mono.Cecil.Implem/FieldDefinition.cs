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

		private bool m_hasInfo;
		private uint m_offset;

		private RVA m_rva;
		private byte [] m_initVal;

		private bool m_hasConstant;
		private object m_const;

		private MarshalDesc m_marshalDesc;

		public IFieldLayoutInfo LayoutInfo {
			get { return this; }
		}

		public bool HasLayoutInfo {
			get { return m_hasInfo; }
		}

		public uint Offset {
			get { return m_offset; }
			set {
				m_hasInfo = true;
				m_offset = value;
			}
		}

		public RVA RVA {
			get { return m_rva; }
			set { m_rva = value; }
		}

		public byte [] InitialValue {
			get { return m_initVal; }
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

		public bool HasConstant {
			get { return m_hasConstant; }
		}

		public object Constant {
			get { return m_const; }
			set {
				m_hasConstant = true;
				m_const = value;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public IMarshalSpec MarshalSpec {
			get { return m_marshalDesc; }
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

		public FieldDefinition (string name, ITypeReference fieldType,
			FieldAttributes attrs) : base (name)
		{
			m_hasInfo = false;
			m_fieldType = fieldType;
			m_attributes = attrs;
		}

		public override string ToString ()
		{
			if (this.DeclaringType == null)
				return string.Concat (m_fieldType.ToString (), ' ', this.Name);

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
