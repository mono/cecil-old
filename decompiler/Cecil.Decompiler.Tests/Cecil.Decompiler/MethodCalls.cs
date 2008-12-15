using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class MethodCalls : DecompilationTestFixture {

		public void Foo ()
		{
			Bar ();
		}

		public void Bar ()
		{
		}

		[Test]
		public void SimpleCall ()
		{
			AssertMethod ("Foo", CSharpV0, @"
{
	this.Bar();
	return;
}
");
		}

		public void Foo (string a, string b, string c)
		{
		}

		public void FooFoo ()
		{
			Foo ("a", "b", "c");
		}

		[Test]
		public void CallMultipleArguments ()
		{
			AssertMethod ("FooFoo", CSharpV0, @"
{
	this.Foo(""a"", ""b"", ""c"");
	return;
}
");
		}

		public int Bam ()
		{
			return 42;
		}

		public void BamBam ()
		{
			Bam ();
			Foo ();
		}

		[Test]
		public void CallIgnoreReturnValue ()
		{
			AssertMethod ("BamBam", CSharpV0, @"
{
	this.Bam();
	this.Foo();
	return;
}
");
		}

        public int Piou (int [] integers)
        {
            return integers.Length;
        }

        [Test]
        public void ArrayLength ()
        {
            AssertMethod("Piou", CSharpV0, @"
{
	return (int)integers.Length;
}");
        }
	}
}
