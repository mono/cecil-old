using System;

class Conditions {

	public int ReturnTernary (int a)
	{
		return a > 10 ? 12 : 42;
	}

	public int ReturnNestedTernary (int a)
	{
		return (a < 0 ? (a < 100 ? 12 : 24) : (a > 0 ? 10 : 42));
	}

	public void SimpleIf (int a)
	{
		if (a == 12) {
			a = 14;
		}
	}

	public void SimpleIfElse (int a)
	{
		if (a == 12) {
			a = 42;
		} else {
			a = 128;
		}
		Console.WriteLine (1);
	}

	public void SimpleIfOr (int a)
	{
		if (a == 12 || a == 14) {
			a = 42;
		}
	}

	public void SimpleIfAnd (int a)
	{
		if (a < 12 && a > 2) {
			a = 42;
		}
	}

	public string NullCoalesce (string s)
	{
		return s ?? "nil";
	}
}
