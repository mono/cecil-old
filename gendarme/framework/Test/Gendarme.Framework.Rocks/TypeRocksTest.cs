// 
// Unit tests for TypeRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using NUnit.Framework;

namespace Test.Framework.Rocks {

	[TestFixture]
	public class TypeRocksTest {

		public enum Enum {
			Value
		}

		[Flags]
		public enum Flags {
			Mask
		}

		interface IDeepCloneable : ICloneable {
		}

		class Deep : IDeepCloneable {
			public object Clone ()
			{
				throw new NotImplementedException ();
			}
		}

		private AssemblyDefinition assembly;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
		}

		private TypeDefinition GetType (string name)
		{
			return assembly.MainModule.Types ["Test.Framework.Rocks.TypeRocksTest" + name];
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void HasAttribute_Null ()
		{
			GetType (String.Empty).HasAttribute (null);
		}

		[Test]
		public void HasAttribute ()
		{
			Assert.IsTrue (GetType (String.Empty).HasAttribute ("NUnit.Framework.TestFixtureAttribute"), "TypeRocksTest");
			Assert.IsFalse (GetType ("/Enum").HasAttribute ("System.FlagsAttribute"), "Enum/System.FlagsAttribute");
			Assert.IsTrue (GetType ("/Flags").HasAttribute ("System.FlagsAttribute"), "Flags/System.FlagsAttribute");
			// fullname is required
			Assert.IsFalse (GetType ("/Flags").HasAttribute ("System.Flags"), "Flags/System.Flags");
			Assert.IsFalse (GetType ("/Flags").HasAttribute ("FlagsAttribute"), "Flags/FlagsAttribute");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Implements_Null ()
		{
			GetType (String.Empty).Implements (null);
		}

		[Test]
		public void Implements ()
		{
			Assert.IsFalse (GetType (String.Empty).Implements ("System.ICloneable"), "ICloneable");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements ("Test.Framework.Rocks.TypeRocksTest/IDeepCloneable"), "itself");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements ("System.ICloneable"), "interface inheritance");
			Assert.IsTrue (GetType ("/Deep").Implements ("Test.Framework.Rocks.TypeRocksTest/IDeepCloneable"), "IDeepCloneable");
			Assert.IsTrue (GetType ("/Deep").Implements ("System.ICloneable"), "second-level ICloneable");
		}

		[Test]
		public void IsFlags ()
		{
			Assert.IsFalse (GetType (String.Empty).IsFlags (), "Type.IsFlags");
			Assert.IsFalse (GetType ("/Enum").IsFlags (), "Enum.IsFlags");
			Assert.IsTrue (GetType ("/Flags").IsFlags (), "Flags.IsFlags");
		}
	}
}
