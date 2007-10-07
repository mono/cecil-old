// 
// Unit tests for EqualShouldHandleNullArgRule
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
	public class EqualShouldHandleNullArgTest {
		public class EqualsChecksForNullArg
		{
			public override bool Equals (System.Object obj)
			{
				if (obj == null)
					return false;
				else
					return this == obj;
			}
			public override int GetHashCode ()
			{
				return 1;
			}
		}
		
		public class EqualsDoesNotCheckForNullArg
		{
			public override bool Equals (System.Object obj)
			{
				return this == obj;
			}
			public override int GetHashCode ()
			{
				return 1;
			}
		}
		
		public class EqualsDoesNotReturnFalseForNullArg
		{
			public override bool Equals (System.Object obj)
			{
				if (obj == null)
					return true;
				else
					return this == obj;
			}
			public override int GetHashCode ()
			{
				return 1;
			}
		}
		
		public class EqualsNotOverriddenNotCheckingNull
		{
			public bool Equals (System.Object obj)
			{
				return this == obj;
			}
			public override int GetHashCode ()
			{
				return 1;
			}
		}
		
		public class EqualsNotOverriddenNotReturningFalseForNull
		{
			public new bool Equals (System.Object obj)
			{
				if (obj == null)
					return true;
				else
					return this == obj;
			}
			public override int GetHashCode ()
			{
				return 1;
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
			methodRule = new EqualShouldHandleNullArgRule ();
			messageCollection = null;
		}
		
		private TypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.BadPractice.EqualShouldHandleNullArgTest/" + name;
			return assembly.MainModule.Types[fullname];
		}
		
		[Test]
		public void equalsChecksForNullArgTest ()
		{
			type = GetTest ("EqualsChecksForNullArg");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Equals")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void equalsDoesNotCheckForNullArgTest ()
		{
			type = GetTest ("EqualsDoesNotCheckForNullArg");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Equals")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void equalsDoesNotReturnFalseForNullArgTest ()
		{
			type = GetTest ("EqualsDoesNotReturnFalseForNullArg");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Equals")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void equalsNotOverriddenNotCheckingNullTest ()
		{
			type = GetTest ("EqualsNotOverriddenNotCheckingNull");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Equals")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void equalsNotOverriddenNotReturningFalseForNullTest ()
		{
			type = GetTest ("EqualsNotOverriddenNotReturningFalseForNull");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Equals")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
	}
}