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
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Signatures;

	internal sealed class MethodDefinition : MethodReference, IMethodDefinition {

		public const string Cctor = ".cctor";
		public const string Ctor = ".ctor";

		private MethodAttributes m_attributes;
		private MethodImplAttributes m_implAttrs;
		private MethodSemanticsAttributes m_semAttrs;
		private SecurityDeclarationCollection m_secDecls;
		private CustomAttributeCollection m_customAttrs;

		private ModuleDefinition m_module;

		private MethodBody m_body;
		private RVA m_rva;
		private OverrideCollection m_overrides;
		private PInvokeInfo m_pinvoke;

		public MethodAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public MethodImplAttributes ImplAttributes {
			get { return m_implAttrs; }
			set { m_implAttrs = value; }
		}

		public MethodSemanticsAttributes SemanticsAttributes {
			get { return m_semAttrs; }
			set { m_semAttrs = value; }
		}

		public override ITypeReference DeclaringType {
			get { return base.DeclaringType; }
			set {
				base.DeclaringType = value;
				TypeDefinition t = value as TypeDefinition;
				if (t != null)
					m_module = t.Mod;
			}
		}

		public ISecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					m_secDecls = new SecurityDeclarationCollection (this);

				return m_secDecls;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public RVA RVA {
			get { return m_rva; }
			set { m_rva = value; }
		}

		public IMethodBody Body {
			get {
				if (m_module != null && m_body == null && m_rva != RVA.Zero) {
					m_body = new MethodBody (this);
					m_module.Controller.Reader.Code.VisitMethodBody (m_body);
				}

				return m_body;
			}
			set { m_body = value as MethodBody; }
		}

		public IPInvokeInfo PInvokeInfo {
			get { return m_pinvoke; }
			set { m_pinvoke = value as PInvokeInfo; }
		}

		public IOverrideCollection Overrides {
			get {
				if (m_overrides == null)
					m_overrides = new OverrideCollection (this);

				return m_overrides;
			}
		}

		public bool IsAbstract {
			get { return (m_attributes & MethodAttributes.Abstract) != 0; }
			set { m_attributes |= value ? MethodAttributes.Abstract : 0; }
		}

		public bool IsFinal {
			get { return (m_attributes & MethodAttributes.Final) != 0; }
			set { m_attributes |= value ? MethodAttributes.Final : 0; }
		}

		public bool IsHideBySignature {
			get { return (m_attributes & MethodAttributes.HideBySig) != 0; }
			set { m_attributes |= value ? MethodAttributes.HideBySig : 0; }
		}

		public bool IsNewSlot {
			get { return (m_attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.NewSlot; }
			set { m_attributes |= value ? (MethodAttributes.VtableLayoutMask & MethodAttributes.NewSlot) : 0; }
		}

		public bool IsRuntimeSpecialName {
			get { return (m_attributes & MethodAttributes.RTSpecialName) != 0; }
			set { m_attributes |= value ? MethodAttributes.RTSpecialName : 0; }
		}

		public bool IsSpecialName {
			get { return (m_attributes & MethodAttributes.SpecialName) != 0; }
			set { m_attributes |= value ? MethodAttributes.SpecialName : 0; }
		}

		public bool IsStatic {
			get { return (m_attributes & MethodAttributes.Static) != 0; }
			set {
				m_attributes |= value ? MethodAttributes.Static : 0;
				this.HasThis = !value;
			}
		}

		public bool IsVirtual {
			get { return (m_attributes & MethodAttributes.Virtual) != 0; }
			set { m_attributes |= value ? MethodAttributes.Virtual : 0; }
		}

		public bool IsConstructor {
			get {
				return this.IsRuntimeSpecialName && this.IsSpecialName &&
					(this.Name == Cctor || this.Name == Ctor);
			}
		}

		public MethodDefinition (string name, RVA rva,
			MethodAttributes attrs, MethodImplAttributes implAttrs,
			bool hasThis, bool explicitThis, MethodCallingConvention callConv) :
			base (name, hasThis, explicitThis, callConv)
		{
			m_rva = rva;
			m_attributes = attrs;
			m_implAttrs = implAttrs;
		}

		public MethodDefinition (string name, MethodAttributes attrs) :
			base (name)
		{
			m_attributes = attrs;
		}

		public IMethodBody CreateBody ()
		{
			return m_body = new MethodBody (this);
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitMethodDefinition (this);
			this.Parameters.Accept (visitor);

			if (this.PInvokeInfo != null)
				this.PInvokeInfo.Accept (visitor);

			this.SecurityDeclarations.Accept (visitor);
			this.Overrides.Accept (visitor);
			this.CustomAttributes.Accept (visitor);
		}
	}
}
