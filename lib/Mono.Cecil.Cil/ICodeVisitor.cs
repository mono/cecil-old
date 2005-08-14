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

	public interface ICodeVisitor {
		void VisitMethodBody (IMethodBody body);
		void VisitInstructionCollection (IInstructionCollection instructions);
		void VisitInstruction (IInstruction instr);
		void VisitExceptionHandlerCollection (IExceptionHandlerCollection seh);
		void VisitExceptionHandler (IExceptionHandler eh);
		void VisitVariableDefinitionCollection (IVariableDefinitionCollection variables);
		void VisitVariableDefinition (IVariableDefinition var);

		void TerminateMethodBody (IMethodBody body);
	}
}
