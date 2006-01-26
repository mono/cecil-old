/*
 * CFG.cs: code for control flow graphs -- graphs made up of basic
 * blocks.
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
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gendarme.Rules.Correctness {

public class CFG : Graph { 

    [NonNull] private IInstructionCollection instructions;
    [NonNull] private IMethodDefinition method;
    [NonNull] private IDictionary branchTable;
    private BasicBlock entryPoint;

    public BasicBlock EntryPoint {
        get { return entryPoint; }
    }

    public CFG([NonNull] IMethodDefinition method)
    {
        Init(method.Body.Instructions, method);
    }

    public CFG(IInstructionCollection instructions)
    {
        Init(instructions, null);
    }

    private void Init([NonNull] IInstructionCollection instructions,
            [NonNull] IMethodDefinition method)
    {
        this.instructions = instructions;
        this.method = method;
        InitBranchTable();
        BuildGraph();
    }

    private bool IsBranch(IInstruction instruction)
    {
        if(instruction == null)
            return false;
        switch(instruction.OpCode.FlowControl) {
            case FlowControl.Branch: return true;
            case FlowControl.Cond_Branch: return true;
            case FlowControl.Return: return true;
            /* Throw creates a new basic block, but it has no target,
             * because the object to be thrown is taken from the stack.
             * Thus, its type is not known before runtime, and we can't
             * know which catch block will recieve it. */
            case FlowControl.Throw: return true;
        }
        return false;
    }

    private bool HasNext([NonNull] IInstruction instruction)
    {
        if(instruction.OpCode.FlowControl == FlowControl.Branch)
            return false;
        if(instruction.OpCode.FlowControl == FlowControl.Throw)
            return false;
        if(instruction.OpCode.FlowControl == FlowControl.Return)
            return false;
        else
            return true;
    }

    private int[] BranchTargets([NonNull] IInstruction instruction)
    {
        int[] result = null;
        switch(instruction.OpCode.OperandType) {
            case OperandType.InlineSwitch:
                IInstruction[] targets = (IInstruction[])instruction.Operand;
                result = new int[targets.Length];
                int i = 0;
                foreach(IInstruction target in targets) {
                    result[i] = target.Offset;
                    i++;
                }
                break;
            case OperandType.InlineBrTarget:
                result = new int[1];
                result[0] = ((IInstruction)instruction.Operand).Offset;
                break;
            case OperandType.ShortInlineBrTarget:
                result = new int[1];
                result[0] = ((IInstruction)instruction.Operand).Offset;
                break;
        }
        return result;
    }

    public bool BeginsCatch(BasicBlock bb)
    {
        IExceptionHandler handler = StartsHandlerRegion(bb.FirstInstruction);
        if(handler != null && handler.Type == ExceptionHandlerType.Catch)
            return true;
        return false;
    }

    private BasicBlock GetNearestFinally(IInstruction insn, Hashtable insnBB)
    {
        BasicBlock nearest = null;
        IInstructionCollection insns = method.Body.Instructions;
        int width = insns[insns.Count - 1].Offset;
        foreach(IExceptionHandler handler in method.Body.ExceptionHandlers) {
            if(handler.Type == ExceptionHandlerType.Finally) {
                if(insn.Offset >= handler.TryStart.Offset &&
                        insn.Offset < handler.TryEnd.Offset &&
                        (handler.TryEnd.Offset -
                         handler.TryStart.Offset) < width)
                    nearest = (BasicBlock)insnBB[handler.HandlerStart.Offset];
            }
        }
        return nearest;
    }

    private bool OffsetsEqual(IInstruction insn1, IInstruction insn2)
    {
        if(insn1 == insn2) return true;
        if(insn1 == null) return false;
        if(insn2 == null) return false;
        if(insn1.Offset == insn2.Offset) return true;
        return false;
    }

    private IExceptionHandler EndsTryRegion([NonNull] IInstruction instruction)
    {
        foreach(IExceptionHandler handler in method.Body.ExceptionHandlers) {
            if(instruction != null)
                if(OffsetsEqual(instruction.Next, handler.TryEnd))
                    return handler;
        }
        return null;
    }

    private IExceptionHandler EndsHandlerRegion(
            [NonNull] IInstruction instruction)
    {
        foreach(IExceptionHandler handler in method.Body.ExceptionHandlers) {
            if(instruction != null)
                if(OffsetsEqual(instruction.Next, handler.HandlerEnd))
                    return handler;
        }
        return null;
    }

    private IExceptionHandler StartsTryRegion(
            [NonNull] IInstruction instruction)
    {
        foreach(IExceptionHandler handler in method.Body.ExceptionHandlers) {
            if(OffsetsEqual(instruction, handler.TryStart))
                return handler;
        }
        return null;
    }

    private IExceptionHandler StartsHandlerRegion(
            [NonNull] IInstruction instruction)
    {
        foreach(IExceptionHandler handler in method.Body.ExceptionHandlers) {
            if(OffsetsEqual(instruction, handler.HandlerStart))
                return handler;
        }
        return null;
    }

    private bool IsLeader([NonNull] IInstruction instruction,
            [NonNull] IInstruction previous)
    {
        /* First instruction in the method */
        if(previous == null)
            return true;

        /* Target of a branch */
        if(branchTable.Contains(instruction.Offset))
            return true;

        /* Follows a control flow instruction */
        if(IsBranch(instruction.Previous))
            return true;

        /* Is the beginning of a try region */
        if(StartsTryRegion(instruction) != null)
            return true;

        /* Is the beginning of a handler region */
        if(StartsHandlerRegion(instruction) != null)
            return true;

        return false;
    }

    private void InitBranchTable() {
        branchTable = new Hashtable();
        foreach(IInstruction instr in instructions) {
            int[] targets = BranchTargets(instr);
            if(targets != null)
                foreach(int target in targets)
                    if(!branchTable.Contains(target)) {
                        IList sources = new ArrayList();
                        sources.Add(target);
                        branchTable.Add(target, sources);
                    } else {
                        IList sources = (IList)branchTable[target];
                        sources.Add(target);
                    }
        }
    }

    private void BuildGraph() {
        BasicBlock tail = null;
        IInstruction prevInsn = null;
        BasicBlock prevBB = null;
        int currentInsnNum = 0;
        Hashtable insnBB = new Hashtable();
        BasicBlock exit = new BasicBlock(instructions);
        exit.first = exit.last = 0;
        exit.isExit = true;
        AddNode(exit);

        foreach(IInstruction insn in instructions) {
            if(IsLeader(insn, prevInsn)) {
                tail = new BasicBlock(instructions);
                tail.first = currentInsnNum;
                AddNode(tail);
                if(prevBB != null) {
                    prevBB.last = currentInsnNum - 1;
                    if(HasNext(instructions[currentInsnNum - 1])) {
                        CFGEdge edge = new CFGEdge(prevBB, tail,
                                CFGEdgeType.Forward);
                        AddEdge(edge);
                    }
                }
            }
            insnBB.Add(insn.Offset, tail);
            prevInsn = insn;
            prevBB = tail;
            currentInsnNum++;
        }
        if(prevBB != null) {
            prevBB.last = currentInsnNum - 1;
        }

        foreach(IInstruction insn in instructions) {
            if((EndsTryRegion(insn) != null) ||
                    (EndsHandlerRegion(insn) != null)) { 
                BasicBlock finallyBB = GetNearestFinally(insn, insnBB);
                if(finallyBB != null)
                    AddEdge(new CFGEdge((BasicBlock)insnBB[insn.Offset],
                                finallyBB, CFGEdgeType.Forward));
            }

            if(insn.OpCode.FlowControl == FlowControl.Return) {
                if(insn.OpCode.Value == OpCodeConstants.Endfinally &&
                        insn.Next != null) {
                    AddEdge(new CFGEdge((BasicBlock)insnBB[insn.Offset],
                                (BasicBlock)insnBB[insn.Next.Offset],
                                CFGEdgeType.Forward));
                } else {
                    AddEdge(new CFGEdge((BasicBlock)insnBB[insn.Offset],
                                exit, CFGEdgeType.Return));
                }
            }
            /*
            if((insn.OpCode.Value != OpCodeConstants.Leave) &&
                    (insn.OpCode.Value != OpCodeConstants.Endfinally)) {
                    */
                int[] targets = BranchTargets(insn);
                if(targets == null)
                    continue;
                foreach(int target in targets) {
                    AddEdge(new CFGEdge((BasicBlock)insnBB[insn.Offset],
                                (BasicBlock)insnBB[target],
                                CFGEdgeType.Branch));
                }
            /*} */
        }

        entryPoint = (BasicBlock)insnBB[0];
    }

    public void PrintBasicBlocks() {
        IInstruction prevInstr = null;

        Console.WriteLine(method.Name);
        foreach(IInstruction instr in instructions) {
            if(StartsTryRegion(instr) != null)
                Console.WriteLine("Try {");
            if(StartsHandlerRegion(instr) != null)
                Console.WriteLine("Handle {");

            if(IsLeader(instr, prevInstr))
                Console.Write("* ");
            else
                Console.Write("  ");
            Console.Write("  {0}: {1}", instr.Offset.ToString("X4"),
                    instr.OpCode.Name);
            int[] targets = BranchTargets(instr);
            if(targets != null)
                foreach(int target in targets)
                    Console.Write(" {0}", target.ToString("X4"));
            else if(instr.Operand is string)
                Console.Write(" \"{0}\"", instr.Operand.ToString());
            else if(instr.Operand != null)
                Console.Write(" {0}", instr.Operand.ToString());
            Console.WriteLine();
            prevInstr = instr;
            if(EndsTryRegion(instr) != null)
                Console.WriteLine("} (Try)");
            if(EndsHandlerRegion(instr) != null)
                Console.WriteLine("} (Handle)");
        }
    }

    public void PrintDot() {
        string name = method.DeclaringType.Name + "." + method.Name + ".dot";
        FileMode mode = FileMode.Create;
        StreamWriter writer = new StreamWriter(new FileStream(name, mode));
        writer.Write("digraph {0} ", method.Name);
        writer.Write("{\n");
        foreach(Node node in Nodes) {
            BasicBlock bb = (BasicBlock)node;
            writer.Write("    \"{0}\" [ label = \"{1}\" ];\n", bb, bb);
        }

        foreach(Edge edge in Edges) {
            CFGEdge ce = (CFGEdge)edge;
            writer.Write("    \"{0}\" -> \"{1}\"", ce.Start, ce.End);
            if(ce.Type == CFGEdgeType.Branch) {
                writer.Write(" [ label = \"branch\" ];\n");
            } else if(ce.Type == CFGEdgeType.Forward) {
                writer.Write(" [ label = \"forward\" ];\n");
            } else if(ce.Type == CFGEdgeType.Return) {
                writer.Write(" [ label = \"return\" ];\n");
            } else if(ce.Type == CFGEdgeType.Exception) {
                writer.Write(" [ label = \"exception\" ];\n");
            } else {
                writer.Write(" [ label = \"unknown\" ];\n");
            }
        }
        writer.Write("}\n");
        writer.Close();
    }
}

}