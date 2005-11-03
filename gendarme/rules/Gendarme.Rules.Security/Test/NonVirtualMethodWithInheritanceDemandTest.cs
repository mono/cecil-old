//
// Unit tests for NonVirtualMethodWithInheritanceDemandRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SSP = System.Security.Permissions;

using Gendarme.Framework;
using Gendarme.Rules.Security;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Security {

	[TestFixture]
	public class NonVirtualMethodWithInheritanceDemandTest {

		public abstract class AbstractMethodsClass {

			[SecurityPermission (SSP.SecurityAction.InheritanceDemand, ControlAppDomain = true)]
			public abstract void Asbtract ();
		}

		public class VirtualMethodsClass: AbstractMethodsClass  {

			public override void Asbtract ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.InheritanceDemand, ControlAppDomain = true)]
			public virtual void Virtual ()
			{
			}
		}

		public class NoVirtualMethodsClass {

			[SecurityPermission (SSP.SecurityAction.InheritanceDemand, ControlAppDomain = true)]
			public void Method ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.InheritanceDemand, ControlAppDomain = true)]
			static void StaticMethod ()
			{
			}
		}

		public abstract class NotInheritanceDemandClass {

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlAppDomain = true)]
			public abstract void Asbtract ();

			[SecurityPermission (SSP.SecurityAction.Demand, ControlAppDomain = true)]
			public virtual void Virtual ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.LinkDemand, ControlAppDomain = true)]
			public void Method ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.Demand, ControlAppDomain = true)]
			static void StaticMethod ()
			{
			}
		}

		private IMethodRule rule;
		private IAssemblyDefinition assembly;
		private IModuleDefinition module;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			module = assembly.MainModule;
			rule = new NonVirtualMethodWithInheritanceDemandRule ();
		}

		private ITypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.Security.NonVirtualMethodWithInheritanceDemandTest/" + name;
			return assembly.MainModule.Types[fullname];
		}

		[Test]
		public void AbstractMethods ()
		{
			ITypeDefinition type = GetTest ("AbstractMethodsClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsTrue (rule.CheckMethod (assembly, module, type, method));
			}
		}

		[Test]
		public void VirtualMethods ()
		{
			ITypeDefinition type = GetTest ("VirtualMethodsClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsTrue (rule.CheckMethod (assembly, module, type, method));
			}
		}

		[Test]
		public void NoVirtualMethods ()
		{
			ITypeDefinition type = GetTest ("NoVirtualMethodsClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsFalse (rule.CheckMethod (assembly, module, type, method));
			}
		}

		[Test]
		public void NotInheritanceDemand ()
		{
			ITypeDefinition type = GetTest ("NotInheritanceDemandClass");
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsTrue (rule.CheckMethod (assembly, module, type, method));
			}
		}
	}
}
