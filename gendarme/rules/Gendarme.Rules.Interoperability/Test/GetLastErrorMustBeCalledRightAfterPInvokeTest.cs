//
// Unit tests for GetLastErrorMustBeCalledRightAfterPInvokeRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//
//  (C) 2007 Andreas Noever
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
using System.Runtime.InteropServices;

using Gendarme.Framework;
using Gendarme.Rules.Interoperability;
using Mono.Cecil;
using NUnit.Framework;

namespace Test.Rules.Interoperability {

	[TestFixture]
	public class GetLastErrorMustBeCalledRightAfterPInvokeTest {

		private GetLastErrorMustBeCalledRightAfterPInvokeRule rule;
		private AssemblyDefinition assembly;
		private TypeDefinition type;
		
		[DllImport ("User32.dll")]
		static extern Boolean MessageBeep (UInt32 beepType);
		
		public void CallNothing ()
		{
			new object ().ToString ();
		}
		
		public void CallNothingNotExternal ()
		{
			this.CallPInvoke ();
			Console.WriteLine ();
			Marshal.GetLastWin32Error ();
		}
		
		public void CallPInvoke ()
		{
			MessageBeep (5);
		}
		
		public void CallPInvoke_GetError ()
		{
			MessageBeep (5);
			Marshal.GetLastWin32Error ();
		}
		
		public void CallPInvoke_GetError_valid ()
		{
			SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle ((IntPtr)0, true);
			MessageBeep (5);
			IntPtr a = IntPtr.Zero;
			IntPtr b = IntPtr.Zero;
			if (a == b)
				return;
			if (handle.IsInvalid)
				return;
			Marshal.GetLastWin32Error ();
		}
		
		public void CallPInvoke_GetError_valid_struct ()
		{
			MessageBeep (5);
			new System.DateTime ();  //valid: initobj System.DateTime
			Marshal.GetLastWin32Error ();
		}
		
		public void CallPInvoke_GetError_invalid ()
		{
			MessageBeep (5);
			Console.WriteLine ("FAIL");
			Marshal.GetLastWin32Error ();
		}
		
		public void CallPInvoke_GetError_invalid_newobj ()
		{
			MessageBeep (5);
			new object ();
			Marshal.GetLastWin32Error ();
		}
		
		public void CallPInvoke_GetError_invalid_struct ()
		{
			MessageBeep (5);
			new System.DateTime (10); //invalid: call System.Void System.DateTime::.ctor(System.Int64)
			Marshal.GetLastWin32Error ();
		}
		
		public void CallGetError ()
		{
			new object().ToString ();
			Marshal.GetLastWin32Error ();
		}

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			type = assembly.MainModule.Types ["Test.Rules.Interoperability.GetLastErrorMustBeCalledRightAfterPInvokeTest"];
			rule = new GetLastErrorMustBeCalledRightAfterPInvokeRule ();
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
		public void TestNothing ()
		{
			MethodDefinition method = GetTest ("CallNothing");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestNothingNotExternal ()
		{
			MethodDefinition method = GetTest ("CallNothingNotExternal");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke ()
		{
			MethodDefinition method = GetTest ("CallPInvoke");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke_GetError ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_GetError");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke_GetError_valid ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_GetError_valid");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke_GetError_invalid ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_GetError_invalid");
			Assert.IsNotNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke_GetError_invalid_newobj ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_GetError_invalid_newobj");
			Assert.IsNotNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke_GetError_invalid_struct ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_GetError_invalid_struct");
			Assert.IsNotNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestPInvoke_GetError_valid_struct ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_GetError_valid_struct");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
		
		[Test]
		public void TestGetError ()
		{
			MethodDefinition method = GetTest ("CallGetError");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		public void CallPInvoke_Branch ()
		{
			bool a = MessageBeep (5);
			if (a) {
				Console.WriteLine ();
			} else {
				Marshal.GetLastWin32Error ();
			}
		}

		[Test]
		public void TestPInvoke_Branch ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_Branch");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		public void CallPInvoke_Switch ()
		{
			bool a = MessageBeep (5);
			switch (a) {
			case true:
				Console.WriteLine ();
				break;
			case false:
				Marshal.GetLastWin32Error ();
				break;
			}
			return;
		}

		[Test]
		public void TestPInvoke_Switch ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_Switch");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		public void CallPInvoke_Endless ()
		{
			bool a = MessageBeep (5);
		loop:
			Console.WriteLine ();
			goto loop;
		}

		[Test]
		public void TestPInvoke_Endless ()
		{
			MethodDefinition method = GetTest ("CallPInvoke_Endless");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}

		public void TwoPInvokes ()
		{
			MessageBeep (5);
			MessageBeep (5);
			Marshal.GetLastWin32Error ();
		}

		[Test]
		public void TestTwoPInvokes ()
		{
			MethodDefinition method = GetTest ("TwoPInvokes");
			Assert.IsNull (rule.CheckMethod (method, new MinimalRunner ()));
		}
	}
}
