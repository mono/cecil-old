namespace Mono.Cecil.Tests

import System
import System.IO
import NUnit.Framework
import Mono.Cecil

[TestFixture]
class RoundtripTestFixture:
	
	[Test]
	def GenericEnumerator():
		code = """
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
}"""
		InstrumentAndExecute("1\n2\n3", code)

	def InstrumentAndExecute(expectedOutput as string, code as string):
		fname = BuildTempPath("source.exe")
		EmitCSharpAssembly(fname, code)
		assembly = AssemblyFactory.GetAssembly(fname)
		
		roundtripped = BuildTempPath("target.exe")
		AssemblyFactory.SaveAssembly(assembly, roundtripped)
		output = shellm(roundtripped, array(string, 0))
		Assert.AreEqual(expectedOutput.Trim(), output.Trim())

