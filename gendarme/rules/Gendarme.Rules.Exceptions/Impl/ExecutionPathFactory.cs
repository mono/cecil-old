using System;
using System.Collections;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gendarme.Rules.Exceptions.Impl {

	public class ExecutionPathFactory {
	
		private IMethodDefinition method;

		public ExecutionPathFactory (IMethodDefinition method)
		{
			this.method = method;
		}

		public ExecutionPath[] CreatePaths (Instruction start, Instruction end)
		{
			if (start == null)
				throw new ArgumentNullException ("start");
			if (end == null)
				throw new ArgumentNullException ("end");
			if (!method.Body.Instructions.Contains (start))
				throw new ArgumentException(
					"start instruction is not contained in method " + 
					method.DeclaringType.FullName + "::" + method.Name,
					"start");

			if (!method.Body.Instructions.Contains (end)) {
				throw new ArgumentException(
					"end instruction is not contained in method " + 
					method.DeclaringType.FullName + "::" + method.Name,
					"end");
			}

			ArrayList paths = new ArrayList ();
			CreatePathHelper (start, end, new ExecutionPath (), paths);
			ExecutionPath[] ret = new ExecutionPath [paths.Count];
			paths.CopyTo (ret);
			return ret;
		}

		private void CreatePathHelper (IInstruction start, 
					       IInstruction end, 
					       ExecutionPath path, 
					       ArrayList completedPaths)
		{
			ExecutionBlock curBlock = new ExecutionBlock ();
			curBlock.First = start;

			IInstruction cur = start;
			bool stop = false;
			do {
				switch (cur.OpCode.FlowControl) {
				case FlowControl.Branch:
				case FlowControl.Cond_Branch:
					if (cur.OpCode == OpCodes.Switch) {
						IInstruction[] targetOffsets = (IInstruction[])cur.Operand;
						foreach (IInstruction target in targetOffsets) {
							if (!path.Contains (target)) {
								curBlock.Last = cur;
								path.Add (curBlock);
								CreatePathHelper (target, 
										end, 
										(ExecutionPath)path.Clone (),
										completedPaths);
							}
						}
						stop = true;
					} else if (cur.OpCode == OpCodes.Leave || 
							   cur.OpCode == OpCodes.Leave_S) {
						curBlock.Last = cur;
						path.Add (curBlock);
						completedPaths.Add (path);
						stop = true;
						break;
					} else {
						IInstruction target = (IInstruction)cur.Operand;
						if (!path.Contains (target)) {
							curBlock.Last = cur;
							path.Add (curBlock);
							CreatePathHelper (target, 
									end, 
									(ExecutionPath)path.Clone (),
									completedPaths);
						} 
						if (!path.Contains (cur.Next)) {
							curBlock = new ExecutionBlock ();
							curBlock.First = cur.Next;
						} else {
							stop = true;
						}
					}
					break;
				case FlowControl.Throw:
				case FlowControl.Return:
					curBlock.Last = cur;
					path.Add (curBlock);
					completedPaths.Add (path);
					stop = true;
					break;
				default:
					break;
				}

				if (cur.Next != null && cur != end && !stop)
					cur = cur.Next;
				else
					stop = true;
			} 
			while (!stop);
		}
	}
}
