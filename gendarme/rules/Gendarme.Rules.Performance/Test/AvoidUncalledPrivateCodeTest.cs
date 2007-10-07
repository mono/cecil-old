// 
// Unit tests for AvoidUncalledPrivateCodeRule
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

using Gendarme.Framework;
using Gendarme.Rules.Performance;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Performance
{
	[TestFixture]
	public class AvoidUncalledPrivateCodeTest {
		
		public class UncalledPrivateMethod
		{
			private void display ()
			{
			}
		}
		
		public class CalledPrivateMethod
		{
			private void display ()
			{
			}
			
			public static void Main (string [] args)
			{
				CalledPrivateMethod c = new CalledPrivateMethod ();
				c.display ();
			}
		}
		
		public class UncalledInternalMethod
		{
			internal void print ()
			{
			}
		}
		
		public class CalledInternalMethod
		{
			internal void CalledMethod ()
			{
			}
		}
		
		public class MethodCallingClass
		{
			public static void Main (string [] args)
			{
				CalledInternalMethod c = new CalledInternalMethod ();
				c.CalledMethod ();
			}
		}
		
		private class PublicMethodNotCalledInPrivateClass
		{
			public void publicMethod ()
			{
			}
		}
		
		internal class PublicMethodNotCalledInInternalClass
		{
			public void publicMethod ()
			{
			}
		}
		
		private class PublicMethodCalledInPrivateClass
		{
			public void publicCalledMethod ()
			{
			}
			
			public static void Main (string [] args)
			{
				PublicMethodCalledInPrivateClass p = new PublicMethodCalledInPrivateClass ();
				p.publicCalledMethod ();
			}
		}
		
		internal class PublicMethodCalledInInternalClass
		{
			public void publicMethodCalled ()
			{
			}
			
			public static void Main (string [] args)
			{
				PublicMethodCalledInInternalClass p = new PublicMethodCalledInInternalClass ();
				p.publicMethodCalled ();
			}
		}
		
		private class PrivateMethodInPrivateClassNotCalled
		{
			private void privateMethodNotCalled ()
			{
			}
		}
		
		internal class NestedClasses
		{
			public class AnotherClass
			{
				public void publicMethodNotCalledInNestedInternalClass ()
				{
				}
			}
		}
		
		interface Iface1
		{
			void IfaceMethod1 ();
		}
		
		interface Iface2
		{
			void IfaceMethod2 ();
		}
		
		public class ImplementingExplicitInterfacesMembers: Iface1, Iface2 
		{
			void Iface1.IfaceMethod1 ()
			{
			}
			
			void Iface2.IfaceMethod2 ()
			{
			}
		
			public static void Main (string [] args)
			{
				ImplementingExplicitInterfacesMembers i = new ImplementingExplicitInterfacesMembers ();
				Iface1 iobject = i;
				iobject.IfaceMethod1 ();
			}
		}
		
		public class PrivateConstructorNotCalled
		{
			private PrivateConstructorNotCalled ()
			{
			}
		}
		
		public class StaticConstructorNotCalled
		{
			static int i = 0;
			static StaticConstructorNotCalled ()
			{
				i = 5;
			}
		}
		
		[SerializableAttribute]
		public class SerializableConstructorNotCalled
		{
			private int i;
			public SerializableConstructorNotCalled (SerializationInfo info, StreamingContext context)
			{
				i = 0;
			}
		}
		
		public class UncalledOverriddenMethod
		{
			public override string ToString ()
			{
				return String.Empty;
			}
		}
		
		public class UsingComRegisterAndUnRegisterFunctionAttribute
		{
			[ComRegisterFunctionAttribute]
			private void register ()
			{
			}
			[ComUnregisterFunctionAttribute]
			private void unregister ()
			{
			}
		}
		
		public class CallingPrivateMethodsThroughDelegates
		{
			delegate string delegateExample ();
			
			private string privateMethod ()
			{
				return String.Empty;
			}
			
			public static void Main (string [] args)
			{
				CallingPrivateMethodsThroughDelegates c = new CallingPrivateMethodsThroughDelegates ();
				delegateExample d = new delegateExample (c.privateMethod);
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
			methodRule = new AvoidUncalledPrivateCodeRule ();
			messageCollection = null;
		}
		
		private TypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.Performance.AvoidUncalledPrivateCodeTest/" + name;
			return assembly.MainModule.Types [fullname];
		}
		
		[Test]
		public void uncalledPrivateMethodTest ()
		{
			type = GetTest ("UncalledPrivateMethod");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "display")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void calledPrivateMethodTest ()
		{
			type = GetTest ("CalledPrivateMethod");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "display")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void uncalledInternalMethodTest ()
		{
			type = GetTest ("UncalledInternalMethod");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "print")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void calledInternalMethodTest ()
		{
			type = GetTest ("CalledInternalMethod");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "CalledMethod")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void checkingForMainMethodTest ()
		{
			type = GetTest ("CalledInternalMethod");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Main")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void publicMethodNotCalledInPrivateClassTest ()
		{
			type = GetTest ("PublicMethodNotCalledInPrivateClass");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "publicMethod")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void publicMethodCalledInPrivateClassTest ()
		{
			type = GetTest ("PublicMethodCalledInPrivateClass");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "publicCalledMethod")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void publicMethodCalledInInternalClassTest ()
		{
			type = GetTest ("PublicMethodCalledInInternalClass");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "publicMethodCalled")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void privateMethodInPrivateClassNotCalledTest ()
		{
			type = GetTest ("PrivateMethodInPrivateClassNotCalled");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "privateMethodNotCalled")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void publicMethodNotCalledInNestedInternalClassTest ()
		{
			type = GetTest ("NestedClasses");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "publicMethodNotCalledInNestedInternalClass")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void implementingInterfacesMembersTest ()
		{
			type = GetTest ("ImplementingExplicitInterfacesMembers");
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == "Test.Rules.Performance.AvoidUncalledPrivateCodeTest+Iface2.IfaceMethod2")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void privateConstructorNotCalledTest ()
		{
			type = GetTest ("PrivateConstructorNotCalled");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == ".ctor")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNotNull (messageCollection);
			Assert.AreEqual (1, messageCollection.Count);
		}
		
		[Test]
		public void staticConstructorNotCalledTest ()
		{
			type = GetTest ("StaticConstructorNotCalled");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == ".cctor")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void serializableConstructorNotCalledTest ()
		{
			type = GetTest ("SerializableConstructorNotCalled");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == ".ctor")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void uncalledOverriddenMethodTest ()
		{
			type = GetTest ("UncalledOverriddenMethod");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == "ToString")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void implementingComRegisterFunctionAttributeTest ()
		{
			type = GetTest ("UsingComRegisterAndUnRegisterFunctionAttribute");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == "register")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void implementingComUnregisterFunctionAttributeTest ()
		{
			type = GetTest ("UsingComRegisterAndUnRegisterFunctionAttribute");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == "unregister")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
		[Test]
		public void callingPrivateMethodsThroughDelegatesTest ()
		{
			type = GetTest ("CallingPrivateMethodsThroughDelegates");
			foreach (MethodDefinition method in type.Constructors)
				if (method.Name == "privateMethod")
					messageCollection = methodRule.CheckMethod (method, new MinimalRunner ());
			Assert.IsNull (messageCollection);
		}
		
	}
}