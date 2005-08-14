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

	internal class Instruction : IInstruction {

		private int m_offset;
		private OpCode m_opCode;
		private object m_operand;

		private Instruction m_previous;
		private Instruction m_next;

		public int Offset {
			get { return m_offset; }
			set { m_offset = value; }
		}

		public OpCode OpCode {
			get { return m_opCode; }
		}

		public object Operand {
			get { return m_operand; }
			set { m_operand = value; }
		}

		public IInstruction Previous {
			get { return m_previous; }
			set { m_previous = value as Instruction; }
		}

		public IInstruction Next {
			get { return m_next; }
			set { m_next = value as Instruction; }
		}

		public Instruction (int offset, OpCode opCode, object operand) : this (offset, opCode)
		{
			m_operand = operand;
		}

		public Instruction (int offset, OpCode opCode)
		{
			m_offset = offset;
			m_opCode = opCode;
		}

		public void Accept (ICodeVisitor visitor)
		{
			visitor.VisitInstruction (this);
		}
	}
}
