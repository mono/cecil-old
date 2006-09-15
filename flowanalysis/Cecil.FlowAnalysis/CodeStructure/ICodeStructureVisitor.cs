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

	public interface ICodeStructureVisitor	{
		void Visit (IMethodInvocationExpression node);
		void Visit (IMethodReferenceExpression node);
		void Visit (ILiteralExpression node);
		void Visit (IUnaryExpression node);
		void Visit (IBinaryExpression node);
		void Visit (IAssignExpression node);
		void Visit (IArgumentReferenceExpression node);
		void Visit (IVariableReferenceExpression node);
		void Visit (IThisReferenceExpression node);
		void Visit (IFieldReferenceExpression node);
		void Visit (IPropertyReferenceExpression node);
		void Visit (IBlockStatement node);
		void Visit (IReturnStatement node);
	}
}
