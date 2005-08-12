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

	internal sealed class CodeWriter : BaseCodeVisitor {

		private ReflectionWriter m_reflectWriter;
		private BinaryWriter m_binaryWriter;
		private BinaryWriter m_codeWriter;
		private RVA m_start = new RVA (0x2050);

		private RVA m_curs;

		public CodeWriter (ReflectionWriter reflectWriter, BinaryWriter writer)
		{
			m_reflectWriter = reflectWriter;
			m_binaryWriter = writer;
			m_codeWriter = new BinaryWriter (new MemoryStream ());
			m_curs = m_start;
		}

		public RVA WriteMethodBody (IMethodDefinition meth)
		{
			if (meth.Body == null)
				return RVA.Zero;

			RVA ret = m_curs;
			meth.Body.Accept (this);
			return ret;
		}

		private void WriteToken (TokenType type, uint rid)
		{
			m_codeWriter.Write (((uint) type) << 24 | rid);
		}

		public override void Visit (IMethodBody body)
		{
			m_codeWriter.BaseStream.Position = 0;
			m_codeWriter.BaseStream.SetLength (0);
		}

		public override void Visit (IInstructionCollection instructions)
		{
			IMethodBody body = instructions.Container;
			long start = m_codeWriter.BaseStream.Position;
			foreach (Instruction instr in instructions) {

				instr.Offset = (int) (start - m_codeWriter.BaseStream.Position);

				if (instr.OpCode.Size == 1)
					m_codeWriter.Write ((byte) instr.OpCode.Value);
				else
					m_codeWriter.Write (instr.OpCode.Value);

				switch (instr.OpCode.OperandType) {
				case OperandType.InlineNone :
					break;
				case OperandType.InlineSwitch :
					IInstruction [] targets = instr.Operand as IInstruction [];
					m_codeWriter.Write (targets.Length);
					for (int i = 0; i < targets.Length; i++)
						m_codeWriter.Write (0);
					break;
				case OperandType.ShortInlineBrTarget :
					instr.Offset = (int) m_codeWriter.BaseStream.Position;
					m_codeWriter.Write ((byte) 0);
					break;
				case OperandType.InlineBrTarget :
					instr.Offset = (int) m_codeWriter.BaseStream.Position;
					m_codeWriter.Write (0);
					break;
				case OperandType.ShortInlineI :
					m_codeWriter.Write ((byte) instr.Operand);
					break;
				case OperandType.ShortInlineVar :
					m_codeWriter.Write ((byte) body.Variables.IndexOf (
							instr.Operand as IVariableDefinition));
					break;
				case OperandType.ShortInlineParam : // see param
					m_codeWriter.Write ((byte) instr.Operand);
					break;
				case OperandType.InlineSig :
				case OperandType.InlineI :
					m_codeWriter.Write ((int) instr.Operand);
					break;
				case OperandType.InlineVar :
					m_codeWriter.Write (body.Variables.IndexOf (
							instr.Operand as IVariableDefinition));
					break;
				case OperandType.InlineParam : // should write param index of
					m_codeWriter.Write ((int) instr.Operand);
					break;
				case OperandType.InlineI8 :
					m_codeWriter.Write ((long) instr.Operand);
					break;
				case OperandType.ShortInlineR :
					m_codeWriter.Write ((float) instr.Operand);
					break;
				case OperandType.InlineR :
					m_codeWriter.Write ((double) instr.Operand);
					break;
				case OperandType.InlineString :
					WriteToken (TokenType.String,
						m_reflectWriter.MetadataWriter.AddUserString (instr.Operand as string));
					break;
				case OperandType.InlineField :
					if (instr.Operand is IFieldDefinition)
						WriteToken (TokenType.Field, m_reflectWriter.GetRidFor (instr.Operand as IFieldDefinition));
					else if (instr.Operand is IFieldReference)
						WriteToken (TokenType.MemberRef, m_reflectWriter.GetRidFor (instr.Operand as IFieldReference));
					else
						throw new ReflectionException (
							"Wrong operand for InlineField OpCode: ", instr.Operand.ToString ());
					break;
				case OperandType.InlineMethod :
					if (instr.Operand is IMethodDefinition)
						WriteToken (TokenType.Method, m_reflectWriter.GetRidFor (instr.Operand as IMethodDefinition));
					else if (instr.Operand is IMethodReference)
						WriteToken (TokenType.MemberRef, m_reflectWriter.GetRidFor (instr.Operand as IMethodReference));
					else
						throw new ReflectionException (
							"Wrong operand for InlineMethod OpCode: {0}", instr.Operand.ToString ());
					break;
				case OperandType.InlineType :
					if (instr.Operand is ITypeDefinition)
						WriteToken (TokenType.TypeDef, m_reflectWriter.GetRidFor (instr.Operand as ITypeDefinition));
					else if (instr.Operand is ITypeReference)
						WriteToken (TokenType.TypeRef, m_reflectWriter.GetRidFor (instr.Operand as ITypeReference));
					else // deal with type spec!
						throw new ReflectionException (
							"Wrong operand for InlineType OpCode: {0}", instr.Operand.ToString ());
					break;
				case OperandType.InlineTok :
					if (instr.Operand is ITypeDefinition)
						WriteToken (TokenType.TypeDef, m_reflectWriter.GetRidFor (instr.Operand as ITypeDefinition));
					else if (instr.Operand is ITypeReference)
						WriteToken (TokenType.TypeRef, m_reflectWriter.GetRidFor (instr.Operand as ITypeReference));
					if (instr.Operand is IFieldDefinition)
						WriteToken (TokenType.Field, m_reflectWriter.GetRidFor (instr.Operand as IFieldDefinition));
					if (instr.Operand is IMethodDefinition)
						WriteToken (TokenType.Method, m_reflectWriter.GetRidFor (instr.Operand as IMethodDefinition));
					else if (instr.Operand is IMemberReference)
						WriteToken (TokenType.MemberRef, m_reflectWriter.GetRidFor (instr.Operand as IMemberReference));
					else // deal with type spec
						throw new ReflectionException (
							"Wrong operand for InlineTok OpCode: {0}", instr.Operand.ToString ());
					break;
				}
			}

			// patch branches
			long pos = m_codeWriter.BaseStream.Position;

			foreach (Instruction instr in instructions) {
				m_codeWriter.BaseStream.Position = instr.Offset + instr.OpCode.Size;
				switch (instr.OpCode.OperandType) {
				case OperandType.InlineSwitch :
					IInstruction [] targets = instr.Operand as IInstruction [];
					foreach (IInstruction tgt in targets)
						m_codeWriter.Write (instr.Offset - tgt.Offset);
					break;
				case OperandType.ShortInlineBrTarget :
					m_codeWriter.Write ((byte) (instr.Offset - (instr.Operand as IInstruction).Offset));
					break;
				case OperandType.InlineBrTarget :
					m_codeWriter.Write (instr.Offset - (instr.Operand as IInstruction).Offset);
					break;
				}
			}

			m_codeWriter.BaseStream.Position = pos;
		}

		public override void Visit (IExceptionHandlerCollection seh)
		{
			// TODO
		}

		public override void Visit (IExceptionHandler eh)
		{
			// TODO
		}

		public override void Visit (IVariableDefinitionCollection variables)
		{
			// TODO
		}

		public override void Visit (IVariableDefinition var)
		{
			// TODO
		}

		public override void Terminate (IMethodBody body)
		{
			if (body.Variables.Count > 0 || body.ExceptionHandlers.Count > 0
				|| m_codeWriter.BaseStream.Length >= 64) {

				// write fat header
			} else {

				m_binaryWriter.Write (
					(ushort) MethodHeader.TinyFormat | m_codeWriter.BaseStream.Length << 2);
				m_binaryWriter.Write ((m_codeWriter.BaseStream as MemoryStream).ToArray ());
			}

			m_curs = m_start + (uint) m_binaryWriter.BaseStream.Position;
		}
	}
}
