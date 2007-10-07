//
// Unit test for UseValueInPropertySetterRule
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
using Gendarme.Framework;
using Gendarme.Rules.Correctness;
using Mono.Cecil;
using NUnit.Framework;
using System.Reflection;

namespace Test.Rules.Correctness
{
	[TestFixture]
	public class UseValueInPropertySetterTest
	{
		public class Item
		{

			public bool UsesValue
			{
				set
				{
					bool val = value;
				}
			}

			public bool DoesNotUseValue
			{
				set
				{
					bool val = true;
				}
			}

			public void set_NotAProperty(bool value)
			{
				bool val = true;
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
			type = module.Types["Test.Rules.Correctness.UseValueInPropertySetterTest/Item"];
			rule = new UseValueInPropertySetterRule();
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
		public void TestUsesValue()
		{
			MethodDefinition method = GetTest("set_UsesValue");
			Assert.IsNull(CheckMethod(method));
		}
		
		[Test]
		public void TestDoesNotUseValue()
		{
			MethodDefinition method = GetTest("set_DoesNotUseValue");
			Assert.IsNotNull(CheckMethod(method));
		}

		[Test]
		public void TestNotAProperty()
		{
			MethodDefinition method = GetTest("set_NotAProperty");
			Assert.IsNull(CheckMethod(method));
		}

	}
}
