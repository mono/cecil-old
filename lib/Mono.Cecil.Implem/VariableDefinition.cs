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

	using Mono.Cecil.Cil;

	internal sealed class VariableDefinition : IVariableDefinition {

		private string m_name;
		private int m_index;
		private MethodDefinition m_method;
		private ITypeReference m_variable;

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public int Index {
			get { return m_index; }
			set { m_index = value; }
		}

		public IMethodDefinition Method {
			get { return m_method; }
			set { m_method = value as MethodDefinition; }
		}

		public ITypeReference Variable {
			get { return m_variable; }
			set { m_variable = value; }
		}

		public VariableDefinition (string name, int index, MethodDefinition method, ITypeReference variable)
		{
			m_name = name;
			m_method = method;
			m_variable = variable;
		}

		public void Accept (ICodeVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
