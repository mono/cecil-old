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

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class CodeWriter : BaseCodeVisitor {

		private ReflectionWriter m_reflectWriter;
		private MemoryBinaryWriter m_binaryWriter;
		private MemoryBinaryWriter m_codeWriter;

		public CodeWriter (ReflectionWriter reflectWriter, MemoryBinaryWriter writer)
		{
			m_reflectWriter = reflectWriter;
			m_binaryWriter = writer;
			m_codeWriter = new MemoryBinaryWriter ();
		}

		public RVA WriteMethodBody (IMethodDefinition meth)
		{
			if (meth.Body == null)
				return RVA.Zero;

			RVA ret = m_reflectWriter.MetadataWriter.GetDataCursor ();
			meth.Body.Accept (this);
			return ret;
		}

		public override void VisitMethodBody (IMethodBody body)
		{
			m_codeWriter.Empty ();
		}

		private void WriteToken (MetadataToken token)
		{
			m_codeWriter.Write (((uint) token.TokenType) | token.RID);
		}

		public override void VisitInstructionCollection (IInstructionCollection instructions)
		{
			IMethodBody body = instructions.Container;
			long start = m_codeWriter.BaseStream.Position;
			foreach (Instruction instr in instructions) {

				instr.Offset = (int) (m_codeWriter.BaseStream.Position - start);

				if (instr.OpCode.Size == 1)
					m_codeWriter.Write ((byte) instr.OpCode.Value);
				else
					m_codeWriter.Write (instr.OpCode.Value);

				if (instr.OpCode.OperandType != OperandType.InlineNone &&
					instr.Operand == null)
					throw new ReflectionException ("OpCode {0} have null operand", instr.OpCode.Name);

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
					m_codeWriter.Write ((byte) 0);
					break;
				case OperandType.InlineBrTarget :
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
					WriteToken (new MetadataToken (TokenType.String,
							m_reflectWriter.MetadataWriter.AddUserString (instr.Operand as string)));
					break;
				case OperandType.InlineField :
				case OperandType.InlineMethod :
				case OperandType.InlineType :
				case OperandType.InlineTok :
					if (instr.Operand is IMetadataTokenProvider)
						WriteToken ((instr.Operand as IMetadataTokenProvider).MetadataToken);
					else
						throw new ReflectionException (
							string.Format ("Wrong operand for {0} OpCode: {1}",
								instr.OpCode.OperandType.ToString (),
								instr.Operand.GetType ().FullName));
					break;
				}
			}

			// patch branches
			long pos = m_codeWriter.BaseStream.Position;

			foreach (Instruction instr in instructions) {
				switch (instr.OpCode.OperandType) {
				case OperandType.InlineSwitch :
					m_codeWriter.BaseStream.Position = instr.Offset + instr.OpCode.Size;
					IInstruction [] targets = instr.Operand as IInstruction [];
					foreach (IInstruction tgt in targets)
						m_codeWriter.Write (instr.Offset - tgt.Offset);
					break;
				case OperandType.ShortInlineBrTarget :
					m_codeWriter.BaseStream.Position = instr.Offset + instr.OpCode.Size;
					m_codeWriter.Write ((byte) (instr.Offset - (instr.Operand as IInstruction).Offset));
					break;
				case OperandType.InlineBrTarget :
					m_codeWriter.BaseStream.Position = instr.Offset + instr.OpCode.Size;
					m_codeWriter.Write (instr.Offset - (instr.Operand as IInstruction).Offset);
					break;
				}
			}

			m_codeWriter.BaseStream.Position = pos;
		}

		public override void VisitExceptionHandlerCollection (IExceptionHandlerCollection seh)
		{
			// TODO
		}

		public override void VisitExceptionHandler (IExceptionHandler eh)
		{
			// TODO
		}

		public override void VisitVariableDefinitionCollection (IVariableDefinitionCollection variables)
		{
			if (variables.Count == 0)
				return;

			MethodBody body = variables.Container as MethodBody;
			StandAloneSigTable sasTable = m_reflectWriter.MetadataTableWriter.GetStandAloneSigTable ();
			StandAloneSigRow sasRow = m_reflectWriter.MetadataRowWriter.CreateStandAloneSigRow (
				m_reflectWriter.SignatureWriter.AddLocalVarSig (
					GetLocalVarSig (variables)));

			sasTable.Rows.Add (sasRow);
			body.LocalVarToken = sasTable.Rows.Count;
		}

		public override void TerminateMethodBody (IMethodBody body)
		{
			long pos = m_binaryWriter.BaseStream.Position;

			MethodBody bdy = body as MethodBody;
			if (body.Variables.Count > 0 || body.ExceptionHandlers.Count > 0
				|| m_codeWriter.BaseStream.Length >= 64) {

				MethodHeader header = (MethodHeader) ((ushort) MethodHeader.FatFormat |
					m_codeWriter.BaseStream.Length << 2);
				if (body.InitLocals)
					header |= MethodHeader.InitLocals;
				if (body.ExceptionHandlers.Count > 0)
					header |= MethodHeader.MoreSects;

				m_binaryWriter.Write ((ushort) header);
				m_binaryWriter.Write ((ushort) body.MaxStack);
				m_binaryWriter.Write ((uint) m_codeWriter.BaseStream.Length);
				m_binaryWriter.Write ((uint) bdy.LocalVarToken);

				m_binaryWriter.Write (m_codeWriter);
				m_binaryWriter.QuadAlign ();

				// TODO write exception table here
			} else {
				m_binaryWriter.Write ((byte) ((byte) MethodHeader.TinyFormat |
					m_codeWriter.BaseStream.Length << 2));
				m_binaryWriter.Write (m_codeWriter);
				m_binaryWriter.QuadAlign ();
			}

			m_reflectWriter.MetadataWriter.AddData (
				(int) (m_binaryWriter.BaseStream.Position - pos));
		}

		private LocalVarSig GetLocalVarSig (IVariableDefinitionCollection vars)
		{
			LocalVarSig lvs = new LocalVarSig ();
			lvs.CallingConvention |= 0x7;
			lvs.Count = vars.Count;
			lvs.LocalVariables = new LocalVarSig.LocalVariable [lvs.Count];
			for (int i = 0; i < lvs.Count; i++) {
				LocalVarSig.LocalVariable lv = new LocalVarSig.LocalVariable ();
				ITypeReference type = vars [i].Variable;
				if (type is PinnedType) {
					lv.Constraint |= Constraint.Pinned;
					type = (type as PinnedType).ElementType;
				}

				if (type is IReferenceType) {
					lv.ByRef = true;
					type = (type as IReferenceType).ElementType;
				}

				lv.Type = m_reflectWriter.GetSigType (type);

				lvs.LocalVariables [i] = lv;
			}
			return lvs;
		}
	}
}
