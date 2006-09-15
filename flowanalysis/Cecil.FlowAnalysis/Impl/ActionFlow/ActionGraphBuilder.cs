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
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CecilUtilities;
using Cecil.FlowAnalysis.ControlFlow;
using Cecil.FlowAnalysis.Impl.ActionFlow;
using Cecil.FlowAnalysis.Impl.CodeStructure;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Cecil.FlowAnalysis.CodeStructure;

namespace Cecil.FlowAnalysis.Impl.ActionFlow {
	/// <summary>
	/// </summary>
	internal class ActionGraphBuilder : AbstractInstructionVisitor {
		private ActionBlockCollection _blocks = new ActionBlockCollection();
		private IDictionary _instruction2block = new Hashtable();
		private IDictionary _processed = new Hashtable();
		private ExpressionDecompiler _expressionDecompiler;
		private MethodDefinition _method;
		private IControlFlowGraph _cfg;
		private Instruction _current;
		private ActionFlowGraph _graph;

		internal ActionGraphBuilder (IControlFlowGraph cfg)
		{
			_method = cfg.MethodBody.Method;

			_cfg = cfg;
			_expressionDecompiler = new ExpressionDecompiler (_method);

			_graph = new ActionFlowGraph (_cfg, _blocks);

			Run ();
		}

		public IActionFlowGraph ActionFlowGraph {
			get {
				return _graph;
			}
		}

		void Run ()
		{
			ProcessBlocks ();
			ConnectActionBlocks ();
			SimplifyActionBlocks ();
		}

		private void SimplifyActionBlocks ()
		{
			int index = 0;
			while (index < _blocks.Count) {
				IActionBlock block = _blocks [index];
				switch (block.ActionType) {
				case ActionType.ConditionalBranch:
					/// if condition goto label
					/// return expression
					/// label: return true
					///         |
					///         V
					/// return (condition || expression)
					///
					/// label: return false
					///         |
					///         V
					/// return (!condition || expression)
					ConditionalBranchActionBlock cbr = (ConditionalBranchActionBlock)block;
					if (IsReturnTrueOrFalse (cbr.Then)
						&& IsReturn (cbr.Else)
						&& 1 == cbr.Then.Predecessors.Count
						&& 1 == cbr.Else.Predecessors.Count) {
						BinaryOperator op = BinaryOperator.LogicalOr;
						IExpression lhs = cbr.Condition;
						IExpression rhs = ((IReturnActionBlock)cbr.Else).Expression;

						if (((ILiteralExpression)((IReturnActionBlock)cbr.Then).Expression).Value.Equals (0)) {
							op = BinaryOperator.LogicalAnd;
							_expressionDecompiler.Negate (lhs);
							lhs = _expressionDecompiler.Pop ();
						}

						IActionBlock newBlock = new ReturnActionBlock (
							block.SourceInstruction,
							new BinaryExpression (op, lhs, rhs));

						_graph.ReplaceAt (index, newBlock);

						index = 0;
						continue;
					}
					break;
				}
				++index;
			}
		}

		private bool IsReturnTrueOrFalse (IActionBlock block)
		{
			IReturnActionBlock ret = block as IReturnActionBlock;
			return ret != null && IsTrueOrFalse (ret.Expression);
		}

		private bool IsTrueOrFalse (IExpression expression)
		{
			ILiteralExpression literal = expression as ILiteralExpression;
			return literal != null
				&& literal.Value != null
				&& (literal.Value.Equals (1)
				|| literal.Value.Equals (0));
		}

		private static bool IsReturn (IActionBlock block)
		{
			return block.ActionType == ActionType.Return;
		}

		private void ConnectActionBlocks ()
		{
			for (int i=0; i < _blocks.Count; ++i) {
				IActionBlock block = _blocks [i];
				switch (block.ActionType) {
				case ActionType.Branch:
					BranchActionBlock br = (BranchActionBlock)block;
					br.SetTarget (GetBranchTarget (br.SourceInstruction));
					break;

				case ActionType.ConditionalBranch:
					ConditionalBranchActionBlock cbr = (ConditionalBranchActionBlock)block;
					cbr.SetTargets (GetBranchTarget (cbr.SourceInstruction), GetBlockAtOrNull (i + 1));
					break;

				case ActionType.Return:
					break;

				default:
					AbstractFallThroughActionBlock ftb = (AbstractFallThroughActionBlock) block;
					ftb.SetNext (GetBlockAtOrNull (i + 1));
					break;
				}
			}
		}

		private IActionBlock GetBlockAtOrNull (int index)
		{
			return index >= _blocks.Count
				? null
				: _blocks [index];
		}

		private IActionBlock GetBranchTarget (Instruction instruction)
		{
			return GetActionBlock ((Instruction)instruction.Operand);
		}

		private void ProcessBlocks ()
		{
			foreach (IInstructionBlock block in _cfg.Blocks) {
				if (WasProcessed (block)) continue;
				_current = block.FirstInstruction;
				ProcessBlock (block);
			}
		}

		private void ProcessBlock (IInstructionBlock block)
		{
			switch (block.Successors.Length) {
			case 0:
			case 1:
				ProcessSimpleBlock (block);
				break;

			case 2:
				ProcessTwoWayBlock (block);
				break;

			default:
				throw new ArgumentException ("n-way block not supported", "block");
			}
			MarkProcessed (block);
		}

		private void ProcessTwoWayBlock (IInstructionBlock block)
		{
			if (IsLogicalExpression (block)) {
				ProcessLogicalExpressionBlock (block);
			} else {
				ProcessSimpleBlock (block);
			}
		}

		/// <summary>
		/// Checks if the subgraph starting at block represents
		/// a logical expression.
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		private bool IsLogicalExpression (IInstructionBlock block)
		{
			return IsLogicalExpression (new Hashtable (), block);
		}

		private bool IsLogicalExpression (Hashtable visited, IInstructionBlock block)
		{
			if (visited.Contains (block)) return false;
			visited.Add (block, block);
			foreach (IInstructionBlock successor in block.Successors) {
				if (GetStackAfter (successor.LastInstruction) > 0) return true;
				if (IsLogicalExpression (visited, successor)) return true;
			}
			return false;
		}


		private void ProcessSimpleBlock (IInstructionBlock block)
		{
			foreach (Instruction instruction in block) {
				_expressionDecompiler.Visit (instruction);
				if (0 == GetStackAfter (instruction)) {
					CreateActionBlock (instruction);
					_current = instruction.Next;
				}
			}
		}

		private void ProcessLogicalExpressionBlock (IInstructionBlock block)
		{
			switch (DetectExpressionPattern (block)) {
			case ExpressionPattern.SimpleOr:
				ProcessOr (block);
				break;

			case ExpressionPattern.SimpleNotAnd:
				ProcessNotAnd (block);
				break;

			case ExpressionPattern.NestedNotAnd:
				ProcessNestedNotAnd (block);
				break;

			default:
				throw new ArgumentException ("Unknown expression pattern starting at " + CecilFormatter.FormatInstruction (block.FirstInstruction), "block");
			}
			MarkProcessed (block.Successors);
		}

		enum ExpressionPattern {
			Unknown,
			SimpleOr,
			SimpleNotAnd,
			NestedNotAnd
		}

		ExpressionPattern DetectExpressionPattern (IInstructionBlock block)
		{
			IInstructionBlock then = GetThen (block);
			if (IsTrue (then)) return ExpressionPattern.SimpleOr;
			if (IsFalse (then)) return ExpressionPattern.SimpleNotAnd;

			// the following flow graph patterns are described in
			// Decompilation of .NET Bytecode, Computer Science Tripos Part II
			// Trinity Hall, May 13, 2004
			// Fig. 3.10 on pg. 43

			// pattern 4: !x && y
			if (GetElse (GetElse (block)) == then) return ExpressionPattern.NestedNotAnd;
			return ExpressionPattern.Unknown;
		}

		void ProcessNestedNotAnd (IInstructionBlock block)
		{
			BuildNegateExpression (block);
			IInstructionBlock y = GetElse (block);
			BuildExpression (y);
			_expressionDecompiler.PushBinaryExpression (BinaryOperator.LogicalAnd);
			ProcessNestedExpression (y);
		}

		void ProcessOr (IInstructionBlock block)
		{
			BuildExpression (block);
			ProcessNestedExpression (GetElse (block));
			_expressionDecompiler.PushBinaryExpression (BinaryOperator.LogicalOr);
		}

		void ProcessNotAnd (IInstructionBlock block)
		{
			BuildNegateExpression (block);
			ProcessNestedExpression (GetElse (block));
			_expressionDecompiler.PushBinaryExpression (BinaryOperator.LogicalAnd);
		}

		private void BuildNegateExpression (IInstructionBlock block)
		{
			BuildExpression (block);
			_expressionDecompiler.Negate ();
		}

		private void ProcessNestedExpression (IInstructionBlock block)
		{
			switch (block.Successors.Length) {
			case 1:
				BuildExpression (block);
				break;

			case 2:
				ProcessLogicalExpressionBlock (block);
				break;

			default:
				throw new ArgumentException ("block");
			}
		}

		private void BuildExpression (IInstructionBlock block)
		{
			if (WasProcessed (block)) return;
			foreach (Instruction instruction in block) {
				_expressionDecompiler.Visit (instruction);
			}
			MarkProcessed (block);
		}

		bool IsTrue (IInstructionBlock block)
		{
			return block.FirstInstruction == block.LastInstruction
				&& block.FirstInstruction.OpCode.Value == OpCodes.Ldc_I4_1.Value;
		}

		bool IsFalse (IInstructionBlock block)
		{
			return block.FirstInstruction == block.LastInstruction
				&& block.FirstInstruction.OpCode.Value == OpCodes.Ldc_I4_0.Value;
		}

		private void AssertStackIsEmpty ()
		{
			if (0 == _expressionDecompiler.Count) return;
			throw new InvalidOperationException ("stack should be empty after a statement");
		}

		private void CreateActionBlock (Instruction instruction)
		{
			Visit (instruction);
			AssertStackIsEmpty ();
		}

		public override void OnBrfalse (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		public override void OnBrtrue (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		public override void OnBle (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		public override void OnBlt (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		public override void OnBeq (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		public override void OnBne_Un (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		public override void OnBge (Instruction instruction)
		{
			AddConditionalBranch (instruction);
		}

		private void AddConditionalBranch (Instruction instruction)
		{
			Add (new ConditionalBranchActionBlock (instruction, Pop ()));
		}

		public override void OnNop (Instruction instruction)
		{
		}

		public override void OnRet (Instruction instruction)
		{
			IExpression expression = null;
			if (1 == GetStackBefore (instruction)) {
				expression = Pop ();
			}
			Add (new ReturnActionBlock (instruction, expression));
		}

		public override void OnCall (Instruction instruction)
		{
			Add (new InvokeActionBlock (instruction, (IMethodInvocationExpression)Pop ()));
		}

		public override void OnBr (Instruction instruction)
		{
			Add (new BranchActionBlock (instruction));
		}

		public override void OnStloc_0 (Instruction instruction)
		{
			Add (new AssignActionBlock (instruction, (IAssignExpression) Pop ()));
		}

		private int GetStackBefore (Instruction instruction)
		{
			return GetInstructionData (instruction).StackBefore;
		}

		private int GetStackAfter (Instruction instruction)
		{
			return GetInstructionData (instruction).StackAfter;
		}

		private IInstructionData GetInstructionData (Instruction instruction)
		{
			return _cfg.GetData (instruction);
		}

		void Add (IActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			_blocks.Add (block);
			_instruction2block.Add (_current, block);
		}

		IActionBlock GetActionBlock (Instruction block)
		{
			return (IActionBlock) _instruction2block [block];
		}

		IExpression Pop ()
		{
			return _expressionDecompiler.Pop ();
		}

		void MarkProcessed (IInstructionBlock[] blocks)
		{
			foreach (IInstructionBlock block in blocks) {
				MarkProcessed (block);
			}
		}

		void MarkProcessed (IInstructionBlock block)
		{
			_processed [block] = block;
		}

		bool WasProcessed (IInstructionBlock block)
		{
			return _processed.Contains (block);
		}

		private static IInstructionBlock GetThen (IInstructionBlock block)
		{
			return block.Successors [0];
		}

		private static IInstructionBlock GetElse (IInstructionBlock block)
		{
			return block.Successors [1];
		}
	}
}
