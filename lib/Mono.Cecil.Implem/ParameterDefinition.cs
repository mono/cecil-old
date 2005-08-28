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

		private bool m_hasConstant;
		private object m_const;

		private MethodReference m_method;
		private CustomAttributeCollection m_customAttrs;

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

		public MethodReference Method {
			get { return m_method; }
			set { m_method = value; }
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

		public ParameterDefinition (ITypeReference paramType) :
			this (string.Empty, -1, (ParamAttributes) 0, paramType)
		{
		}

		public ParameterDefinition (string name, int seq, ParamAttributes attrs, ITypeReference paramType)
		{
			m_name = name;
			m_sequence = seq;
			m_attributes = attrs;
			m_paramType = paramType;
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
