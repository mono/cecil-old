//
// Gendarme.Rules.BadPractice.CheckNewExceptionWithoutThrowingRule
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Gendarme.Rules.BadPractice {

	public class CheckNewExceptionWithoutThrowingRule : IMethodRule {

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			if (!method.HasBody)
				return runner.RuleSuccess;

			MessageCollection results = null;

			StackEntryAnalysis sea = null;

			foreach (Instruction ins in method.Body.Instructions) {
				if (ins.OpCode.Code != Code.Newobj)
					continue;

				MethodReference constructor = (MethodReference) ins.Operand;

				//if (!constructor.DeclaringType.Inherits ("System.Exception")) //Not ready yet, assembly resolver is missing
				if (!constructor.DeclaringType.Name.EndsWith ("Exception"))
					continue;
				if (ins.Next != null && ins.Next.OpCode.Code == Code.Throw) //quick check to safe resources
					continue;

				if (sea == null)
					sea = new StackEntryAnalysis (method);

				StackEntryAnalysis.UsageResult [] usageResults = sea.GetStackEntryUsage (ins);

				bool exceptionUsed = false;
				foreach (var usage in usageResults) {
					switch (usage.Instruction.OpCode.Code) {
					case Code.Throw: //throw
					case Code.Ret: //return
					case Code.Stind_I: //out / ref
					case Code.Stind_I1:
					case Code.Stind_I2:
					case Code.Stind_I4:
					case Code.Stind_I8:
					case Code.Stind_R4:
					case Code.Stind_R8:
					case Code.Stind_Ref:
						//case Code.Stfld:
						exceptionUsed = true;
						break;
					case Code.Call: //call (to the exception or as an argument)
					case Code.Calli:
					case Code.Callvirt:
					case Code.Newobj:
					case Code.Initobj: {
							IMethodSignature calledMethod = (IMethodSignature) usage.Instruction.Operand;
							if (calledMethod.Parameters.Count <= usage.StackOffset)
								break; //not used as a parameter
							exceptionUsed = true;
							break;
						}
					}
					if (exceptionUsed)
						break;
				}

				if (!exceptionUsed) {
					if (results == null)
						results = new MessageCollection ();
					Location loc = new Location (method, ins.Offset);
					Message msg = new Message ("This exception is not thrown, passed as an argument or returned by this method.", loc, MessageType.Warning);
					results.Add (msg);
				}
			}

			return results;
		}
	}
}