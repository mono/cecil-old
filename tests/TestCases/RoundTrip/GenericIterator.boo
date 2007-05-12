"""
1
2
3
"""

CODE = """
using System;
using System.Collections.Generic;

public class Program
{
	public static void Main(string[] args)
	{
		foreach (int i in GetNumbers())
		{
			Console.WriteLine(i);
		}
	}

	static IEnumerable<int> GetNumbers()
	{
		for (int i=0; i<3; ++i)
		{
			yield return i+1;
		}
	}
}
"""
