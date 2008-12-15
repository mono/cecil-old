using System.Collections.Generic;

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps {

	class RebuildOperators : BaseCodeTransformer, IDecompilationStep {

		public static readonly IDecompilationStep Instance = new RebuildOperators();

		static readonly Dictionary<string, BinaryOperator> binary_operators 
			= new Dictionary<string, BinaryOperator> ();

		static readonly Dictionary<string, UnaryOperator> unary_operators 
			= new Dictionary<string, UnaryOperator> ();

		static RebuildOperators ()
		{
			RegisterBinaryOperators ();
			RegisterUnaryOperators ();
		}

		static void RegisterUnaryOperators ()
		{
			unary_operators.Add ("op_UnaryNegation", UnaryOperator.Negate);
			unary_operators.Add ("op_LogicalNot", UnaryOperator.Not);

			//todo : handle theses cases
			// unary_operators.Add("op_Decrement", UnaryOperator.??);
			// unary_operators.Add("op_Increment", UnaryOperator.??);
			// unary_operators.Add("op_OnesComplement", UnaryOperator.??);
		}

		static void RegisterBinaryOperators ()
		{
			binary_operators.Add ("op_Equality", BinaryOperator.ValueEquality);
			binary_operators.Add ("op_Inequality", BinaryOperator.ValueInequality);
			binary_operators.Add ("op_GreaterThan", BinaryOperator.GreaterThan);
			binary_operators.Add ("op_GreaterThanOrEqual", BinaryOperator.GreaterThanOrEqual);
			binary_operators.Add ("op_LessThan", BinaryOperator.LessThan);
			binary_operators.Add ("op_LessThanOrEqual", BinaryOperator.LessThanOrEqual);
			binary_operators.Add ("op_Addition", BinaryOperator.Add);
			binary_operators.Add ("op_Subtraction", BinaryOperator.Subtract);
			binary_operators.Add ("op_Division", BinaryOperator.Divide);
			binary_operators.Add ("op_Multiply", BinaryOperator.Multiply);
			binary_operators.Add ("op_Modulus", BinaryOperator.Modulo);
			binary_operators.Add ("op_BitwiseAnd", BinaryOperator.BitwiseAnd);
			binary_operators.Add ("op_BitwiseOr", BinaryOperator.BitwiseOr);
			binary_operators.Add ("op_ExclusiveOr", BinaryOperator.BitwiseXor);
			binary_operators.Add ("op_RightShift", BinaryOperator.RightShift);
			binary_operators.Add ("op_LeftShift", BinaryOperator.LeftShift);
		}

		public void Process (DecompilationContext context, BlockStatement body)
		{
			Visit (body);
		}

		public override ICodeNode VisitMethodInvocationExpression (MethodInvocationExpression node)
		{
			var method_reference = node.Method as MethodReferenceExpression;
			if (method_reference == null)
				return base.VisitMethodInvocationExpression (node);

			BinaryOperator binary_operator;
			if (binary_operators.TryGetValue (method_reference.Method.Name, out binary_operator))
				return BuildBinaryExpression (binary_operator, node.Arguments [0], node.Arguments [1]);

			UnaryOperator unary_operator;
			if (unary_operators.TryGetValue (method_reference.Method.Name, out unary_operator))
				return BuildUnaryExpression (unary_operator, node.Arguments [0]);

			return base.VisitMethodInvocationExpression (node);
		}

		ICodeNode BuildUnaryExpression (UnaryOperator unary_operator, Expression expression)
		{
			base.Visit (expression);
			return new UnaryExpression (unary_operator, expression);
		}

		ICodeNode BuildBinaryExpression (BinaryOperator binary_operator, Expression left, Expression right)
		{
			base.Visit (left);
			base.Visit (right);
			return new BinaryExpression (binary_operator, left, right);
		}
	}
}