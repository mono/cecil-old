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

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Signatures;

	internal sealed class MethodBody : IMethodBody {

		private MethodDefinition m_method;
		private int m_maxStack;
		private int m_codeSize;
		private bool m_initLocals;
		private int m_localVarToken;

		private InstructionCollection m_instructions;
		private ExceptionHandlerCollection m_exceptions;
		private VariableDefinitionCollection m_variables;

		public IMethodDefinition Method {
			get { return m_method; }
		}

		public int MaxStack {
			get { return m_maxStack; }
			set { m_maxStack = value; }
		}

		public int CodeSize {
			get { return m_codeSize; }
			set { m_codeSize = value; }
		}

		public bool InitLocals {
			get { return m_initLocals; }
			set { m_initLocals = value; }
		}

		public int LocalVarToken {
			get { return m_localVarToken; }
			set { m_localVarToken = value; }
		}

		public IInstructionCollection Instructions {
			get { return m_instructions; }
		}

		public IExceptionHandlerCollection ExceptionHandlers {
			get { return m_exceptions; }
		}

		public IVariableDefinitionCollection Variables {
			get { return m_variables; }
		}

		public MethodBody (MethodDefinition meth)
		{
			m_method = meth;
			m_instructions = new InstructionCollection (this);
			m_exceptions = new ExceptionHandlerCollection (this);
			m_variables = new VariableDefinitionCollection (this);
		}

		public IExceptionHandler DefineExceptionHandler (ExceptionHandlerType type)
		{
			ExceptionHandler eh = new ExceptionHandler (type);
			m_exceptions.Add (eh);
			return eh;
		}

		public void Accept (ICodeVisitor visitor)
		{
			visitor.Visit (this);
			m_variables.Accept (visitor);
			m_instructions.Accept (visitor);
			m_exceptions.Accept (visitor);
		}
	}
}
