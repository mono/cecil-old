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

	using Mono.Cecil;

	public interface IExceptionHandler : ICodeVisitable {

		int TryOffset { get; set; }
		// IInstruction TryStart { get; set; }
		// IInstruction TryEnd { get; set; }
		int TryLength { get; set; }

		// IInstruction HandlerStart { get; set; }
		// IInstruction HandlerEnd { get; set; }
		int HandlerOffset { get; set; }
		int HandlerLength { get; set; }

		// IInstruction FilterStart { get; set; }
		// IInstruction FilterEnd { get; set; }
		int FilterOffset { get; set; }

		ITypeReference CatchType { get; set; }
		ExceptionHandlerType Type { get; set; }
	}
}
