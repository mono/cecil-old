//
// MethodDefinition.cs
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

	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Signatures;

	public sealed class MethodDefinition : MethodReference, IMethodDefinition, ICloneable {

		public const string Cctor = ".cctor";
		public const string Ctor = ".ctor";

		MethodAttributes m_attributes;
		MethodImplAttributes m_implAttrs;
		MethodSemanticsAttributes m_semAttrs;
		SecurityDeclarationCollection m_secDecls;
		CustomAttributeCollection m_customAttrs;
		GenericParameterCollection m_genparams;

		ModuleDefinition m_module;

		MethodBody m_body;
		RVA m_rva;
		OverrideCollection m_overrides;
		PInvokeInfo m_pinvoke;

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

		public override TypeReference DeclaringType {
			get { return base.DeclaringType; }
			set {
				base.DeclaringType = value;
				TypeDefinition t = value as TypeDefinition;
				if (t != null)
					m_module = t.Module;
			}
		}

		public SecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					m_secDecls = new SecurityDeclarationCollection (this);

				return m_secDecls;
			}
		}

		public CustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public GenericParameterCollection GenericParameters {
			get {
				if (m_genparams == null) {
					m_genparams = new GenericParameterCollection (this);
					m_genparams.OnGenericParameterAdded += new GenericParameterEventHandler (OnGenericParameterAdded);
				}

				return m_genparams;
			}
		}

		public RVA RVA {
			get { return m_rva; }
			set { m_rva = value; }
		}

		public MethodBody Body {
			get {
				LoadBody ();
				return m_body;
			}
			set { m_body = value; }
		}

		public PInvokeInfo PInvokeInfo {
			get { return m_pinvoke; }
			set { m_pinvoke = value; }
		}

		public OverrideCollection Overrides {
			get {
				if (m_overrides == null)
					m_overrides = new OverrideCollection (this);

				return m_overrides;
			}
		}

		public bool IsAbstract {
			get { return (m_attributes & MethodAttributes.Abstract) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.Abstract;
				else
					m_attributes &= ~MethodAttributes.Abstract;
			}
		}

		public bool IsFinal {
			get { return (m_attributes & MethodAttributes.Final) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.Final;
				else
					m_attributes &= ~MethodAttributes.Final;
			}
		}

		public bool IsHideBySignature {
			get { return (m_attributes & MethodAttributes.HideBySig) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.HideBySig;
				else
					m_attributes &= ~MethodAttributes.HideBySig;
			}
		}

		public bool IsNewSlot {
			get { return (m_attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.NewSlot; }
			set {
				if (value)
					m_attributes |= (MethodAttributes.VtableLayoutMask & MethodAttributes.NewSlot);
				else
					m_attributes &= ~(MethodAttributes.VtableLayoutMask & MethodAttributes.NewSlot);
			}
		}

		public bool IsRuntimeSpecialName {
			get { return (m_attributes & MethodAttributes.RTSpecialName) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.RTSpecialName;
				else
					m_attributes &= ~MethodAttributes.RTSpecialName;
			}
		}

		public bool IsSpecialName {
			get { return (m_attributes & MethodAttributes.SpecialName) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.SpecialName;
				else
					m_attributes &= ~MethodAttributes.SpecialName;
			}
		}

		public bool IsStatic {
			get { return (m_attributes & MethodAttributes.Static) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.Static;
				else
					m_attributes &= ~MethodAttributes.Static;
				this.HasThis = !value;
			}
		}

		public bool IsVirtual {
			get { return (m_attributes & MethodAttributes.Virtual) != 0; }
			set {
				if (value)
					m_attributes |= MethodAttributes.Virtual;
				else
					m_attributes &= ~MethodAttributes.Virtual;
			}
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

		internal MethodDefinition (string name, MethodAttributes attrs) : base (name)
		{
			m_attributes = attrs;
		}

		public MethodDefinition (string name, MethodAttributes attrs, TypeReference returnType) :
			this (name, attrs)
		{
			this.ReturnType.ReturnType = returnType;
		}

		public MethodBody CreateBody ()
		{
			return m_body = new MethodBody (this);
		}

		internal void LoadBody ()
		{
			if (m_module != null && m_body == null && m_rva != RVA.Zero) {
				m_body = new MethodBody (this);
				m_module.Controller.Reader.Code.VisitMethodBody (m_body);
			}
		}

		object ICloneable.Clone ()
		{
			return this.Clone ();
		}

		public MethodDefinition Clone ()
		{
			return Clone (this, null);
		}

		void OnGenericParameterAdded (object sender, GenericParameterEventArgs ea)
		{
			ea.GenericParameter.Position = m_genparams.Count + 1;
			GenericArguments.Add (ea.GenericParameter);
		}

		internal static MethodDefinition Clone (MethodDefinition meth, ReflectionHelper helper)
		{
			MethodDefinition nm = new MethodDefinition (
				meth.Name,
				RVA.Zero,
				meth.Attributes,
				meth.ImplAttributes,
				meth.HasThis,
				meth.ExplicitThis,
				meth.CallingConvention);

			nm.ReturnType.ReturnType =
				helper == null ? meth.ReturnType.ReturnType :
					helper.ImportTypeReference (meth.ReturnType.ReturnType);

			if (meth.ReturnType.HasConstant)
				nm.ReturnType.Constant = meth.ReturnType.Constant;

			if (meth.ReturnType.MarshalSpec != null)
				nm.ReturnType.MarshalSpec = meth.ReturnType.MarshalSpec;

			foreach (CustomAttribute ca in meth.ReturnType.CustomAttributes)
				nm.ReturnType.CustomAttributes.Add (CustomAttribute.Clone (ca, helper));

			if (meth.PInvokeInfo != null)
				nm.PInvokeInfo = meth.PInvokeInfo; // TODO: import module ?
			foreach (ParameterDefinition param in meth.Parameters)
				nm.Parameters.Add (ParameterDefinition.Clone (param, helper));
			foreach (MethodReference ov in meth.Overrides)
				nm.Overrides.Add (helper == null ? ov : helper.ImportMethodReference (ov));
			foreach (CustomAttribute ca in meth.CustomAttributes)
				nm.CustomAttributes.Add (CustomAttribute.Clone (ca, helper));
			foreach (SecurityDeclaration sec in meth.SecurityDeclarations)
				nm.SecurityDeclarations.Add (SecurityDeclaration.Clone (sec));

			if (meth.Body != null)
				nm.Body = MethodBody.Clone (meth.Body, nm, helper);

			return nm;
		}

		public override void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitMethodDefinition (this);

			this.GenericParameters.Accept (visitor);
			this.Parameters.Accept (visitor);

			if (this.PInvokeInfo != null)
				this.PInvokeInfo.Accept (visitor);

			this.SecurityDeclarations.Accept (visitor);
			this.Overrides.Accept (visitor);
			this.CustomAttributes.Accept (visitor);
		}
	}
}
