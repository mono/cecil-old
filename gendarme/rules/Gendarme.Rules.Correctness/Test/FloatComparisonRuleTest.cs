//
// Unit test for FloatComparisonRule
//
// Authors:
//	Lukasz Knop <lukasz.knop@gmail.com>
//
// Copyright (C) 2007 Lukasz Knop
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
using System.Text;

using System.Reflection;

using Gendarme.Framework;
using Gendarme.Rules.Correctness;
using Mono.Cecil;
using NUnit.Framework;


namespace Test.Rules.Correctness
{
	[TestFixture]
	public class FloatComparisonRuleTest
	{

		public class Item
		{
			public float floatField = 0f;
			public static float staticFloatField = 0f;

			public float FloatProperty
			{
				get { return floatField; }
			}

			public static float StaticFloatMethod()
			{
				return 0f;
			}

			public void FloatFields()
			{
				bool a = floatField == staticFloatField;
				bool b = staticFloatField == floatField;
				
			}

			public bool FloatMethodResult()
			{
				bool a = FloatProperty == StaticFloatMethod();
				bool b = StaticFloatMethod() == FloatProperty;
				return a && b;
			}

			public void FloatParameters(float f, float g)
			{
				bool a = f == g;
				
			}

			public void FloatLocal()
			{
				float f1 = 0f;
				float f2 = 0f;
				bool a = f1 == f2;
				bool b = f2 == f1;
			}

			public void FloatLocalAbove4()
			{
				string s1 = String.Empty;
				string s2 = String.Empty;
				string s3 = String.Empty;
				string s4 = String.Empty;
				float f1 = 0f;
				float f2 = 0f;
				bool a = f1 == f2;
				bool b = f2 == f1;
			}

			public void FloatComparisonAfterArithmeticOperation()
			{
				float f1 = 0f;
				float f2 = 0f;
				bool a = f1 * 1 == f2 * 1;
			}

			public void LegalFloatComparisons()
			{
				float f = 0f;
				bool a;
				bool b;
				
				//a = float.PositiveInfinity == f;
				b = f == float.PositiveInfinity;
				
				//a = float.NegativeInfinity == f;
				b = f == float.NegativeInfinity;
				
			}

			public void LegalIntegerComparisons()
			{
				int i = 0;
				int j = 1;
				bool a = i == j;
				
			}


			public void FloatEqualsCall()
			{
				float f1 = 0f;
				float f2 = 0f;
				bool a = f1.Equals(f2);
			}

			public bool NoFloatComparison()
			{
				return false;
			}

		}

		private IMethodRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		private ModuleDefinition module;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			string unit = Assembly.GetExecutingAssembly().Location;
			assembly = AssemblyFactory.GetAssembly(unit);
			module = assembly.MainModule;
			type = module.Types["Test.Rules.Correctness.FloatComparisonRuleTest/Item"];
			rule = new FloatComparisonRule();
		}

		MethodDefinition GetTest(string name)
		{
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == name)
					return method;

			return null;
		}

		MessageCollection CheckMethod(MethodDefinition method)
		{
			return rule.CheckMethod(method, new MinimalRunner());
		}

		[Test]
		public void TestFloatFields()
		{
			MethodDefinition method = GetTest("FloatFields");
			Assert.IsNotNull(CheckMethod(method));
		}

		[Test]
		public void TestFloatMethodResult()
		{
			MethodDefinition method = GetTest("FloatMethodResult");
			Assert.IsNotNull(CheckMethod(method));
		}
		
		[Test]
		public void TestFloatParameters()
		{
			MethodDefinition method = GetTest("FloatParameters");
			Assert.IsNotNull(CheckMethod(method));
		}

		[Test]
		public void TestFloatLocal()
		{
			MethodDefinition method = GetTest("FloatLocal");
			Assert.IsNotNull(CheckMethod(method));
		}

		[Test]
		public void TestFloatLocalAbove4()
		{
			MethodDefinition method = GetTest("FloatLocalAbove4");
			Assert.IsNotNull(CheckMethod(method));
		}

		[Test]
		public void TestFloatComparisonAfterArithmeticOperation()
		{
			MethodDefinition method = GetTest("FloatComparisonAfterArithmeticOperation");
			Assert.IsNotNull(CheckMethod(method));
		}

		[Test]
		public void TestLegalFloatComparisons()
		{
			MethodDefinition method = GetTest("LegalFloatComparisons");
			Assert.IsNull(CheckMethod(method));
		}

		[Test]
		public void TestLegalIntegerComparisons()
		{
			MethodDefinition method = GetTest("LegalIntegerComparisons");
			Assert.IsNull(CheckMethod(method));
		}


		[Test]
		public void TestFloatEqualsCall()
		{
			MethodDefinition method = GetTest("FloatEqualsCall");
			Assert.IsNotNull(CheckMethod(method));
		}
		

		[Test]
		public void TestNoFloatComparison()
		{
			MethodDefinition method = GetTest("NoFloatComparison");
			Assert.IsNull(CheckMethod(method));
		}

	}
}
