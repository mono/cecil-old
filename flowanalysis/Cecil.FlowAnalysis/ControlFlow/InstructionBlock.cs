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
using Mono.Cecil.Cil;

using Mono.Collections.Generic;

namespace Cecil.FlowAnalysis.ControlFlow {

	public class InstructionBlock : IComparable, IEnumerable {

		public static readonly InstructionBlock [] NoSuccessors = new InstructionBlock [0];

		Instruction _firstInstruction;
		Instruction _lastInstruction;
		InstructionBlock [] _successors = NoSuccessors;

		public Instruction FirstInstruction {
			get { return _firstInstruction; }
		}

		public Instruction LastInstruction {
			get { return _lastInstruction; }
		}

		public InstructionBlock [] Successors {
			get { return _successors; }
		}

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

		internal void SetSuccessors (InstructionBlock [] successors)
		{
			_successors = successors;
		}

		public int CompareTo (object obj)
		{
			return _firstInstruction.Offset.CompareTo (((InstructionBlock)obj).FirstInstruction.Offset);
		}

		public IEnumerator GetEnumerator ()
		{
			var instructions = new Collection<Instruction> ();
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
