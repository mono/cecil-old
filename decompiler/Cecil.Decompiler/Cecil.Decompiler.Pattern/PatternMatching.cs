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
		void Match (MatchContext context, object @object);
	}

	public abstract class CodePattern : ICodePattern {

		public static MatchContext Match (ICodePattern pattern, object @object)
		{
			var context = new MatchContext ();
			pattern.Match (context, @object);
			return context;
		}

		public abstract void Match (MatchContext context, object @object);
	}

	public abstract class CodePattern<TNode> : CodePattern where TNode : class, Ast.ICodeNode {

		public Func<TNode, MatchData> Bind { get; set; }

		public override void Match (MatchContext context, object node)
		{
			var current = node as TNode;
			if (current == null) {
				context.Success = false;
				return;
			}

			if (Bind != null)
				context.AddData (Bind (current));

			OnMatch (context, current);
		}

		protected abstract void OnMatch (MatchContext context, TNode node);
	}

	public class Statement : CodePattern<Ast.Statement> {

		public ICodePattern Pattern { get; set; }

		protected override void OnMatch (MatchContext context, Ast.Statement node)
		{
			if (Pattern != null)
				Pattern.Match (context, node);
		}
	}

	public class ExpressionStatement : CodePattern<Ast.ExpressionStatement> {

		public ICodePattern Expression { get; set; }

		protected override void OnMatch (MatchContext context, Ast.ExpressionStatement node)
		{
			if (Expression == null)
				return;

			Expression.Match (context, node.Expression);
		}
	}

	public class Assignment : CodePattern<Ast.AssignExpression> {

		public ICodePattern Target { get; set; }
		public ICodePattern Expression { get; set; }

		protected override void OnMatch (MatchContext context, Ast.AssignExpression node)
		{
			if (Target != null)
				Target.Match (context, node.Target);

			if (!context.Success)
				return;

			if (Expression != null)
				Expression.Match (context, node.Expression);
		}
	}

	public class VariableReference : CodePattern<Ast.VariableReferenceExpression> {

		public ICodePattern Variable { get; set; }

		protected override void OnMatch (MatchContext context, Ast.VariableReferenceExpression node)
		{
			if (Variable != null)
				Variable.Match (context, node.Variable);
		}
	}

	public class Binary : CodePattern<Ast.BinaryExpression> {

		public ICodePattern Left { get; set; }
		public ICodePattern Operator { get; set; }
		public ICodePattern Right { get; set; }

		protected override void OnMatch (MatchContext context, BinaryExpression node)
		{
			if (Left != null)
				Left.Match (context, node.Left);

			if (!context.Success)
				return;

			if (Operator != null)
				Operator.Match (context, node.Operator);

			if (!context.Success)
				return;

			if (Right != null)
				Right.Match (context, node.Right);
		}
	}

	public class Literal : CodePattern<Ast.LiteralExpression> {

		object value;
		bool check_value;

		public object Value {
			get { return value; }
			set { this.value = value; check_value = true; }
		}

		protected override void OnMatch (MatchContext context, Ast.LiteralExpression node)
		{
			if (!check_value)
				return;

			context.Success = Value == null ? node.Value == null : Value.Equals (node.Value);
		}
	}

	public class ContextData : CodePattern {

		public string Name { get; set; }

		public override void Match (MatchContext context, object @object)
		{
			object data;

			if (!context.TryGetData (Name, out data)) {
				context.Success = false;
				return;
			}

			context.Success = data == null ? @object == null : data.Equals (@object);
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
