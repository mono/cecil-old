using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps
{
    public class RebuildBooleanReturns : BaseCodeTransformer, IDecompilationStep
    {
        public static IDecompilationStep Instance = new RebuildBooleanReturns();

        public override ICodeNode VisitReturnStatement(ReturnStatement node)
        {
            if(node.Expression.CodeNodeType != CodeNodeType.LiteralExpression)
            {
                return base.VisitReturnStatement(node);
            }

            var returnExpression = (LiteralExpression) node.Expression;

            if(returnExpression.Value.Equals(1))
            {
                returnExpression.Value = true;
            }
            else if(returnExpression.Value.Equals(0))
            {
                returnExpression.Value = false;
            }

            return node;
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement block)
        {
            if (context.Method.ReturnType.FullName != "System.Boolean")
            {
                return block;
            }
            return (BlockStatement)VisitBlockStatement(block);
        }
    }
}
