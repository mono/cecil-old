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

	using System.Text;

	using Mono.Cecil;

	internal sealed class PropertyDefinition : MemberDefinition, IPropertyDefinition {

		private ITypeReference m_propertyType;
		private ParameterDefinitionCollection m_parameters;
		private PropertyAttributes m_attributes;

		private CustomAttributeCollection m_customAttrs;

		private bool m_semanticLoaded;
		private IMethodDefinition m_getMeth;
		private IMethodDefinition m_setMeth;

		private bool m_constLoaded;
		private bool m_hasConstant;
		private object m_const;

		public ITypeReference PropertyType {
			get { return m_propertyType; }
			set { m_propertyType = value; }
		}

		public bool SemanticLoaded {
			get { return m_semanticLoaded; }
			set { m_semanticLoaded = value; }
		}

		public PropertyAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					if (this.DecTypeDef.IsLazyLoadable) {
						m_customAttrs = new CustomAttributeCollection (this, this.DecTypeDef.Mod.Controller);
						m_customAttrs.Load ();
					} else
						m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public IMethodDefinition GetMethod {
			get {
				if (!this.DecTypeDef.IsLazyLoadable && !m_semanticLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadSemantic (this);

				return m_getMeth;
			}
			set { m_getMeth = value; }
		}

		public IMethodDefinition SetMethod {
			get {
				if (!this.DecTypeDef.IsLazyLoadable && !m_semanticLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadSemantic (this);

				return m_setMeth;
			}
			set { m_setMeth = value; }
		}

		public IParameterDefinitionCollection Parameters {
			get {
				if (this.GetMethod != null)
					return ParameterDefinition.Clone (this.GetMethod.Parameters);
				else if (this.SetMethod != null) {
					IParameterDefinitionCollection parameters =
						ParameterDefinition.Clone (this.SetMethod.Parameters);
					if (parameters.Count > 0)
						parameters.RemoveAt (parameters.Count - 1);
					return parameters;
				}

				if (m_parameters == null)
					m_parameters = new ParameterDefinitionCollection (this);

				return m_parameters;
			}
		}

		public bool ConstantLoaded {
			get { return m_constLoaded; }
			set { m_constLoaded = value; }
		}

		public bool HasConstant {
			get {
				if (!this.DecTypeDef.IsLazyLoadable && !m_constLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadConstant (this);

				return m_hasConstant;
			}
		}

		public object Constant {
			get {
				if (!this.DecTypeDef.IsLazyLoadable && !m_constLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadConstant (this);

				return m_const;
			}
			set {
				m_hasConstant = true;
				m_const = value;
			}
		}

		public bool IsRuntimeSpecialName {
			get { return (m_attributes & PropertyAttributes.RTSpecialName) != 0; }
			set { m_attributes |= value ? PropertyAttributes.RTSpecialName : 0; }
		}

		public bool IsSpecialName {
			get { return (m_attributes & PropertyAttributes.SpecialName) != 0; }
			set { m_attributes |= value ? PropertyAttributes.SpecialName : 0; }
		}

		public PropertyDefinition (string name, ITypeReference propType, PropertyAttributes attrs) : base (name)
		{
			m_propertyType = propType;
			m_attributes = attrs;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (m_propertyType.ToString ());
			sb.Append (' ');

			if (this.DeclaringType == null) {
				sb.Append (this.DeclaringType.ToString ());
				sb.Append ("::");
			}

			sb.Append (this.Name);
			sb.Append ('(');
			IParameterDefinitionCollection parameters = this.Parameters;
			for (int i = 0; i < parameters.Count; i++) {
				if (i > 0)
					sb.Append (',');
				sb.Append (parameters [i].ParameterType.ToString ());
			}
			sb.Append (')');
			return sb.ToString ();
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitPropertyDefinition (this);

			this.CustomAttributes.Accept (visitor);
		}
	}
}
