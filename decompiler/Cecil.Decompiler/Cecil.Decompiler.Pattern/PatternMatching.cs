using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Cecil.Decompiler;
using Ast = Cecil.Decompiler.Ast;
using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Pattern {

	public struct MatchData {

		public readonly string Name;
		public readonly object Value;

		public MatchData (string name, object value)
		{
			Name = name;
			Value = value;
		}
	}

	public interface ICodePattern {
		bool Match (MatchContext context, object @object);
	}

	public static class Extensions {

		public static bool TryMatch (this ICodePattern pattern, MatchContext context, object @object)
		{
			if (pattern == null)
				return true;

			return pattern.Match (context, @object);
		}
	}

	public abstract class CodePattern : ICodePattern {

		public static MatchContext Match (ICodePattern pattern, object @object)
		{
			var context = new MatchContext ();
			context.Success = pattern.Match (context, @object);
			return context;
		}

		public abstract bool Match (MatchContext context, object @object);
	}

	public abstract class CodePattern<TNode> : CodePattern where TNode : class, Ast.ICodeNode {

		public Func<TNode, MatchData> Bind { get; set; }

		public override bool Match (MatchContext context, object node)
		{
			var current = node as TNode;
			if (current == null)
				return false;

			if (Bind != null)
				context.AddData (Bind (current));

			return OnMatch (context, current);
		}

		protected abstract bool OnMatch (MatchContext context, TNode node);
	}

	public class ExpressionStatement : CodePattern<Ast.ExpressionStatement> {

		public ICodePattern Expression { get; set; }

		protected override bool OnMatch (MatchContext context, Ast.ExpressionStatement node)
		{
			return Expression.TryMatch (context, node.Expression);
		}
	}

	public class Assignment : CodePattern<Ast.AssignExpression> {

		public ICodePattern Target { get; set; }
		public ICodePattern Expression { get; set; }

		protected override bool OnMatch (MatchContext context, Ast.AssignExpression node)
		{
			if (!Target.TryMatch (context, node.Target))
				return false;

			return Expression.TryMatch (context, node.Expression);
		}
	}

	public class VariableReference : CodePattern<Ast.VariableReferenceExpression> {

		public ICodePattern Variable { get; set; }

		protected override bool OnMatch (MatchContext context, Ast.VariableReferenceExpression node)
		{
			return Variable.TryMatch (context, node.Variable);
		}
	}

	public class ContextVariableReference : CodePattern<Ast.VariableReferenceExpression> {

		public string Name { get; set; }

		protected override bool OnMatch (MatchContext context, VariableReferenceExpression node)
		{
			object data;
			if (!context.TryGetData (Name, out data))
				return false;

			return node.Variable == data;
		}
	}

	public class Binary : CodePattern<Ast.BinaryExpression> {

		public ICodePattern Left { get; set; }
		public ICodePattern Operator { get; set; }
		public ICodePattern Right { get; set; }

		protected override bool OnMatch (MatchContext context, BinaryExpression node)
		{
			if (!Left.TryMatch (context, node.Left))
				return false;

			if (!Operator.TryMatch (context, node.Operator))
				return false;

			return Right.TryMatch (context, node.Right);
		}
	}

	public class Literal : CodePattern<Ast.LiteralExpression> {

		object value;
		bool check_value;

		public object Value {
			get { return value; }
			set { this.value = value; check_value = true; }
		}

		protected override bool OnMatch (MatchContext context, Ast.LiteralExpression node)
		{
			if (!check_value)
				return true;

			return value == null ? node.Value == null : value.Equals (node.Value);
		}
	}

	public class ContextData : CodePattern {

		public string Name { get; set; }

		public override bool Match (MatchContext context, object @object)
		{
			object data;
			if (!context.TryGetData (Name, out data))
				return false;

			return data == null ? @object == null : data.Equals (@object);
		}
	}

	public class MatchContext {

		bool success = true;
		Dictionary<string, object> datas;

		public bool Success {
			get { return success; }
			set { success = value; }
		}

		public object this [string name] {
			get { return Store [name]; }
		}

		Dictionary<string, object> Store {
			get {
				if (datas == null)
					datas = new Dictionary<string, object> ();

				return datas;
			}
		}

		public bool TryGetData (string name, out object value)
		{
			return Store.TryGetValue (name, out value);
		}

		public void AddData (MatchData data)
		{
			Store [data.Name] = data.Value;
		}
	}
}
