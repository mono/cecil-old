using System.Collections.Generic;
using Cecil.Decompiler.Ast;
using Mono.Cecil;

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
                var variableBase = ToString(node.Variable.VariableType);
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

        string ToString(TypeReference reference)
        {
            string name = reference.Name;
            if (reference is TypeSpecification)
                name = ToString((TypeSpecification)reference);
            var index = name.LastIndexOf('`');
            if (index > 0)
                name = name.Substring(0, index);
            return name;
        }

        string ToString(TypeSpecification specification)
        {
            var pointer = specification as PointerType;
            if (pointer != null)
            {
                return ToString(specification.ElementType) + "Pointer";
            }

            var reference = specification as ByReferenceType;
            if (reference != null)
            {
                return ToString(specification.ElementType) + "Reference";
            }

            var array = specification as ArrayType;
            if (array != null)
            {
                return ToString(specification.ElementType) + "Array";
            }

            var generic = specification as GenericInstanceType;
            if (generic != null)
            {
                return ToString(generic);
            }

            return specification.Name;
        }

        string ToString(GenericInstanceType generic)
        {
            return ToString(generic.ElementType);
        }
    }
}