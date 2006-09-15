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
using Cecil.FlowAnalysis.ControlFlow;
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.Impl.ControlFlow {
	internal class InstructionBlock : IInstructionBlock {
		public static readonly IInstructionBlock[] NoSuccessors = new IInstructionBlock[0];

		private Instruction _firstInstruction;
		private Instruction _lastInstruction;
		private IInstructionBlock[] _successors = NoSuccessors;

		internal InstructionBlock (Instruction first)
		{
			if (null == first) throw new ArgumentNullException ("first");
			_firstInstruction = first;
		}

		internal void SetLastInstruction (Instruction last)
		{
			if (null == last) throw new ArgumentNullException ("last");
			_lastInstruction = last;
		}

		internal void SetSuccessors (IInstructionBlock[] successors)
		{
			_successors = successors;
		}

		public Instruction FirstInstruction {
			get {
				return _firstInstruction;
			}
		}

		public Instruction LastInstruction {
			get {
				return _lastInstruction;
			}
		}

		public IInstructionBlock[] Successors {
			get {
				return _successors;
			}
		}

		public int CompareTo (object obj)
		{
			return _firstInstruction.Offset.CompareTo (((InstructionBlock)obj).FirstInstruction.Offset);
		}

		public IEnumerator GetEnumerator ()
		{
			ArrayList instructions = new ArrayList ();
			Instruction instruction = _firstInstruction;
			while (true) {
				instructions.Add (instruction);
				if (instruction == _lastInstruction) break;
				instruction = instruction.Next;
			}
			return instructions.GetEnumerator ();
		}
	}
}
