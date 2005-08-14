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

		private MethodAttributes m_attributes;
		private MethodImplAttributes m_implAttrs;
		private MethodSemanticsAttributes m_semAttrs;
		private SecurityDeclarationCollection m_secDecls;
		private CustomAttributeCollection m_customAttrs;

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

		public ISecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					m_secDecls = new SecurityDeclarationCollection (
						this, (this.DeclaringType as TypeDefinition).Module.Controller);
				return m_secDecls;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (
						this, (this.DeclaringType as TypeDefinition).Module.Controller);
				return m_customAttrs;
			}
		}

		public RVA RVA {
			get { return m_rva; }
			set { m_rva = value; }
		}

		public IMethodBody Body {
			get {
				if (m_body == null && m_rva != RVA.Zero) {
					m_body = new MethodBody (this);
					((TypeDefinition)this.DeclaringType).Module.Controller.Reader.Code.Visit (m_body);
				}
				return m_body;
			}
			set { m_body = value as MethodBody; }
		}

		public IPInvokeInfo PInvokeInfo {
			get {
				if (m_pinvoke == null && (m_attributes & MethodAttributes.PInvokeImpl) != 0) {
					m_pinvoke = new PInvokeInfo (this);
					((TypeDefinition)this.DeclaringType).Module.Controller.Reader.Visit (m_pinvoke);
				}
				return m_pinvoke;
			}
			set { m_pinvoke = value as PInvokeInfo; }
		}

		public IOverrideCollection Overrides {
			get {
				if (m_overrides == null)
					m_overrides = new OverrideCollection (
						this, (this.DeclaringType as TypeDefinition).Module.Controller);
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

		public MethodDefinition (string name, TypeDefinition decType, RVA rva,
			MethodAttributes attrs, MethodImplAttributes implAttrs,
			bool hasThis, bool explicitThis, MethodCallingConvention callConv) :
			base (name, decType, hasThis, explicitThis, callConv)
		{
			m_rva = rva;
			m_attributes = attrs;
			m_implAttrs = implAttrs;
		}

		public MethodDefinition (string name, TypeDefinition decType, MethodAttributes attrs) : base (name, decType)
		{
			m_attributes = attrs;
		}

		public void DefineOverride (IMethodReference meth)
		{
			(this.Overrides as OverrideCollection).Add (meth);
		}

		public void DefineOverride (System.Reflection.MethodInfo meth)
		{
			DefineOverride (
				(this.DeclaringType as TypeDefinition).Module.Controller.Helper.RegisterMethod (meth));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
		{
			CustomAttribute ca = new CustomAttribute(ctor);
			(this.CustomAttributes as CustomAttributeCollection).Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
		{
			return DefineCustomAttribute (
				(this.DeclaringType as TypeDefinition).Module.Controller.Helper.RegisterConstructor(ctor));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor, byte [] data)
		{
			CustomAttribute ca =
				(this.DeclaringType as TypeDefinition).Module.Controller.Reader.GetCustomAttribute (ctor, data);
			(this.CustomAttributes as CustomAttributeCollection).Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor, byte [] data)
		{
			return DefineCustomAttribute (
				(this.DeclaringType as TypeDefinition).Module.Controller.Helper.RegisterConstructor(ctor), data);
		}

		public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action)
		{
			SecurityDeclaration dec = new SecurityDeclaration (action);
			(this.SecurityDeclarations as SecurityDeclarationCollection).Add (dec);
			return dec;
		}

		public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action, byte [] declaration)
		{
			SecurityDeclaration dec =
				(this.DeclaringType as TypeDefinition).Module.Controller.Reader.BuildSecurityDeclaration (action, declaration);
			(this.SecurityDeclarations as SecurityDeclarationCollection).Add (dec);
			return dec;
		}

		public void DefinePInvokeInfo (string ep, string module, PInvokeAttributes attrs)
		{
			this.PInvokeInfo.EntryPoint = ep;
			this.PInvokeInfo.Attributes = attrs;

			foreach (IModuleReference modref in (this.DeclaringType as TypeDefinition).Module.ModuleReferences) {
				if (modref.Name == module) {
					this.PInvokeInfo.Module = modref;
					return;
				}
			}

			this.PInvokeInfo.Module	 =
				(this.DeclaringType as TypeDefinition).Module.DefineModuleReference (module);
		}

		public IParameterDefinition DefineParameter (string name, ParamAttributes attributes, ITypeReference type)
		{
			ParameterDefinition param = new ParameterDefinition (
				name, this.Parameters.Count == 0 ? 1 : this.Parameters.Count + 1,
				attributes, type);

			(this.Parameters as ParameterDefinitionCollection).Add (param);
			return param;
		}

		public IParameterDefinition DefineParameter (string name, ParamAttributes attributes, Type type)
		{
			return DefineParameter (name, attributes,
				(this.DeclaringType as TypeDefinition).Module.Controller.Helper.RegisterType (type));
		}

		public IMethodBody DefineBody ()
		{
			return m_body = new MethodBody (this);
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.Visit (this);
			this.Parameters.Accept (visitor);

			if (this.PInvokeInfo != null)
				this.PInvokeInfo.Accept (visitor);

			this.SecurityDeclarations.Accept (visitor);
			this.Overrides.Accept (visitor);
			this.CustomAttributes.Accept (visitor);
		}
	}
}
