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
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.CecilUtilities {
	
	public class AbstractInstructionVisitor : IInstructionVisitor {
		public virtual void OnNop (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBreak (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdarg_0 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdarg_1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdarg_2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdarg_3 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdloc_0 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdloc_1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdloc_2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdloc_3 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStloc_0 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStloc_1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStloc_2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStloc_3 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdarg (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdarga (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStarg (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdloc (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdloca (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStloc (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdnull (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_M1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_0 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_3 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_5 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_6 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_7 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4_8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_R4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdc_R8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnDup (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnPop (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnJmp (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCall (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCalli (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnRet (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBr (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBrfalse (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBrtrue (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBeq (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBge (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBgt (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBle (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBlt (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBne_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBge_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBgt_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBle_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBlt_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnSwitch (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_I1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_U1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_I2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_U2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_U4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_I (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_R4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_R8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdind_Ref (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_Ref (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_I1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_I2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_R4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_R8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnAdd (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnSub (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnMul (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnDiv (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnDiv_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnRem (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnRem_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnAnd (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnOr (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnXor (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnShl (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnShr (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnShr_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnNeg (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnNot (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_I1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_I2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_R4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_R8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_U4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_U8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCallvirt (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCpobj (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdobj (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdstr (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnNewobj (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCastclass (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnIsinst (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_R_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnUnbox (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnThrow (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdfld (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdflda (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStfld (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdsfld (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdsflda (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStsfld (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStobj (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I1_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I2_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I4_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I8_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U1_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U2_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U4_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U8_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnBox (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnNewarr (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdlen (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelema (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_I1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_U1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_I2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_U2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_U4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_I (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_R4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_R8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_Ref (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_I (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_I1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_I2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_R4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_R8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_Ref (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdelem_Any (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStelem_Any (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnUnbox_Any (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U4 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U8 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnRefanyval (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCkfinite (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnMkrefany (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdtoken (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_U2 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_U1 (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_I (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_I (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_Ovf_U (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnAdd_Ovf (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnAdd_Ovf_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnMul_Ovf (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnMul_Ovf_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnSub_Ovf (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnSub_Ovf_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnEndfinally (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLeave (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnStind_I (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnConv_U (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnArglist (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCeq (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCgt (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCgt_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnClt (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnClt_Un (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdftn (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLdvirtftn (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnLocalloc (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnEndfilter (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnUnaligned (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnVolatile (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnTail (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnInitobj (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnCpblk (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnInitblk (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnRethrow (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnSizeof (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public virtual void OnRefanytype (IInstruction instruction)
		{
			throw new NotImplementedException (CecilFormatter.FormatInstruction (instruction));
		}

		public void Visit (IInstruction instruction)
		{
			InstructionDispatcher.Dispatch (instruction, this);
		}
	}
}
