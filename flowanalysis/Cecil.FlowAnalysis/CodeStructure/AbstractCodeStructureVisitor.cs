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

namespace Cecil.FlowAnalysis.CodeStructure {

	public class AbstractCodeStructureVisitor : ICodeStructureVisitor {

		public virtual void Visit (ICodeElement node)
		{
			if (null == node) return;
			node.Accept (this);
		}

		public virtual void Visit (System.Collections.ICollection collection)
		{
			foreach (ICodeElement node in collection)
			{
				Visit (node);
			}
		}

		public virtual void Visit (IMethodInvocationExpression node)
		{
			Visit (node.Target);
			Visit (node.Arguments);
		}

		public virtual void Visit (IMethodReferenceExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (ILiteralExpression node)
		{
		}

		public virtual void Visit (IUnaryExpression node)
		{
			Visit (node.Operand);
		}

		public virtual void Visit (IBinaryExpression node)
		{
			Visit (node.Left);
			Visit (node.Right);
		}

		public virtual void Visit (IAssignExpression node)
		{
			Visit (node.Target);
			Visit (node.Expression);
		}

		public virtual void Visit (IArgumentReferenceExpression node)
		{
		}

		public virtual void Visit (IVariableReferenceExpression node)
		{
		}

		public virtual void Visit (IThisReferenceExpression node)
		{
		}

		public virtual void Visit (IFieldReferenceExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (IPropertyReferenceExpression node)
		{
			Visit (node.Target);
		}

		public virtual void Visit (IBlockStatement node)
		{
			Visit (node.Statements);
		}

		public virtual void Visit (IReturnStatement node)
		{
			Visit (node.Expression);
		}
	}
}
