using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class Exceptions : DecompilationTestFixture {

		public void Alarm (int a)
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

		[Test]
		public void TryCatchFinally ()
		{
			AssertMethod ("Alarm", CSharpV1, @"
{
	Console.WriteLine(""1"");
	try
	{
		Console.WriteLine(""2"");
	}
	catch (ArgumentException V_0)
	{
		Console.WriteLine(""3 {0}"", V_0);
	}
	catch
	{
		Console.WriteLine(""4"");
	}
	finally
	{
		Console.WriteLine(""5"");
	}
	Console.WriteLine(""6"");
}
");
		}
	}
}
