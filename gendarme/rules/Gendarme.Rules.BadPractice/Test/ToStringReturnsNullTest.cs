// 
// Unit tests for ToStringReturnsNullRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//
// Copyright (c) <2007> Nidhi Rawal
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
using Gendarme.Rules.BadPractice;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.BadPractice
{
	[TestFixture]
	public class ToStringReturnsNullTest {
		
		public class ToStringReturningNull
		{
			public override string ToString ()
			{
				return null;
			}
		}
		
		public class ToStringReturningEmptyString
		{
			public override string ToString ()
			{
				return String.Empty;
			}
		} 
		
		public class ToStringNotReturningNull
		{
			string s = "ab";
			public override string ToString ()
			{
				return s;
			}
		} 
		
		public class ConvertToStringReturningEmptyString
		{
			object obj = null;
			public override string ToString ()
			{
				return Convert.ToString (obj);
			}
		} 
		
		public class ConvertToStringReturningNull
		{
			string s = null;
			public override string ToString ()
			{ 
				return Convert.ToString (s);
			}
		} 
		
		private IMethodRule methodRule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		MessageCollection messageCollection;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			methodRule = new ToStringReturnsNullRule();
			messageCollection = null;
		}
		
		private TypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.BadPractice.ToStringReturnsNullTest/" + name;
			return assembly.MainModule.Types[fullname];
		}
		
		[Test]
		public void toStringReturningNullTest ()
		{
			type = GetTest ("ToStringReturningNull");
			foreach (MethodDefinition method in type.Methods) {
				messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			}
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void toStringReturningEmptyStringTest ()
		{
			type = GetTest ("ToStringReturningEmptyString");
			foreach (MethodDefinition method in type.Methods) {
				messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			}
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void toStringNotReturningNullTest ()
		{
			type = GetTest ("ToStringNotReturningNull");
			foreach (MethodDefinition method in type.Methods) {
				messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			}
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void convertToStringReturningEmptyStringTest ()
		{
			type = GetTest ("ConvertToStringReturningEmptyString");
			foreach (MethodDefinition method in type.Methods) {
				messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			}
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void convertToStringReturningNullTest ()
		{
			type = GetTest ("ConvertToStringReturningNull");
			foreach (MethodDefinition method in type.Methods) {
				messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			}
			Assert.IsNotNull (messageCollection);
		}
	}
}