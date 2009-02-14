using System;

class Program {

	public void TryCatchFinally ()
	{
		Console.WriteLine ("1");
		try {
			Console.WriteLine ("2");
		} catch (ArgumentException e) {
			Console.WriteLine ("3 {0}", e);
		} catch {
			Console.WriteLine ("4");
		} finally {
			Console.WriteLine ("5");
		}
		Console.WriteLine ("6");
	}

	public void NestedTryCatch ()
	{
		try {
			Console.WriteLine (1);
			try {
				Console.WriteLine (2);
			} catch {
				Console.WriteLine (3);
			}
			Console.WriteLine (4);
		} catch {
			try {
				Console.WriteLine (5);
			} catch {
				Console.WriteLine (6);
			}
		}
		Console.WriteLine (7);
	}
}
