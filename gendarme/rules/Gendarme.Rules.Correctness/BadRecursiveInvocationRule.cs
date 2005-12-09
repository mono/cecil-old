/*
 * BadRecChecker.cs: looks for instances of problematic recursive
 * invocations.
 *
 * Authors:
 *   Aaron Tomb <atomb@soe.ucsc.edu>
 *
 * Copyright (c) 2005 Aaron Tomb and the contributors listed
 * in the ChangeLog.
 *
 * This is free software, distributed under the MIT/X11 license.
 * See the included MIT.X11 file for details.
 **********************************************************************/

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

public class BadRecursiveInvocationRule : IMethodRule {

    public bool CheckMethod (IAssemblyDefinition assembly, IModuleDefinition module,
            ITypeDefinition type, IMethodDefinition method)
    {
        if(method.Body == null)
            return true;

        IInstructionCollection instructions = method.Body.Instructions;

        for(int i = 0; i < instructions.Count; i++) {
            IInstruction insn = instructions[i];
            if(insn.OpCode.Value == OpCodeConstants.Call ||
               insn.OpCode.Value == OpCodeConstants.Callvirt) {
                IMethodReference mref = (IMethodReference)insn.Operand;
                string rName = mref.Name;
                string rDecl = mref.DeclaringType.Name;
                string mName = method.Name;
                string mDecl = method.DeclaringType.Name;
                bool argsEqual = 
                    (mref.Parameters.Count == method.Parameters.Count);
                if(argsEqual) {
                    for(int j = 0; j < mref.Parameters.Count; j++) {
                        IParameterDefinition p1 =
                            (IParameterDefinition)mref.Parameters[j];
                        IParameterDefinition p2 =
                            (IParameterDefinition)method.Parameters[j];

                        if(!p1.Name.Equals(p2.Name))
                            argsEqual = false;

                        string tn1 = p1.ParameterType.FullName;
                        string tn2 = p2.ParameterType.FullName;
                        if(!tn1.Equals(tn2))
                            argsEqual = false;
                    }
                }
                if(rName.Equals(mName) && rDecl.Equals(mDecl) && argsEqual) {
                    if(LoadsVerbatimArgs(method, i)) {
                        return false;
                        /* TODO: restore the ability to generate
                         * messages. */
                        /*
                        string etype = method.DeclaringType.FullName;
                        Location loc = new Location(etype, method.Name,
                                insn.Offset);
                        report.AddMessage(new Message("suspicious recursive call",
                                    loc, MessageType.Warning));
                                    */
                    }
                }
            }
        }
        return true;
    }

    private bool LoadsVerbatimArgs([NonNull] IMethodDefinition method,
            int callIndex)
    {
        int pcount = method.Parameters.Count;
        IInstructionCollection instructions = method.Body.Instructions;

        if(callIndex <= pcount)
            return false;
        if(method.HasThis)
            pcount++;

        for(int i = 0; i < pcount; i++)
           if(!IsLoadParam(instructions[(callIndex - pcount) + i], i))
               return false;
        return true;
    }

    private bool IsLoadParam([NonNull] IInstruction insn, int paramNum)
    {
        switch((int)insn.OpCode.Value) {
            case OpCodeConstants.Ldarg:
                if((int)insn.Operand == paramNum) return true; break;
            case OpCodeConstants.Ldarg_S:
                if((byte)insn.Operand == paramNum) return true; break;
            case OpCodeConstants.Ldarg_0: if(paramNum == 0) return true; break;
            case OpCodeConstants.Ldarg_1: if(paramNum == 1) return true; break;
            case OpCodeConstants.Ldarg_2: if(paramNum == 2) return true; break;
            case OpCodeConstants.Ldarg_3: if(paramNum == 3) return true; break;
        }
        return false;
    }
}
