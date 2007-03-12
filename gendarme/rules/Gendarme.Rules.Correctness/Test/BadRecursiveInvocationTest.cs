//
// Unit tests for BadRecursiveInvocationRule
//
// Authors:
//	Aaron Tomb <atomb@soe.ucsc.edu>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005 Aaron Tomb
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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

using Gendarme.Framework;
using Gendarme.Rules.Correctness;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Correctness {

	[TestFixture]
	public class BadRecursiveInvocationTest {

		class BadRec {

			/* This should be an error. */
			public int Foo {
				get { return Foo; }
			}

			/* correct */
			public int Bar {
				get { return -1; }
			}

			/* a more complex recursion */
			public int FooBar {
				get { return BarFoo; }
			}

			public int BarFoo {
				get { return FooBar; }
			}

			/* This should be fine, as it uses 'base.' */
			public override int GetHashCode ()
			{
				return base.GetHashCode ();
			}
			
			/* not fine, missing 'base.' */
			public override bool Equals (object obzekt)
			{
				return Equals (obzekt);
			}
			
			public static int Fibonacci (int n)
			{
				if (n < 2)
					return n;
					
				return Fibonacci (n - 1) + Fibonacci (n - 2);
			}
		}
		
		private IMethodRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		private ModuleDefinition module;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			module = assembly.MainModule;
			type = module.Types["Test.Rules.Correctness.BadRecursiveInvocationTest/BadRec"];
			rule = new BadRecursiveInvocationRule ();
		}

		private MethodDefinition GetTest (string name)
		{
			foreach (MethodDefinition method in type.Methods) {
				if (method.Name == name)
					return method;
			}
			return null;
		}

		[Test]
		public void RecursiveProperty ()
		{
			MethodDefinition method = GetTest ("get_Foo"); 
			Assert.IsNotNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void Property ()
		{
			MethodDefinition method = GetTest ("get_Bar"); 
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		[Test, Ignore ("uncatched by rule")]
		public void IndirectRecursiveProperty ()
		{
			MethodDefinition method = GetTest ("get_FooBar"); 
			Assert.IsNotNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		[Test]
		public void OverriddenMethod ()
		{
			MethodDefinition method = GetTest ("GetHashCode"); 
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void BadRecursiveMethod ()
		{
			MethodDefinition method = GetTest ("Equals"); 
			Assert.IsNotNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		[Test]
		public void Fibonacci ()
		{
			MethodDefinition method = GetTest ("Fibonacci"); 
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
	}
}
