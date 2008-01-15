// 
// Unit tests for ConstructorShouldNotCallVirtualMethodsRule
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//
// Copyright (C) Daniel Abramov
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

using Mono.Cecil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;
using Gendarme.Rules.BadPractice;

using NUnit.Framework;

namespace Test.Rules.BadPractice {

	internal class ClassNotCallingVirtualMethods {
		public ClassNotCallingVirtualMethods ()
		{
			this.NormalMethod ();
		}

		private void NormalMethod () { }
	}

	internal class ClassCallingVirtualMethodOnce {
		public ClassCallingVirtualMethodOnce ()
		{
			this.NormalMethod ();
			// call virtual method - bad thing
			this.VirtualMethod ();
			this.NormalMethod ();
		}

		protected virtual void VirtualMethod () { }
		private void NormalMethod () { }
	}

	internal class ClassCallingVirtualMethodThreeTimes {
		public ClassCallingVirtualMethodThreeTimes ()
		{
			this.NormalMethod ();
			// call virtual method - bad thing
			this.VirtualMethod ();
			this.VirtualMethod2 ();
			this.VirtualMethod ();
			this.NormalMethod ();
		}
		protected virtual void VirtualMethod () { }
		protected virtual void VirtualMethod2 () { }
		private void NormalMethod () { }
	}

	[TestFixture]
	public class ConstructorShouldNotCallVirtualMethodsTest {

		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private Runner runner;


		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new ConstructorShouldNotCallVirtualMethodsRule ();
			runner = new MinimalRunner ();
		}

		private TypeDefinition GetTest<T> ()
		{
			return assembly.MainModule.Types [typeof (T).FullName];
		}

		[Test]
		public void TestClassCallingVirtualMethodOnce ()
		{
			MessageCollection messages = rule.CheckType (GetTest<ClassCallingVirtualMethodOnce> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (1, messages.Count);
		}

		[Test]
		public void TestClassCallingVirtualMethodThreeTimes ()
		{
			MessageCollection messages = rule.CheckType (GetTest<ClassCallingVirtualMethodThreeTimes> (), runner);
			Assert.IsNotNull (messages);
			Assert.AreEqual (3, messages.Count);
		}

		[Test]
		public void TestClassNotCallingVirtualMethods ()
		{
			MessageCollection messages = rule.CheckType (GetTest<ClassNotCallingVirtualMethods> (), runner);
			Assert.IsNull (messages);
		}
	}
}
