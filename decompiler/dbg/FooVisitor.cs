using System;

using Cecil.Decompiler;
using Cecil.Decompiler.Ast;

interface IFooExpression {
	FooExpressionType FooExpressionType { get; }
}

enum FooExpressionType {
	Foo,
	Bar
}

class FooExpression : Expression, IFooExpression {

	public override CodeNodeType CodeNodeType {
		get { return (CodeNodeType) - 1; } // should actually be CodeNodeType.None or Unknown
	}

	public FooExpressionType FooExpressionType {
		get { return FooExpressionType.Foo; }
	}
}

class BarExpression : Expression, IFooExpression {

	public override CodeNodeType CodeNodeType {
		get { return (CodeNodeType) - 1; }
	}

	public FooExpressionType FooExpressionType {
		get { return FooExpressionType.Bar; }
	}
}

class FooVisitor : BaseCodeVisitor {

	public override void Visit (ICodeNode node)
	{
		var foo_node = node as IFooExpression;
		if (foo_node == null) {
			base.Visit (node);
			return;
		}

		switch (foo_node.FooExpressionType) {
		case FooExpressionType.Foo:
			VisitFoo ((FooExpression) node);
			break;
		case FooExpressionType.Bar:
			VisitBar ((BarExpression) node);
			break;
		}
	}

	public virtual void VisitFoo (FooExpression node)
	{
	}

	public virtual void VisitBar (BarExpression node)
	{
	}
}

class FooPrinter : FooVisitor {

	public override void VisitAssignExpression (AssignExpression node)
	{
		Console.Write ("assign ");

		Visit (node.Expression);

		Console.Write (" to ");

		Visit (node.Target);

		Console.WriteLine ();
	}

	public override void VisitFoo (FooExpression node)
	{
		Console.Write ("Foo");
	}

	public override void VisitBar (BarExpression node)
	{
		Console.Write ("Bar");
	}
}

class Test {

	public static void Main ()
	{
		var assign = new AssignExpression (new BarExpression (), new FooExpression ());

		var printer = new FooPrinter ();

		printer.Visit (assign);
	}
}
