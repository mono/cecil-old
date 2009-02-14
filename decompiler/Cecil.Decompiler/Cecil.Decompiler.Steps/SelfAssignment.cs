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

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps {

	class SelfAssignement : BaseCodeTransformer, IDecompilationStep {

		public static readonly IDecompilationStep Instance = new SelfAssignement ();

		public override ICodeNode VisitAssignExpression (AssignExpression node)
		{
			var variable = node.Target as VariableReferenceExpression;
			if (variable == null)
				return base.VisitAssignExpression (node);

			var binary = node.Expression as BinaryExpression;
			if (binary == null)
				return base.VisitAssignExpression (node);

			var left = binary.Left as VariableReferenceExpression;
			if (left == null)
				return base.VisitAssignExpression (node);

			if (variable.Variable != left.Variable)
				return base.VisitAssignExpression (node);

			var literal = binary.Right as LiteralExpression;
			if (literal == null)
				return base.VisitAssignExpression (node);

			if (literal.Value != null && !literal.Value.Equals (1))
				return base.VisitAssignExpression (node);

			if (binary.Operator == BinaryOperator.Add)
				return new UnaryExpression (UnaryOperator.PostIncrement, variable);
			else if (binary.Operator == BinaryOperator.Subtract)
				return new UnaryExpression (UnaryOperator.PostDecrement, variable);

			return base.VisitAssignExpression (node);
		}

		public BlockStatement Process (DecompilationContext context, BlockStatement body)
		{
			return (BlockStatement) VisitBlockStatement (body);
		}
	}
}
