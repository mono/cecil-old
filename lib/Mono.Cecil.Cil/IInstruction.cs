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

	public interface IInstruction : ICodeVisitable {

		int Offset { get; }
		OpCode OpCode { get; set; }
		object Operand { get; set; }

		IInstruction Previous { get; }
		IInstruction Next { get; }
	}
}
