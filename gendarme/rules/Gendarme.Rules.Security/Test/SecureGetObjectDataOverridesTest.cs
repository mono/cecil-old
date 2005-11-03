//
// Unit tests for SecureGetObjectDataOverridesRule
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
	public class SecureGetObjectDataOverridesTest {

		[Serializable]
		public class SerializableClass {

			public SerializableClass ()
			{
			}
		}

		public class ISerializableClass : ISerializable {

			public ISerializableClass ()
			{
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
			}
		}

		public class InheritISerializableClass : NameValueCollection {

			public InheritISerializableClass ()
			{
			}

			public override void GetObjectData (SerializationInfo info, StreamingContext context)
			{
			}
		}

		public class LinkDemandClass: ISerializable {

			public LinkDemandClass ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.LinkDemand, SerializationFormatter = true)]
			public void GetObjectData (SerializationInfo info, StreamingContext context)
			{
			}
		}

		public class InheritanceDemandClass: ISerializable {

			public InheritanceDemandClass ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.InheritanceDemand, SerializationFormatter = true)]
			public void GetObjectData (SerializationInfo info, StreamingContext context)
			{
			}
		}

		public class DemandClass: ISerializable {

			public DemandClass ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.Demand, SerializationFormatter = true)]
			public void GetObjectData (SerializationInfo info, StreamingContext context)
			{
			}
		}

		public class DemandWrongPermissionClass: ISerializable {

			public DemandWrongPermissionClass ()
			{
			}

			[SecurityPermission (SSP.SecurityAction.Demand, ControlAppDomain = true)]
			public void GetObjectData (SerializationInfo info, StreamingContext context)
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
			rule = new SecureGetObjectDataOverridesRule ();
		}

		private ITypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.Security.SecureGetObjectDataOverridesTest/" + name;
			return assembly.MainModule.Types[fullname];
		}

		private IMethodDefinition GetObjectData (ITypeDefinition type)
		{
			foreach (IMethodDefinition method in type.Methods) {
				if (method.Name == "GetObjectData")
					return method;
			}
			return null;
		}

		[Test]
		public void Serializable ()
		{
			ITypeDefinition type = GetTest ("SerializableClass");
			// there's no GetObjectData method here so the test should never fail
			foreach (IMethodDefinition method in type.Methods) {
				Assert.IsTrue (rule.CheckMethod (assembly, module, type, method));
			}
		}

		[Test]
		public void ISerializable ()
		{
			ITypeDefinition type = GetTest ("ISerializableClass");
			IMethodDefinition method = GetObjectData (type);
			Assert.IsFalse (rule.CheckMethod (assembly, module, type, method));
		}

		[Test]
		public void InheritISerializable ()
		{
			ITypeDefinition type = GetTest ("InheritISerializableClass");
			IMethodDefinition method = GetObjectData (type);
			Assert.IsFalse (rule.CheckMethod (assembly, module, type, method));
		}

		[Test]
		public void LinkDemand ()
		{
			ITypeDefinition type = GetTest ("LinkDemandClass");
			IMethodDefinition method = GetObjectData (type);
			Assert.IsTrue (rule.CheckMethod (assembly, module, type, method));
		}

		[Test]
		public void InheritanceDemand ()
		{
			ITypeDefinition type = GetTest ("InheritanceDemandClass");
			IMethodDefinition method = GetObjectData (type);
			Assert.IsFalse (rule.CheckMethod (assembly, module, type, method));
		}

		[Test]
		public void Demand ()
		{
			ITypeDefinition type = GetTest ("DemandClass");
			IMethodDefinition method = GetObjectData (type);
			Assert.IsTrue (rule.CheckMethod (assembly, module, type, method));
		}

		[Test]
		public void DemandWrongPermission ()
		{
			ITypeDefinition type = GetTest ("DemandWrongPermissionClass");
			IMethodDefinition method = GetObjectData (type);
			Assert.IsFalse (rule.CheckMethod (assembly, module, type, method));
		}
	}
}
