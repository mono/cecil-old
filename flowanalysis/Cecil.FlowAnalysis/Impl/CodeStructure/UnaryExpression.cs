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

using Mono.Cecil;
using Mono.Cecil.Cil;
using Cecil.FlowAnalysis.CodeStructure;

namespace Cecil.FlowAnalysis.Impl.CodeStructure {

	internal class UnaryExpression : IUnaryExpression	{
		UnaryOperator _operator;
		IExpression _operand;

		public UnaryExpression (UnaryOperator operator_, IExpression operand)
		{
			_operator = operator_;
			_operand = operand;
		}

		public UnaryOperator Operator
		{
			get	{ return _operator; }
		}

		public IExpression Operand
		{
			get	{ return _operand; }
		}

		public CodeElementType CodeElementType
		{
			get { return CodeElementType.UnaryExpression; }
		}

		public void Accept (ICodeStructureVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
