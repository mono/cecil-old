using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class Switches : DecompilationTestFixture {

		public void Foo (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 0:
				Console.WriteLine ("0");
				break;
			case 1:
				Console.WriteLine ("1");
				break;
			}
			Console.WriteLine (a);
		}

		[Test]
		public void SimpleSwitch ()
		{
			AssertMethod ("Foo", CSharpV1, @"
{
	Console.WriteLine(a);
	switch (a)
	{
		case 0:
		{
			Console.WriteLine(""0"");
			break;
		}
		case 1:
		{
			Console.WriteLine(""1"");
			break;
		}
	}
	Console.WriteLine(a);
}
");
		}

		public void FooFoo (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 0:
				Console.WriteLine ("0");
				break;
			case 1:
				Console.WriteLine ("1");
				break;
			case 42:
				Console.WriteLine ("42");
				break;
			}
			Console.WriteLine (a);
		}

		[Test]
		public void SwitchAndIfCase ()
		{
			AssertMethod ("FooFoo", CSharpV1, @"
{
	Console.WriteLine(a);
	switch (a)
	{
		case 0:
		{
			Console.WriteLine(""0"");
			break;
		}
		case 1:
		{
			Console.WriteLine(""1"");
			break;
		}
		case 42:
		{
			Console.WriteLine(""42"");
			break;
		}
	}
	Console.WriteLine(a);
}
");
		}

		public void FooFooFoo (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 0:
				Console.WriteLine ("0");
				break;
			case 1:
				Console.WriteLine ("1");
				break;
			case 42:
				Console.WriteLine ("42");
				break;
			case 96:
				Console.WriteLine ("96");
				break;
			}
			Console.WriteLine (a);
		}

		[Test]
		public void SwitchAndDoubleIfCase ()
		{
			AssertMethod ("FooFooFoo", CSharpV1, @"
{
	Console.WriteLine(a);
	switch (a)
	{
		case 0:
		{
			Console.WriteLine(""0"");
			break;
		}
		case 1:
		{
			Console.WriteLine(""1"");
			break;
		}
		case 42:
		{
			Console.WriteLine(""42"");
			break;
		}
		case 96:
		{
			Console.WriteLine(""96"");
			break;
		}
	}
	Console.WriteLine(a);
}
");
		}

		public void Bar (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 0:
				Console.WriteLine ("0");
				break;
			case 1:
				Console.WriteLine ("1");
				break;
			default:
				Console.WriteLine ("?");
				break;
			}
			Console.WriteLine (a);
		}

		[Test]
		public void SimpleSwitchDefault ()
		{
			AssertMethod ("Bar", CSharpV1, @"
{
	Console.WriteLine(a);
	switch (a)
	{
		case 0:
		{
			Console.WriteLine(""0"");
			break;
		}
		case 1:
		{
			Console.WriteLine(""1"");
			break;
		}
		default:
		{
			Console.WriteLine(""?"");
			break;
		}
	}
	Console.WriteLine(a);
}
");
		}

		public void BarBar (int a)
		{
			Console.WriteLine (a);
			switch (a) {
			case 0:
				Console.WriteLine ("0");
				break;
			case 1:
				Console.WriteLine ("1");
				break;
			case 42:
				Console.WriteLine ("42");
				break;
			case 96:
				Console.WriteLine ("96");
				break;
			default:
				Console.WriteLine ("?");
				break;
			}
			Console.WriteLine (a);
		}

		[Test]
		public void SwitchIfCaseDefault ()
		{
			AssertMethod ("BarBar", CSharpV1, @"
{
	Console.WriteLine(a);
	switch (a)
	{
		case 0:
		{
			Console.WriteLine(""0"");
			break;
		}
		case 1:
		{
			Console.WriteLine(""1"");
			break;
		}
		case 42:
		{
			Console.WriteLine(""42"");
			break;
		}
		case 96:
		{
			Console.WriteLine(""96"");
			break;
		}
		default:
		{
			Console.WriteLine(""?"");
			break;
		}
	}
	Console.WriteLine(a);
}
");
		}
	}
}
