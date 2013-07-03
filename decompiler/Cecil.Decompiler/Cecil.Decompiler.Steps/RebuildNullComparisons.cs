using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps
{
    public class RebuildNullComparisons : BaseCodeTransformer, IDecompilationStep
    {
        public static IDecompilationStep Instance = new RebuildNullComparisons();

        public override ICodeNode VisitIfStatement(IfStatement node)
        {
            switch(node.Condition.CodeNodeType)
            {
                case CodeNodeType.ArgumentReferenceExpression:
                case CodeNodeType.VariableReferenceExpression:
                    node.Condition = RebuildNullComparison(node.Condition);
                    break;
                case CodeNodeType.UnaryExpression:
                    node.Condition = TraverseUnaryExpression((UnaryExpression) node.Condition);
                    break;
            }
            return base.VisitIfStatement(node);
        }

        private static Expression RebuildNullComparison(Expression condition)
        {
            switch(condition.CodeNodeType)
            {
                case CodeNodeType.ArgumentReferenceExpression:
                    var argRef = (ArgumentReferenceExpression) condition;
                    if(argRef.Parameter.ParameterType.FullName != "System.Boolean")
                    {
                        condition = new BinaryExpression(BinaryOperator.ValueInequality, argRef, new LiteralExpression(null));
                    }
                    break;
                case CodeNodeType.VariableReferenceExpression:
                    var variableRef = (VariableReferenceExpression) condition;
                    if(variableRef.Variable.VariableType.FullName != "System.Boolean")
                    {
                        condition = new BinaryExpression(BinaryOperator.ValueInequality, variableRef, new LiteralExpression(null));
                    }
                    break;
            }
            return condition;           
        }

        private static Expression TraverseUnaryExpression(UnaryExpression expression)
        {
            var nullComparison = RebuildNullComparison(expression.Operand);
            if(nullComparison.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                if(expression.Operator == UnaryOperator.LogicalNot)
                    ((BinaryExpression) nullComparison).Operator = BinaryOperator.ValueEquality;
                return nullComparison;
            }
            return expression;
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement block)
        {
            return (BlockStatement)Visit(block);
        }
    }
}
