//
// CodeReader.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil.Cil {

	using System;
	using System.Collections;
	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	class CodeReader : BaseCodeVisitor {

		ReflectionReader m_reflectReader;
		MetadataRoot m_root;

		public CodeReader (ReflectionReader reflectReader)
		{
			m_reflectReader = reflectReader;
			m_root = m_reflectReader.MetadataRoot;
		}

		public override void VisitMethodBody (MethodBody body)
		{
			MethodDefinition meth = body.Method;
			MethodBody methBody = body;
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

		public static uint GetRid (int token)
		{
			return (uint) token & 0x00ffffff;
		}

		public static bool IsToken (int token, TokenType t)
		{
			return token >> 24 == (int) t >> 24;
		}

		ParameterDefinition GetParameter (MethodBody body, int index)
		{
			if (body.Method.HasThis) {
				if (index == 0)
					return body.Method.This;
				index--;
			}

			return body.Method.Parameters [index];
		}

		void ReadCilBody (MethodBody body, BinaryReader br, out IDictionary instructions)
		{
			long start = br.BaseStream.Position, offset;
			Instruction last = null;
			InstructionCollection code = body.Instructions;
			instructions = new Hashtable ();
			GenericContext context = new GenericContext (body.Method);

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
				case OperandType.InlineSwitch :
					uint length = br.ReadUInt32 ();
					int [] branches = new int [length];
					int [] buf = new int [length];
					for (int i = 0; i < length; i++)
						buf [i] = br.ReadInt32 ();
					for (int i = 0; i < length; i++)
						branches [i] = Convert.ToInt32 (br.BaseStream.Position - start + buf [i]);
					instr.Operand = branches;
					break;
				case OperandType.ShortInlineBrTarget :
					sbyte sbrtgt = br.ReadSByte ();
					instr.Operand = Convert.ToInt32 (br.BaseStream.Position - start + sbrtgt);
					break;
				case OperandType.InlineBrTarget :
					int brtgt = br.ReadInt32 ();
					instr.Operand = Convert.ToInt32 (br.BaseStream.Position - start + brtgt);
					break;
				case OperandType.ShortInlineI :
					if (op == OpCodes.Ldc_I4_S)
						instr.Operand = br.ReadSByte ();
					else
						instr.Operand = br.ReadByte ();
					break;
				case OperandType.ShortInlineVar :
					instr.Operand = body.Variables [br.ReadByte ()];
					break;
				case OperandType.ShortInlineParam :
					instr.Operand = GetParameter (body, br.ReadByte ());
					break;
				case OperandType.InlineSig :
					instr.Operand = GetCallSiteAt (br.ReadInt32 (), context);
					break;
				case OperandType.InlineI :
					instr.Operand = br.ReadInt32 ();
					break;
				case OperandType.InlineVar :
					instr.Operand = body.Variables [br.ReadInt16 ()];
					break;
				case OperandType.InlineParam :
					instr.Operand = GetParameter (body, br.ReadInt16 ());
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
						instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (field), context);
					else
						throw new ReflectionException ("Wrong token for InlineField Operand: {0}", field.ToString ("x8"));
					break;
				case OperandType.InlineMethod :
					int meth = br.ReadInt32 ();
					if (IsToken (meth, TokenType.Method))
						instr.Operand = m_reflectReader.GetMethodDefAt (GetRid (meth));
					else if (IsToken (meth, TokenType.MemberRef))
						instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (meth), context);
					else if (IsToken (meth, TokenType.MethodSpec))
						instr.Operand = m_reflectReader.GetMethodSpecAt (GetRid (meth), context);
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
						instr.Operand = m_reflectReader.GetTypeSpecAt (GetRid (type), context);
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
						instr.Operand = m_reflectReader.GetTypeSpecAt (GetRid (token), context);
					else if (IsToken (token, TokenType.Field))
						instr.Operand = m_reflectReader.GetFieldDefAt (GetRid (token));
					else if (IsToken (token, TokenType.Method))
						instr.Operand = m_reflectReader.GetMethodDefAt (GetRid (token));
					else if (IsToken (token, TokenType.MethodSpec))
						instr.Operand = m_reflectReader.GetMethodSpecAt (GetRid (token), context);
					else if (IsToken (token, TokenType.MemberRef))
						instr.Operand = m_reflectReader.GetMemberRefAt (GetRid (token), context);
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
					i.Operand = GetInstruction (body, instructions, (int) i.Operand);
					break;
				case OperandType.InlineSwitch:
					int [] lbls = (int []) i.Operand;
					Instruction [] instrs = new Instruction [lbls.Length];
					for (int j = 0; j < lbls.Length; j++)
						instrs [j] = GetInstruction (body, instructions, lbls [j]);
					i.Operand = instrs;
					break;
				}
			}
		}

		Instruction GetInstruction (MethodBody body, IDictionary instructions, int offset)
		{
			Instruction instruction = instructions [offset] as Instruction;
			if (instruction != null)
				return instruction;

			return body.Instructions.Outside;
		}

		void ReadSection (MethodBody body, BinaryReader br, IDictionary instructions)
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
					eh.TryStart = GetInstruction (body, instructions, Convert.ToInt32 (br.ReadInt16 ()));
					eh.TryEnd = GetInstruction (body, instructions, eh.TryStart.Offset + Convert.ToInt32 (br.ReadByte ()));
					eh.HandlerStart = GetInstruction (body, instructions, Convert.ToInt32 (br.ReadInt16 ()));
					eh.HandlerEnd = GetInstruction (body, instructions, eh.HandlerStart.Offset + Convert.ToInt32 (br.ReadByte ()));
					switch (eh.Type) {
					case ExceptionHandlerType.Catch :
						int token = br.ReadInt32 ();
						if (IsToken (token, TokenType.TypeDef))
							eh.CatchType = m_reflectReader.GetTypeDefAt (GetRid (token));
						else
							eh.CatchType = m_reflectReader.GetTypeRefAt (GetRid (token));
						break;
					case ExceptionHandlerType.Filter :
						eh.FilterStart = GetInstruction (body, instructions, br.ReadInt32 ());
						eh.FilterEnd = GetInstruction (body, instructions, eh.HandlerStart.Previous.Offset);
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
					eh.TryStart = GetInstruction (body, instructions, br.ReadInt32 ());
					eh.TryEnd = GetInstruction (body, instructions, eh.TryStart.Offset + br.ReadInt32 ());
					eh.HandlerStart = GetInstruction (body, instructions, br.ReadInt32 ());
					eh.HandlerEnd = GetInstruction (body, instructions, eh.HandlerStart.Offset + br.ReadInt32 ());
					switch (eh.Type) {
					case ExceptionHandlerType.Catch :
						int token = br.ReadInt32 ();
						if (IsToken (token, TokenType.TypeDef))
							eh.CatchType = m_reflectReader.GetTypeDefAt (GetRid (token));
						else
							eh.CatchType = m_reflectReader.GetTypeRefAt (GetRid (token));
						break;
					case ExceptionHandlerType.Filter :
						eh.FilterStart = GetInstruction (body, instructions, br.ReadInt32 ());
						eh.FilterEnd = GetInstruction (body, instructions, eh.HandlerStart.Previous.Offset);
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

		CallSite GetCallSiteAt (int token, GenericContext context)
		{
			StandAloneSigTable sasTable = m_reflectReader.TableReader.GetStandAloneSigTable ();
			MethodSig ms = m_reflectReader.SigReader.GetStandAloneMethodSig (
				sasTable [(int) GetRid (token) - 1].Signature);
			CallSite cs = new CallSite (ms.HasThis, ms.ExplicitThis,
				ms.MethCallConv, m_reflectReader.GetMethodReturnType (ms, context));
			cs.MetadataToken = new MetadataToken (TokenType.Signature, GetRid (token));

			for (int i = 0; i < ms.ParamCount; i++) {
				Param p = ms.Parameters [i];
				cs.Parameters.Add (m_reflectReader.BuildParameterDefinition (
						string.Concat ("A_", i),
						i, (ParameterAttributes) 0,
						p, context));
			}

			return cs;
		}

		public override void VisitVariableDefinitionCollection (VariableDefinitionCollection variables)
		{
			MethodBody body = variables.Container;
			if (body.LocalVarToken == 0)
				return;

			StandAloneSigTable sasTable = m_reflectReader.TableReader.GetStandAloneSigTable ();
			StandAloneSigRow sasRow = sasTable [(int) GetRid (body.LocalVarToken) - 1];
			LocalVarSig sig = m_reflectReader.SigReader.GetLocalVarSig (sasRow.Signature);
			for (int i = 0; i < sig.Count; i++) {
				LocalVarSig.LocalVariable lv = sig.LocalVariables [i];
				TypeReference varType = m_reflectReader.GetTypeRefFromSig (
					lv.Type, new GenericContext (body.Method));

				if (lv.ByRef)
					varType = new ReferenceType (varType);
				if ((lv.Constraint & Constraint.Pinned) != 0)
					varType = new PinnedType (varType);

				if (lv.CustomMods.Length > 0)
					varType = m_reflectReader.GetModifierType (lv.CustomMods, varType);

				body.Variables.Add (new VariableDefinition (
						string.Concat ("V_", i), i, body.Method, varType));
			}
		}
	}
}
