using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class Casts : DecompilationTestFixture {

		public short Foo (int a)
		{
			return (short) a;
		}

		[Test]
		public void ToShort ()
		{
			AssertMethod ("Foo", CSharpV1, @"
{
	return (short)a;
}
");
		}

		public class Bar {
		}

		public Bar ToBar (object o)
		{
			return (Bar) o;
		}

		[Test]
		public void CastClass ()
		{
			AssertMethod ("ToBar", CSharpV1, @"
{
	return (Bar)o;
}
");
		}

		public Bar AsBar (object o)
		{
			return o as Bar;
		}

		[Test]
		public void SafeCast ()
		{
			AssertMethod ("AsBar", CSharpV1, @"
{
	return o as Bar;
}
");
		}

		public bool IsBar (object o)
		{
			return o is Bar;
		}

		[Test]
		public void CanCast ()
		{
			AssertMethod ("IsBar", CSharpV1, @"
{
	return o is Bar;
}
");
		}
	}
}
