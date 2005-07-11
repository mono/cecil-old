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

	using System;

	public interface IMethodBody : ICodeVisitable {

		int MaxStack { get; set; }
		int CodeSize { get; set; }
		bool InitLocals { get; set; }

		IMethodDefinition Method { get; }
		IInstructionCollection Instructions { get; }
		IExceptionHandlerCollection ExceptionHandlers { get; }
		IVariableDefinitionCollection Variables { get; }

		IVariableDefinition DefineLocalVariable (ITypeReference type);
		IVariableDefinition DefineLocalVariable (Type type);

		IExceptionHandler DefineExceptionHandler (ExceptionHandlerType type);

		//IInstruction DefineInstruction (OpCode code);
	}
}
