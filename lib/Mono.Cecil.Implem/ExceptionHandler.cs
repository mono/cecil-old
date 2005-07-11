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
	using Mono.Cecil.Cil;

	internal sealed class ExceptionHandler : IExceptionHandler {

		private Instruction m_tryStart;
		private Instruction m_tryEnd;
		private Instruction m_filterStart;
		private Instruction m_filterEnd;
		private Instruction m_handlerStart;
		private Instruction m_handlerEnd;

		private ITypeReference m_catchType;
		private ExceptionHandlerType m_type;

		public IInstruction TryStart {
			get { return m_tryStart; }
			set { m_tryStart = value as Instruction; }
		}

		public IInstruction TryEnd {
			get { return m_tryEnd; }
			set { m_tryEnd = value as Instruction; }
		}

		public IInstruction FilterStart {
			get { return m_filterStart; }
			set { m_filterStart = value as Instruction; }
		}

		public IInstruction FilterEnd {
			get { return m_filterEnd; }
			set { m_filterEnd = value as Instruction; }
		}

		public IInstruction HandlerStart {
			get { return m_handlerStart; }
			set { m_handlerStart = value as Instruction; }
		}

		public IInstruction HandlerEnd {
			get { return m_handlerEnd; }
			set { m_handlerEnd = value as Instruction; }
		}

		public ITypeReference CatchType {
			get { return m_catchType; }
			set { m_catchType = value; }
		}

		public ExceptionHandlerType Type {
			get { return m_type; }
			set { m_type = value; }
		}

		public ExceptionHandler (ExceptionHandlerType type)
		{
			m_type = type;
		}

		public void Accept (ICodeVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
