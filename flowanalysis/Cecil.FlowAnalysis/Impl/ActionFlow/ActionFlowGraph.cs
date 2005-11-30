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

namespace Cecil.FlowAnalysis.Impl.ActionFlow {
	internal class ActionFlowGraph : IActionFlowGraph {
		private ActionBlockCollection _blocks;
		private IControlFlowGraph _cfg;

		public ActionFlowGraph (IControlFlowGraph cfg, ActionBlockCollection blocks)
		{
			if (null == cfg) throw new ArgumentNullException ("cfg");
			if (null == blocks) throw new ArgumentNullException ("blocks");

			_cfg = cfg;
			_blocks = blocks;
		}

		public IControlFlowGraph ControlFlowGraph {
			get { return _cfg; }
		}

		public IActionBlockCollection Blocks {
			get { return _blocks; }
		}

		public bool IsBranchTarget (IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			foreach (IActionBlock p in block.Predecessors) {
				switch (p.ActionType) {
				case ActionType.Branch:
					IBranchActionBlock br = (IBranchActionBlock) p;
					if (br.Target == block) return true;
					break;

				case ActionType.ConditionalBranch:
					IConditionalBranchActionBlock cbr = (IConditionalBranchActionBlock) p;
					if (cbr.Then == block) return true;
					break;
				}
			}
			return false;
		}

		public void ReplaceAt (int index, IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");

			IActionBlock existing = _blocks [index];
			foreach (AbstractActionBlock p in existing.Predecessors.ToArray ()) {
				p.ReplaceSuccessor (existing, block);
			}
			Remove (existing);
			_blocks.Insert (index, block);
		}

		private void Remove (IActionBlock block)
		{
			foreach (AbstractActionBlock s in block.Successors) {
				s.RemovePredecessor (block);
				if (0 == s.Predecessors.Count) {
					Remove (s);
				}
			}
			_blocks.Remove (block);
		}
	}
}
