//
// Gendarme.Rules.BadPractice.EqualShouldHandleNullArgRule
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
using System.Collections;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.BadPractice {

	public class EqualShouldHandleNullArgRule: IMethodRule {

		// copy-paste from gendarme\rules\Gendarme.Rules.Correctness\CallingEqualsWithNullArgRule.cs
		private static bool IsEquals (MethodReference md)
		{
			if ((md == null) || (md.Name != "Equals"))
				return false;

			return (md.ReturnType.ReturnType.FullName == "System.Boolean");
		}

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			// rule applies only if a body is available (e.g. not for pinvokes...)
			if (!method.HasBody)
				return runner.RuleSuccess;

			// rules applies for Equals overrides
			if (!IsEquals (method) || !method.IsVirtual)
				return runner.RuleSuccess;

			foreach (ParameterDefinition param in method.Parameters) {
				if (param.ParameterType.FullName == "System.Object") {
					if (!HandlesNullArg (method)) {
						Location location = new Location (method.DeclaringType.FullName, method.Name, 0);
						Message message = new Message ("The overridden method Object.Equals (Object) does not return false if null value is found", location, MessageType.Error);
						return new MessageCollection (message);
					}
				}
			}
			return runner.RuleSuccess;
		}

		// FIXME: this logic seems to work only on mono generated assemblies
		// and can fail on VS.NET (e.g. debug)
		private static bool HandlesNullArg (MethodDefinition method)
		{
			Instruction prevIns;
			foreach (Instruction ins in method.Body.Instructions) {
				prevIns = ins.Previous;
				if (ins.OpCode == OpCodes.Ret && prevIns.OpCode == OpCodes.Ldc_I4_0) {
					// added check for case where Equals simpli returns true (or false)
					if (prevIns.Previous == null)
						return false;
					else if (prevIns.Previous.OpCode == OpCodes.Brtrue)
						return true;
				}
			}
			return false;
		}
	}
}
