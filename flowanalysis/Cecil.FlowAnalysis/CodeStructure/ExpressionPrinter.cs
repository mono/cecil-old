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
using Cecil.FlowAnalysis.CecilUtilities;
using Mono.Cecil;

namespace Cecil.FlowAnalysis.CodeStructure {
	/// <summary>
	/// </summary>
	public class ExpressionPrinter : AbstractCodeStructureVisitor {
		public static string ToString (IExpression expression)
		{
			ExpressionPrinter printer = new ExpressionPrinter ();
			expression.Accept (printer);
			return printer.Writer.ToString ();
		}

		private TextWriter _writer;

		public ExpressionPrinter ()
		{
			_writer = new StringWriter ();
		}

		public ExpressionPrinter (TextWriter writer)
		{
			if (null == writer) throw new ArgumentNullException ("writer");
			_writer = writer;
		}

		public TextWriter Writer {
			get { return _writer; }
		}

		public override void Visit (IAssignExpression node)
		{
			Visit (node.Target);
			Write (" = ");
			Visit (node.Expression);
		}

		public override void Visit (IBinaryExpression node)
		{
			Write ("(");
			Visit (node.Left);
			Write (" {0} ", ToString (node.Operator));
			Visit (node.Right);
			Write (")");
		}

		public override void Visit (IArgumentReferenceExpression node)
		{
			Write (node.Parameter.Name);
		}

		public override void Visit (IThisReferenceExpression node)
		{
			Write ("this");
		}

		public override void Visit (IFieldReferenceExpression node)
		{
			Visit (node.Target);
			Write (".{0}", node.Field.Name);
		}

		public override void Visit (IMethodReferenceExpression node)
		{
			MethodReference method = node.Method;
			if (null == node.Target) {
				Write (CecilFormatter.FormatTypeReference (method.DeclaringType));
			} else {
				Visit (node.Target);
			}
			Write (".{0}", method.Name);
		}

		public override void Visit (IMethodInvocationExpression node)
		{
			Visit (node.Target);
			Write ("(");
			for (int i=0; i < node.Arguments.Count; ++i) {
				if (i > 0) Write (", ");
				Visit (node.Arguments [i]);
			}
			Write (")");
		}

		public override void Visit (IVariableReferenceExpression node)
		{
			string name = "local" + node.Variable.Index;
			Write (name);
		}

		public override void Visit (ILiteralExpression node)
		{
			object value = node.Value;
			if (value is string) {
				Write ("\"{0}\"", value);
				return;
			}

			Write (value == null ? "null" : CecilFormatter.ToInvariantCultureString (value).ToLower ());
		}

		public override void Visit (IUnaryExpression node)
		{
			Write ("(");
			Write (ToString (node.Operator));
			Visit (node.Operand);
			Write (")");
		}

		private string ToString (UnaryOperator op)
		{
			switch (op) {
			case UnaryOperator.Not: return "!";

			}
			throw new ArgumentException (op.ToString (), "op");
		}

		private string ToString (BinaryOperator op)
		{
			switch (op) {
			case BinaryOperator.LogicalAnd: return "&&";
			case BinaryOperator.LogicalOr: return "||";
			case BinaryOperator.Multiply: return "*";
			case BinaryOperator.ValueEquality: return "==";
			case BinaryOperator.ValueInequality: return "!=";
			case BinaryOperator.LessThan: return "<";
			case BinaryOperator.LessThanOrEqual: return "<=";
			case BinaryOperator.GreaterThan: return ">";
			case BinaryOperator.GreaterThanOrEqual: return ">=";
			}
			throw new ArgumentException (op.ToString (), "op");
		}

		private void Write (string text)
		{
			_writer.Write (text);
		}

		private void Write (string format, params object[] args)
		{
			_writer.Write (format, args);
		}
	}
}
