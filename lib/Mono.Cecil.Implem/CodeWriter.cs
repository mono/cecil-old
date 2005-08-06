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

	using System;
	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class CodeWriter : ICodeVisitor {

		private BinaryWriter m_binaryWriter;
		private RVA m_start = new RVA (0x2050);

		public CodeWriter (BinaryWriter writer)
		{
			m_binaryWriter = writer;
		}

		public void Visit (IMethodBody body)
		{
		}

		public void Visit (IInstructionCollection instructions)
		{
		}

		public void Visit (IInstruction instr)
		{
		}

		public void Visit (IExceptionHandlerCollection seh)
		{
		}

		public void Visit (IExceptionHandler eh)
		{
		}

		public void Visit (IVariableDefinitionCollection variables)
		{
		}

		public void Visit (IVariableDefinition var)
		{
		}
	}
}
