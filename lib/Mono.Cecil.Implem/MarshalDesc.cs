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

	internal class MarshalDesc : IMarshalSpec {

		private NativeType m_natIntr;
		private IHasMarshalSpec m_container;

		public NativeType NativeIntrinsic {
			get { return m_natIntr; }
			set { m_natIntr = value; }
		}

		public IHasMarshalSpec Container {
			get { return m_container; }
			set { m_container = value; }
		}

		public MarshalDesc (NativeType natIntr, IHasMarshalSpec container)
		{
			m_natIntr = natIntr;
			m_container = container;
		}

		public virtual void Accept (IReflectionVisitor visitor)
		{
			visitor.Visit (this);
		}
	}

	internal sealed class ArrayMarshalDesc : MarshalDesc, IArrayDesc {

		private NativeType m_elemType;
		private int m_paramNum;
		private int m_elemMult;
		private int m_numElem;

		public NativeType ElemType {
			get { return m_elemType; }
			set { m_elemType = value; }
		}

		public int ParamNum {
			get { return m_paramNum; }
			set { m_paramNum = value; }
		}

		public int ElemMult {
			get { return m_elemMult; }
			set { m_elemMult = value; }
		}

		public int NumElem {
			get { return m_numElem; }
			set { m_numElem = value; }
		}

		public ArrayMarshalDesc (IHasMarshalSpec container) : base (NativeType.ARRAY, container)
		{
		}
	}

	internal sealed class CustomMarshalerDesc : MarshalDesc, ICustomMarshalerDesc {

		private Guid m_guid;
		private string m_unmanagedType;
		private ITypeDefinition m_managedType;
		private string m_cookie;

		public Guid Guid {
			get { return m_guid; }
			set { m_guid = value; }
		}

		public String UnmanagedType {
			get { return m_unmanagedType; }
			set { m_unmanagedType = value; }
		}

		public ITypeDefinition ManagedType {
			get { return m_managedType; }
			set { m_managedType = value; }
		}

		public String Cookie {
			get { return m_cookie; }
			set { m_cookie = value; }
		}

		public CustomMarshalerDesc (IHasMarshalSpec container) : base (NativeType.CUSTOMMARSHALER, container)
		{
		}
	}

	internal sealed class SafeArrayDesc : MarshalDesc, ISafeArrayDesc {

		private VariantType m_elemType;

		public VariantType ElemType {
			get { return m_elemType; }
			set { m_elemType = value; }
		}

		public SafeArrayDesc (IHasMarshalSpec container) : base (NativeType.SAFEARRAY, container)
		{
		}
	}

	internal sealed class FixedArrayDesc : MarshalDesc, IFixedArrayDesc {

		private int m_numElem;
		private NativeType m_elemType;

		public int NumElem {
			get { return m_numElem; }
			set { m_numElem = value; }
		}

		public NativeType ElemType {
			get { return m_elemType; }
			set { m_elemType = value; }
		}

		public FixedArrayDesc (IHasMarshalSpec container) : base (NativeType.FIXEDARRAY, container)
		{
		}
	}

	internal sealed class FixedSysStringDesc : MarshalDesc, IFixedSysStringDesc {

		private int m_size;

		public int Size {
			get { return m_size; }
			set { m_size = value; }
		}

		public FixedSysStringDesc (IHasMarshalSpec container) : base (NativeType.FIXEDSYSSTRING, container)
		{
		}
	}
}
