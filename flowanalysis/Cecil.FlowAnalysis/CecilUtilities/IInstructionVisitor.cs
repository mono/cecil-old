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

using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.CecilUtilities {
	
	public interface IInstructionVisitor {
		void OnNop (IInstruction instruction);
		void OnBreak (IInstruction instruction);
		void OnLdarg_0 (IInstruction instruction);
		void OnLdarg_1 (IInstruction instruction);
		void OnLdarg_2 (IInstruction instruction);
		void OnLdarg_3 (IInstruction instruction);
		void OnLdloc_0 (IInstruction instruction);
		void OnLdloc_1 (IInstruction instruction);
		void OnLdloc_2 (IInstruction instruction);
		void OnLdloc_3 (IInstruction instruction);
		void OnStloc_0 (IInstruction instruction);
		void OnStloc_1 (IInstruction instruction);
		void OnStloc_2 (IInstruction instruction);
		void OnStloc_3 (IInstruction instruction);
		void OnLdarg (IInstruction instruction);
		void OnLdarga (IInstruction instruction);
		void OnStarg (IInstruction instruction);
		void OnLdloc (IInstruction instruction);
		void OnLdloca (IInstruction instruction);
		void OnStloc (IInstruction instruction);
		void OnLdnull (IInstruction instruction);
		void OnLdc_I4_M1 (IInstruction instruction);
		void OnLdc_I4_0 (IInstruction instruction);
		void OnLdc_I4_1 (IInstruction instruction);
		void OnLdc_I4_2 (IInstruction instruction);
		void OnLdc_I4_3 (IInstruction instruction);
		void OnLdc_I4_4 (IInstruction instruction);
		void OnLdc_I4_5 (IInstruction instruction);
		void OnLdc_I4_6 (IInstruction instruction);
		void OnLdc_I4_7 (IInstruction instruction);
		void OnLdc_I4_8 (IInstruction instruction);
		void OnLdc_I4 (IInstruction instruction);
		void OnLdc_I8 (IInstruction instruction);
		void OnLdc_R4 (IInstruction instruction);
		void OnLdc_R8 (IInstruction instruction);
		void OnDup (IInstruction instruction);
		void OnPop (IInstruction instruction);
		void OnJmp (IInstruction instruction);
		void OnCall (IInstruction instruction);
		void OnCalli (IInstruction instruction);
		void OnRet (IInstruction instruction);
		void OnBr (IInstruction instruction);
		void OnBrfalse (IInstruction instruction);
		void OnBrtrue (IInstruction instruction);
		void OnBeq (IInstruction instruction);
		void OnBge (IInstruction instruction);
		void OnBgt (IInstruction instruction);
		void OnBle (IInstruction instruction);
		void OnBlt (IInstruction instruction);
		void OnBne_Un (IInstruction instruction);
		void OnBge_Un (IInstruction instruction);
		void OnBgt_Un (IInstruction instruction);
		void OnBle_Un (IInstruction instruction);
		void OnBlt_Un (IInstruction instruction);
		void OnSwitch (IInstruction instruction);
		void OnLdind_I1 (IInstruction instruction);
		void OnLdind_U1 (IInstruction instruction);
		void OnLdind_I2 (IInstruction instruction);
		void OnLdind_U2 (IInstruction instruction);
		void OnLdind_I4 (IInstruction instruction);
		void OnLdind_U4 (IInstruction instruction);
		void OnLdind_I8 (IInstruction instruction);
		void OnLdind_I (IInstruction instruction);
		void OnLdind_R4 (IInstruction instruction);
		void OnLdind_R8 (IInstruction instruction);
		void OnLdind_Ref (IInstruction instruction);
		void OnStind_Ref (IInstruction instruction);
		void OnStind_I1 (IInstruction instruction);
		void OnStind_I2 (IInstruction instruction);
		void OnStind_I4 (IInstruction instruction);
		void OnStind_I8 (IInstruction instruction);
		void OnStind_R4 (IInstruction instruction);
		void OnStind_R8 (IInstruction instruction);
		void OnAdd (IInstruction instruction);
		void OnSub (IInstruction instruction);
		void OnMul (IInstruction instruction);
		void OnDiv (IInstruction instruction);
		void OnDiv_Un (IInstruction instruction);
		void OnRem (IInstruction instruction);
		void OnRem_Un (IInstruction instruction);
		void OnAnd (IInstruction instruction);
		void OnOr (IInstruction instruction);
		void OnXor (IInstruction instruction);
		void OnShl (IInstruction instruction);
		void OnShr (IInstruction instruction);
		void OnShr_Un (IInstruction instruction);
		void OnNeg (IInstruction instruction);
		void OnNot (IInstruction instruction);
		void OnConv_I1 (IInstruction instruction);
		void OnConv_I2 (IInstruction instruction);
		void OnConv_I4 (IInstruction instruction);
		void OnConv_I8 (IInstruction instruction);
		void OnConv_R4 (IInstruction instruction);
		void OnConv_R8 (IInstruction instruction);
		void OnConv_U4 (IInstruction instruction);
		void OnConv_U8 (IInstruction instruction);
		void OnCallvirt (IInstruction instruction);
		void OnCpobj (IInstruction instruction);
		void OnLdobj (IInstruction instruction);
		void OnLdstr (IInstruction instruction);
		void OnNewobj (IInstruction instruction);
		void OnCastclass (IInstruction instruction);
		void OnIsinst (IInstruction instruction);
		void OnConv_R_Un (IInstruction instruction);
		void OnUnbox (IInstruction instruction);
		void OnThrow (IInstruction instruction);
		void OnLdfld (IInstruction instruction);
		void OnLdflda (IInstruction instruction);
		void OnStfld (IInstruction instruction);
		void OnLdsfld (IInstruction instruction);
		void OnLdsflda (IInstruction instruction);
		void OnStsfld (IInstruction instruction);
		void OnStobj (IInstruction instruction);
		void OnConv_Ovf_I1_Un (IInstruction instruction);
		void OnConv_Ovf_I2_Un (IInstruction instruction);
		void OnConv_Ovf_I4_Un (IInstruction instruction);
		void OnConv_Ovf_I8_Un (IInstruction instruction);
		void OnConv_Ovf_U1_Un (IInstruction instruction);
		void OnConv_Ovf_U2_Un (IInstruction instruction);
		void OnConv_Ovf_U4_Un (IInstruction instruction);
		void OnConv_Ovf_U8_Un (IInstruction instruction);
		void OnConv_Ovf_I_Un (IInstruction instruction);
		void OnConv_Ovf_U_Un (IInstruction instruction);
		void OnBox (IInstruction instruction);
		void OnNewarr (IInstruction instruction);
		void OnLdlen (IInstruction instruction);
		void OnLdelema (IInstruction instruction);
		void OnLdelem_I1 (IInstruction instruction);
		void OnLdelem_U1 (IInstruction instruction);
		void OnLdelem_I2 (IInstruction instruction);
		void OnLdelem_U2 (IInstruction instruction);
		void OnLdelem_I4 (IInstruction instruction);
		void OnLdelem_U4 (IInstruction instruction);
		void OnLdelem_I8 (IInstruction instruction);
		void OnLdelem_I (IInstruction instruction);
		void OnLdelem_R4 (IInstruction instruction);
		void OnLdelem_R8 (IInstruction instruction);
		void OnLdelem_Ref (IInstruction instruction);
		void OnStelem_I (IInstruction instruction);
		void OnStelem_I1 (IInstruction instruction);
		void OnStelem_I2 (IInstruction instruction);
		void OnStelem_I4 (IInstruction instruction);
		void OnStelem_I8 (IInstruction instruction);
		void OnStelem_R4 (IInstruction instruction);
		void OnStelem_R8 (IInstruction instruction);
		void OnStelem_Ref (IInstruction instruction);
		void OnLdelem_Any (IInstruction instruction);
		void OnStelem_Any (IInstruction instruction);
		void OnUnbox_Any (IInstruction instruction);
		void OnConv_Ovf_I1 (IInstruction instruction);
		void OnConv_Ovf_U1 (IInstruction instruction);
		void OnConv_Ovf_I2 (IInstruction instruction);
		void OnConv_Ovf_U2 (IInstruction instruction);
		void OnConv_Ovf_I4 (IInstruction instruction);
		void OnConv_Ovf_U4 (IInstruction instruction);
		void OnConv_Ovf_I8 (IInstruction instruction);
		void OnConv_Ovf_U8 (IInstruction instruction);
		void OnRefanyval (IInstruction instruction);
		void OnCkfinite (IInstruction instruction);
		void OnMkrefany (IInstruction instruction);
		void OnLdtoken (IInstruction instruction);
		void OnConv_U2 (IInstruction instruction);
		void OnConv_U1 (IInstruction instruction);
		void OnConv_I (IInstruction instruction);
		void OnConv_Ovf_I (IInstruction instruction);
		void OnConv_Ovf_U (IInstruction instruction);
		void OnAdd_Ovf (IInstruction instruction);
		void OnAdd_Ovf_Un (IInstruction instruction);
		void OnMul_Ovf (IInstruction instruction);
		void OnMul_Ovf_Un (IInstruction instruction);
		void OnSub_Ovf (IInstruction instruction);
		void OnSub_Ovf_Un (IInstruction instruction);
		void OnEndfinally (IInstruction instruction);
		void OnLeave (IInstruction instruction);
		void OnStind_I (IInstruction instruction);
		void OnConv_U (IInstruction instruction);
		void OnArglist (IInstruction instruction);
		void OnCeq (IInstruction instruction);
		void OnCgt (IInstruction instruction);
		void OnCgt_Un (IInstruction instruction);
		void OnClt (IInstruction instruction);
		void OnClt_Un (IInstruction instruction);
		void OnLdftn (IInstruction instruction);
		void OnLdvirtftn (IInstruction instruction);
		void OnLocalloc (IInstruction instruction);
		void OnEndfilter (IInstruction instruction);
		void OnUnaligned (IInstruction instruction);
		void OnVolatile (IInstruction instruction);
		void OnTail (IInstruction instruction);
		void OnInitobj (IInstruction instruction);
		void OnCpblk (IInstruction instruction);
		void OnInitblk (IInstruction instruction);
		void OnRethrow (IInstruction instruction);
		void OnSizeof (IInstruction instruction);
		void OnRefanytype (IInstruction instruction);
	}
}
