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

    public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
    {
        if(method.Body == null)
            return runner.RuleSuccess;

        InstructionCollection instructions = method.Body.Instructions;

        for(int i = 0; i < instructions.Count; i++) {
            Instruction insn = instructions[i];
            if(insn.OpCode.Code == Code.Call ||
	       insn.OpCode.Code == Code.Callvirt) {
                MethodReference mref = (MethodReference)insn.Operand;
                string rName = mref.Name;
                string rDecl = mref.DeclaringType.Name;
                string mName = method.Name;
                string mDecl = method.DeclaringType.Name;
                bool argsEqual =
                    (mref.Parameters.Count == method.Parameters.Count);
                bool namesEqual = rName.Equals(mName);
                // Don't need to compare declaring types if this is a
                // virtual call and the recieving object is the same
                bool typesEqual =
                    insn.OpCode.Code == Code.Callvirt ||
                    rDecl.Equals(mDecl);
                if (argsEqual) {
			argsEqual = CheckParameters (method, mref);
                }
                if(namesEqual && typesEqual && argsEqual) {
                    if(LoadsVerbatimArgs(method, i)) {
                        MessageCollection messages = new MessageCollection();
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

	// note: parameter names do not have to match because we can be
	// calling a base class virtual method
	private bool CheckParameters (MethodReference caller, MethodReference callee)
	{
		for (int j = 0; j < callee.Parameters.Count; j++) {
			ParameterDefinition p1 = (ParameterDefinition) callee.Parameters[j];
			ParameterDefinition p2 = (ParameterDefinition) caller.Parameters[j];

			if (p1.ParameterType.FullName != p2.ParameterType.FullName)
				return false;
		}
		// complete match (of types)
		return true;
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
        switch (insn.OpCode.Code) {
            case Code.Ldarg:
                if((int)insn.Operand == paramNum) return true; break;
	    case Code.Ldarg_S:
                ParameterDefinition param = (ParameterDefinition)insn.Operand;
                if((param.Sequence - 1) == paramNum) return true;
                break;
	    case Code.Ldarg_0:
		if (paramNum == 0)
			return true;
		break;
	    case Code.Ldarg_1:
		if (paramNum == 1)
			return true;
		break;
	    case Code.Ldarg_2:
		if (paramNum == 2)
			return true;
		break;
	    case Code.Ldarg_3:
		if (paramNum == 3)
			return true;
		break;
        }
        return false;
    }
}

}
