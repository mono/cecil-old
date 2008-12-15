using System;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation 
{
	[TestFixture]
	public class OperatorStep : DecompilationTestFixture
	{

		class Jambon {

			public static Jambon operator +(Jambon a, Jambon b) { return a; }
			public static Jambon operator -(Jambon a, Jambon b) { return a; }
			public static Jambon operator /(Jambon a, Jambon b) { return a; }
			public static Jambon operator *(Jambon a, Jambon b) { return a; }
			public static Jambon operator %(Jambon a, Jambon b) { return a; }

			public static bool operator <(Jambon a, Jambon b) { return true; }
			public static bool operator <=(Jambon a, Jambon b) { return true; }
			public static bool operator >(Jambon a, Jambon b) { return true; }
			public static bool operator >=(Jambon a, Jambon b) { return true; }

			public static bool operator ==(Jambon a, Jambon b) { return true; }
			public static bool operator !=(Jambon a, Jambon b) { return true; }
 
			public static Jambon operator &(Jambon a, Jambon b) { return a; }
			public static Jambon operator |(Jambon a, Jambon b) { return a; }
			public static Jambon operator ^(Jambon a, Jambon b) { return a; }
			public static bool operator >>(Jambon a, int b) { return true; }
			public static bool operator <<(Jambon a, int b) { return true; }

			public static Jambon operator -(Jambon a) { return a; }
			public static Jambon operator !(Jambon a) { return a; }
			public static Jambon operator ~(Jambon a) { return a; }
			public static Jambon operator ++(Jambon a) { return a; }
			public static Jambon operator --(Jambon a) { return a; }

			public static bool operator true(Jambon a) { return true; }
			public static bool operator false(Jambon a) { return false; }
		}

		void SimpleUnaryOperatorCalls_target(Jambon j)
		{
			j = -j;
			j = --j;
			j = j--;
			j = j++;
			j = ++j;
			j = ~j;
			j = !j;
		}

		[Test]
		public void SimpleUnaryOperatorCalls()
		{
			AssertMethod("SimpleUnaryOperatorCalls_target", this.CSharpV1, @"
{
	j = -j;
	j = --j;
	j = j--;
	j = j++;
	j = ++j;
	j = ~(j);
	j = !(j);
}
");
		}

		void SimpleBinaryOperatorCalls_target(Jambon j)
		{
			j = j + j;
			j = j - j;
			j = j/j;
			j = j*j;
			j = j%j;
			j = j & j;
			j = j | j;
			j = j ^ j;
			bool V_0 = j < j;
			V_0 = j <= j;
			V_0 = j > j;
			V_0 = j >= j;
			V_0 = j == j;
			V_0 = j != j;
			V_0 = j >> 1;
			V_0 = j << 1;
		}

		[Test]
		public void SimpleBinaryOperatorCalls()
		{
			AssertMethod ("SimpleBinaryOperatorCalls_target", this.CSharpV1, @"
{
	j = j + j;
	j = j - j;
	j = j / j;
	j = j * j;
	j = j % j;
	j = j & j;
	j = j | j;
	j = j ^ j;
	bool V_0 = j < j;
	V_0 = j <= j;
	V_0 = j > j;
	V_0 = j >= j;
	V_0 = j == j;
	V_0 = j != j;
	V_0 = j >> 1;
	V_0 = j << 1;
}
");
		}
	}
}
