/*
 * BadRecursiveInvocationRule.cs: looks for instances of problematic
 * recursive invocations.
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
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Correctness {

public class BadRecursiveInvocationRule : IMethodRule {

    public IList CheckMethod (AssemblyDefinition assembly,
            ModuleDefinition module, TypeDefinition type,
            MethodDefinition method, Runner runner)
    {
        if(method.Body == null)
            return runner.RuleSuccess;

        InstructionCollection instructions = method.Body.Instructions;

        for(int i = 0; i < instructions.Count; i++) {
            Instruction insn = instructions[i];
            if(insn.OpCode.Value == OpCodeConstants.Call ||
               insn.OpCode.Value == OpCodeConstants.Callvirt) {
                MethodReference mref = (MethodReference)insn.Operand;
                string rName = mref.Name;
                string rDecl = mref.DeclaringType.Name;
                string mName = method.Name;
                string mDecl = method.DeclaringType.Name;
                bool argsEqual = 
                    (mref.Parameters.Count == method.Parameters.Count);
                if(argsEqual) {
                    for(int j = 0; j < mref.Parameters.Count; j++) {
                        ParameterDefinition p1 =
                            (ParameterDefinition)mref.Parameters[j];
                        ParameterDefinition p2 =
                            (ParameterDefinition)method.Parameters[j];

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
                        IList messages = new ArrayList();
                        string etype = method.DeclaringType.FullName;
                        Location loc = new Location(etype, method.Name,
                                insn.Offset);
                        Message msg = new Message("suspicious recursive call",
                                    loc, MessageType.Warning);
                        messages.Add(msg);
                        return messages;
                    }
                }
            }
        }
        return runner.RuleSuccess;
    }

    private bool LoadsVerbatimArgs([NonNull] MethodDefinition method,
            int callIndex)
    {
        int pcount = method.Parameters.Count;
        InstructionCollection instructions = method.Body.Instructions;

        if(callIndex <= pcount)
            return false;
        if(method.HasThis)
            pcount++;

        for(int i = 0; i < pcount; i++)
           if(!IsLoadParam(instructions[(callIndex - pcount) + i], i))
               return false;
        return true;
    }

    private bool IsLoadParam([NonNull] Instruction insn, int paramNum)
    {
        switch((int)insn.OpCode.Value) {
            case OpCodeConstants.Ldarg:
                if((int)insn.Operand == paramNum) return true; break;
            case OpCodeConstants.Ldarg_S:
                ParameterDefinition param = (ParameterDefinition)insn.Operand;
                if((param.Sequence - 1) == paramNum) return true;
                break;
            case OpCodeConstants.Ldarg_0: if(paramNum == 0) return true; break;
            case OpCodeConstants.Ldarg_1: if(paramNum == 1) return true; break;
            case OpCodeConstants.Ldarg_2: if(paramNum == 2) return true; break;
            case OpCodeConstants.Ldarg_3: if(paramNum == 3) return true; break;
        }
        return false;
    }
}

}
