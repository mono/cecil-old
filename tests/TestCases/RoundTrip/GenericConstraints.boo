"""
ok
"""

CODE = """
using System;

public class Program {
	public static void Main ()
	{
		Console.WriteLine ("ok");
	}
}

public class TestClass<T1, T2, T3>
	where T1 : T2
	where T3 : T2 {

}
"""
