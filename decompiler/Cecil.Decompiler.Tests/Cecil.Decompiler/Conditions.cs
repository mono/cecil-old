using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class Conditions : DecompilationTestFixture {

		public int Foo (int a)
		{
			return a > 10 ? 12 : 42; 
		}

		[Test]
		public void ReturnTernary ()
		{
			AssertMethod ("Foo", CSharpV0, @"
{
	return (a > 10 ? 12 : 42);
}
");
		}

		public int Bar (int a)
		{
			return (a < 0 ? (a < 100 ? 12 : 24) : (a > 0 ? 10 : 42));
		}

		[Test]
		public void ReturnNestedTernary ()
		{
			AssertMethod ("Bar", CSharpV0, @"
{
	return (a < 0 ? (a < 100 ? 12 : 24) : (a > 0 ? 10 : 42));
}
");
		}

		public void Baz (int a)
		{
			if (a == 12) {
				a = 14;
			}
		}

		[Test]
		public void SimpleIf ()
		{
			AssertMethod ("Baz", CSharpV1, @"
{
	if (a == 12)
	{
		a = 14;
	}
}
");
		}

		public void Bazz (int a)
		{
			if (a == 12) {
				a = 42;
			} else {
				a = 128;
			}
		}

		[Test]
		public void SimpleIfElse ()
		{
			AssertMethod ("Bazz", CSharpV1, @"
{
	if (a == 12)
	{
		a = 42;
	}
	else
	{
		a = 128;
	}
}
");
		}

		public void Bazzou (int a)
		{
			if (a == 12 || a == 14) {
				a = 42;
			}
		}

		[Test]
		public void SimpleIfOr ()
		{
			AssertMethod ("Bazzou", CSharpV1, @"
{
	if ((a == 12) || (a == 14))
	{
		a = 42;
	}
}
");
		}

		public void Bazzouzzou (int a)
		{
			if (a < 12 && a > 2) {
				a = 42;
			}
		}

		[Test]
		public void SimpleIfAnd ()
		{
			AssertMethod ("Bazzouzzou", CSharpV1, @"
{
	if ((a < 12) && (a > 2))
	{
		a = 42;
	}
}
");
		}

		public string Pazuzu (string s)
		{
			return s ?? "nil";
		}

		[Test]
		public void NullCoalesce ()
		{
			AssertMethod ("Pazuzu", CSharpV0, @"
{
	return s ?? ""nil"";
}
");
		}
	}
}
