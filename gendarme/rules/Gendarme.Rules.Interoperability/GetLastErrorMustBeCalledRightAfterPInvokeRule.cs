//
// Gendarme.Rules.Interoperability.GetLastErrorMustBeCalledRightAfterPInvokeRule
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
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Interoperability {

	public class GetLastErrorMustBeCalledRightAfterPInvokeRule : IMethodRule {

		private const string GetLastError = "System.Int32 System.Runtime.InteropServices.Marshal::GetLastWin32Error()";
		private List<string> AllowedCalls;

		public GetLastErrorMustBeCalledRightAfterPInvokeRule ()
		{
			AllowedCalls = new List<string> ();
			AllowedCalls.Add ("System.Boolean System.Runtime.InteropServices.SafeHandle::get_IsInvalid()");
			AllowedCalls.Add ("System.Boolean System.IntPtr::op_Inequality(System.IntPtr,System.IntPtr)");
			AllowedCalls.Add ("System.Boolean System.IntPtr::op_Equality(System.IntPtr,System.IntPtr)");
		}

		private static void EnsureExists (ref MessageCollection msg)
		{
			if (msg == null)
				msg = new MessageCollection ();
		}

		private static Instruction GetNextCall (Instruction ins)
		{
			while ((ins = ins.Next) != null) {
				switch (ins.OpCode.Code) {
				case Code.Call:
				case Code.Calli:
				case Code.Callvirt:
				case Code.Newobj:
					return ins;
				default:
					break;
				}
			}
			return null;
		}

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			if (!method.HasBody)
				return runner.RuleSuccess;

			MessageCollection results = null;
			string fullname = method.DeclaringType.FullName;

			foreach (Instruction ins in method.Body.Instructions) {

				switch (ins.OpCode.Code) {
				case Code.Call:
				case Code.Calli:
				case Code.Callvirt:

					MethodDefinition pinvoke = ins.Operand as MethodDefinition;
					if (pinvoke == null) //FIXME: calls to external PInvoke methods are ignored.
						break;

					if (!pinvoke.IsPInvokeImpl)
						break;

					bool allClean = true;
					Instruction nextCallIns = ins;

					while ((nextCallIns = GetNextCall (nextCallIns)) != null) { //find GetLastError()
						string calledMethod = nextCallIns.Operand.ToString ();

						if (calledMethod == GetLastError)
							break; //found						
						if (AllowedCalls.Contains (calledMethod))
							continue; //allowed method
						allClean = false; //wrong method. still searching for GetLastError()
					}
					if (nextCallIns == null) //We did not find GetLastError()
						break;

					if (allClean)
						break;

					EnsureExists (ref results);
					Location loc = new Location (fullname, method.Name, ins.Offset);
					Message msg = new Message ("GetLastError() should be called immediately after this the PInvoke call.", loc, MessageType.Error);
					results.Add (msg);
					break;
				default:
					break;
				}
			}

			return results;
		}
	}
}
