#region license
//
// (C) db4objects Inc. http://www.db4o.com
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
#endregion

using System;
using System.Collections;
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.CecilUtilities {

	public class InstructionDispatcher {

		public static void Dispatch (Instruction instruction, IInstructionVisitor visitor)
		{
			InstructionVisitorDelegate handler = (InstructionVisitorDelegate)_handlers[instruction.OpCode.Value];
			if (null == handler) throw new ArgumentException (CecilFormatter.FormatInstruction (instruction), "instruction");
			handler (visitor, instruction);
		}

		delegate void InstructionVisitorDelegate (IInstructionVisitor visitor, Instruction instruction);

		static IDictionary _handlers = new Hashtable ();

		static InstructionDispatcher ()
		{
			Bind (new InstructionVisitorDelegate (DispatchNop), OpCodes.Nop);
			Bind (new InstructionVisitorDelegate (DispatchBreak), OpCodes.Break);
			Bind (new InstructionVisitorDelegate (DispatchLdarg_0), OpCodes.Ldarg_0);
			Bind (new InstructionVisitorDelegate (DispatchLdarg_1), OpCodes.Ldarg_1);
			Bind (new InstructionVisitorDelegate (DispatchLdarg_2), OpCodes.Ldarg_2);
			Bind (new InstructionVisitorDelegate (DispatchLdarg_3), OpCodes.Ldarg_3);
			Bind (new InstructionVisitorDelegate (DispatchLdloc_0), OpCodes.Ldloc_0);
			Bind (new InstructionVisitorDelegate (DispatchLdloc_1), OpCodes.Ldloc_1);
			Bind (new InstructionVisitorDelegate (DispatchLdloc_2), OpCodes.Ldloc_2);
			Bind (new InstructionVisitorDelegate (DispatchLdloc_3), OpCodes.Ldloc_3);
			Bind (new InstructionVisitorDelegate (DispatchStloc_0), OpCodes.Stloc_0);
			Bind (new InstructionVisitorDelegate (DispatchStloc_1), OpCodes.Stloc_1);
			Bind (new InstructionVisitorDelegate (DispatchStloc_2), OpCodes.Stloc_2);
			Bind (new InstructionVisitorDelegate (DispatchStloc_3), OpCodes.Stloc_3);
			Bind (new InstructionVisitorDelegate (DispatchLdarg), OpCodes.Ldarg, OpCodes.Ldarg_S);
			Bind (new InstructionVisitorDelegate (DispatchLdarga), OpCodes.Ldarga, OpCodes.Ldarga_S);
			Bind (new InstructionVisitorDelegate (DispatchStarg), OpCodes.Starg, OpCodes.Starg_S);
			Bind (new InstructionVisitorDelegate (DispatchLdloc), OpCodes.Ldloc, OpCodes.Ldloc_S);
			Bind (new InstructionVisitorDelegate (DispatchLdloca), OpCodes.Ldloca, OpCodes.Ldloca_S);
			Bind (new InstructionVisitorDelegate (DispatchStloc), OpCodes.Stloc, OpCodes.Stloc_S);
			Bind (new InstructionVisitorDelegate (DispatchLdnull), OpCodes.Ldnull);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_M1), OpCodes.Ldc_I4_M1);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_0), OpCodes.Ldc_I4_0);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_1), OpCodes.Ldc_I4_1);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_2), OpCodes.Ldc_I4_2);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_3), OpCodes.Ldc_I4_3);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_4), OpCodes.Ldc_I4_4);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_5), OpCodes.Ldc_I4_5);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_6), OpCodes.Ldc_I4_6);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_7), OpCodes.Ldc_I4_7);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4_8), OpCodes.Ldc_I4_8);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I4), OpCodes.Ldc_I4, OpCodes.Ldc_I4_S);
			Bind (new InstructionVisitorDelegate (DispatchLdc_I8), OpCodes.Ldc_I8);
			Bind (new InstructionVisitorDelegate (DispatchLdc_R4), OpCodes.Ldc_R4);
			Bind (new InstructionVisitorDelegate (DispatchLdc_R8), OpCodes.Ldc_R8);
			Bind (new InstructionVisitorDelegate (DispatchDup), OpCodes.Dup);
			Bind (new InstructionVisitorDelegate (DispatchPop), OpCodes.Pop);
			Bind (new InstructionVisitorDelegate (DispatchJmp), OpCodes.Jmp);
			Bind (new InstructionVisitorDelegate (DispatchCall), OpCodes.Call);
			Bind (new InstructionVisitorDelegate (DispatchCalli), OpCodes.Calli);
			Bind (new InstructionVisitorDelegate (DispatchRet), OpCodes.Ret);
			Bind (new InstructionVisitorDelegate (DispatchBr), OpCodes.Br, OpCodes.Br_S);
			Bind (new InstructionVisitorDelegate (DispatchBrfalse), OpCodes.Brfalse, OpCodes.Brfalse_S);
			Bind (new InstructionVisitorDelegate (DispatchBrtrue), OpCodes.Brtrue, OpCodes.Brtrue_S);
			Bind (new InstructionVisitorDelegate (DispatchBeq), OpCodes.Beq, OpCodes.Beq_S);
			Bind (new InstructionVisitorDelegate (DispatchBge), OpCodes.Bge, OpCodes.Bge_S);
			Bind (new InstructionVisitorDelegate (DispatchBgt), OpCodes.Bgt, OpCodes.Bgt_S);
			Bind (new InstructionVisitorDelegate (DispatchBle), OpCodes.Ble, OpCodes.Ble_S);
			Bind (new InstructionVisitorDelegate (DispatchBlt), OpCodes.Blt, OpCodes.Blt_S);
			Bind (new InstructionVisitorDelegate (DispatchBne_Un), OpCodes.Bne_Un, OpCodes.Bne_Un_S);
			Bind (new InstructionVisitorDelegate (DispatchBge_Un), OpCodes.Bge_Un, OpCodes.Bge_Un_S);
			Bind (new InstructionVisitorDelegate (DispatchBgt_Un), OpCodes.Bgt_Un, OpCodes.Bgt_Un_S);
			Bind (new InstructionVisitorDelegate (DispatchBle_Un), OpCodes.Ble_Un, OpCodes.Ble_Un_S);
			Bind (new InstructionVisitorDelegate (DispatchBlt_Un), OpCodes.Blt_Un, OpCodes.Blt_Un_S);
			Bind (new InstructionVisitorDelegate (DispatchSwitch), OpCodes.Switch);
			Bind (new InstructionVisitorDelegate (DispatchLdind_I1), OpCodes.Ldind_I1);
			Bind (new InstructionVisitorDelegate (DispatchLdind_U1), OpCodes.Ldind_U1);
			Bind (new InstructionVisitorDelegate (DispatchLdind_I2), OpCodes.Ldind_I2);
			Bind (new InstructionVisitorDelegate (DispatchLdind_U2), OpCodes.Ldind_U2);
			Bind (new InstructionVisitorDelegate (DispatchLdind_I4), OpCodes.Ldind_I4);
			Bind (new InstructionVisitorDelegate (DispatchLdind_U4), OpCodes.Ldind_U4);
			Bind (new InstructionVisitorDelegate (DispatchLdind_I8), OpCodes.Ldind_I8);
			Bind (new InstructionVisitorDelegate (DispatchLdind_I), OpCodes.Ldind_I);
			Bind (new InstructionVisitorDelegate (DispatchLdind_R4), OpCodes.Ldind_R4);
			Bind (new InstructionVisitorDelegate (DispatchLdind_R8), OpCodes.Ldind_R8);
			Bind (new InstructionVisitorDelegate (DispatchLdind_Ref), OpCodes.Ldind_Ref);
			Bind (new InstructionVisitorDelegate (DispatchStind_Ref), OpCodes.Stind_Ref);
			Bind (new InstructionVisitorDelegate (DispatchStind_I1), OpCodes.Stind_I1);
			Bind (new InstructionVisitorDelegate (DispatchStind_I2), OpCodes.Stind_I2);
			Bind (new InstructionVisitorDelegate (DispatchStind_I4), OpCodes.Stind_I4);
			Bind (new InstructionVisitorDelegate (DispatchStind_I8), OpCodes.Stind_I8);
			Bind (new InstructionVisitorDelegate (DispatchStind_R4), OpCodes.Stind_R4);
			Bind (new InstructionVisitorDelegate (DispatchStind_R8), OpCodes.Stind_R8);
			Bind (new InstructionVisitorDelegate (DispatchAdd), OpCodes.Add);
			Bind (new InstructionVisitorDelegate (DispatchSub), OpCodes.Sub);
			Bind (new InstructionVisitorDelegate (DispatchMul), OpCodes.Mul);
			Bind (new InstructionVisitorDelegate (DispatchDiv), OpCodes.Div);
			Bind (new InstructionVisitorDelegate (DispatchDiv_Un), OpCodes.Div_Un);
			Bind (new InstructionVisitorDelegate (DispatchRem), OpCodes.Rem);
			Bind (new InstructionVisitorDelegate (DispatchRem_Un), OpCodes.Rem_Un);
			Bind (new InstructionVisitorDelegate (DispatchAnd), OpCodes.And);
			Bind (new InstructionVisitorDelegate (DispatchOr), OpCodes.Or);
			Bind (new InstructionVisitorDelegate (DispatchXor), OpCodes.Xor);
			Bind (new InstructionVisitorDelegate (DispatchShl), OpCodes.Shl);
			Bind (new InstructionVisitorDelegate (DispatchShr), OpCodes.Shr);
			Bind (new InstructionVisitorDelegate (DispatchShr_Un), OpCodes.Shr_Un);
			Bind (new InstructionVisitorDelegate (DispatchNeg), OpCodes.Neg);
			Bind (new InstructionVisitorDelegate (DispatchNot), OpCodes.Not);
			Bind (new InstructionVisitorDelegate (DispatchConv_I1), OpCodes.Conv_I1);
			Bind (new InstructionVisitorDelegate (DispatchConv_I2), OpCodes.Conv_I2);
			Bind (new InstructionVisitorDelegate (DispatchConv_I4), OpCodes.Conv_I4);
			Bind (new InstructionVisitorDelegate (DispatchConv_I8), OpCodes.Conv_I8);
			Bind (new InstructionVisitorDelegate (DispatchConv_R4), OpCodes.Conv_R4);
			Bind (new InstructionVisitorDelegate (DispatchConv_R8), OpCodes.Conv_R8);
			Bind (new InstructionVisitorDelegate (DispatchConv_U4), OpCodes.Conv_U4);
			Bind (new InstructionVisitorDelegate (DispatchConv_U8), OpCodes.Conv_U8);
			Bind (new InstructionVisitorDelegate (DispatchCallvirt), OpCodes.Callvirt);
			Bind (new InstructionVisitorDelegate (DispatchCpobj), OpCodes.Cpobj);
			Bind (new InstructionVisitorDelegate (DispatchLdobj), OpCodes.Ldobj);
			Bind (new InstructionVisitorDelegate (DispatchLdstr), OpCodes.Ldstr);
			Bind (new InstructionVisitorDelegate (DispatchNewobj), OpCodes.Newobj);
			Bind (new InstructionVisitorDelegate (DispatchCastclass), OpCodes.Castclass);
			Bind (new InstructionVisitorDelegate (DispatchIsinst), OpCodes.Isinst);
			Bind (new InstructionVisitorDelegate (DispatchConv_R_Un), OpCodes.Conv_R_Un);
			Bind (new InstructionVisitorDelegate (DispatchUnbox), OpCodes.Unbox);
			Bind (new InstructionVisitorDelegate (DispatchThrow), OpCodes.Throw);
			Bind (new InstructionVisitorDelegate (DispatchLdfld), OpCodes.Ldfld);
			Bind (new InstructionVisitorDelegate (DispatchLdflda), OpCodes.Ldflda);
			Bind (new InstructionVisitorDelegate (DispatchStfld), OpCodes.Stfld);
			Bind (new InstructionVisitorDelegate (DispatchLdsfld), OpCodes.Ldsfld);
			Bind (new InstructionVisitorDelegate (DispatchLdsflda), OpCodes.Ldsflda);
			Bind (new InstructionVisitorDelegate (DispatchStsfld), OpCodes.Stsfld);
			Bind (new InstructionVisitorDelegate (DispatchStobj), OpCodes.Stobj);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I1_Un), OpCodes.Conv_Ovf_I1_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I2_Un), OpCodes.Conv_Ovf_I2_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I4_Un), OpCodes.Conv_Ovf_I4_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I8_Un), OpCodes.Conv_Ovf_I8_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U1_Un), OpCodes.Conv_Ovf_U1_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U2_Un), OpCodes.Conv_Ovf_U2_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U4_Un), OpCodes.Conv_Ovf_U4_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U8_Un), OpCodes.Conv_Ovf_U8_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I_Un), OpCodes.Conv_Ovf_I_Un);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U_Un), OpCodes.Conv_Ovf_U_Un);
			Bind (new InstructionVisitorDelegate (DispatchBox), OpCodes.Box);
			Bind (new InstructionVisitorDelegate (DispatchNewarr), OpCodes.Newarr);
			Bind (new InstructionVisitorDelegate (DispatchLdlen), OpCodes.Ldlen);
			Bind (new InstructionVisitorDelegate (DispatchLdelema), OpCodes.Ldelema);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_I1), OpCodes.Ldelem_I1);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_U1), OpCodes.Ldelem_U1);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_I2), OpCodes.Ldelem_I2);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_U2), OpCodes.Ldelem_U2);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_I4), OpCodes.Ldelem_I4);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_U4), OpCodes.Ldelem_U4);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_I8), OpCodes.Ldelem_I8);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_I), OpCodes.Ldelem_I);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_R4), OpCodes.Ldelem_R4);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_R8), OpCodes.Ldelem_R8);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_Ref), OpCodes.Ldelem_Ref);
			Bind (new InstructionVisitorDelegate (DispatchStelem_I), OpCodes.Stelem_I);
			Bind (new InstructionVisitorDelegate (DispatchStelem_I1), OpCodes.Stelem_I1);
			Bind (new InstructionVisitorDelegate (DispatchStelem_I2), OpCodes.Stelem_I2);
			Bind (new InstructionVisitorDelegate (DispatchStelem_I4), OpCodes.Stelem_I4);
			Bind (new InstructionVisitorDelegate (DispatchStelem_I8), OpCodes.Stelem_I8);
			Bind (new InstructionVisitorDelegate (DispatchStelem_R4), OpCodes.Stelem_R4);
			Bind (new InstructionVisitorDelegate (DispatchStelem_R8), OpCodes.Stelem_R8);
			Bind (new InstructionVisitorDelegate (DispatchStelem_Ref), OpCodes.Stelem_Ref);
			Bind (new InstructionVisitorDelegate (DispatchLdelem_Any), OpCodes.Ldelem_Any);
			Bind (new InstructionVisitorDelegate (DispatchStelem_Any), OpCodes.Stelem_Any);
			Bind (new InstructionVisitorDelegate (DispatchUnbox_Any), OpCodes.Unbox_Any);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I1), OpCodes.Conv_Ovf_I1);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U1), OpCodes.Conv_Ovf_U1);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I2), OpCodes.Conv_Ovf_I2);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U2), OpCodes.Conv_Ovf_U2);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I4), OpCodes.Conv_Ovf_I4);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U4), OpCodes.Conv_Ovf_U4);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I8), OpCodes.Conv_Ovf_I8);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U8), OpCodes.Conv_Ovf_U8);
			Bind (new InstructionVisitorDelegate (DispatchRefanyval), OpCodes.Refanyval);
			Bind (new InstructionVisitorDelegate (DispatchCkfinite), OpCodes.Ckfinite);
			Bind (new InstructionVisitorDelegate (DispatchMkrefany), OpCodes.Mkrefany);
			Bind (new InstructionVisitorDelegate (DispatchLdtoken), OpCodes.Ldtoken);
			Bind (new InstructionVisitorDelegate (DispatchConv_U2), OpCodes.Conv_U2);
			Bind (new InstructionVisitorDelegate (DispatchConv_U1), OpCodes.Conv_U1);
			Bind (new InstructionVisitorDelegate (DispatchConv_I), OpCodes.Conv_I);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_I), OpCodes.Conv_Ovf_I);
			Bind (new InstructionVisitorDelegate (DispatchConv_Ovf_U), OpCodes.Conv_Ovf_U);
			Bind (new InstructionVisitorDelegate (DispatchAdd_Ovf), OpCodes.Add_Ovf);
			Bind (new InstructionVisitorDelegate (DispatchAdd_Ovf_Un), OpCodes.Add_Ovf_Un);
			Bind (new InstructionVisitorDelegate (DispatchMul_Ovf), OpCodes.Mul_Ovf);
			Bind (new InstructionVisitorDelegate (DispatchMul_Ovf_Un), OpCodes.Mul_Ovf_Un);
			Bind (new InstructionVisitorDelegate (DispatchSub_Ovf), OpCodes.Sub_Ovf);
			Bind (new InstructionVisitorDelegate (DispatchSub_Ovf_Un), OpCodes.Sub_Ovf_Un);
			Bind (new InstructionVisitorDelegate (DispatchEndfinally), OpCodes.Endfinally);
			Bind (new InstructionVisitorDelegate (DispatchLeave), OpCodes.Leave, OpCodes.Leave_S);
			Bind (new InstructionVisitorDelegate (DispatchStind_I), OpCodes.Stind_I);
			Bind (new InstructionVisitorDelegate (DispatchConv_U), OpCodes.Conv_U);
			Bind (new InstructionVisitorDelegate (DispatchArglist), OpCodes.Arglist);
			Bind (new InstructionVisitorDelegate (DispatchCeq), OpCodes.Ceq);
			Bind (new InstructionVisitorDelegate (DispatchCgt), OpCodes.Cgt);
			Bind (new InstructionVisitorDelegate (DispatchCgt_Un), OpCodes.Cgt_Un);
			Bind (new InstructionVisitorDelegate (DispatchClt), OpCodes.Clt);
			Bind (new InstructionVisitorDelegate (DispatchClt_Un), OpCodes.Clt_Un);
			Bind (new InstructionVisitorDelegate (DispatchLdftn), OpCodes.Ldftn);
			Bind (new InstructionVisitorDelegate (DispatchLdvirtftn), OpCodes.Ldvirtftn);
			Bind (new InstructionVisitorDelegate (DispatchLocalloc), OpCodes.Localloc);
			Bind (new InstructionVisitorDelegate (DispatchEndfilter), OpCodes.Endfilter);
			Bind (new InstructionVisitorDelegate (DispatchUnaligned), OpCodes.Unaligned);
			Bind (new InstructionVisitorDelegate (DispatchVolatile), OpCodes.Volatile);
			Bind (new InstructionVisitorDelegate (DispatchTail), OpCodes.Tail);
			Bind (new InstructionVisitorDelegate (DispatchInitobj), OpCodes.Initobj);
			Bind (new InstructionVisitorDelegate (DispatchCpblk), OpCodes.Cpblk);
			Bind (new InstructionVisitorDelegate (DispatchInitblk), OpCodes.Initblk);
			Bind (new InstructionVisitorDelegate (DispatchRethrow), OpCodes.Rethrow);
			Bind (new InstructionVisitorDelegate (DispatchSizeof), OpCodes.Sizeof);
			Bind (new InstructionVisitorDelegate (DispatchRefanytype), OpCodes.Refanytype);
		}

		static void Bind (InstructionVisitorDelegate handler, params OpCode[] opcodes)
		{
			foreach (OpCode op in opcodes)
			{
				_handlers.Add (op.Value, handler);
			}
		}

		static void DispatchNop (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnNop (instruction);
		}

		static void DispatchBreak (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBreak (instruction);
		}

		static void DispatchLdarg_0 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdarg_0 (instruction);
		}

		static void DispatchLdarg_1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdarg_1 (instruction);
		}

		static void DispatchLdarg_2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdarg_2 (instruction);
		}

		static void DispatchLdarg_3 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdarg_3 (instruction);
		}

		static void DispatchLdloc_0 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdloc_0 (instruction);
		}

		static void DispatchLdloc_1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdloc_1 (instruction);
		}

		static void DispatchLdloc_2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdloc_2 (instruction);
		}

		static void DispatchLdloc_3 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdloc_3 (instruction);
		}

		static void DispatchStloc_0 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStloc_0 (instruction);
		}

		static void DispatchStloc_1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStloc_1 (instruction);
		}

		static void DispatchStloc_2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStloc_2 (instruction);
		}

		static void DispatchStloc_3 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStloc_3 (instruction);
		}

		static void DispatchLdarg (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdarg (instruction);
		}

		static void DispatchLdarga (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdarga (instruction);
		}

		static void DispatchStarg (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStarg (instruction);
		}

		static void DispatchLdloc (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdloc (instruction);
		}

		static void DispatchLdloca (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdloca (instruction);
		}

		static void DispatchStloc (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStloc (instruction);
		}

		static void DispatchLdnull (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdnull (instruction);
		}

		static void DispatchLdc_I4_M1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_M1 (instruction);
		}

		static void DispatchLdc_I4_0 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_0 (instruction);
		}

		static void DispatchLdc_I4_1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_1 (instruction);
		}

		static void DispatchLdc_I4_2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_2 (instruction);
		}

		static void DispatchLdc_I4_3 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_3 (instruction);
		}

		static void DispatchLdc_I4_4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_4 (instruction);
		}

		static void DispatchLdc_I4_5 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_5 (instruction);
		}

		static void DispatchLdc_I4_6 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_6 (instruction);
		}

		static void DispatchLdc_I4_7 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_7 (instruction);
		}

		static void DispatchLdc_I4_8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4_8 (instruction);
		}

		static void DispatchLdc_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I4 (instruction);
		}

		static void DispatchLdc_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_I8 (instruction);
		}

		static void DispatchLdc_R4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_R4 (instruction);
		}

		static void DispatchLdc_R8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdc_R8 (instruction);
		}

		static void DispatchDup (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnDup (instruction);
		}

		static void DispatchPop (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnPop (instruction);
		}

		static void DispatchJmp (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnJmp (instruction);
		}

		static void DispatchCall (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCall (instruction);
		}

		static void DispatchCalli (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCalli (instruction);
		}

		static void DispatchRet (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnRet (instruction);
		}

		static void DispatchBr (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBr (instruction);
		}

		static void DispatchBrfalse (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBrfalse (instruction);
		}

		static void DispatchBrtrue (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBrtrue (instruction);
		}

		static void DispatchBeq (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBeq (instruction);
		}

		static void DispatchBge (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBge (instruction);
		}

		static void DispatchBgt (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBgt (instruction);
		}

		static void DispatchBle (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBle (instruction);
		}

		static void DispatchBlt (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBlt (instruction);
		}

		static void DispatchBne_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBne_Un (instruction);
		}

		static void DispatchBge_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBge_Un (instruction);
		}

		static void DispatchBgt_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBgt_Un (instruction);
		}

		static void DispatchBle_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBle_Un (instruction);
		}

		static void DispatchBlt_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBlt_Un (instruction);
		}

		static void DispatchSwitch (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnSwitch (instruction);
		}

		static void DispatchLdind_I1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_I1 (instruction);
		}

		static void DispatchLdind_U1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_U1 (instruction);
		}

		static void DispatchLdind_I2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_I2 (instruction);
		}

		static void DispatchLdind_U2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_U2 (instruction);
		}

		static void DispatchLdind_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_I4 (instruction);
		}

		static void DispatchLdind_U4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_U4 (instruction);
		}

		static void DispatchLdind_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_I8 (instruction);
		}

		static void DispatchLdind_I (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_I (instruction);
		}

		static void DispatchLdind_R4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_R4 (instruction);
		}

		static void DispatchLdind_R8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_R8 (instruction);
		}

		static void DispatchLdind_Ref (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdind_Ref (instruction);
		}

		static void DispatchStind_Ref (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_Ref (instruction);
		}

		static void DispatchStind_I1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_I1 (instruction);
		}

		static void DispatchStind_I2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_I2 (instruction);
		}

		static void DispatchStind_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_I4 (instruction);
		}

		static void DispatchStind_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_I8 (instruction);
		}

		static void DispatchStind_R4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_R4 (instruction);
		}

		static void DispatchStind_R8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_R8 (instruction);
		}

		static void DispatchAdd (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnAdd (instruction);
		}

		static void DispatchSub (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnSub (instruction);
		}

		static void DispatchMul (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnMul (instruction);
		}

		static void DispatchDiv (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnDiv (instruction);
		}

		static void DispatchDiv_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnDiv_Un (instruction);
		}

		static void DispatchRem (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnRem (instruction);
		}

		static void DispatchRem_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnRem_Un (instruction);
		}

		static void DispatchAnd (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnAnd (instruction);
		}

		static void DispatchOr (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnOr (instruction);
		}

		static void DispatchXor (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnXor (instruction);
		}

		static void DispatchShl (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnShl (instruction);
		}

		static void DispatchShr (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnShr (instruction);
		}

		static void DispatchShr_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnShr_Un (instruction);
		}

		static void DispatchNeg (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnNeg (instruction);
		}

		static void DispatchNot (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnNot (instruction);
		}

		static void DispatchConv_I1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_I1 (instruction);
		}

		static void DispatchConv_I2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_I2 (instruction);
		}

		static void DispatchConv_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_I4 (instruction);
		}

		static void DispatchConv_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_I8 (instruction);
		}

		static void DispatchConv_R4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_R4 (instruction);
		}

		static void DispatchConv_R8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_R8 (instruction);
		}

		static void DispatchConv_U4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_U4 (instruction);
		}

		static void DispatchConv_U8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_U8 (instruction);
		}

		static void DispatchCallvirt (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCallvirt (instruction);
		}

		static void DispatchCpobj (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCpobj (instruction);
		}

		static void DispatchLdobj (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdobj (instruction);
		}

		static void DispatchLdstr (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdstr (instruction);
		}

		static void DispatchNewobj (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnNewobj (instruction);
		}

		static void DispatchCastclass (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCastclass (instruction);
		}

		static void DispatchIsinst (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnIsinst (instruction);
		}

		static void DispatchConv_R_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_R_Un (instruction);
		}

		static void DispatchUnbox (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnUnbox (instruction);
		}

		static void DispatchThrow (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnThrow (instruction);
		}

		static void DispatchLdfld (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdfld (instruction);
		}

		static void DispatchLdflda (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdflda (instruction);
		}

		static void DispatchStfld (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStfld (instruction);
		}

		static void DispatchLdsfld (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdsfld (instruction);
		}

		static void DispatchLdsflda (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdsflda (instruction);
		}

		static void DispatchStsfld (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStsfld (instruction);
		}

		static void DispatchStobj (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStobj (instruction);
		}

		static void DispatchConv_Ovf_I1_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I1_Un (instruction);
		}

		static void DispatchConv_Ovf_I2_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I2_Un (instruction);
		}

		static void DispatchConv_Ovf_I4_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I4_Un (instruction);
		}

		static void DispatchConv_Ovf_I8_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I8_Un (instruction);
		}

		static void DispatchConv_Ovf_U1_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U1_Un (instruction);
		}

		static void DispatchConv_Ovf_U2_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U2_Un (instruction);
		}

		static void DispatchConv_Ovf_U4_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U4_Un (instruction);
		}

		static void DispatchConv_Ovf_U8_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U8_Un (instruction);
		}

		static void DispatchConv_Ovf_I_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I_Un (instruction);
		}

		static void DispatchConv_Ovf_U_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U_Un (instruction);
		}

		static void DispatchBox (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnBox (instruction);
		}

		static void DispatchNewarr (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnNewarr (instruction);
		}

		static void DispatchLdlen (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdlen (instruction);
		}

		static void DispatchLdelema (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelema (instruction);
		}

		static void DispatchLdelem_I1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_I1 (instruction);
		}

		static void DispatchLdelem_U1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_U1 (instruction);
		}

		static void DispatchLdelem_I2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_I2 (instruction);
		}

		static void DispatchLdelem_U2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_U2 (instruction);
		}

		static void DispatchLdelem_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_I4 (instruction);
		}

		static void DispatchLdelem_U4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_U4 (instruction);
		}

		static void DispatchLdelem_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_I8 (instruction);
		}

		static void DispatchLdelem_I (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_I (instruction);
		}

		static void DispatchLdelem_R4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_R4 (instruction);
		}

		static void DispatchLdelem_R8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_R8 (instruction);
		}

		static void DispatchLdelem_Ref (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_Ref (instruction);
		}

		static void DispatchStelem_I (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_I (instruction);
		}

		static void DispatchStelem_I1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_I1 (instruction);
		}

		static void DispatchStelem_I2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_I2 (instruction);
		}

		static void DispatchStelem_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_I4 (instruction);
		}

		static void DispatchStelem_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_I8 (instruction);
		}

		static void DispatchStelem_R4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_R4 (instruction);
		}

		static void DispatchStelem_R8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_R8 (instruction);
		}

		static void DispatchStelem_Ref (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_Ref (instruction);
		}

		static void DispatchLdelem_Any (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdelem_Any (instruction);
		}

		static void DispatchStelem_Any (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStelem_Any (instruction);
		}

		static void DispatchUnbox_Any (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnUnbox_Any (instruction);
		}

		static void DispatchConv_Ovf_I1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I1 (instruction);
		}

		static void DispatchConv_Ovf_U1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U1 (instruction);
		}

		static void DispatchConv_Ovf_I2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I2 (instruction);
		}

		static void DispatchConv_Ovf_U2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U2 (instruction);
		}

		static void DispatchConv_Ovf_I4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I4 (instruction);
		}

		static void DispatchConv_Ovf_U4 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U4 (instruction);
		}

		static void DispatchConv_Ovf_I8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I8 (instruction);
		}

		static void DispatchConv_Ovf_U8 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U8 (instruction);
		}

		static void DispatchRefanyval (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnRefanyval (instruction);
		}

		static void DispatchCkfinite (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCkfinite (instruction);
		}

		static void DispatchMkrefany (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnMkrefany (instruction);
		}

		static void DispatchLdtoken (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdtoken (instruction);
		}

		static void DispatchConv_U2 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_U2 (instruction);
		}

		static void DispatchConv_U1 (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_U1 (instruction);
		}

		static void DispatchConv_I (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_I (instruction);
		}

		static void DispatchConv_Ovf_I (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_I (instruction);
		}

		static void DispatchConv_Ovf_U (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_Ovf_U (instruction);
		}

		static void DispatchAdd_Ovf (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnAdd_Ovf (instruction);
		}

		static void DispatchAdd_Ovf_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnAdd_Ovf_Un (instruction);
		}

		static void DispatchMul_Ovf (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnMul_Ovf (instruction);
		}

		static void DispatchMul_Ovf_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnMul_Ovf_Un (instruction);
		}

		static void DispatchSub_Ovf (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnSub_Ovf (instruction);
		}

		static void DispatchSub_Ovf_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnSub_Ovf_Un (instruction);
		}

		static void DispatchEndfinally (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnEndfinally (instruction);
		}

		static void DispatchLeave (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLeave (instruction);
		}

		static void DispatchStind_I (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnStind_I (instruction);
		}

		static void DispatchConv_U (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnConv_U (instruction);
		}

		static void DispatchArglist (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnArglist (instruction);
		}

		static void DispatchCeq (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCeq (instruction);
		}

		static void DispatchCgt (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCgt (instruction);
		}

		static void DispatchCgt_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCgt_Un (instruction);
		}

		static void DispatchClt (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnClt (instruction);
		}

		static void DispatchClt_Un (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnClt_Un (instruction);
		}

		static void DispatchLdftn (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdftn (instruction);
		}

		static void DispatchLdvirtftn (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLdvirtftn (instruction);
		}

		static void DispatchLocalloc (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnLocalloc (instruction);
		}

		static void DispatchEndfilter (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnEndfilter (instruction);
		}

		static void DispatchUnaligned (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnUnaligned (instruction);
		}

		static void DispatchVolatile (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnVolatile (instruction);
		}

		static void DispatchTail (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnTail (instruction);
		}

		static void DispatchInitobj (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnInitobj (instruction);
		}

		static void DispatchCpblk (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnCpblk (instruction);
		}

		static void DispatchInitblk (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnInitblk (instruction);
		}

		static void DispatchRethrow (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnRethrow (instruction);
		}

		static void DispatchSizeof (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnSizeof (instruction);
		}

		static void DispatchRefanytype (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.OnRefanytype (instruction);
		}
	}
}

