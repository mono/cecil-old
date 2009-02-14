using System;

class Program {

	public void SimpleFor (int a)
	{
		for (int i = 0; i < 10; i++) {
			a++;
		}
	}

	public void SimpleDoWhile (int a)
	{
		Console.WriteLine (a);
		do {
			a++;
			Console.WriteLine (a);
		} while (a < 100);
		Console.WriteLine (a);
	}

	public void SimpleWhile (int a)
	{
		Console.WriteLine (a);
		while (a < 100) {
			a++;
			Console.WriteLine (a);
		}
		Console.WriteLine (a);
	}

	public void NestedDoWhile (int a)
	{
		while (a < 100) {
			do {
				a *= 10;
			} while (a < 10);
			a += 2;
		}
	}

	public void BreakInWhile (int a)
	{
		while (a < 100) {
			if (a == 12) {
				Console.WriteLine (a);
				break;
			} else
				Console.WriteLine (a);

			a--;
		}
		Console.WriteLine (a);
	}
}
