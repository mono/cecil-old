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
	using System.Text;
	using System.Reflection;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Signatures;

	internal class MethodReference : MemberReference, IMethodReference {

		private ParameterDefinitionCollection m_parameters;
		private MethodReturnType m_returnType;

		private bool m_hasThis;
		private bool m_explicitThis;
		private MethodCallingConvention m_callConv;

		public bool HasThis {
			get { return m_hasThis; }
			set { m_hasThis = value; }
		}

		public bool ExplicitThis {
			get { return m_explicitThis; }
			set { m_explicitThis = value; }
		}

		public MethodCallingConvention CallingConvention {
			get { return m_callConv; }
			set { m_callConv = value; }
		}

		public IParameterDefinitionCollection Parameters {
			get {
				if (m_parameters == null)
					m_parameters = new ParameterDefinitionCollection (this);
				return m_parameters;
			}
		}

		public IMethodReturnType ReturnType {
			get { return m_returnType;}
			set { m_returnType = value as MethodReturnType; }
		}

		public MethodReference (string name, bool hasThis,
			bool explicitThis, MethodCallingConvention callConv) : base (name)
		{
			m_parameters = new ParameterDefinitionCollection (this);
			m_hasThis = hasThis;
			m_explicitThis = explicitThis;
			m_callConv = callConv;
			m_returnType = new MethodReturnType (null);
		}

		public MethodReference (string name) : base (name)
		{
			m_returnType = new MethodReturnType (null);
		}

		public virtual void Accept (IReflectionVisitor visitor)
		{
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (m_returnType.ReturnType.FullName);
			sb.Append (" ");
			sb.Append (base.ToString ());
			sb.Append ("(");
			for (int i = 0; i < this.Parameters.Count; i++) {
				sb.Append (this.Parameters [i].ParameterType.FullName);
				if (i < this.Parameters.Count - 1)
					sb.Append (",");
			}
			sb.Append (")");
			return sb.ToString ();
		}
	}
}
