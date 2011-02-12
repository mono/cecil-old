using System.Collections.Generic;
using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps
{
    class GenerateVariableNames : BaseCodeTransformer, IDecompilationStep
    {
        private HashSet<string> usedIdentifiers = new HashSet<string>();
        public static IDecompilationStep Instance = new GenerateVariableNames();

        public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            if(string.IsNullOrEmpty(node.Variable.Name))
            {
                var variableBase = node.Variable.VariableType.Name;
                variableBase = char.ToLower(variableBase[0]) + variableBase.Substring(1);
                int sequence = 1;
                while(usedIdentifiers.Contains(variableBase + sequence))
                {
                    sequence++;
                }
                node.Variable.Name = variableBase + sequence;
            }
            return base.VisitVariableDeclarationExpression(node);
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement block)
        {
            PopulateUsedIdentifiers(context);
            return (BlockStatement)Visit(block);
        }

        private void PopulateUsedIdentifiers(DecompilationContext context)
        {
            usedIdentifiers.Clear();
            foreach (var parameter in context.Method.Parameters)
            {
                usedIdentifiers.Add(parameter.Name);
            }
            foreach (var property in context.Method.DeclaringType.Properties)
            {
                usedIdentifiers.Add(property.Name);
            }
            foreach (var method in context.Method.DeclaringType.Methods)
            {
                usedIdentifiers.Add(method.Name);
            }
        }
    }
}