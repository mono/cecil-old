using System;

class Program {

	public void Foo ()
	{
	}

	public void VoidMethodCall ()
	{
		Bar ();
	}

	public void Bar ()
	{
	}

	public void Foo (string a, string b, string c)
	{
	}

	public void CallMultipleArguments ()
	{
		Foo ("a", "b", "c");
	}

	public int Bam ()
	{
		return 42;
	}

	public void CallIgnoreReturnValue ()
	{
		Bam ();
		Foo ();
	}

	public int ArrayLength (int [] integers)
	{
		return integers.Length;
	}
}
