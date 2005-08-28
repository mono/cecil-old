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
	using System.Collections;
	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class CodeReader : BaseCodeVisitor {

		private ReflectionReader m_reflectReader;
		private MetadataRoot m_root;

		public CodeReader (ReflectionReader reflectReader)
		{
			m_reflectReader = reflectReader;
			m_root = m_reflectReader.MetadataRoot;
		}

		public override void VisitMethodBody (IMethodBody body)
		{
			MethodDefinition meth = body.Method as MethodDefinition;
			MethodBody methBody = body as MethodBody;
			BinaryReader br = m_reflectReader.Module.ImageReader.MetadataReader.GetDataReader (meth.RVA);

			// lets read the method
			IDictionary instrs;
			int flags = br.ReadByte ();
			switch (flags & 0x3) {
			case (int) MethodHeader.TinyFormat :
				methBody.CodeSize = flags >> 2;
				methBody.MaxStack = 8;
				ReadCilBody (methBody, br, out instrs);
				return;
			case (int) MethodHeader.FatFormat :
				br.BaseStream.Position--;
				int fatflags = br.ReadUInt16 ();
				//int headersize = (fatflags >> 12) & 0xf;
				methBody.MaxStack = br.ReadUInt16 ();
				methBody.CodeSize = br.ReadInt32 ();
				methBody.LocalVarToken = br.ReadInt32 ();
				body.InitLocals = (fatflags & (int) MethodHeader.InitLocals) != 0;
				VisitVariableDefinitionCollection (methBody.Variables);
				ReadCilBody (methBody, br, out instrs);
				if ((fatflags & (int) MethodHeader.MoreSects) != 0)
					ReadSection (methBody, br, instrs);
				return;
			}
		}

		private uint GetRid (int token)
		{
			return (uint) token & 0x00ffffff;
		}

		private bool IsToken (int token, TokenType t)
		{
			return token >> 24 == (int) t >> 24;
		}

		private void ReadCilBody (MethodBody body, BinaryReader br, out IDictionary instructions)
		{
			long start = br.BaseStream.Position, offset;
			Instruction last = null;
			InstructionCollection code = body.Instructions as InstructionCollection;
			instructions = new Hashtable ();

			while (br.BaseStream.Position < start + body.CodeSize) {
				OpCode op;
				offset = br.BaseStream.Position - start;
				int cursor = br.ReadByte ();
				if (cursor == 0xfe)
					op = OpCodes.Cache.Instance.TwoBytesOpCode [br.ReadByte ()];
				else
					op = OpCodes.Cache.Instance.OneByteOpCode [cursor];

				Instruction instr = new Instruction ((int) offset, op);
				switch (op.OperandType) {
				case OperandType.InlineNone :
					break;
				// nasty hack: use Labels as operand to resolve branches later
				case OperandType.InlineSwitch :
					uint length = br.ReadUInt32 ();
					Label [] branches = new Label [length];
					int [] buf = new int [length];
					for (int i = 0; i < length; i++)
						buf [i] = br.ReadInt32 ();
					for (int i = 0; i < length; i++)
						branches [i] = new Label (Convert.ToInt32 (br.BaseStream.Position - start + buf [i]));
					instr.Operand = branches;
					break;
				case OperandType.ShortInlineBrTarget :
					sbyte sbrtgt = br.ReadSByte ();
					instr.Operand = new Label (Convert.ToInt32 (br.BaseStream.Position - start + sbrtgt));
					break;
				case OperandType.InlineBrTarget :
					int brtgt = br.ReadInt32 ();
					instr.Operand = new Label (Convert.ToInt32 (br.BaseStream.Position - start + brtgt));
					break;
				case OperandType.ShortInlineI :
					instr.Operand = br.ReadByte ();
					break;
				case OperandType.ShortInlineVar :
					instr.Operand = body.Variables [(int) br.ReadByte ()];
					break;
				case OperandType.ShortInlineParam : // see param
					instr.Operand = br.ReadByte ();
					break;
				case OperandType.InlineSig :
				case OperandType.InlineI :
					instr.Operand = br.ReadInt32 ();
					break;
				case OperandType.InlineVar :
					instr.Operand = body.Variables [(int) br.ReadInt16 ()];
					break;
				case OperandType.InlineParam : // TODO get an IParamDef as operand, adjust the index if static
					instr.Operand = br.ReadInt32 ();
					break;
				case OperandType.InlineI8 :
					instr.Operand = br.ReadInt64 ();
					break;
				case OperandType.ShortInlineR :
					instr.Operand = br.ReadSingle ();
					break;
				case OperandType.InlineR :
					instr.Operand = br.ReadDouble ();
					break;
				case OperandType.InlineString :
					instr.Operand = m_root.Streams.UserStringsHeap [GetRid (br.ReadInt32 ())];
					break;
				case OperandType.InlineField :
					int field = br.ReadInt32 ();
					if (IsToken (field, TokenType.Field))
						instr.Operand = m_reflectReader.GetFieldDefAt (GetRid (field));
					else if (IsToken (field, TokenType.MemberRef))
						instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (field));
					else
						throw new ReflectionException ("Wrong token for InlineField Operand: {0}", field.ToString ("x8"));
					break;
				case OperandType.InlineMethod :
					int meth = br.ReadInt32 ();
					if (IsToken (meth, TokenType.Method))
						instr.Operand = m_reflectReader.GetMethodDefAt (GetRid (meth));
					else if (IsToken (meth, TokenType.MemberRef))
						instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (meth));
					else
						throw new ReflectionException ("Wrong token for InlineMethod Operand: {0}", meth.ToString ("x8"));
					break;
				case OperandType.InlineType :
					int type = br.ReadInt32 ();
					if (IsToken (type, TokenType.TypeDef))
						instr.Operand = m_reflectReader.GetTypeDefAt (GetRid (type));
					else if (IsToken (type, TokenType.TypeRef))
						instr.Operand = m_reflectReader.GetTypeRefAt (GetRid (type));
					else if (IsToken (type, TokenType.TypeSpec))
						instr.Operand = m_reflectReader.GetTypeSpecAt (GetRid (type));
					else
						throw new ReflectionException ("Wrong token for InlineType Operand: {0}", type.ToString ("x8"));
					break;
				case OperandType.InlineTok :
					int token = br.ReadInt32 ();
					if (IsToken (token, TokenType.TypeDef))
						instr.Operand = m_reflectReader.GetTypeDefAt (GetRid (token));
					else if (IsToken (token, TokenType.TypeRef))
						instr.Operand = m_reflectReader.GetTypeRefAt (GetRid (token));
					else if (IsToken (token, TokenType.TypeSpec))
						instr.Operand = m_reflectReader.GetTypeSpecAt (GetRid (token));
					else if (IsToken (token, TokenType.Field))
						instr.Operand = m_reflectReader.GetFieldDefAt (GetRid (token));
					else if (IsToken (token, TokenType.Method))
						instr.Operand = m_reflectReader.GetMethodDefAt (GetRid (token));
					else if (IsToken (token, TokenType.MemberRef))
						instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (token));
					else
						throw new ReflectionException ("Wrong token following ldtoken: {0}", token.ToString ("x8"));
					break;
				}

				instructions.Add (instr.Offset, instr);

				if (last != null) {
					last.Next = instr;
					instr.Previous = last;
				}

				last = instr;

				code.Add (instr);
			}

			// resolve branches
			foreach (Instruction i in code) {
				switch (i.OpCode.OperandType) {
				case OperandType.ShortInlineBrTarget:
				case OperandType.InlineBrTarget:
					Label lbl = (Label) i.Operand;
					i.Operand = instructions [lbl.Offset];
					break;
				case OperandType.InlineSwitch:
					Label [] lbls = (Label []) i.Operand;
					Instruction [] instrs = new Instruction [lbls.Length];
					for (int j = 0; j < lbls.Length; j++)
						instrs [j] = instructions [lbls [j].Offset] as Instruction;
					i.Operand = instrs;
					break;
				}
			}
		}

		private void ReadSection (MethodBody body, BinaryReader br, IDictionary instructions)
		{
			br.BaseStream.Position += 3;
			br.BaseStream.Position &= ~3;

			byte flags = br.ReadByte ();
			if ((flags & (byte) MethodDataSection.FatFormat) == 0) {
				int length = br.ReadByte () / 12;
				br.ReadBytes (2);

				for (int i = 0; i < length; i++) {
					ExceptionHandler eh = new ExceptionHandler (
						(ExceptionHandlerType) (br.ReadInt16 () & 0x7));
					eh.TryStart = instructions [Convert.ToInt32 (br.ReadInt16 ())] as Instruction;
					eh.TryEnd = instructions [eh.TryStart.Offset + Convert.ToInt32 (br.ReadByte ())] as Instruction;
					eh.HandlerStart = instructions [Convert.ToInt32 (br.ReadInt16 ())] as Instruction;
					eh.HandlerEnd = instructions [eh.HandlerStart.Offset + Convert.ToInt32 (br.ReadByte ())] as Instruction;
					switch (eh.Type) {
					case ExceptionHandlerType.Catch :
						int token = br.ReadInt32 ();
						if (IsToken (token, TokenType.TypeDef))
							eh.CatchType = m_reflectReader.GetTypeDefAt (GetRid (token));
						else
							eh.CatchType = m_reflectReader.GetTypeRefAt (GetRid (token));
						break;
					case ExceptionHandlerType.Filter :
						eh.FilterStart = instructions [br.ReadInt32 ()] as Instruction;
						eh.FilterEnd = instructions [eh.HandlerStart.Previous.Offset] as Instruction;
						break;
					default :
						br.ReadInt32 ();
						break;
					}
					body.ExceptionHandlers.Add (eh);
				}
			} else {
				br.BaseStream.Position--;
				int length = (br.ReadInt32 () >> 8) / 24;
				if ((flags & (int) MethodDataSection.EHTable) == 0)
					br.ReadBytes (length * 24);
				for (int i = 0; i < length; i++) {
					ExceptionHandler eh = new ExceptionHandler (
						(ExceptionHandlerType) (br.ReadInt32 () & 0x7));
					eh.TryStart = instructions [br.ReadInt32 ()] as Instruction;
					eh.TryEnd = instructions [eh.TryStart.Offset + br.ReadInt32 ()] as Instruction;
					eh.HandlerStart = instructions [br.ReadInt32 ()] as Instruction;
					eh.HandlerEnd = instructions [eh.HandlerStart.Offset + br.ReadInt32 ()] as Instruction;
					switch (eh.Type) {
					case ExceptionHandlerType.Catch :
						int token = br.ReadInt32 ();
						if (IsToken (token, TokenType.TypeDef))
							eh.CatchType = m_reflectReader.GetTypeDefAt (GetRid (token));
						else
							eh.CatchType = m_reflectReader.GetTypeRefAt (GetRid (token));
						break;
					case ExceptionHandlerType.Filter :
						eh.FilterStart = instructions [br.ReadInt32 ()] as Instruction;
						eh.FilterEnd = instructions [eh.HandlerStart.Previous.Offset] as Instruction;
						break;
					default :
						br.ReadInt32 ();
						break;
					}
					body.ExceptionHandlers.Add (eh);
				}
			}

			if ((flags & (byte) MethodDataSection.MoreSects) != 0)
				ReadSection (body, br, instructions);
		}

		public override void VisitVariableDefinitionCollection (IVariableDefinitionCollection variables)
		{
			MethodBody body = variables.Container as MethodBody;
			if (body.LocalVarToken == 0)
				return;

			StandAloneSigTable sasTable = m_root.Streams.TablesHeap [typeof (StandAloneSigTable)] as StandAloneSigTable;
			StandAloneSigRow sasRow = sasTable [(int) GetRid (body.LocalVarToken) - 1];
			LocalVarSig sig = m_reflectReader.SigReader.GetLocalVarSig (sasRow.Signature);
			for (int i = 0; i < sig.Count; i++) {
				LocalVarSig.LocalVariable lv = sig.LocalVariables [i];
				ITypeReference varType = m_reflectReader.GetTypeRefFromSig (lv.Type);
				if (lv.ByRef)
					varType = new ReferenceType (varType);
				if ((lv.Constraint & Constraint.Pinned) != 0)
					varType = new PinnedType (varType);

				body.Variables.Add (new VariableDefinition (varType));
			}
		}
	}
}
