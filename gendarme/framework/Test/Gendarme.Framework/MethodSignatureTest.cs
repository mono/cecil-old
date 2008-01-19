//
// Unit tests for MethodSignature
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//
//  (C) 2008 Andreas Noever
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

using Mono.Cecil;
using Gendarme.Framework;

using NUnit.Framework;

namespace Test.Framework {

	[TestFixture]
	public class MethodSignatureTest {

		private TypeDefinition type;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			type = AssemblyFactory.GetAssembly (unit).MainModule.Types ["Test.Framework.MethodSignatureTest"];
		}

		private MethodDefinition GetMethod (string name)
		{
			foreach (MethodDefinition method in type.Methods)
				if (method.Name == name)
					return method;
			return null;
		}

		[Test]
		public void TestDefaultConstructor ()
		{
			MethodSignature sig = new MethodSignature ();
			Assert.IsNull (sig.Name);
			Assert.IsNull (sig.Parameters);
			Assert.IsNull (sig.ReturnType);
			Assert.AreEqual ((MethodAttributes) 0, sig.Attributes);
		}

		[Test]
		public void TestCopyConstructor ()
		{
			MethodSignature sig1 = new MethodSignature () {
				Name = "name",
				ReturnType = "ret",
				Attributes = MethodAttributes.FamORAssem,
				Parameters = new string [] { "1", "2" }
			};
			MethodSignature sig2 = new MethodSignature (sig1);
			Assert.AreEqual (sig1.Name, sig2.Name, "Name");
			Assert.AreEqual (sig1.ReturnType, sig2.ReturnType, "ReturnType");
			Assert.AreEqual (sig1.Attributes, sig2.Attributes, "Attributes");

			Assert.IsFalse (Object.ReferenceEquals (sig1.Parameters, sig2.Parameters), "copy array");
			Assert.AreEqual (sig1.Parameters, sig2.Parameters, "AreEqual compares members");
		}

		public void Method (bool parameter) { }

		[Test]
		public void TestMatch ()
		{
			Assert.IsTrue (new MethodSignature ().Matches (GetMethod ("TestMatch")));
/* commented until #354928 is fixed
			Assert.IsTrue (new MethodSignature () { Name = "TestMatch" }.Matches (GetMethod ("TestMatch")));
			Assert.IsFalse (new MethodSignature () { Name = "TestMatch_" }.Matches (GetMethod ("TestMatch")));

			Assert.IsTrue (new MethodSignature () { ReturnType = "System.Void" }.Matches (GetMethod ("TestMatch")));
			Assert.IsFalse (new MethodSignature () { ReturnType = "System.Void_" }.Matches (GetMethod ("TestMatch")));

			Assert.IsFalse (new MethodSignature () { Parameters = new string [1] }.Matches (GetMethod ("TestMatch")));

			Assert.IsTrue (new MethodSignature () { Parameters = new string [] { "System.Boolean" } }.Matches (GetMethod ("Method")));
			Assert.IsTrue (new MethodSignature () { Parameters = new string [] { null } }.Matches (GetMethod ("Method")));
			Assert.IsFalse (new MethodSignature () { Parameters = new string [] { "System.Object" } }.Matches (GetMethod ("Method")));

			Assert.IsTrue (new MethodSignature () { Attributes = MethodAttributes.Public }.Matches (GetMethod ("TestMatch")));
			Assert.IsFalse (new MethodSignature () { Attributes = MethodAttributes.Virtual }.Matches (GetMethod ("TestMatch")));
*/
		}
	}
}
