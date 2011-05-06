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
using Cecil.FlowAnalysis.Utilities;
using Cecil.FlowAnalysis.CodeStructure;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Cecil.FlowAnalysis.ActionFlow {

	internal class Stack<T> : IEnumerable<T> {

		readonly Collection<T> collection = new Collection<T> ();

		public int Count {
			get { return collection.Count; }
		}

		public T Peek ()
		{
			if (collection.Count == 0)
				throw new InvalidOperationException ();

			return collection [collection.Count - 1];
		}

		public void Push (T value)
		{
			collection.Add (value);
		}

		public T Pop ()
		{
			if (collection.Count == 0)
				throw new InvalidOperationException ();

			var top = collection.Count - 1;
			var value = collection [top];
			collection.RemoveAt (top);
			return value;
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return collection.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	internal class ExpressionDecompiler : AbstractInstructionVisitor {

		Stack<Expression> _expressionStack;
		MethodDefinition _method;

		public int Count {
			get { return _expressionStack.Count; }
		}

		public ExpressionDecompiler (MethodDefinition method)
		{
			_method = method;
			_expressionStack = new Stack<Expression> ();
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
			PushVariableAssignement (0);
		}

		public override void OnStloc_1 (Instruction instruction)
		{
			PushVariableAssignement (1);
		}

		public override void OnStloc_2 (Instruction instruction)
		{
			PushVariableAssignement (2);
		}

		public override void OnStloc_3 (Instruction instruction)
		{
			PushVariableAssignement (3);
		}

		public override void OnStloc (Instruction instruction)
		{
			PushVariableAssignement ((VariableReference) instruction.Operand);
		}

		private void PushVariableAssignement (VariableReference variable)
		{
			PushVariableAssignement (variable.Index);
		}

		private void PushVariableAssignement (int index)
		{
			PushVariableReference (index);
			PushAssignment (Pop (), Pop ());
		}

		public override void OnStsfld (Instruction instruction)
		{
			FieldReference field = (FieldReference) instruction.Operand;
			PushAssignment (new FieldReferenceExpression (null, field), Pop());
		}

		public override void OnStfld (Instruction instruction)
		{
			FieldReference field = (FieldReference) instruction.Operand;
			Expression expression = Pop ();
			Expression target = Pop ();
			PushAssignment(new FieldReferenceExpression (target, field), expression);
		}

		private void PushAssignment (Expression lvalue, Expression rvalue)
		{
			Push (
				new AssignExpression (
					lvalue,
					rvalue));
		}

		public override void OnCallvirt (Instruction instruction)
		{
			OnCall (instruction);
		}

		public override void OnBox (Instruction instruction)
		{
			Push(
				new CastExpression (
					Pop (),
					(TypeReference) instruction.Operand));
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

			Collection<Expression> args = PopRange (method.Parameters.Count);
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
			PushBinaryExpression (BinaryOperator.Add);
		}

		public override void OnSub (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.Subtract);
		}

		public override void OnMul (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.Multiply);
		}

		public override void OnDiv (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.Divide);
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

		public override void OnLdloc_1 (Instruction instruction)
		{
			PushVariableReference (1);
		}

		public override void OnLdloc_2 (Instruction instruction)
		{
			PushVariableReference (2);
		}

		public override void OnLdloc_3 (Instruction instruction)
		{
			PushVariableReference (3);
		}

		public override void OnLdloc (Instruction instruction)
		{
			PushVariableReference ((VariableReference) instruction.Operand);
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
				if (null != mre) return mre.Method.ReturnType.MetadataType == MetadataType.Boolean;
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

		public override void OnClt_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThan);
		}

		public override void OnCgt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThan);
		}

		public override void OnCgt_Un (Instruction instruction)
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

		public override void OnBle_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThanOrEqual);
		}

		public override void OnBgt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThan);
		}

		public override void OnBgt_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThan);
		}

		public override void OnBge (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThanOrEqual);
		}

		public override void OnBge_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.GreaterThanOrEqual);
		}

		public override void OnBlt (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThan);
		}

		public override void OnBlt_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LessThan);
		}

		public override void OnShr (Instruction instruction)
		{
 			PushBinaryExpression (BinaryOperator.RightShift);
		}

		public override void OnShr_Un (Instruction instruction)
		{
 			PushBinaryExpression (BinaryOperator.RightShift);
		}

		public override void OnShl (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.LeftShift);
		}

		public override void OnOr (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.BitwiseOr);
		}

		public override void OnAnd (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.BitwiseAnd);
		}

		public override void OnXor (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.BitwiseXor);
		}

		public override void OnRem (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.Modulo);
		}

		public override void OnRem_Un (Instruction instruction)
		{
			PushBinaryExpression (BinaryOperator.Modulo);
		}

		public override void OnNot (Instruction instruction)
		{
			PushUnaryExpression (UnaryOperator.BitwiseComplement, Pop ());
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
			Push (new FieldReferenceExpression (Pop (), (FieldReference) instruction.Operand));
		}

		public override void OnLdsfld (Instruction instruction)
		{
			Push (new FieldReferenceExpression (null, (FieldReference) instruction.Operand));
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

		public override void OnLdarg_2 (Instruction instruction)
		{
			PushArgumentReference (2);
		}

		public override void OnLdarg_3 (Instruction instruction)
		{
			PushArgumentReference (3);
		}

		public override void OnLdarg (Instruction instruction)
		{
			PushArgumentReference (((VariableDefinition)instruction.Operand).Index);
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
			PushUnaryExpression (UnaryOperator.Not, expression);
		}

		void PushUnaryExpression (UnaryOperator op, Expression expression)
		{
			Push (new UnaryExpression (op, expression));
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

		Collection<Expression> PopRange (int count)
		{
			Collection<Expression> range = new Collection<Expression> ();
			for (int i=0; i < count; ++i) {
				range.Insert (0, Pop ());
			}
			return range;
		}
	}
}
