#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
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
using System.Collections.Generic;
using System.Text;

using Mono.Cecil;

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Languages {

    public class CSharpWriter : BaseLanguageWriter {

        bool inside_binary;

        public CSharpWriter (ILanguage language, IFormatter formatter)
            : base (language, formatter)
        {
        }

        public override void Write(MethodDefinition method)
        {
            WriteMethodVisibility(method);
            Formatter.WriteSpace();

            if (method.IsStatic)
            {
                Formatter.WriteKeyword("static");
                Formatter.WriteSpace();
            }

            WriteMethodReturnType(method);
            Formatter.WriteSpace();

            Formatter.WriteNameReference(method.Name, method);

            Formatter.WriteParenthesisOpen("(");

            WriteParameters(method);

            Formatter.WriteParenthesisClose(")");

            if (method.HasBody)
            {
                Formatter.WriteLine();
                Write(method.Body.Decompile(Language));
            }
            else
            {
                Formatter.WriteGenericToken(";");
            }
            Formatter.WriteLine();
        }

        void WriteMethodVisibility (MethodDefinition method)
        {
            if (method.IsPrivate)
                Formatter.WriteKeyword("private");
            else if (method.IsPublic)
                Formatter.WriteKeyword("public");
            else if (method.IsFamily)
                Formatter.WriteKeyword("protected");
            else if (method.IsAssembly)
                Formatter.WriteKeyword("internal");
            else if (method.IsFamilyOrAssembly)
            {
                Formatter.WriteKeyword("protected");
                Formatter.WriteSpace();
                Formatter.WriteKeyword("internal");
            }
            else
            {
                Formatter.WriteComment("The IsFamilyAndAssembly visibility is not supported by C#.");
                Formatter.WriteLine();
                Formatter.WriteKeyword("protected");
                Formatter.WriteSpace();
                Formatter.WriteKeyword("internal");
            }
        }

        void WriteMethodReturnType (MethodDefinition method)
        {
            WriteReference (method.ReturnType);
            Formatter.WriteSpace();
        }

        void WriteParameters (MethodDefinition method)
        {
            for (int i = 0; i < method.Parameters.Count; i++) {
                var parameter = method.Parameters [i];

                if (i > 0) {
                    Formatter.WriteGenericToken(",");
                    Formatter.WriteSpace();
                }

                WriteReference (parameter.ParameterType);
                Formatter.WriteSpace();
                Formatter.WriteParameterName(parameter.Name);
            }
        }

        public override void Write (Statement statement)
        {
            Visit (statement);
            Formatter.WriteLine();
        }

        public override void Write (Expression expression)
        {
            Visit (expression);
        }

        public override void VisitBlockStatement (BlockStatement node)
        {
            WriteBlock (() => Visit (node.Statements));
        }

        void WriteBlock (Action action)
        {
            Formatter.WriteBlockStart("{");
            Formatter.WriteLine();
            Formatter.Indent();

            action ();

            Formatter.Outdent();
            Formatter.WriteBlockEnd("}");
            Formatter.WriteLine();
        }

        public override void VisitExpressionStatement (ExpressionStatement node)
        {
            Visit (node.Expression);
            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        public override void VisitVariableDeclarationExpression (VariableDeclarationExpression node)
        {
            var variable = node.Variable;

            WriteReference(variable.VariableType);
            Formatter.WriteSpace();
            Formatter.WriteVariableReference(variable.Name);
        }

        public override void VisitAssignExpression (AssignExpression node)
        {
            Write (node.Target);
            WriteOperatorBetweenSpace ("=");
            Write (node.Expression);
        }

        public override void VisitArgumentReferenceExpression (ArgumentReferenceExpression node)
        {
            Formatter.WriteVariableReference(node.Parameter.Name);
        }

        public override void VisitVariableReferenceExpression (VariableReferenceExpression node)
        {
            Formatter.WriteVariableReference(node.Variable.Name);
        }

        public override void VisitLiteralExpression(LiteralExpression node)
        {
            var value = node.Value;
            if (value == null)
            {
                Formatter.WriteNamedLiteral("null");
                return;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    Formatter.WriteNamedLiteral((bool)value ? "true" : "false");
                    return;
                case TypeCode.Char:
                    Formatter.WriteGenericToken("'");
                    Formatter.WriteLiteralChar(value.ToString());
                    Formatter.WriteGenericToken("'");
                    return;
                case TypeCode.String:
                    Formatter.WriteGenericToken("\"");
                    Formatter.WriteLiteralString(value.ToString());
                    Formatter.WriteGenericToken("\"");
                    return;
                // complete
                default:
                    Formatter.WriteLiteralNumber(value.ToString());
                    return;
            }
        }

        public override void VisitMethodInvocationExpression (MethodInvocationExpression node)
        {
            Visit (node.Method);
            Formatter.WriteParenthesisOpen("(");
            VisitList (node.Arguments);
            Formatter.WriteParenthesisClose(")");
        }

        public override void VisitBlockExpression (BlockExpression node)
        {
            VisitList (node.Expressions);
        }

        void VisitList (IList<Expression> list)
        {
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) {
                    Formatter.WriteGenericToken(",");
                    Formatter.WriteSpace();
                }

                Visit (list [i]);
            }
        }

        public override void VisitMethodReferenceExpression (MethodReferenceExpression node)
        {
            if (node.Target != null) {
                Visit (node.Target);
                Formatter.WriteGenericToken(".");
            }

            if (!node.Method.HasThis) {
                WriteReference(node.Method.DeclaringType);
                Formatter.WriteGenericToken(".");
            }

            Formatter.WriteNameReference(node.Method.Name, node.Method);
        }

        public override void VisitThisReferenceExpression (ThisReferenceExpression node)
        {
            Formatter.WriteKeyword("this");
        }

        public override void VisitBaseReferenceExpression (BaseReferenceExpression node)
        {
            Formatter.WriteKeyword("base");
        }

        public override void VisitBinaryExpression (BinaryExpression node)
        {
            var was_inside = inside_binary;
            inside_binary = true;

            if (was_inside)
                Formatter.WriteParenthesisOpen ("(");
            Visit (node.Left);
            Formatter.WriteSpace ();
            Formatter.WriteGenericToken (ToString (node.Operator));
            Formatter.WriteSpace ();
            Visit (node.Right);
            if (was_inside)
                Formatter.WriteParenthesisClose (")");

            inside_binary = was_inside;
        }

        public override void VisitFieldReferenceExpression (FieldReferenceExpression node)
        {
            if (node.Target != null)
                Visit (node.Target);
            else
                WriteReference (node.Field.DeclaringType);

            Formatter.WriteGenericToken(".");
            Formatter.WriteNameReference(node.Field.Name, node.Field);
        }

        static string ToString (BinaryOperator op)
        {
            switch (op) {
            case BinaryOperator.Add: return "+";
            case BinaryOperator.BitwiseAnd: return "&";
            case BinaryOperator.BitwiseOr: return "|";
            case BinaryOperator.BitwiseXor: return "^";
            case BinaryOperator.Divide: return "/";
            case BinaryOperator.GreaterThan: return ">";
            case BinaryOperator.GreaterThanOrEqual: return ">=";
            case BinaryOperator.LeftShift: return "<<";
            case BinaryOperator.LessThan: return "<";
            case BinaryOperator.LessThanOrEqual: return "<=";
            case BinaryOperator.LogicalAnd: return "&&";
            case BinaryOperator.LogicalOr: return "||";
            case BinaryOperator.Modulo: return "%";
            case BinaryOperator.Multiply: return "*";
            case BinaryOperator.RightShift: return ">>";
            case BinaryOperator.Subtract: return "-";
            case BinaryOperator.ValueEquality: return "==";
            case BinaryOperator.ValueInequality: return "!=";
            default: throw new ArgumentException ();
            }
        }

        public override void VisitUnaryExpression(UnaryExpression node)
        {
            bool is_post_op = IsPostUnaryOperator(node.Operator);

            if (!is_post_op)
                Formatter.WriteOperator(ToString(node.Operator));

            Visit(node.Operand);

            if (is_post_op)
                Formatter.WriteOperator(ToString(node.Operator));
        }

        static bool IsPostUnaryOperator (UnaryOperator op)
        {
            switch (op) {
            case UnaryOperator.PostIncrement:
            case UnaryOperator.PostDecrement:
                return true;
            default:
                return false;
            }
        }

        static string ToString (UnaryOperator op)
        {
            switch (op) {
            case UnaryOperator.BitwiseNot:
                return "~";
            case UnaryOperator.LogicalNot:
                return "!";
            case UnaryOperator.Negate:
                return "-";
            case UnaryOperator.PostDecrement:
            case UnaryOperator.PreDecrement:
                return "--";
            case UnaryOperator.PostIncrement:
            case UnaryOperator.PreIncrement:
                return "++";
            default: throw new ArgumentException ();
            }
        }

        void WriteReference(TypeReference reference)
        {
            Write(reference);
        }

        void Write(TypeReference type)
        {
            var spec = type as TypeSpecification;
            if (spec != null)
            {
                WriteSpecification(spec);
                return;
            }
            else if (type.Namespace == "System")
            {
                switch (type.Name)
                {
                    case "Decimal": Formatter.WriteAliasTypeKeyword("decimal", type); return;
                    case "Single": Formatter.WriteAliasTypeKeyword("float", type); return;
                    case "Byte": Formatter.WriteAliasTypeKeyword("byte", type); return;
                    case "SByte": Formatter.WriteAliasTypeKeyword("sbyte", type); return;
                    case "Char": Formatter.WriteAliasTypeKeyword("char", type); return;
                    case "Double": Formatter.WriteAliasTypeKeyword("double", type); return;
                    case "Boolean": Formatter.WriteAliasTypeKeyword("bool", type); return;
                    case "Int16": Formatter.WriteAliasTypeKeyword("short", type); return;
                    case "Int32": Formatter.WriteAliasTypeKeyword("int", type); return;
                    case "Int64": Formatter.WriteAliasTypeKeyword("long", type); return;
                    case "UInt16": Formatter.WriteAliasTypeKeyword("ushort", type); return;
                    case "UInt32": Formatter.WriteAliasTypeKeyword("uint", type); return;
                    case "UInt64": Formatter.WriteAliasTypeKeyword("ulong", type); return;
                    case "String": Formatter.WriteAliasTypeKeyword("string", type); return;
                    case "Void": Formatter.WriteAliasTypeKeyword("void", type); return;
                    case "Object": Formatter.WriteAliasTypeKeyword("object", type); return;
                }
            }

            // Remove generic arg count.
            var name = type.FullName;
            var index = name.LastIndexOf('`');
            if (index > 0)
                name = name.Substring(0, index);
            Formatter.WriteNameReference(name, type);
        }

        void WriteSpecification(TypeSpecification specification)
        {
            var pointer = specification as PointerType;
            if (pointer != null)
            {
                Write(specification.ElementType);
                Formatter.WriteGenericToken("*");
                return;
            }

            var reference = specification as ByReferenceType;
            if (reference != null)
            {
                Write(specification.ElementType);
                Formatter.WriteGenericToken("&");
                return;
            }

            var array = specification as ArrayType;
            if (array != null)
            {
                Write(specification.ElementType);
                Formatter.WriteGenericToken("[]");
                return;
            }

            var generic = specification as GenericInstanceType;
            if (generic != null)
            {
                WriteGenericInstanceType(generic);
                return;
            }

            throw new NotSupportedException();
        }

        void WriteGenericInstanceType(GenericInstanceType generic)
        {
            Write(generic.ElementType);
            Formatter.WriteParenthesisOpen("<");

            for (int i = 0; i < generic.GenericArguments.Count; i++)
            {
                if (i > 0)
                {
                    Formatter.WriteGenericToken(",");
                    Formatter.WriteSpace();
                }

                Write(generic.GenericArguments[i]);
            }

            Formatter.WriteParenthesisClose(">");
        }

        public override void VisitGotoStatement (GotoStatement node)
        {
            Formatter.WriteKeyword("goto");            
            Formatter.WriteLabelReference(node.Label);
            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        public override void VisitLabeledStatement (LabeledStatement node)
        {
            Formatter.Outdent();
            Formatter.WriteLabelReference(node.Label);
            Formatter.WriteGenericToken(":");
            Formatter.WriteLine();
            Formatter.Indent();
        }

        public override void VisitIfStatement (IfStatement node)
        {
            Formatter.WriteKeyword("if");
            Formatter.WriteSpace();
            WriteBetweenParenthesis(node.Condition);
            Formatter.WriteLine();

            Visit(node.Then);

            if (node.Else == null)
                return;

            Formatter.WriteKeyword("else");
            Formatter.WriteLine();

            Visit(node.Else);
        }

        public override void VisitContinueStatement (ContinueStatement node)
        {
            Formatter.WriteKeyword("continue");
            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        public override void VisitBreakStatement (BreakStatement node)
        {
            Formatter.WriteKeyword("break");
            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        void WriteBetweenParenthesis (Expression expression)
        {
            Formatter.WriteParenthesisOpen("(");
            Visit(expression);
            Formatter.WriteParenthesisClose(")");
        }

        public override void VisitConditionExpression (ConditionExpression node)
        {
            Formatter.WriteParenthesisOpen("(");
            Visit (node.Condition);
            WriteOperatorBetweenSpace("?");
            Visit (node.Then);
            WriteOperatorBetweenSpace(":");
            Visit (node.Else);
            Formatter.WriteParenthesisClose(")");
        }

        public override void VisitNullCoalesceExpression (NullCoalesceExpression node)
        {
            Visit (node.Condition);
            WriteOperatorBetweenSpace("??");
            Visit (node.Expression);
        }

        void WriteTokenBetweenSpace (string token)
        {
            Formatter.WriteSpace();
            Formatter.WriteGenericToken(token);
            Formatter.WriteSpace();
        }

        void WriteOperatorBetweenSpace(string token)
        {
            Formatter.WriteSpace();
            Formatter.WriteOperator(token);
            Formatter.WriteSpace();
        }

        public override void VisitThrowStatement (ThrowStatement node)
        {
            Formatter.WriteKeyword("throw");
            if (node.Expression != null) {
                Formatter.WriteSpace();
                Visit (node.Expression);
            }
            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        public override void VisitReturnStatement (ReturnStatement node)
        {
            Formatter.WriteKeyword("return");

            if (node.Expression != null) {
                Formatter.WriteSpace();
                Visit (node.Expression);
            }

            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        public override void VisitSwitchStatement (SwitchStatement node)
        {
            Formatter.WriteKeyword("switch");

            Formatter.WriteSpace();

            Formatter.WriteParenthesisOpen("(");
            Visit (node.Expression);
            Formatter.WriteParenthesisClose(")");
            Formatter.WriteLine();

            WriteBlock (() => Visit (node.Cases));
        }

        public override void VisitConditionCase (ConditionCase node)
        {
            Formatter.WriteKeyword("case");
            Formatter.WriteSpace();
            Visit (node.Condition);
            Formatter.WriteGenericToken(":");
            Formatter.WriteLine();

            Visit (node.Body);
        }

        public override void VisitDefaultCase (DefaultCase node)
        {
            Formatter.WriteKeyword("default");
            Formatter.WriteGenericToken(":");
            Formatter.WriteLine();

            Visit (node.Body);
        }

        public override void VisitWhileStatement (WhileStatement node)
        {
            Formatter.WriteKeyword("while");
            Formatter.WriteSpace();
            WriteBetweenParenthesis (node.Condition);
            Formatter.WriteLine();
            Visit (node.Body);
        }

        public override void VisitDoWhileStatement (DoWhileStatement node)
        {
            Formatter.WriteKeyword("do");
            Formatter.WriteLine();
            Visit (node.Body);
            Formatter.WriteKeyword("while");
            Formatter.WriteSpace();
            WriteBetweenParenthesis (node.Condition);
            Formatter.WriteGenericToken(";");
            Formatter.WriteLine();
        }

        void VisitExpressionStatementExpression (Statement statement)
        {
            var expression_statement = statement as ExpressionStatement;
            if (expression_statement == null)
                throw new ArgumentException ();

            Visit (expression_statement.Expression);
        }

        public override void VisitForStatement (ForStatement node)
        {
            Formatter.WriteKeyword("for");
            Formatter.WriteSpace();
            Formatter.WriteParenthesisOpen("(");
            VisitExpressionStatementExpression (node.Initializer);
            Formatter.WriteGenericToken(";");
            Formatter.WriteSpace();
            Visit (node.Condition);
            Formatter.WriteGenericToken(";");
            Formatter.WriteSpace();
            VisitExpressionStatementExpression (node.Increment);
            Formatter.WriteParenthesisClose(")");
            Formatter.WriteLine();
            Visit (node.Body);
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            Formatter.WriteKeyword("foreach");
            Formatter.WriteSpace();
            Formatter.WriteParenthesisOpen("(");
            Visit(node.Variable);
            Formatter.WriteSpace();
            Formatter.WriteKeyword("in");
            Formatter.WriteSpace();
            Visit(node.Expression);
            Formatter.WriteParenthesisClose(")");
            Formatter.WriteLine();
            Visit(node.Body);
        }

        public override void VisitCatchClause (CatchClause node)
        {
            Formatter.WriteKeyword("catch");

            if (node.Type.FullName != "System.Object") {
                Formatter.WriteSpace();
                Formatter.WriteParenthesisOpen("(");
                if (node.Variable != null)
                    Visit (node.Variable);
                else
                    WriteReference (node.Type);
                Formatter.WriteParenthesisClose(")");
            }

            Formatter.WriteLine();
            Visit (node.Body);
        }

        public override void VisitTryStatement (TryStatement node)
        {
            Formatter.WriteKeyword("try");
            Formatter.WriteLine();
            Visit (node.Try);
            Visit (node.CatchClauses);

            if (node.Finally != null) {
                Formatter.WriteKeyword("finally");
                Formatter.WriteLine();
                Visit (node.Finally);
            }
        }

        public override void VisitArrayCreationExpression (ArrayCreationExpression node)
        {
            Formatter.WriteOperatorWord("new");
            Formatter.WriteSpace();
            WriteReference (node.Type);
            Formatter.WriteParenthesisOpen("[");
            Visit (node.Dimensions);
            Formatter.WriteParenthesisClose("]");
        }

        public override void VisitArrayIndexerExpression (ArrayIndexerExpression node)
        {
            Visit(node.Target);
            Formatter.WriteParenthesisOpen("[");
            Visit(node.Indices);
            Formatter.WriteParenthesisClose("]");
        }

        public override void VisitCastExpression (CastExpression node)
        {
            Formatter.WriteOperator("(");
            WriteReference (node.TargetType);
            Formatter.WriteOperator(")");
            Visit (node.Expression);
        }

        public override void VisitSafeCastExpression (SafeCastExpression node)
        {
            Visit (node.Expression);
            Formatter.WriteSpace ();
            Formatter.WriteOperatorWord("as");
            Formatter.WriteSpace();
            WriteReference (node.TargetType);
        }

        public override void VisitCanCastExpression (CanCastExpression node)
        {
            Visit (node.Expression);
            Formatter.WriteSpace();
            Formatter.WriteOperatorWord("is");
            Formatter.WriteSpace();
            WriteReference (node.TargetType);
        }

        public override void VisitAddressOfExpression (AddressOfExpression node)
        {
            Formatter.WriteOperator("&");
            Visit (node.Expression);
        }

        public override void VisitObjectCreationExpression (ObjectCreationExpression node)
        {
            Formatter.WriteKeyword("new");
            Formatter.WriteSpace();
            WriteReference (node.Constructor != null ? node.Constructor.DeclaringType : node.Type);
            Formatter.WriteParenthesisOpen("(");
            Visit (node.Arguments);
            Formatter.WriteParenthesisClose(")");
        }

        public override void VisitPropertyReferenceExpression (PropertyReferenceExpression node)
        {
            if (node.Target != null)
                Visit (node.Target);
            else
                WriteReference (node.Property.DeclaringType);

            Formatter.WriteGenericToken(".");
            Formatter.WriteNameReference(node.Property.Name, node.Property);
        }

        public override void VisitTypeReferenceExpression (TypeReferenceExpression node)
        {
            WriteReference (node.Type);
        }

        public override void VisitTypeOfExpression (TypeOfExpression node)
        {
            Formatter.WriteOperatorWord("typeof");
            Formatter.WriteGenericToken("(");
            WriteReference (node.Type);
            Formatter.WriteGenericToken(")");
        }
    }
}
