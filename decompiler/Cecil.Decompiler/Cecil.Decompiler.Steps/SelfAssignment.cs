#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
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

using Mono.Cecil.Cil;

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps {

	class SelfAssignement : BaseCodeTransformer, IDecompilationStep {

		public static readonly IDecompilationStep Instance = new SelfAssignement ();
		const string VariableKey = "Variable";
		const string OperatorKey = "Operator";

		static readonly Pattern.ICodePattern SelfAssignmentPattern = new Pattern.Assignment {
			Target = new Pattern.VariableReference {
				Bind = var => new Pattern.MatchData (VariableKey, var.Variable)
			},
			Expression = new Pattern.Binary {
				Bind = binary => new Pattern.MatchData (OperatorKey, binary.Operator),
				Left = new Pattern.ContextVariableReference { Name = VariableKey },
				Right = new Pattern.Literal { Value = 1 }
			}
		};

		public override ICodeNode VisitAssignExpression (AssignExpression node)
		{
			var result = Pattern.CodePattern.Match (SelfAssignmentPattern, node);
			if (!result.Success)
				return base.VisitAssignExpression (node);

			var variable = (VariableReference) result [VariableKey];
			var @operator = (BinaryOperator) result [OperatorKey];

			switch (@operator) {
			case BinaryOperator.Add:
			case BinaryOperator.Subtract:
				return new UnaryExpression (
					GetCorrespondingOperator (@operator),
					new VariableReferenceExpression (variable));
			default:
				return base.VisitAssignExpression (node);
			}
		}

		static UnaryOperator GetCorrespondingOperator (BinaryOperator @operator)
		{
			switch (@operator) {
			case BinaryOperator.Add:
				return UnaryOperator.PostIncrement;
			case BinaryOperator.Subtract:
				return UnaryOperator.PostDecrement;
			default:
				throw new ArgumentException ();
			}
		}

		public BlockStatement Process (DecompilationContext context, BlockStatement body)
		{
			return (BlockStatement) VisitBlockStatement (body);
		}
	}
}
