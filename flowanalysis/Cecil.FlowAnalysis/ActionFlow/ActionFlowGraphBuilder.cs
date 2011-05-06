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
using System.Collections.Generic;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.ControlFlow;
using Cecil.FlowAnalysis.CodeStructure;
using Cecil.FlowAnalysis.Utilities;
using Mono.Cecil;
using Mono.Cecil.Cil;

using Mono.Collections.Generic;

namespace Cecil.FlowAnalysis.ActionFlow {

	/// <summary>
	/// </summary>
	internal class ActionFlowGraphBuilder : AbstractInstructionVisitor {

		Collection<ActionBlock> _blocks = new Collection<ActionBlock> ();
		Dictionary<Instruction, ActionBlock> _instruction2block = new Dictionary<Instruction, ActionBlock> ();
		HashSet<InstructionBlock> _processed = new HashSet<InstructionBlock> ();
		ExpressionDecompiler _expressionDecompiler;
		MethodDefinition _method;
		ControlFlowGraph _cfg;
		Instruction _current;
		ActionFlowGraph _graph;

		internal ActionFlowGraphBuilder (ControlFlowGraph cfg)
		{
			_method = cfg.MethodBody.Method;

			_cfg = cfg;
			_expressionDecompiler = new ExpressionDecompiler (_method);

			_graph = new ActionFlowGraph (_cfg, _blocks);
		}

		public ActionFlowGraph BuildGraph ()
		{
			Run ();

			return _graph;
		}

		void Run ()
		{
			ProcessBlocks ();
			ConnectActionBlocks ();
			SimplifyActionBlocks ();
		}

		void SimplifyActionBlocks ()
		{
			int index = 0;
			while (index < _blocks.Count) {
				ActionBlock block = _blocks [index];
				switch (block.ActionType) {
				case ActionType.ConditionalBranch:
					/// if condition goto label
					/// return expression
					/// label: return true
					/// 		|
					/// 		V
					/// return (condition || expression)
					///
					/// label: return false
					/// 		|
					/// 		V
					/// return (!condition || expression)
					ConditionalBranchActionBlock cbr = (ConditionalBranchActionBlock)block;
					if (IsReturnTrueOrFalse (cbr.Then)
						&& IsReturn (cbr.Else)
						&& 1 == cbr.Then.Predecessors.Count
						&& 1 == cbr.Else.Predecessors.Count) {
						BinaryOperator op = BinaryOperator.LogicalOr;
						Expression lhs = cbr.Condition;
						Expression rhs = ((ReturnActionBlock) cbr.Else).Expression;

						if (((LiteralExpression) ((ReturnActionBlock) cbr.Then).Expression).Value.Equals (0)) {
							op = BinaryOperator.LogicalAnd;
							_expressionDecompiler.Negate (lhs);
							lhs = _expressionDecompiler.Pop ();
						}

						ActionBlock newBlock = new ReturnActionBlock (
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

		static bool IsReturnTrueOrFalse (ActionBlock block)
		{
			ReturnActionBlock ret = block as ReturnActionBlock;
			return ret != null && IsTrueOrFalse (ret.Expression);
		}

		static bool IsTrueOrFalse (Expression expression)
		{
			LiteralExpression literal = expression as LiteralExpression;
			return literal != null
				&& literal.Value != null
				&& (literal.Value.Equals (1)
				|| literal.Value.Equals (0));
		}

		static bool IsReturn (ActionBlock block)
		{
			return block.ActionType == ActionType.Return;
		}

		void ConnectActionBlocks ()
		{
			for (int i=0; i < _blocks.Count; ++i) {
				ActionBlock block = _blocks [i];
				switch (block.ActionType) {
				case ActionType.Branch:
					BranchActionBlock br = (BranchActionBlock) block;
					br.SetTarget (GetBranchTarget (br.SourceInstruction));
					break;

				case ActionType.ConditionalBranch:
					ConditionalBranchActionBlock cbr = (ConditionalBranchActionBlock) block;
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

		ActionBlock GetBlockAtOrNull (int index)
		{
			return index >= _blocks.Count
				? null
				: _blocks [index];
		}

		ActionBlock GetBranchTarget (Instruction instruction)
		{
			return GetActionBlock ((Instruction) instruction.Operand);
		}

		void ProcessBlocks ()
		{
			foreach (InstructionBlock block in _cfg.Blocks) {
				if (WasProcessed (block)) continue;
				_current = block.FirstInstruction;
				ProcessBlock (block);
			}
		}

		void ProcessBlock (InstructionBlock block)
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

		void ProcessTwoWayBlock (InstructionBlock block)
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
		bool IsLogicalExpression (InstructionBlock block)
		{
			return IsLogicalExpression (new HashSet<InstructionBlock> (), block);
		}

		bool IsLogicalExpression (HashSet<InstructionBlock> visited, InstructionBlock block)
		{
			if (visited.Contains (block)) return false;
			visited.Add (block);
			foreach (InstructionBlock successor in block.Successors) {
				if (GetStackAfter (successor.LastInstruction) > 0) return true;
				if (IsLogicalExpression (visited, successor)) return true;
			}

			return false;
		}

		void ProcessSimpleBlock (InstructionBlock block)
		{
			foreach (Instruction instruction in block) {
				_expressionDecompiler.Visit (instruction);
				if (0 == GetStackAfter (instruction)) {
					CreateActionBlock (instruction);
					_current = instruction.Next;
				}
			}
		}

		void ProcessLogicalExpressionBlock (InstructionBlock block)
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
				throw new ArgumentException ("Unknown expression pattern starting at " + Formatter.FormatInstruction (block.FirstInstruction), "block");
			}
			MarkProcessed (block.Successors);
		}

		enum ExpressionPattern {
			Unknown,
			SimpleOr,
			SimpleNotAnd,
			NestedNotAnd
		}

		static ExpressionPattern DetectExpressionPattern (InstructionBlock block)
		{
			InstructionBlock then = GetThen (block);
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

		void ProcessNestedNotAnd (InstructionBlock block)
		{
			BuildNegateExpression (block);
			InstructionBlock y = GetElse (block);
			BuildExpression (y);
			_expressionDecompiler.PushBinaryExpression (BinaryOperator.LogicalAnd);
			ProcessNestedExpression (y);
		}

		void ProcessOr (InstructionBlock block)
		{
			BuildExpression (block);
			ProcessNestedExpression (GetElse (block));
			_expressionDecompiler.PushBinaryExpression (BinaryOperator.LogicalOr);
		}

		void ProcessNotAnd (InstructionBlock block)
		{
			BuildNegateExpression (block);
			ProcessNestedExpression (GetElse (block));
			_expressionDecompiler.PushBinaryExpression (BinaryOperator.LogicalAnd);
		}

		void BuildNegateExpression (InstructionBlock block)
		{
			BuildExpression (block);
			_expressionDecompiler.Negate ();
		}

		void ProcessNestedExpression (InstructionBlock block)
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

		void BuildExpression (InstructionBlock block)
		{
			if (WasProcessed (block)) return;
			foreach (Instruction instruction in block) {
				_expressionDecompiler.Visit (instruction);
			}
			MarkProcessed (block);
		}

		static bool IsTrue (InstructionBlock block)
		{
			return block.FirstInstruction == block.LastInstruction
				&& block.FirstInstruction.OpCode.Value == OpCodes.Ldc_I4_1.Value;
		}

		static bool IsFalse (InstructionBlock block)
		{
			return block.FirstInstruction == block.LastInstruction
				&& block.FirstInstruction.OpCode.Value == OpCodes.Ldc_I4_0.Value;
		}

		void AssertStackIsEmpty ()
		{
			if (0 == _expressionDecompiler.Count) return;
			throw new InvalidOperationException ("stack should be empty after a statement");
		}

		void CreateActionBlock (Instruction instruction)
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

		void AddConditionalBranch (Instruction instruction)
		{
			Add (new ConditionalBranchActionBlock (instruction, Pop ()));
		}

		public override void OnNop (Instruction instruction)
		{
		}

		public override void OnRet (Instruction instruction)
		{
			Expression expression = null;
			if (1 == GetStackBefore (instruction))
				expression = Pop ();

			Add (new ReturnActionBlock (instruction, expression));
		}

		public override void OnCall (Instruction instruction)
		{
			Add (new InvokeActionBlock (instruction, (MethodInvocationExpression) Pop ()));
		}

		public override void OnCallvirt (Instruction instruction)
		{
			OnCall (instruction);
		}

		public override void OnPop (Instruction instruction)
		{
			Visit (instruction.Previous);
		}

		public override void OnBr (Instruction instruction)
		{
			Add (new BranchActionBlock (instruction));
		}

		public override void OnStloc_0 (Instruction instruction)
		{
			Add (new AssignActionBlock (instruction, (AssignExpression) Pop ()));
		}

		public override void OnStfld (Instruction instruction)
		{
			Add (new AssignActionBlock (instruction, (AssignExpression) Pop ()));
		}

		public override void OnStsfld (Instruction instruction)
		{
			OnStfld (instruction);
		}

		int GetStackBefore (Instruction instruction)
		{
			return GetInstructionData (instruction).StackBefore;
		}

		int GetStackAfter (Instruction instruction)
		{
			return GetInstructionData (instruction).StackAfter;
		}

		InstructionData GetInstructionData (Instruction instruction)
		{
			return _cfg.GetData (instruction);
		}

		void Add (ActionBlock block)
		{
			if (null == block) throw new ArgumentNullException ("block");
			_blocks.Add (block);
			_instruction2block.Add (_current, block);
		}

		ActionBlock GetActionBlock (Instruction block)
		{
			return _instruction2block [block];
		}

		Expression Pop ()
		{
			return _expressionDecompiler.Pop ();
		}

		void MarkProcessed (InstructionBlock [] blocks)
		{
			foreach (InstructionBlock block in blocks) {
				MarkProcessed (block);
			}
		}

		void MarkProcessed (InstructionBlock block)
		{
			_processed.Add (block);
		}

		bool WasProcessed (InstructionBlock block)
		{
			return _processed.Contains (block);
		}

		static InstructionBlock GetThen (InstructionBlock block)
		{
			return block.Successors [0];
		}

		static InstructionBlock GetElse (InstructionBlock block)
		{
			return block.Successors [1];
		}
	}
}
