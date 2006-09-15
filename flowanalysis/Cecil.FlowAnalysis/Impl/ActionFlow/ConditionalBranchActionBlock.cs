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
using Cecil.FlowAnalysis.CodeStructure;
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.Impl.ActionFlow {
	/// <summary>
	/// </summary>
	internal class ConditionalBranchActionBlock : AbstractActionBlock, IConditionalBranchActionBlock {
		IExpression _condition;
		private IActionBlock _then;
		private IActionBlock _else;

		public ConditionalBranchActionBlock (Instruction sourceInstruction, IExpression condition)
		: base(sourceInstruction)
		{
			if (null == condition) throw new ArgumentNullException ("condition");
			_condition = condition;
		}

		public void SetTargets (IActionBlock then, IActionBlock else_)
		{
			if (null == then) throw new ArgumentNullException ("then");
			AddAsPredecessorOf (then);
			if (null != else_) AddAsPredecessorOf (else_);
			_then = then;
			_else = else_;
		}

		override public ActionType ActionType {
			get { return ActionType.ConditionalBranch; }
		}

		public IExpression Condition {
			get { return _condition; }
		}

		public IActionBlock Then {
			get { return _then; }
		}

		public IActionBlock Else {
			get { return _else; }
		}

		public override IActionBlock[] Successors {
			get {
				return _else != null
					? new IActionBlock [] { _then, _else }
					: new IActionBlock [] { _then };
			}
		}

		public override void ReplaceSuccessor (IActionBlock existing, IActionBlock newBlock)
		{
			if (existing == _then) {
				_then = newBlock;
			} else if (existing == _else) {
				_else = newBlock;
			} else {
				throw new ArgumentException ("existing");
			}
		}
	}
}
