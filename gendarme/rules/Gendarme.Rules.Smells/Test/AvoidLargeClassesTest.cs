//
// Unit Test for AvoidLargeClasses Rule.
//
// Authors:
//      Néstor Salceda <nestor.salceda@gmail.com>
//
//      (C) 2007 Néstor Salceda
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Reflection;

using Mono.Cecil;
using NUnit.Framework;
using Gendarme.Framework;
using Gendarme.Rules.Smells;

namespace Test.Rules.Smells {
	
	public class LargeClass {
		int x, x1, x2, x3;
		string foo, foo1, foo2, foo3;
		DateTime bar, bar1, bar2, bar3;
		float f, f1, f2, f3;
		char c, c1, c2, c3;
		short s, s1, s2, s3;
		string[] array;
	}

	public class ClassWithPrefixedFieldsWithCamelCasing {
		int fooBar;
		int fooBaz;
	}

	public class ClassWithoutPrefixedFieldsWithMDashCasing {
		int m_member;
		int m_other;
	}

	public class ClassWithPrefixedFieldsWithMDashCasing {
		int m_foo_bar;
		int m_foo_baz;
	}

	public class ClassWithPrefixedFieldsWithDashCasing {
		int phone_number;
		int phone_area_code;
	}

	public class NotLargeClass {
	}

	[TestFixture]
	public class AvoidLargeClassesTest {
		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		private MessageCollection messageCollection;

		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AvoidLargeClassesRule ();
			messageCollection = null;
		}

		[Test]
		public void LargeClassTest () 
		{
			type = assembly.MainModule.Types["Test.Rules.Smells.LargeClass"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (2, messageCollection.Count);
		}

		[Test]
		public void NotLargeClassTest () 
		{
			type = assembly.MainModule.Types["Test.Rules.Smells.NotLargeClass"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void ClassWithPrefixedFieldsWithCamelCasingTest ()
		{
			type = assembly.MainModule.Types["Test.Rules.Smells.ClassWithPrefixedFieldsWithCamelCasing"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
			
		[Test]
		public void ClassWithoutPrefixedFieldsWithMDashCasingTest () 
		{
			type = assembly.MainModule.Types["Test.Rules.Smells.ClassWithoutPrefixedFieldsWithMDashCasing"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void ClassWithPrefixedFieldsWithDashCasingTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.ClassWithPrefixedFieldsWithDashCasing"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}

		[Test]
		public void ClassWithPrefixedFieldsWithMDashCasingTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.ClassWithPrefixedFieldsWithMDashCasing"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
	}
}
