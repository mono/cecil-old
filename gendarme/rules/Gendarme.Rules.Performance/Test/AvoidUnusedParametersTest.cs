//
// Unit Test for AvoidUnusedParameters Rule.
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
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

using Mono.Cecil;
using NUnit.Framework;
using Gendarme.Framework;
using Gendarme.Rules.Performance;

namespace Test.Rules.Performance {

	public abstract class AbstractClass {
		public abstract void AbstractMethod (int x);
	}

	public class VirtualClass {
		public virtual void VirtualMethod (int x) 
		{
		}
	}

	public class OverrideClass : VirtualClass {
		public override void VirtualMethod (int x) 
		{
		}
	}
	
	[TestFixture]
	public class AvoidUnusedParametersTest {
		
		private IMethodRule rule;
		private AssemblyDefinition assembly;
		private MethodDefinition method;
		private MessageCollection messageCollection;

		public void PrintBannerUsingParameter (Version version) 
		{
			Console.WriteLine ("Welcome to the foo program {0}", version);
		}

		public void PrintBannerUsingAssembly (Version version)
		{
			Console.WriteLine ("Welcome to the foo program {0}", Assembly.GetExecutingAssembly ().GetName ().Version);
		}

		public void PrintBannerWithoutParameters () 
		{
			Console.WriteLine ("Welcome to the foo program {0}", Assembly.GetExecutingAssembly ().GetName ().Version);
		}

		public void MethodWithUnusedParameters (IEnumerable enumerable, int x) 
		{
			Console.WriteLine ("Method with unused parameters");
		}

		public void MethodWith5UsedParameters (int x, IEnumerable enumerable, string foo, char c, float f) 
		{
			Console.WriteLine (f);
			Console.WriteLine (c);
			Console.WriteLine (foo);
			Console.WriteLine (enumerable);
			Console.WriteLine (x);
		}

		public static void StaticMethodWithUnusedParameters (int x, string foo) 
		{
		}

		public static void StaticMethodWithUsedParameters (int x, string foo) 
		{
			Console.WriteLine (x);
			Console.WriteLine (foo);
		}

		public static void StaticMethodWith5UsedParameters (int x, string foo, IEnumerable enumerable, char c, float f) 
		{
			Console.WriteLine (f);
			Console.WriteLine (c);
			Console.WriteLine (foo);
			Console.WriteLine (enumerable);
			Console.WriteLine (x);
		}

		public static void StaticMethodWith5UnusedParameters (int x, string foo, IEnumerable enumerable, char c, float f)
		{
		}
		
		[DllImport ("libc.so")]
		private static extern double cos (double x);

		public delegate void SimpleCallback (int x);
		public void SimpleCallbackImpl (int x) 
		{
		}

		public delegate void SimpleEventHandler (int x);
		public event SimpleEventHandler SimpleEvent;
		public void OnSimpleEvent (int x) 
		{
		}

		[TestFixtureSetUp]
		public void FixtureSetUp () 
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new AvoidUnusedParametersRule ();
			messageCollection = null;
		}

		[Test]
		public void PrintBannerUsingParameterTest () 
		{
			method = GetMethodForTest ("PrintBannerUsingParameter");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void PrintBannerUsingAssemblyTest ()
		{
			method = GetMethodForTest ("PrintBannerUsingAssembly");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}

		[Test]
		public void PrintBannerWithoutParametersTest () 
		{
			method = GetMethodForTest ("PrintBannerWithoutParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void MethodWithUnusedParametersTest () 
		{
			method = GetMethodForTest ("MethodWithUnusedParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (2, messageCollection.Count);
		}
		
		[Test]
		public void AbstractMethodTest () 
		{
			method = GetMethodForTestFrom ("Test.Rules.Performance.AbstractClass", "AbstractMethod");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void VirtualMethodTest () 
		{
			method = GetMethodForTestFrom ("Test.Rules.Performance.VirtualClass", "VirtualMethod");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void OverrideMethodTest () 
		{
			method = GetMethodForTestFrom ("Test.Rules.Performance.OverrideClass", "VirtualMethod");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void ExternMethodTest () 
		{
			method = GetMethodForTest ("cos");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void DelegateMethodTest () 
		{
			SimpleCallback callback = new SimpleCallback (SimpleCallbackImpl);
			method = GetMethodForTest ("SimpleCallbackImpl");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void EventTest () 
		{
			SimpleEvent += new SimpleEventHandler (OnSimpleEvent);
			method = GetMethodForTest ("OnSimpleEvent");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		} 

		[Test]
		public void MethodWith5UsedParametersTest () 
		{
			method = GetMethodForTest ("MethodWith5UsedParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void StaticMethodWithUnusedParametersTest () 
		{
			method = GetMethodForTest ("StaticMethodWithUnusedParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (2, messageCollection.Count);
		}

		[Test]
		public void StaticMethodWithUsedParametersTest () 
		{
			method = GetMethodForTest ("StaticMethodWithUsedParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}

		[Test]
		public void StaticMethodWith5UsedParametersTest () 
		{
			method = GetMethodForTest ("StaticMethodWith5UsedParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void StaticMethodWith5UnusedParametersTest () 
		{
			method = GetMethodForTest ("StaticMethodWith5UnusedParameters");
			messageCollection = rule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (5, messageCollection.Count);
		}
	
		private MethodDefinition GetMethodForTest (string methodName) 
		{
			return GetMethodForTestFrom ("Test.Rules.Performance.AvoidUnusedParametersTest", methodName);
		}

		private MethodDefinition GetMethodForTestFrom (string fullTypeName, string methodName) 
		{
			TypeDefinition type =  assembly.MainModule.Types[fullTypeName];
			if (type != null) {
				foreach (MethodDefinition method in type.Methods) {
				if (method.Name == methodName)
					return method;
				}
			}
			return null;
		}
	}
}
