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
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.ControlFlow;

using Mono.Collections.Generic;

namespace Cecil.FlowAnalysis.ActionFlow {

	public class ActionFlowGraph {

		Collection<ActionBlock> _blocks;
		ControlFlowGraph _cfg;

		public ActionFlowGraph (ControlFlowGraph cfg, Collection<ActionBlock> blocks)
		{
			if (null == cfg) throw new ArgumentNullException ("cfg");
			if (null == blocks) throw new ArgumentNullException ("blocks");

			_cfg = cfg;
			_blocks = blocks;
		}

		public ControlFlowGraph ControlFlowGraph {
			get { return _cfg; }
		}

		public Collection<ActionBlock> Blocks {
			get { return _blocks; }
		}

		public bool IsBranchTarget (ActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			foreach (ActionBlock p in block.Predecessors) {
				switch (p.ActionType) {
				case ActionType.Branch:
					BranchActionBlock br = (BranchActionBlock) p;
					if (br.Target == block) return true;
					break;

				case ActionType.ConditionalBranch:
					ConditionalBranchActionBlock cbr = (ConditionalBranchActionBlock) p;
					if (cbr.Then == block) return true;
					break;
				}
			}
			return false;
		}

		internal void ReplaceAt (int index, ActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");

			ActionBlock existing = _blocks [index];
			foreach (ActionBlock p in existing.Predecessors.ToArray ()) {
				p.ReplaceSuccessor (existing, block);
			}
			Remove (existing);
			_blocks.Insert (index, block);
		}

		void Remove (ActionBlock block)
		{
			foreach (ActionBlock s in block.Successors) {
				s.RemovePredecessor (block);
				if (0 == s.Predecessors.Count) {
					Remove (s);
				}
			}
			_blocks.Remove (block);
		}
	}
}
