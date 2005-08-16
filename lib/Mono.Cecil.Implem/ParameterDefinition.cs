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
	using Mono.Cecil.Metadata;

	internal sealed class ParameterDefinition : IParameterDefinition {

		private string m_name;
		private int m_sequence;
		private ParamAttributes m_attributes;
		private ITypeReference m_paramType;
		private MetadataToken m_token;

		private bool m_constLoaded;
		private bool m_hasConstant;
		private object m_const;

		private ModuleDefinition m_module;
		private MethodReference m_method;
		private CustomAttributeCollection m_customAttrs;

		private bool m_marshalLoaded;
		private MarshalDesc m_marshalDesc;

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public int Sequence {
			get { return m_sequence; }
			set { m_sequence = value; }
		}

		public ParamAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public ITypeReference ParameterType {
			get { return m_paramType; }
			set { m_paramType = value; }
		}

		public MetadataToken MetadataToken {
			get { return m_token; }
			set { m_token = value; }
		}

		private ModuleDefinition Module {
			get {
				if (m_module != null)
					return m_module;

				if (m_paramType != null)
					m_module = (m_paramType as TypeReference).Module;

				if (m_module == null && m_method != null)
					return (m_method.DeclaringType as TypeReference).Module;

				if (m_module != null)
					return m_module;

				throw new ReflectionException ("Can get no module");
			}
		}

		public bool ConstantLoaded {
			get { return m_constLoaded; }
			set { m_constLoaded = value; }
		}

		public bool HasConstant {
			get {
				if (!this.Module.IsNew && !m_constLoaded)
					this.Module.Controller.Reader.ReadConstant (this);

				return m_hasConstant;
			}
		}

		public object Constant {
			get {
				this.Module.Controller.Reader.ReadConstant (this);
				return m_const;
			}
			set {
				m_hasConstant = true;
				m_const = value;
			}
		}

		public MethodReference Method {
			get { return m_method; }
			set { m_method = value; }
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null && m_method != null)
					m_customAttrs = new CustomAttributeCollection (
						this, this.Module.Controller);

				else if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				if (m_method != null && !this.Module.IsNew && !m_customAttrs.Loaded)
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
				if (!this.Module.IsNew && !m_marshalLoaded)
					this.Module.Controller.Reader.ReadMarshalSpec (this);

				return m_marshalDesc;
			}
			set { m_marshalDesc = value as MarshalDesc; }
		}

		public ParameterDefinition (string name, int seq, ParamAttributes attrs, ITypeReference paramType)
		{
			m_name = name;
			m_sequence = seq;
			m_attributes = attrs;
			m_paramType = paramType;
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
		{
			CustomAttribute ca = new CustomAttribute(ctor);
			this.CustomAttributes.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
		{
			return DefineCustomAttribute (
				this.Module.Controller.Helper.RegisterConstructor(ctor));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor, byte [] data)
		{
			CustomAttribute ca = this.Module.Controller.Reader.GetCustomAttribute (ctor, data);
			this.CustomAttributes.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor, byte [] data)
		{
			return DefineCustomAttribute (
				this.Module.Controller.Helper.RegisterConstructor (ctor), data);
		}

		public static IParameterDefinitionCollection Clone (IParameterDefinitionCollection original)
		{
			ParameterDefinitionCollection clone = new ParameterDefinitionCollection (
				original as MemberReference);
			foreach (IParameterDefinition param in original)
				clone.Add (param);
			return clone;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitParameterDefinition (this);

			if (this.MarshalSpec != null)
				this.MarshalSpec.Accept (visitor);

			this.CustomAttributes.Accept (visitor);
		}
	}
}
