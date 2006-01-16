/*
 * BasicBlock.cs: simple representation of basic blocks.
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

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gendarme.Rules.Correctness {

public class BasicBlock : Node {
    /* All instructions in the method */
    [NonNull] private IInstructionCollection instructions;

    /* Index of the first instruction in this basic block */
    public int first;

    /* Index of the last instruction in this basic block */
    public int last;

    public bool isExit = false;
    public bool isException = false;

    public BasicBlock([NonNull] IInstructionCollection instructions)
    {
        this.instructions = instructions;
    }

    public IInstructionCollection Instructions {
        [NonNull]
        get { return instructions; }
    }

    public IInstruction FirstInstruction {
        get { return instructions[first]; }
    }

    [NonNull]
    public override string ToString() {
        if(isExit)
            return "exit";
        if(isException)
            return "exception";
        return instructions[first].Offset.ToString("X4") + "-" +
            instructions[last].Offset.ToString("X4");
    }
}

}
