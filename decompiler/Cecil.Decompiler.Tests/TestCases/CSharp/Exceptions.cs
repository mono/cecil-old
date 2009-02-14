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
}
