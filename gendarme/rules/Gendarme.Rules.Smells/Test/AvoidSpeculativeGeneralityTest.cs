//
// Unit Test for AvoidSpeculativeGenerality Rule.
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
	
	//
	public abstract class AbstractClass {
		public abstract void MakeStuff ();
	}

	public class OverriderClass : AbstractClass {
		public override void MakeStuff () 
		{
		
		}
	}

	//
	public abstract class OtherAbstractClass {
		public abstract void MakeStuff ();
	}

	public class OtherOverriderClass : OtherAbstractClass {
		public override void MakeStuff () 
		{
		}
	}

	public class YetAnotherOverriderClass : OtherAbstractClass {
		public override void MakeStuff () 
		{
		}
	}

	//
	public class Person {
		int age;
		string name;
		Telephone phone;
	}

	public class Telephone {
		int areaCode;
		int phone;
	}

	//
	public class Keyboard {
		Key pressedKey;
	}

	public class Calculator {
		Key pointKey;
	}

	public class Key {
	}
	
	//
	public class Motorbike {
		Wheel first;
		Wheel last;
	}

	public class Wheel {
	}

	//
	public class ClassWithUnusedParameter {
		public void Foo (int x) 
		{
		}
	}

	public class ClassWithFourUnusedParameters {
		public void Foo (int x) 
		{
		}

		public void Bar (int x, char f) 
		{
		}

		public void Baz (float f) 
		{
		}

	}

	[TestFixture]
	public class AvoidSpeculativeGeneralityTest {
		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		private MessageCollection messageCollection;

		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AvoidSpeculativeGeneralityRule ();
			messageCollection = null;
		}

		[Test]
		public void AbstractClassesWithoutResponsabilityTest () 
		{
			type = assembly.MainModule.Types["Test.Rules.Smells.AbstractClass"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}

		[Test]
		public void AbstractClassesWithResponsabilityTest ()
		{
			type = assembly.MainModule.Types["Test.Rules.Smells.OtherAbstractClass"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void ClassWithUnnecesaryDelegationTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.Telephone"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}

		[Test]
		public void ClassWithoutUnnecesaryDelegationTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.Key"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void OtherClassWithUnnecesaryDelegationTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.Wheel"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void ClassWithUnusedParameterTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.ClassWithUnusedParameter"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void ClassWithFourUnusedParametersTest () 
		{
			type = assembly.MainModule.Types ["Test.Rules.Smells.ClassWithFourUnusedParameters"];
			messageCollection = rule.CheckType (type, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (4, messageCollection.Count);
		}
	}
}
