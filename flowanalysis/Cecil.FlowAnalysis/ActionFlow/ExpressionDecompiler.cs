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
using Cecil.FlowAnalysis.Utilities;
using Cecil.FlowAnalysis.CodeStructure;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.ActionFlow {

	internal class ExpressionDecompiler : AbstractInstructionVisitor {

		Stack _expressionStack;
		MethodDefinition _method;
		TypeReference _SystemBoolean;

		public int Count {
			get { return _expressionStack.Count; }
		}

		public ExpressionDecompiler (MethodDefinition method)
		{
			_method = method;
			_expressionStack = new Stack ();
			_SystemBoolean = _method.DeclaringType.Module.Import (typeof(bool));
		}

		public override void OnNop (Instruction instruction)
		{
		}

		public override void OnRet (Instruction instruction)
		{
		}

		public override void OnBr (Instruction instruction)
		{
		}

		public override void OnStloc_0 (Instruction instruction)
		{
			PushVariableReference (0);
			Push (new AssignExpression (Pop (), Pop ()));
		}

		public override void OnStfld (Instruction instruction)
		{
			FieldReference field = (FieldReference) instruction.Operand;
			Expression expression = Pop ();
			Expression target = Pop ();
			Push (
				new AssignExpression (
					new FieldReferenceExpression (target, field),
					expression));
		}

		public override void OnCallvirt (Instruction instruction)
		{
			OnCall (instruction);
		}

		public override void OnCastclass (Instruction instruction)
		{
			Push (
				new CastExpression (
					Pop (),
					(TypeReference) instruction.Operand));
		}

		public override void OnIsinst (Instruction instruction)
		{
			Push (
				new TryCastExpression (
					Pop (),
					(TypeReference) instruction.Operand));
		}

		public override void OnCall (Instruction instruction)
		{
			MethodReference method = (MethodReference) instruction.Operand;

			ExpressionCollection args = PopRange (method.Parameters.Count);
			Expression target = method.HasThis ? Pop () : null;

			Push (
				new MethodInvocationExpression (
					new MethodReferenceExpression (target, method), args));
		}

		public override void OnDup (Instruction instruction)
		{
			Expression expression = Pop ();
			Push (expression);
			Push (expression);
		}

		public override void OnPop (Instruction instruction)
		{
		}

		public override void OnAdd (Instruction instruction)
		{
			BinaryOperator op = BinaryOperator.Add;
			PushBinaryExpression (op);
		}

		public override void OnMul (Instruction instruction)
		{
			BinaryOperator op = BinaryOperator.Multiply;
			PushBinaryExpression (op);
		}

		public void PushBinaryExpression (BinaryOperator op)
		{
			Expression rhs = Pop ();
			Expression lhs = Pop ();
			Push (new BinaryExpression (op, lhs, rhs));
		}

		public override void OnLdstr (Instruction instruction)
		{
			PushLiteral (instruction.Operand);
		}

		public override void OnLdc_R4 (Instruction instruction)
		{
			PushLiteral (instruction.Operand);
		}

		public override void OnLdc_R8 (Instruction instruction)
		{
			PushLiteral (instruction.Operand);
		}

		public override void OnLdc_I8 (Instruction instruction)
		{
			PushLiteral (instruction.Operand);
		}

		public override void OnLdc_I4 (Instruction instruction)
		{
			PushLiteral (instruction.Operand);
		}

		public override void OnLdc_I4_M1 (Instruction instruction)
		{
			PushLiteral (-1);
		}

		public override void OnLdc_I4_0 (Instruction instruction)
		{
			PushLiteral (0);
		}

		public override void OnLdc_I4_1 (Instruction instruction)
		{
			PushLiteral (1);
		}

		public override void OnLdc_I4_2 (Instruction instruction)
		{
			PushLiteral (2);
		}

		public override void OnLdc_I4_3 (Instruction instruction)
		{
			PushLiteral (3);
		}

		public override void OnLdc_I4_4 (Instruction instruction)
		{
			PushLiteral (4);
		}

		public override void OnLdc_I4_5 (Instruction instruction)
		{
			PushLiteral (5);
		}

		public override void OnLdc_I4_6 (Instruction instruction)
		{
			PushLiteral (6);
		}

		public override void OnLdc_I4_7 (Instruction instruction)
		{
			PushLiteral (7);
		}

		public override void OnLdc_I4_8 (Instruction instruction)
		{
			PushLiteral (8);
		}

		public override void OnLdloc_0 (Instruction instruction)
		{
			PushVariableReference (0);
		}

		public override void OnCeq (Instruction instruction)
		{
			// XXX: ceq might be used for reference equality as well

			Expression rhs = Pop ();
			Expression lhs = Pop ();

			// simplify common expression patterns
			// ((x < y) == 0) => x >= y
			// ((x > y) == 0) => x <= y
			// ((x == y) == 0) => x != y
			// (BooleanMethod(x) == 0) => !BooleanMethod(x)
			if (IsBooleanExpression (lhs) && IsFalse (rhs)) {
				Negate (lhs);
			} else {
				Push (new BinaryExpression (BinaryOperator.ValueEquality, lhs, rhs));
			}
		}

		static bool IsFalse (Expression expression)
		{
			LiteralExpression literal = expression as LiteralExpression;
			return literal != null && literal.Value.Equals (0);
		}

		bool IsBooleanExpression (Expression expression)
		{
			switch (expression.CodeElementType) {
			case CodeElementType.BinaryExpression:
				return IsComparisonOperator (((BinaryExpression) expression).Operator);
			case CodeElementType.MethodInvocationExpression:
				MethodReferenceExpression mre = ((MethodInvocationExpression) expression).Target as MethodReferenceExpression;
				if (null != mre) return mre.Method.ReturnType.ReturnType == _SystemBoolean;
				break;
			}
			return false;
		}

		static bool IsComparisonOperator (BinaryOperator op)
		{
			switch (op) {
			case BinaryOperator.GreaterThan:
			case BinaryOperator.LessThan:
			case BinaryOperator.GreaterThanOrEqual:
			case BinaryOperator.LessThanOrEqual:
			case BinaryOperator.ValueEquality:
			case BinaryOperator.ValueInequality:
				return true;
			}
			return false;
		}

		public override void OnClt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThan);
		}

		public override void OnCgt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThan);
		}

		public override void OnBeq (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.ValueEquality);
		}

		public override void OnBne_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.ValueInequality);
		}

		public override void OnBle (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThanOrEqual);
		}

		public override void OnBgt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThan);
		}

		public override void OnBge (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThanOrEqual);
		}

		public override void OnBlt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThan);
		}

		public override void OnBrtrue (Instruction instruction)
		{
		}

		public override void OnBrfalse (Instruction instruction)
		{
			Negate ();
		}

		private void PushVariableReference (int index)
		{
			PushVariableReference (_method.Body.Variables [index]);
		}

		private void PushVariableReference (VariableReference variable)
		{
			Push (new VariableReferenceExpression (variable));
		}

		public override void OnLdfld (Instruction instruction)
		{
			FieldReference field = (FieldReference)instruction.Operand;
			Push (new FieldReferenceExpression (Pop (), field));
		}

		public override void OnLdnull (Instruction instruction)
		{
			PushLiteral (null);
		}

		public override void OnLdarg_0 (Instruction instruction)
		{
			PushArgumentReference (0);
		}

		public override void OnLdarg_1 (Instruction instruction)
		{
			PushArgumentReference (1);
		}

		public void Negate ()
		{
			Negate (Pop ());
		}

		public void Negate (Expression expression)
		{
			switch (expression.CodeElementType) {
			case CodeElementType.BinaryExpression:
				BinaryExpression be = (BinaryExpression) expression;
				BinaryOperator op = GetInverseOperator (be.Operator);
				if (BinaryOperator.None != op) {
					Push (new BinaryExpression (op, be.Left, be.Right));
				} else {
					if (BinaryOperator.LogicalAnd == be.Operator) {
						Negate (be.Left);
						Negate (be.Right);
						PushBinaryExpression (BinaryOperator.LogicalOr);
					} else {
						PushNotExpression (expression);
					}
				}
				break;

			case CodeElementType.UnaryExpression:
				UnaryExpression ue = (UnaryExpression) expression;
				switch (ue.Operator) {
				case UnaryOperator.Not:
					Push (ue.Operand);
					break;

				default:
					throw new ArgumentException ("expression");
				}
				break;

			default:
				PushNotExpression (expression);
				break;
			}
		}

		void PushNotExpression (Expression expression)
		{
			Push (new UnaryExpression (UnaryOperator.Not, expression));
		}

		static BinaryOperator GetInverseOperator (BinaryOperator op)
		{
			switch (op) {
			case BinaryOperator.ValueEquality:
				return BinaryOperator.ValueInequality;
			case BinaryOperator.ValueInequality:
				return BinaryOperator.ValueEquality;
			case BinaryOperator.LessThan:
				return BinaryOperator.GreaterThanOrEqual;
			case BinaryOperator.LessThanOrEqual:
				return BinaryOperator.GreaterThan;
			case BinaryOperator.GreaterThan:
				return BinaryOperator.LessThanOrEqual;
			case BinaryOperator.GreaterThanOrEqual:
				return BinaryOperator.LessThan;
			}
			return BinaryOperator.None;
		}

		void PushArgumentReference (int index)
		{
			if (_method.HasThis) {
				if (index == 0) {
					Push (new ThisReferenceExpression ());
					return;
				}
				index -= 1; // the Parameters collection dos not contain the implict this argument
			}
			Push (new ArgumentReferenceExpression (_method.Parameters [index]));
		}

		void PushLiteral (object value)
		{
			Push (new LiteralExpression (value));
		}

		void Push (Expression expression)
		{
			if (null == expression) throw new ArgumentNullException ("expression");
			_expressionStack.Push (expression);
		}

		public Expression Pop ()
		{
			return (Expression) _expressionStack.Pop ();
		}

		ExpressionCollection PopRange (int count)
		{
			ExpressionCollection range = new ExpressionCollection ();
			for (int i=0; i < count; ++i) {
				range.Insert (0, Pop ());
			}
			return range;
		}
	}
}
