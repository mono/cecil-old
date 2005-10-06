//
// MethodBody.cs
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

namespace Mono.Cecil.Cil {

	using System;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Signatures;

	public sealed class MethodBody : IMethodBody {

		MethodDefinition m_method;
		int m_maxStack;
		int m_codeSize;
		bool m_initLocals;
		int m_localVarToken;

		InstructionCollection m_instructions;
		ExceptionHandlerCollection m_exceptions;
		VariableDefinitionCollection m_variables;

		private CilWorker m_cilWorker;

		public MethodDefinition Method {
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

		internal int LocalVarToken {
			get { return m_localVarToken; }
			set { m_localVarToken = value; }
		}

		public CilWorker CilWorker {
			get {
				if (m_cilWorker == null)
					m_cilWorker = new CilWorker (this);
				return m_cilWorker;
			}
			set { m_cilWorker = value; }
		}

		public InstructionCollection Instructions {
			get { return m_instructions; }
		}

		public ExceptionHandlerCollection ExceptionHandlers {
			get { return m_exceptions; }
		}

		public VariableDefinitionCollection Variables {
			get { return m_variables; }
		}

		public MethodBody (MethodDefinition meth)
		{
			m_method = meth;
			m_instructions = new InstructionCollection (this);
			m_exceptions = new ExceptionHandlerCollection (this);
			m_variables = new VariableDefinitionCollection (this);
		}

		public void Accept (ICodeVisitor visitor)
		{
			visitor.VisitMethodBody (this);
			m_variables.Accept (visitor);
			m_instructions.Accept (visitor);
			m_exceptions.Accept (visitor);

			visitor.TerminateMethodBody (this);
		}
	}
}
