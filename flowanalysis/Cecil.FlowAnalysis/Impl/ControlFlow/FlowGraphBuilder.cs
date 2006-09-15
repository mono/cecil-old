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
using Cecil.FlowAnalysis.CecilUtilities;
using Cecil.FlowAnalysis.ControlFlow;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.Impl.ControlFlow {
	/// <summary>
	/// </summary>
	internal class FlowGraphBuilder {
		private MethodBody _body;
		private Hashtable _instructionData;
		private Hashtable _blocks = new Hashtable();
		private TypeReference _SystemVoid;

		internal FlowGraphBuilder (MethodDefinition method)
		{
			if (method.Body.ExceptionHandlers.Count > 0) {
				throw new ArgumentException ("Exception handlers are not supported.", "body");
			}
			_SystemVoid = method.DeclaringType.Module.Import (typeof(void));
			_body = method.Body;
			DelimitBlocks ();
			ConnectBlocks ();
			ComputeInstructionData ();
		}

		internal IControlFlowGraph ControlFlowGraph {
			get {
				return new FlowGraph (_body, RegisteredBlocks, _instructionData);
			}
		}

		private void DelimitBlocks ()
		{
			InstructionCollection instructions = _body.Instructions;

			MarkBlockStarts (instructions);
			MarkBlockEnds (instructions);
		}

		private void MarkBlockStarts (InstructionCollection instructions)
		{
			Instruction instruction = instructions [0];

			// the first instruction starts a block
			MarkBlockStart (instruction);
			for (int i=1; i < instructions.Count; ++i) {
				instruction = instructions [i];
				if (IsBlockDelimiter (instruction)) {
					// the target of a branch starts a block
					Instruction target = GetBranchTarget (instruction);
					if (null != target) MarkBlockStart (target);

					// the next instruction after a branch starts a block
					if (null != instruction.Next) MarkBlockStart (instruction.Next);
				}
			}
		}

		private void MarkBlockEnds (InstructionCollection instructions)
		{
			InstructionBlock[] blocks = this.RegisteredBlocks;
			InstructionBlock current = blocks [0];

			for (int i=1; i < blocks.Length; ++i) {
				InstructionBlock block = blocks [i];
				current.SetLastInstruction (block.FirstInstruction.Previous);
				current = block;
			}
			current.SetLastInstruction (instructions [instructions.Count - 1]);
		}

		private bool IsBlockDelimiter (Instruction instruction)
		{
			FlowControl control = instruction.OpCode.FlowControl;
			switch (control) {
			case FlowControl.Break:
			case FlowControl.Branch:
			case FlowControl.Return:
			case FlowControl.Cond_Branch:
				return true;
			}
			return false;
		}

		private void MarkBlockStart (Instruction instruction)
		{
			InstructionBlock block = GetBlock (instruction);
			if (null != block) return;

			block = new InstructionBlock (instruction);
			RegisterBlock (block);
		}

		private void ComputeInstructionData ()
		{
			_instructionData = new Hashtable ();
			Hashtable visited = new Hashtable ();
			ComputeInstructionData (visited, 0, GetFirstBlock ());
		}

		private InstructionBlock GetFirstBlock ()
		{
			return GetBlock (_body.Instructions [0]);
		}

		private void ComputeInstructionData (IDictionary visited, int stackHeight, InstructionBlock block)
		{
			if (visited.Contains (block)) return;
			visited.Add (block, block);

			foreach (Instruction instruction in block) {
				stackHeight = ComputeInstructionData (stackHeight, instruction);
			}

			foreach (InstructionBlock successor in block.Successors) {
				ComputeInstructionData (visited, stackHeight, successor);
			}
		}

		private int ComputeInstructionData (int stackHeight, Instruction instruction)
		{
			int before = stackHeight;
			int after = ComputeNewStackHeight (stackHeight, instruction);
			_instructionData.Add (instruction.Offset, new InstructionData (before, after));
			return after;
		}

		private int ComputeNewStackHeight (int stackHeight, Instruction instruction)
		{
			return stackHeight + GetPushDelta (instruction) - GetPopDelta (stackHeight, instruction);
		}

		private int GetPushDelta (Instruction instruction)
		{
			OpCode code = instruction.OpCode;
			switch (code.StackBehaviourPush) {
			case StackBehaviour.Push0:
				return 0;

			case StackBehaviour.Push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
				return 1;

			case StackBehaviour.Push1_push1:
				return 2;

			case StackBehaviour.Varpush:
				if (code.FlowControl == FlowControl.Call) {
					MethodReference method = (MethodReference)instruction.Operand;
					return IsVoid (method.ReturnType.ReturnType)
						? 0
						: 1;
				}
				break;
			}
			throw new ArgumentException (CecilFormatter.FormatInstruction (instruction));
		}

		private int GetPopDelta (int stackHeight, Instruction instruction)
		{
			OpCode code = instruction.OpCode;
			switch (code.StackBehaviourPop) {
			case StackBehaviour.Pop0:
				return 0;
			case StackBehaviour.Popi:
			case StackBehaviour.Popref:
			case StackBehaviour.Pop1:
				return 1;

			case StackBehaviour.Pop1_pop1:
			case StackBehaviour.Popi_pop1:
			case StackBehaviour.Popi_popi:
			case StackBehaviour.Popi_popi8:
			case StackBehaviour.Popi_popr4:
			case StackBehaviour.Popi_popr8:
			case StackBehaviour.Popref_pop1:
			case StackBehaviour.Popref_popi:
				return 2;

			case StackBehaviour.Popi_popi_popi:
			case StackBehaviour.Popref_popi_popi:
			case StackBehaviour.Popref_popi_popi8:
			case StackBehaviour.Popref_popi_popr4:
			case StackBehaviour.Popref_popi_popr8:
			case StackBehaviour.Popref_popi_popref:
				return 3;

			case StackBehaviour.PopAll:
				return stackHeight;

			case StackBehaviour.Varpop:
				if (code.FlowControl == FlowControl.Call) {
					MethodReference method = (MethodReference)instruction.Operand;
					int count = method.Parameters.Count;
					if (method.HasThis && OpCodes.Newobj.Value != code.Value) {
						++count;
					}
					return count;
				}
				if (code.Value == OpCodes.Ret.Value) {
					return IsVoidMethod ()
						? 0
						: 1;
				}
				break;
			}
			throw new ArgumentException (CecilFormatter.FormatInstruction (instruction));
		}

		private bool IsVoidMethod ()
		{
			TypeReference type = _body.Method.ReturnType.ReturnType;
			return IsVoid (type);
		}

		private bool IsVoid (TypeReference type)
		{
			return type == _SystemVoid;
		}

		private InstructionBlock[] RegisteredBlocks {
			get {
				return (InstructionBlock[]) ToArray (new InstructionBlock [BlockCount]);
			}
		}

		private int BlockCount {
			get {
				return _blocks.Count;
			}
		}

		private Array ToArray (Array blocks)
		{
			_blocks.Values.CopyTo (blocks, 0);
			Array.Sort (blocks);
			return blocks;
		}

		private void ConnectBlocks ()
		{
			foreach (InstructionBlock block in _blocks.Values) {
				if (block.LastInstruction == null) {
					throw new ApplicationException ("Undelimited block at offset " + block.FirstInstruction.Offset);
				}
				Instruction instruction = block.LastInstruction;
				switch (instruction.OpCode.FlowControl) {
				case FlowControl.Branch:
				case FlowControl.Cond_Branch:
					{
						IInstructionBlock target = GetBranchTargetBlock (instruction);
						if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch
							&& instruction.Next != null) {
							block.SetSuccessors (new IInstructionBlock [] { target, GetBlock (instruction.Next) });
						} else {
							block.SetSuccessors (new IInstructionBlock [] { target })	;
						}
						break;
					}

				case FlowControl.Call:
				case FlowControl.Next:
					{
						if (null != instruction.Next) {
							block.SetSuccessors (new IInstructionBlock [] { GetBlock (instruction.Next) });
						}
						break;
					}

				case FlowControl.Return:
					{
						break;
					}

				default:
					{
						throw new ApplicationException (
							string.Format ("Unhandled instruction flow behavior {0}: {1}",
								instruction.OpCode.FlowControl,
								CecilFormatter.FormatInstruction (instruction)));
					}
				}
			}
		}

		private IInstructionBlock GetBranchTargetBlock (Instruction instruction)
		{
			return GetBlock (GetBranchTarget (instruction));
		}

		private Instruction GetBranchTarget (Instruction instruction)
		{
			return (Instruction)instruction.Operand;
		}

		private void RegisterBlock (InstructionBlock block)
		{
			_blocks.Add (block.FirstInstruction.Offset, block);
		}

		private InstructionBlock GetBlock (Instruction firstInstruction)
		{
			return (InstructionBlock)_blocks [firstInstruction.Offset];
		}
	}
}
