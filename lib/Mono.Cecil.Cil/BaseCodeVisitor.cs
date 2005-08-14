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

namespace Mono.Cecil.Cil {

	public abstract class BaseCodeVisitor : ICodeVisitor {

		public virtual void VisitMethodBody (IMethodBody body)
		{
		}

		public virtual void VisitInstructionCollection (IInstructionCollection instructions)
		{
		}

		public virtual void VisitInstruction (IInstruction instr)
		{
		}

		public virtual void VisitExceptionHandlerCollection (IExceptionHandlerCollection seh)
		{
		}

		public virtual void VisitExceptionHandler (IExceptionHandler eh)
		{
		}

		public virtual void VisitVariableDefinitionCollection (IVariableDefinitionCollection variables)
		{
		}

		public virtual void VisitVariableDefinition (IVariableDefinition var)
		{
		}

		public virtual void TerminateMethodBody (IMethodBody body)
		{
		}
	}
}
