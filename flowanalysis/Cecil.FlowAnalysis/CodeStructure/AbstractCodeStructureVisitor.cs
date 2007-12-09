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

// Warning: generated do not edit

using System.Collections;

namespace Cecil.FlowAnalysis.CodeStructure {

	public class AbstractCodeStructureVisitor : ICodeStructureVisitor {

		public virtual void Visit (ICodeElement node)
		{
			if (null == node) return;
			node.Accept (this);
		}

		public virtual void Visit (ICollection collection)
		{
			foreach (ICodeElement node in collection)
			{
				Visit (node);
			}
		}

		public virtual void Visit (MethodInvocationExpression node)
		{
			Visit (node.Target);
			Visit (node.Arguments);
		}

		public virtual void Visit (MethodReferenceExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (LiteralExpression node)
		{
		}

		public virtual void Visit (UnaryExpression node)
		{
			Visit (node.Operand);
		}

		public virtual void Visit (BinaryExpression node)
		{
			Visit (node.Left);
			Visit (node.Right);
		}

		public virtual void Visit (AssignExpression node)
		{
			Visit (node.Target);
			Visit (node.Expression);
		}

		public virtual void Visit (ArgumentReferenceExpression node)
		{
		}

		public virtual void Visit (VariableReferenceExpression node)
		{
		}

		public virtual void Visit (ThisReferenceExpression node)
		{
		}

		public virtual void Visit (FieldReferenceExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (PropertyReferenceExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (BlockStatement node)
		{
			Visit (node.Statements);
		}

		public virtual void Visit (ReturnStatement node)
		{
			Visit (node.Expression);
		}

		public virtual void Visit (CastExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (TryCastExpression node)
		{
			Visit (node.Target);
		}
	}
}
