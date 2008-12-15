using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Cecil.Decompiler.Tests.Decompilation {

	[TestFixture]
	public class ForeachStep : DecompilationTestFixture {

		public void SimpleForeach_target (IEnumerable<string> strings)
		{
			foreach (string V_0 in strings)
				Console.WriteLine (V_0);
		}

		[Test]
		public void SimpleForeach ()
		{
			AssertMethod ("SimpleForeach_target", this.CSharpV1, @"
{
	foreach (string V_0 in strings)
	{
		Console.WriteLine(V_0);
	}
}
");
		}

		public void BreakInForeach_target (IEnumerable<string> strings)
		{
			foreach (string V_0 in strings) {
				Console.WriteLine (V_0);
				break;
			}
		}

		[Test]
		public void BreakInForeach ()
		{
			AssertMethod ("BreakInForeach_target", this.CSharpV1, @"
{
	foreach (string V_0 in strings)
	{
		Console.WriteLine(V_0);
		break;
	}
}
");
		}

		public void ContinueInForeach_target (IEnumerable<string> strings)
		{
			foreach (string V_0 in strings) {
				if (V_0 == null)
					continue;
				Console.WriteLine (V_0);
			}
		}

		[Test]
		public void ContinueInForeach ()
		{
			AssertMethod ("ContinueInForeach_target", this.CSharpV1, @"
{
	foreach (string V_0 in strings)
	{
		if (V_0 != null)
		{
			Console.WriteLine(V_0);
		}
	}
}
");
		}

		public void ForeachOnString_target (string s)
		{
			foreach (var c in s)
				Console.WriteLine (c);
		}

		[Test]
		public void ForeachOnString ()
		{
			AssertMethod ("ForeachOnString_target", this.CSharpV1, @"
{
	foreach (char V_0 in s)	
	{
		Console.WriteLine(V_0);
	}
}
");
		}

		public void SimpleForeachConstruction_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachConstruction ()
		{
			AssertMethod ("SimpleForeachConstruction_target", this.CSharpV1, @"
{
	foreach (string V_1 in strings)
	{
		Console.WriteLine(V_1);
	}
}
");
		}

		public void NestedForeach_target (IEnumerable<string> strings)
		{
			foreach (var V_0 in strings) {
				foreach (var V_1 in strings)
					Console.WriteLine (V_1);
				Console.WriteLine (V_0);
			}
		}

		[Test]
		public void NestedForeach ()
		{
			AssertMethod ("NestedForeach_target", this.CSharpV1, @"
{
	foreach (var V_0 in strings)
	{
		foreach (var V_1 in strings)
		{
			Console.WriteLine(V_1);
		}
		Console.WriteLine(V_0);
	}
}
");
		}

		public void SimpleForeachTrap0_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				Console.WriteLine (V_0);
			}
		}

		[Test]
		public void SimpleForeachTrap0 ()
		{
			AssertMethod ("SimpleForeachTrap0_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		Console.WriteLine(V_0);
	}
}
");
		}

		public void SimpleForeachTrap1_target(IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				Console.WriteLine (1);
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap1 ()
		{
			AssertMethod ("SimpleForeachTrap1_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		Console.WriteLine(1);
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap2_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					Console.WriteLine (1);
					string V_1 = V_0.Current;
				}
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap2 ()
		{
			AssertMethod ("SimpleForeachTrap2_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			Console.WriteLine(1);
			string V_1 = V_0.get_Current();
		}
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap3_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
				Console.WriteLine (1);
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap3 ()
		{
			AssertMethod ("SimpleForeachTrap3_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
		Console.WriteLine(1);
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap4_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				Console.WriteLine (1);
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap4 ()
		{
			AssertMethod ("SimpleForeachTrap4_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		Console.WriteLine(1);
		if (V_0 != null)
		{
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap5_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
				Console.WriteLine (1);
			}
		}

		[Test]
		public void SimpleForeachTrap5 ()
		{
			AssertMethod ("SimpleForeachTrap5_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
		Console.WriteLine(1);
	}
}
");
		}

		public void SimpleForeachTrap6_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
				else
					Console.WriteLine (1);
			}
		}

		[Test]
		public void SimpleForeachTrap6 ()
		{
			AssertMethod ("SimpleForeachTrap6_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
		else
		{
			Console.WriteLine(1);
		}
	}
}
");
		}

		public void SimpleForeachTrap7_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null) {
					Console.WriteLine (1);
					V_0.Dispose ();
				}
			}
		}

		[Test]
		public void SimpleForeachTrap7 ()
		{
			AssertMethod ("SimpleForeachTrap7_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0 != null)
		{
			Console.WriteLine(1);
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap8_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null) {
					V_0.Dispose ();
					Console.WriteLine (1);
				}
			}
		}

		[Test]
		public void SimpleForeachTrap8 ()
		{
			AssertMethod ("SimpleForeachTrap8_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
			Console.WriteLine(1);
		}
	}
}
");
		}

		public void SimpleForeachTrap9_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			catch (Exception ex) {
				Console.WriteLine (ex);
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap9 ()
		{
			AssertMethod ("SimpleForeachTrap9_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	catch (Exception V_2)
	{
		Console.WriteLine(V_2);
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap10_target (IEnumerable<string> strings)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0.GetHashCode () == 12)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap10 ()
		{
			AssertMethod ("SimpleForeachTrap10_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0.GetHashCode() == 12)
		{
			V_0.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap11_target (IEnumerable<string> strings, IDisposable lol)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (V_0.MoveNext ()) {
					string V_1 = V_0.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (lol != null)
					lol.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap11 ()
		{
			AssertMethod ("SimpleForeachTrap11_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (V_0.MoveNext())
		{
			string V_1 = V_0.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (lol != null)
		{
			lol.Dispose();
		}
	}
}
");
		}

		public void SimpleForeachTrap12_target (IEnumerable<string> strings, IEnumerator<string> lol)
		{
			IEnumerator<string> V_0 = strings.GetEnumerator ();
			try {
				while (lol.MoveNext ()) {
					string V_1 = lol.Current;
					Console.WriteLine (V_1);
				}
			}
			finally {
				if (V_0 != null)
					V_0.Dispose ();
			}
		}

		[Test]
		public void SimpleForeachTrap12 ()
		{
			AssertMethod ("SimpleForeachTrap12_target", this.CSharpV1, @"
{
	IEnumerator<string> V_0 = strings.GetEnumerator();
	try
	{
		while (lol.MoveNext())
		{
			string V_1 = lol.get_Current();
			Console.WriteLine(V_1);
		}
	}
	finally
	{
		if (V_0 != null)
		{
			V_0.Dispose();
		}
	}
}
");
		}
	}
}