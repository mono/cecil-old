//
// ParameterDefinition.cs
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

	using Mono.Cecil.Metadata;

	public sealed class ParameterDefinition : IParameterDefinition {

		string m_name;
		int m_sequence;
		ParamAttributes m_attributes;
		TypeReference m_paramType;
		MetadataToken m_token;

		bool m_hasConstant;
		object m_const;

		MethodReference m_method;
		CustomAttributeCollection m_customAttrs;

		MarshalDesc m_marshalDesc;

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

		public TypeReference ParameterType {
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

		public CustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public MarshalDesc MarshalSpec {
			get { return m_marshalDesc; }
			set { m_marshalDesc = value; }
		}

		public ParameterDefinition (TypeReference paramType) :
			this (string.Empty, -1, (ParamAttributes) 0, paramType)
		{
		}

		public ParameterDefinition (string name, int seq, ParamAttributes attrs, TypeReference paramType)
		{
			m_name = name;
			m_sequence = seq;
			m_attributes = attrs;
			m_paramType = paramType;
		}

		public ParameterDefinition Clone ()
		{
			return Clone (this, null);
		}

		internal static ParameterDefinition Clone (ParameterDefinition param, ReflectionHelper helper)
		{
			ParameterDefinition np = new ParameterDefinition (
				param.Name,
				param.Sequence,
				param.Attributes,
				helper == null ? param.ParameterType : helper.ImportTypeReference (param.ParameterType));

			if (param.HasConstant)
				np.Constant = param.Constant;

			if (param.MarshalSpec != null)
				np.MarshalSpec = param.MarshalSpec;

			foreach (CustomAttribute ca in param.CustomAttributes)
				np.CustomAttributes.Add (CustomAttribute.Clone (ca, helper));

			return np;
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
