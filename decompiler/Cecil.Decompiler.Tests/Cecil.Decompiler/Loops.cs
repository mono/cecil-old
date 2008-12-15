using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class Loops : DecompilationTestFixture {

		public void Foo (int a)
		{
			for (int i = 0; i < 10; i++) {
				a++;
			}
		}

		[Test]
		public void SimpleFor ()
		{
			AssertMethod ("Foo", CSharpV1, @"
{
	for (int V_0 = 0; V_0 < 10; V_0 = V_0 + 1)
	{
		a = a + 1;
	}
}
");
		}

		public void FooFoo (int a)
		{
			Console.WriteLine (a);
			do {
				a++;
				Console.WriteLine (a);
			} while (a < 100);
			Console.WriteLine (a);
		}

		[Test]
		public void SimpleDoWhile ()
		{
			AssertMethod ("FooFoo", CSharpV1, @"
{
	Console.WriteLine(a);
	do
	{
		a = a + 1;
		Console.WriteLine(a);
	}
	while (a < 100);
	Console.WriteLine(a);
}
");
		}

		public void FooFooFoo (int a)
		{
			Console.WriteLine (a);
			while (a < 100) {
				a++;
				Console.WriteLine (a);
			}
			Console.WriteLine (a);
		}

		[Test]
		public void SimpleWhile ()
		{
			AssertMethod ("FooFooFoo", CSharpV1, @"
{
	Console.WriteLine(a);
	while (a < 100)
	{
		a = a + 1;
		Console.WriteLine(a);
	}
	Console.WriteLine(a);
}
");
		}

		public void FooBar (int a)
		{
			while (a < 100) {
				do {
					a *= 10;
				} while (a < 10);
				a += 2;
			}
		}

		[Test]
		public void NestedDoWhile ()
		{
			AssertMethod ("FooBar", CSharpV1, @"
{
	while (a < 100)
	{
		do
		{
			a = a * 10;
		}
		while (a < 10);
		a = a + 2;
	}
}
");
		}

		public void BalleDeBreak (int a)
		{
			while (a < 100) {
				if (a == 12) {
					Console.WriteLine (a);
					break;
				} else
					Console.WriteLine (a);

				a--;
			}
		}

		[Test]
		public void BreakInWhile ()
		{
			AssertMethod ("BalleDeBreak", CSharpV1, @"
{
	while (a < 100)
	{
		if (a == 12)
		{
			Console.WriteLine(a);
			break;
		}
		Console.WriteLine(a);
		a = a - 1;
	}
}
");
		}
	}
}
