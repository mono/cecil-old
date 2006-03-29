/*
 * NullDerefAnalysis.cs: dataflow analysis details for null-pointer
 * dereference detection.
 *
 * Authors:
 *   Aaron Tomb <atomb@soe.ucsc.edu>
 *
 * Copyright (c) 2005 Aaron Tomb and the contributors listed
 * in the ChangeLog.
 *
 * This is free software, distributed under the MIT/X11 license.
 * See the included LICENSE.MIT file for details.
 **********************************************************************/

using System;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Correctness {

public class NullDerefAnalysis : OpCodeConstants, IDataflowAnalysis {

    int stackDepth;
    int locals;
    int args;
    [NonNull] private IMethodDefinition method;
    [NonNull] private NonNullAttributeCollector nnaCollector;
    [NonNull] private IList messages;
    [NonNull] private Runner runner;

    public NullDerefAnalysis([NonNull] IMethodDefinition method,
            [NonNull] IList messages,
            [NonNull] NonNullAttributeCollector nnaCollector,
            [NonNull] Runner runner)
    {
        this.stackDepth = method.Body.MaxStack;
        this.locals = method.Body.Variables.Count;
        if(method.HasThis)
            this.args = method.Parameters.Count + 1;
        else
            this.args = method.Parameters.Count;
        this.method = method;
        this.messages = messages;
        this.nnaCollector = nnaCollector;
        this.runner = runner;
    }
    
    [NonNull]
    public object NewTop()
    {
        return new NullDerefFrame(stackDepth, locals, args, false, runner);
    }

    [NonNull]
    public object NewEntry()
    {
        NullDerefFrame result =
            new NullDerefFrame(stackDepth, locals, args, true, runner);
        if(method.HasThis)
            result.SetArgNullity(0, Nullity.NonNull);
        foreach(IParameterReference param in method.Parameters)
            if(nnaCollector.HasNonNullAttribute(method, param))
                result.SetArgNullity(param.Sequence - 1, Nullity.NonNull);
        return result;
    }

    [NonNull]
    public object NewCatch()
    {
        NullDerefFrame result =
            new NullDerefFrame(stackDepth, locals, args, true, runner);
        if(method.HasThis)
            result.SetArgNullity(0, Nullity.NonNull);
        foreach(IParameterReference param in method.Parameters)
            if(nnaCollector.HasNonNullAttribute(method, param))
                result.SetArgNullity(param.Sequence - 1, Nullity.NonNull);
        /* The exception being caught is pushed onto the stack. */
        result.PushStack(Nullity.NonNull);
        return result;
    }

    /* Changes originalFact. */
    public void MeetInto([NonNull] object originalFact,
            [NonNull] object newFact, bool warn)
    {
        NullDerefFrame original = (NullDerefFrame)originalFact;
        NullDerefFrame incoming = (NullDerefFrame)newFact;
        original.MergeWith(incoming);
    }

    private bool IsVoid([NonNull] ITypeReference type)
    {
        if(type.FullName.Equals("System.Void"))
            return true;
        return false;
    }

    public void Transfer([NonNull] Node node, [NonNull] object inFact,
            [NonNull] object outFact, bool warn)
    {
        BasicBlock bb = (BasicBlock)node;

        /* Exit and exception nodes don't cover any real instructions. */
        if(bb.isExit || bb.isException)
            return;

        //NullDerefFrame inFrame = (NullDerefFrame)inFact;
        NullDerefFrame outFrame = (NullDerefFrame)outFact;
        IVariableDefinitionCollection vars = method.Body.Variables;

        if(runner.Debug) {
            Console.WriteLine("Basic block {0}", bb.ToString());
            Console.WriteLine("Input frame:");
            Console.Write(outFrame.ToString());
        }
        
        for(int i = bb.first; i <= bb.last; i++) {
            IInstruction insn = bb.Instructions[i];
            OpCode opcode = insn.OpCode;

            if(runner.Debug) {
                Console.Write("{0}", opcode.Name);
                if(insn.Operand != null && !(insn.Operand is IInstruction)) {
                    Console.WriteLine(" {0}", insn.Operand.ToString());
                } else if(insn.Operand is IInstruction) {
                    Console.WriteLine(" {0}",
                            ((IInstruction)insn.Operand).Offset.ToString("X4"));
                } else {
                    Console.WriteLine();
                }
            }

            switch((int)((ushort)opcode.Value)) {
                /* Load argument */
                /* Stored nullities are set to declared values on method
                 * entry. Starg and kin can change this over time. */
                case Ldarg_0:
                    outFrame.PushStack(outFrame.GetArgNullity(0));
                    break;
                case Ldarg_1:
                    outFrame.PushStack(outFrame.GetArgNullity(1));
                    break;
                case Ldarg_2:
                    outFrame.PushStack(outFrame.GetArgNullity(2));
                    break;
                case Ldarg_3:
                    outFrame.PushStack(outFrame.GetArgNullity(3));
                    break;
                case Ldarg:
                    outFrame.PushStack(
                            outFrame.GetArgNullity((int)insn.Operand));
                    break;
                case Ldarg_S: {
                    IParameterReference param =
                        (IParameterReference)insn.Operand;
                    outFrame.PushStack(
                            outFrame.GetArgNullity(param.Sequence - 1));
                    break;
                }
                case Ldarga:
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldarga_S:
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                /* Store argument */
                case Starg:
                    outFrame.SetArgNullity((int)insn.Operand,
                            outFrame.PopStack());
                    break;
                case Starg_S: {
                    IParameterReference param =
                        (IParameterReference)insn.Operand;
                    outFrame.SetArgNullity(param.Sequence - 1, 
                            outFrame.PopStack());
                    break;
                }

                /* Load local */
                case Ldloc_0:
                    outFrame.PushStack(outFrame.GetLocNullity(0));
                    break;
                case Ldloc_1:
                    outFrame.PushStack(outFrame.GetLocNullity(1));
                    break;
                case Ldloc_2:
                    outFrame.PushStack(outFrame.GetLocNullity(2));
                    break;
                case Ldloc_3:
                    outFrame.PushStack(outFrame.GetLocNullity(3));
                    break;
                case Ldloc:
                    outFrame.PushStack(outFrame.GetLocNullity(
                        vars.IndexOf((VariableDefinition)insn.Operand)));
                    break;
                case Ldloc_S:
                    outFrame.PushStack(outFrame.GetLocNullity(
                        vars.IndexOf((VariableDefinition)insn.Operand)));
                    break;
                case Ldloca:
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldloca_S:
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                /* Store local */
                case Stloc_0:
                    outFrame.SetLocNullity(0, outFrame.PopStack());
                    break;
                case Stloc_1:
                    outFrame.SetLocNullity(1, outFrame.PopStack());
                    break;
                case Stloc_2:
                    outFrame.SetLocNullity(2, outFrame.PopStack());
                    break;
                case Stloc_3:
                    outFrame.SetLocNullity(3, outFrame.PopStack());
                    break;
                case Stloc:
                    outFrame.SetLocNullity(
                        vars.IndexOf((VariableDefinition)insn.Operand),
                        outFrame.PopStack());
                    break;
                case Stloc_S:
                    outFrame.SetLocNullity(
                        vars.IndexOf((VariableDefinition)insn.Operand),
                        outFrame.PopStack());
                    break;

                /* Load other things */
                case Ldftn:
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldvirtftn:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldstr:
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldnull:
                    outFrame.PushStack(Nullity.Null);
                    break;
                case Ldlen:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldtoken:
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                 /* Object operations */
                case Cpobj: outFrame.PopStack(2); break;
                case Newobj:
                    outFrame.PopStack(
                        ((IMethodReference)insn.Operand).Parameters.Count);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldobj:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Stobj: outFrame.PopStack(2); break;
                case Initobj: outFrame.PopStack(); break;

                 /* Load field */
                case Ldfld: {
                    Check(insn, warn, outFrame.PopStack(), "field");
                    IFieldReference field = (IFieldReference)insn.Operand;
                    if(nnaCollector.HasNonNullAttribute(field))
                        outFrame.PushStack(Nullity.NonNull);
                    else
                        outFrame.PushStack(Nullity.Unknown);
                    break;
                }
                case Ldflda:
                    Check(insn, warn, outFrame.PopStack(), "field");
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldsfld: {
                    IFieldReference field = (IFieldReference)insn.Operand;
                    if(nnaCollector.HasNonNullAttribute(field))
                        outFrame.PushStack(Nullity.NonNull);
                    else
                        outFrame.PushStack(Nullity.Unknown);
                    break;
                }
                case Ldsflda: outFrame.PushStack(Nullity.NonNull); break;

                /* Store field */
                case Stfld: {
                    /* FIXME: warn if writing null to non-null field */
                    Nullity n = outFrame.PopStack();
                    Check(insn, warn, outFrame.PopStack(), "field");
                    IFieldReference field = (IFieldReference)insn.Operand;
                    if(warn && nnaCollector.HasNonNullAttribute(field)) {
                        string etype = method.DeclaringType.FullName;
                        Location loc = new Location(etype,
                                method.Name, insn.Offset);
                        if(n == Nullity.Unknown)
                            messages.Add(new Message(
                                        "storing possibly null value in " +
                                        "field declared non-null",
                                        loc, MessageType.Warning));
                        else if(n == Nullity.Null)
                            messages.Add(new Message(
                                        "storing null value in " +
                                        "field declared non-null",
                                        loc, MessageType.Warning));
                    }
                    break;
                }
                case Stsfld: {
                    Nullity n = outFrame.PopStack();
                    IFieldReference field = (IFieldReference)insn.Operand;
                    if(warn && nnaCollector.HasNonNullAttribute(field)) {
                        string etype = method.DeclaringType.FullName;
                        Location loc = new Location(etype,
                                method.Name, insn.Offset);
                        if(n == Nullity.Unknown)
                            messages.Add(new Message(
                                        "storing possibly null value in " +
                                        "field declared non-null",
                                        loc, MessageType.Warning));
                        else if(n == Nullity.Null)
                            messages.Add(new Message(
                                        "storing null value in " +
                                        "field declared non-null",
                                        loc, MessageType.Warning));
                    }
                    break;
                }

                /* Stack operations */
                case Dup: outFrame.PushStack(outFrame.PeekStack()); break;
                case Pop: outFrame.PopStack(); break;

                 /* Method call and return */
                case Call:
                    ProcessCall(insn, warn, false, outFrame);
                    break;
                case Calli:
                    ProcessCall(insn, warn, true, outFrame);
                    break;
                case Callvirt:
                    ProcessCall(insn, warn, false, outFrame);
                    break;
                case Ret:
                    if(!IsVoid(method.ReturnType.ReturnType)) {
                        Nullity n = outFrame.PopStack();
                        if(nnaCollector.HasNonNullAttribute(method) && warn) {
                            string etype = method.DeclaringType.FullName;
                            Location loc = new Location(etype,
                                    method.Name, insn.Offset);
                            if(n == Nullity.Null)
                                messages.Add(new Message(
                                            "returning null value from " +
                                            "method declared non-null",
                                            loc, MessageType.Warning));
                            else if(n == Nullity.Unknown)
                                messages.Add(new Message(
                                            "returning possibly null value " +
                                            "from method declared non-null",
                                            loc, MessageType.Warning));
                        }
                    }
                    break;

                /* Indirect load */
                case Ldind_I1:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_U1:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_I2:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_U2:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_I4:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_U4:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_I8:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_I:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_R4:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_R8:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldind_Ref:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.Unknown);
                    break;

                /* Indirect store */
                case Stind_Ref: outFrame.PopStack(2); break;
                case Stind_I: outFrame.PopStack(2); break;
                case Stind_I1: outFrame.PopStack(2); break;
                case Stind_I2: outFrame.PopStack(2); break;
                case Stind_I4: outFrame.PopStack(2); break;
                case Stind_I8: outFrame.PopStack(2); break;
                case Stind_R4: outFrame.PopStack(2); break;
                case Stind_R8: outFrame.PopStack(2); break;

                /* Class-related operations */
                case Box:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Unbox:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Unbox_Any:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Castclass: break;
                case Isinst: break;

                /* Exception handling */
                case Throw: outFrame.EmptyStack(); break;
                case Rethrow: break;
                case Leave: outFrame.EmptyStack(); break;
                case Leave_S: outFrame.EmptyStack(); break;
                case Endfinally: outFrame.EmptyStack(); break;
                case Endfilter: outFrame.PopStack(); break;

                /* Array operations */
                case Newarr:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                /* Load element */
                case Ldelema:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_I1:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_U1:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_I2:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_U2:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_I4:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_U4:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_I8:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_I:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_R4:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_R8:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Ldelem_Ref:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.Unknown);
                    break;
                case Ldelem_Any: /* This may or may not be a reference. */
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.Unknown);
                    break;
                /* Store element */
                /* Pop 3 */
                case Stelem_I: outFrame.PopStack(3); break;
                case Stelem_I1: outFrame.PopStack(3); break;
                case Stelem_I2: outFrame.PopStack(3); break;
                case Stelem_I4: outFrame.PopStack(3); break;
                case Stelem_I8: outFrame.PopStack(3); break;
                case Stelem_R4: outFrame.PopStack(3); break;
                case Stelem_R8: outFrame.PopStack(3); break;
                case Stelem_Ref: outFrame.PopStack(3); break;
                case Stelem_Any: outFrame.PopStack(3); break;

                case Mkrefany:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Arglist:
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Sizeof:
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Refanyval:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Refanytype:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                /* Prefixes */
                case Unaligned: break;
                case Volatile: break;
                case Tail: break;

                /* Effect-free instructions */
                case Nop: break;
                case Break: break;

                /* Load constant */
                /* Push non-ref. */
                case Ldc_I4_M1: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_0: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_1: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_2: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_3: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_4: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_5: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_6: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_7: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_8: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4_S: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I4: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_I8: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_R4: outFrame.PushStack(Nullity.NonNull); break;
                case Ldc_R8: outFrame.PushStack(Nullity.NonNull); break;

                /* Unconditional control flow */
                /* Do nothing */
                case Br: break;
                case Br_S: break;

                /* Conditional branches */
                /* Pop 1 */
                case Brfalse: outFrame.PopStack(); break;
                case Brtrue: outFrame.PopStack(); break;
                case Brfalse_S: outFrame.PopStack(); break;
                case Brtrue_S: outFrame.PopStack(); break;

                /* Comparison branches */
                /* Pop 2. */
                case Beq: outFrame.PopStack(2); break;
                case Bge: outFrame.PopStack(2); break;
                case Bgt: outFrame.PopStack(2); break;
                case Ble: outFrame.PopStack(2); break;
                case Blt: outFrame.PopStack(2); break;
                case Bne_Un: outFrame.PopStack(2); break;
                case Bge_Un: outFrame.PopStack(2); break;
                case Bgt_Un: outFrame.PopStack(2); break;
                case Ble_Un: outFrame.PopStack(2); break;
                case Blt_Un: outFrame.PopStack(2); break;
                case Beq_S: outFrame.PopStack(2); break;
                case Bge_S: outFrame.PopStack(2); break;
                case Bgt_S: outFrame.PopStack(2); break;
                case Ble_S: outFrame.PopStack(2); break;
                case Blt_S: outFrame.PopStack(2); break;
                case Bne_Un_S: outFrame.PopStack(2); break;
                case Bge_Un_S: outFrame.PopStack(2); break;
                case Bgt_Un_S: outFrame.PopStack(2); break;
                case Ble_Un_S: outFrame.PopStack(2); break;
                case Blt_Un_S: outFrame.PopStack(2); break;

                case Switch: outFrame.PopStack(); break;

                /* Comparisons */
                /* Pop 2, push non-ref */
                case Ceq:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Cgt:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Cgt_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Clt:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Clt_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                /* Arithmetic and logical binary operators */
                /* Pop 2, push non-ref */
                case Add:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Sub:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Mul:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Div:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Div_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Rem:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Rem_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case And:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Or:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Xor:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Shl:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Shr:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Shr_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Add_Ovf:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Add_Ovf_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Mul_Ovf:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Mul_Ovf_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Sub_Ovf:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Sub_Ovf_Un:
                    outFrame.PopStack(2);
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                /* Arithmetic and logical unary operators */
                /* Pop 1, push non-ref */
                case Neg:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;
                case Not:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                /* Conversions. */
                /* Do nothing. */
                case Conv_I1: break;
                case Conv_I2: break;
                case Conv_I4: break;
                case Conv_I8: break;
                case Conv_R4: break;
                case Conv_R8: break;
                case Conv_U4: break;
                case Conv_U8: break;
                case Conv_U: break;
                case Conv_R_Un: break;
                case Conv_Ovf_I1_Un: break;
                case Conv_Ovf_I2_Un: break;
                case Conv_Ovf_I4_Un: break;
                case Conv_Ovf_I8_Un: break;
                case Conv_Ovf_U1_Un: break;
                case Conv_Ovf_U2_Un: break;
                case Conv_Ovf_U4_Un: break;
                case Conv_Ovf_U8_Un: break;
                case Conv_Ovf_I_Un: break;
                case Conv_Ovf_U_Un: break;
                case Conv_Ovf_I1: break;
                case Conv_Ovf_U1: break;
                case Conv_Ovf_I2: break;
                case Conv_Ovf_U2: break;
                case Conv_Ovf_I4: break;
                case Conv_Ovf_U4: break;
                case Conv_Ovf_I8: break;
                case Conv_Ovf_U8: break;
                case Conv_U2: break;
                case Conv_U1: break;
                case Conv_I: break;
                case Conv_Ovf_I: break;
                case Conv_Ovf_U: break;

                case Ckfinite: break;

                /* Unverifiable instructions. */
                case Jmp: break;
                case Cpblk: outFrame.PopStack(3); break;
                case Initblk: outFrame.PopStack(3); break;
                case Localloc:
                    outFrame.PopStack();
                    outFrame.PushStack(Nullity.NonNull);
                    break;

                default:
                    Console.WriteLine("Unknown instruction: {0} {1}",
                            opcode.Name, opcode.Value.ToString("X4"));
                    break;
            } /* switch */
        } /* for */

        if(runner.Debug) {
            Console.WriteLine("Output frame:");
            Console.Write(outFrame.ToString());
        }
    } /* Transfer */

    private void Check([NonNull]IInstruction insn, bool warn, Nullity n,
            [NonNull] string type)
    {
        if(!warn) return;

        string etype = method.DeclaringType.FullName;
        Location loc = new Location(etype, method.Name, insn.Offset);
        string name = insn.Operand.ToString();
        int nameOffset = name.LastIndexOf("::");
        if(nameOffset != -1)
            name = name.Substring(nameOffset + 2);
        if(type.Equals("method")) {
            string prefix = name.Substring(0, 4);
            if(prefix.Equals("get_") || prefix.Equals("set_")) {
                name = name.Substring(4);
                type = "property";
            }
        }
        if(n == Nullity.Unknown) {
            messages.Add(new Message(
                        "accessing " + type + " " + name +
                        " from potentially null object",
                        loc, MessageType.Warning));
        } else if(n == Nullity.Null) {
            messages.Add(new Message(
                        "accessing " + type + " " + name +
                        " from null object",
                        loc, MessageType.Warning));
        }
    }

    private void ProcessCall([NonNull] IInstruction insn, bool warn,
            bool indirect, [NonNull] NullDerefFrame frame)
    {
        string etype = method.DeclaringType.FullName;
        Location loc = new Location(etype, method.Name, insn.Offset);
        IMethodSignature csig = (IMethodSignature)insn.Operand;
        if(indirect)
            frame.PopStack(); /* Function pointer */
        foreach(IParameterReference param in csig.Parameters) {
            Nullity n = frame.PopStack();
            if(warn && nnaCollector.HasNonNullAttribute(method, param)) {
                if(n == Nullity.Null)
                    messages.Add(new Message(
                                "passing null value as argument " +
                                "declared non-null",
                                loc, MessageType.Warning));
                else if(n == Nullity.Unknown)
                    messages.Add(new Message(
                                "passing possibly null value as argument " +
                                "declared non-null",
                                loc, MessageType.Warning));
            }
        }
        if(csig.HasThis && !Ignoring(csig)) /* Add 'this' parameter. */
            Check(insn, warn, frame.PopStack(), "method");
        if(!IsVoid(csig.ReturnType.ReturnType)) {
            if(csig.ReturnType.ReturnType.IsValueType)
                frame.PushStack(Nullity.NonNull);
            else if(nnaCollector.HasNonNullAttribute(csig))
                frame.PushStack(Nullity.NonNull);
            else
                frame.PushStack(Nullity.Unknown);
        }
    }

    private bool Ignoring([NonNull] IMethodSignature msig)
    {
        /* FIXME: Ignoring is a temporary hack! */
        /* Right now, it always returns false, as it should. */
        return false;
    }
}

}
