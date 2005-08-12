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

		public virtual void Visit (IMethodBody body)
		{
		}

		public virtual void Visit (IInstructionCollection instructions)
		{
		}

		public virtual void Visit (IInstruction instr)
		{
		}

		public virtual void Visit (IExceptionHandlerCollection seh)
		{
		}

		public virtual void Visit (IExceptionHandler eh)
		{
		}

		public virtual void Visit (IVariableDefinitionCollection variables)
		{
		}

		public virtual void Visit (IVariableDefinition var)
		{
		}

		public virtual void Terminate (IMethodBody body)
		{
		}
	}
}
