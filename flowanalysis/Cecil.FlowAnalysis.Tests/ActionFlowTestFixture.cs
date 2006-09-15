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
using System.IO;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CodeStructure;
using Mono.Cecil;
using NUnit.Framework;

namespace Cecil.FlowAnalysis.Tests {
	[TestFixture]
	public class ActionFlowTestFixture : AbstractFlowAnalysisTestFixture {

		[Test]
		public void IntPropertyEquals1 ()
		{
			RunTestCase ("IntPropertyEquals1");
		}

		[Test]
		public void IntPropertyEquals2 ()
		{
			RunTestCase ("IntPropertyEquals2");
		}

		[Test]
		public void FloatGreaterThan ()
		{
			RunTestCase ("FloatGreaterThan");
		}

		[Test]
		public void FloatEquals ()
		{
			RunTestCase ("FloatEquals");
		}

		[Test]
		public void BoolOrLessOrEqualThan ()
		{
			RunTestCase ("BoolOrLessOrEqualThan");
		}

		[Test]
		[Ignore("TODO")]
		public void OptimizedNestedOr ()
		{
			RunTestCase ("OptimizedNestedOr");
		}

		[Test]
		public void NestedOrGreaterThan ()
		{
			RunTestCase ("NestedOrGreaterThan");
		}

		[Test]
		public void InRange ()
		{
			RunTestCase ("InRange");
		}

		[Test]
		public void OptimizedAnd ()
		{
			RunTestCase ("OptimizedAnd");
		}

		[Test]
		public void BoolAndGreaterOrEqualThan ()
		{
			RunTestCase ("BoolAndGreaterOrEqualThan");
		}

		[Test]
		public void OptimizedOr ()
		{
			RunTestCase ("OptimizedOr");
		}

		[Test]
		public void NotStringEquality ()
		{
			RunTestCase ("NotStringEquality");
		}

		[Test]
		public void IsNull ()
		{
			RunTestCase ("IsNull");
		}

		[Test]
		public void FieldAccessor ()
		{
			RunTestCase ("FieldAccessor");
		}

		[Test]
		public void NotEqual ()
		{
			RunTestCase ("NotEqual");
		}

		[Test]
		public void GreaterThanOrEqual ()
		{
			RunTestCase ("GreaterThanOrEqual");
		}

		[Test]
		public void LessThanOrEqual ()
		{
			RunTestCase ("LessThanOrEqual");
		}

		[Test]
		public void Empty ()
		{
			RunTestCase ("Empty");
		}

		[Test]
		public void SimpleReturn ()
		{
			RunTestCase ("SimpleReturn");
		}

		[Test]
		public void SimpleCalculation ()
		{
			RunTestCase ("SimpleCalculation");
		}

		[Test]
		public void SimpleCondition ()
		{
			RunTestCase ("SimpleCondition");
		}

		[Test]
		public void SimpleIf ()
		{
			RunTestCase ("SimpleIf");
		}

		[Test]
		public void ConditionalBranchActionBlock ()
		{
			IActionFlowGraph afg = GetActionFlowGraph ("SimpleIf");
			IConditionalBranchActionBlock cbr = (IConditionalBranchActionBlock) afg.Blocks [0];
			Assert.AreSame (afg.Blocks [2], cbr.Then, "Then");
			Assert.AreSame (afg.Blocks [1], cbr.Else, "Else");
		}

		[Test]
		public void SingleAnd ()
		{
			RunTestCase ("SingleAnd");
		}

		[Test]
		public void SingleOr ()
		{
			RunTestCase ("SingleOr");
		}

		[Test]
		public void MultipleOr ()
		{
			RunTestCase ("MultipleOr");
		}

		[Test]
		public void FalseIf ()
		{
			RunTestCase ("FalseIf");
		}

		[Test]
		public void PropertyPredicate ()
		{
			RunTestCase ("PropertyPredicate");
		}

		[Test]
		public void MixedAndOr ()
		{
			RunTestCase ("MixedAndOr");
		}

		[Test]
		public void MultipleAndOr ()
		{
			RunTestCase ("MultipleAndOr");
		}

		[Test]
		public void StringPredicate ()
		{
			RunTestCase ("StringPredicate");
		}

		protected void RunTestCase (string name)
		{
			IActionFlowGraph afg = GetActionFlowGraph (name);
			Assert.AreEqual (normalize (LoadTestCaseFile (name + "-afg.txt")), normalize (ToString (afg)));
		}

		private IActionFlowGraph GetActionFlowGraph (string name)
		{
			MethodDefinition method = LoadTestCaseMethod (name);
			return FlowGraphFactory.CreateActionFlowGraph (FlowGraphFactory.CreateControlFlowGraph (method));
		}

		class ActionFlowGraphPrinter {
			private ExpressionPrinter _expressionPrinter;
			private TextWriter _writer;

			public ActionFlowGraphPrinter (TextWriter writer)
			{
				_writer = writer;
				_expressionPrinter = new ExpressionPrinter (writer);
			}

			public void Print (IActionFlowGraph afg)
			{
				int i = 1;
				foreach (IActionBlock block in afg.Blocks) {
					if (afg.IsBranchTarget (block)) {
						WriteLabel (i);
					}
					switch (block.ActionType) {
					case ActionType.Return:
						WriteReturn ((IReturnActionBlock) block);
						break;

					case ActionType.Branch:
						WriteBranch (afg, (IBranchActionBlock) block);
						break;

					case ActionType.ConditionalBranch:
						WriteConditionalBranch (afg, (IConditionalBranchActionBlock) block);
						break;

					case ActionType.Assign:
						WriteAssign ((IAssignActionBlock) block);
						break;

					case ActionType.Invoke:
						WriteInvoke ((IInvokeActionBlock) block);
						break;

					default:
						throw new InvalidOperationException ();
					}
					_writer.WriteLine ();
					++i;
				}
			}

			void WriteLabel (int index)
			{
				_writer.Write ("block{0}: ", index);
			}

			void WriteConditionalBranch (IActionFlowGraph afg,  IConditionalBranchActionBlock block)
			{
				_writer.Write ("if ");
				WriteExpression (block.Condition);
				_writer.Write (' ');
				WriteGoto (afg, block.Then);
			}

			void WriteBranch (IActionFlowGraph afg,  IBranchActionBlock block)
			{
				WriteGoto (afg, block.Target);
			}

			private void WriteGoto (IActionFlowGraph afg, IActionBlock target)
			{
				_writer.Write ("goto block{0}", afg.Blocks.IndexOf (target) + 1);
			}

			void WriteAssign (IAssignActionBlock block)
			{
				WriteExpression (block.AssignExpression);
			}

			void WriteReturn (IReturnActionBlock block)
			{
				_writer.Write ("return");
				if (null != block.Expression) {
					_writer.Write (" ");
					WriteExpression (block.Expression);
				}
			}

			void WriteInvoke (IInvokeActionBlock block)
			{
				WriteExpression (block.Expression);
			}

			private void WriteExpression (IExpression expression)
			{
				_expressionPrinter.Visit (expression);
			}
		}

		public static string ToString (IActionFlowGraph afg)
		{
			StringWriter writer = new StringWriter ();
			new ActionFlowGraphPrinter (writer).Print (afg);
			return writer.ToString ();
		}
	}
}
