using System;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gendarme.Rules.Exceptions.Impl {

	public class ExecutionBlock : ICloneable {
	
		private IInstruction firstInstruction;
		private IInstruction lastInstruction;

		public ExecutionBlock ()
		{
			firstInstruction = null;
			lastInstruction = null;
		}

		public IInstruction First {
			get { return firstInstruction; }
			set { firstInstruction = value; }
		}

		public IInstruction Last {
			get { return lastInstruction; }
			set { lastInstruction = value; }
		}

		public bool Contains (IInstruction instruction)
		{
			if (firstInstruction == null || lastInstruction == null ||
				firstInstruction.Offset > lastInstruction.Offset) {
				return false;
			} else {
				return ((instruction.Offset >= firstInstruction.Offset) &&
					    (instruction.Offset <= lastInstruction.Offset));
			}
		}

		public void Print () 
		{
			Console.WriteLine ("{0:X} {1:X}", First.Offset, Last.Offset);
		}

		#region ICloneable Members

		public object Clone ()
		{
			ExecutionBlock other = new ExecutionBlock ();
			other.First = firstInstruction;
			other.Last = lastInstruction;
			return other;
		}

		#endregion
	}
}
