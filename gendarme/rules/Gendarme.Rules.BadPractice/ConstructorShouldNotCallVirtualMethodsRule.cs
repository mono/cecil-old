// 
// Gendarme.Rules.BadPractice.ConstructorShouldNotCallVirtualMethodsRule
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//
// Copyright (C) 2008 Daniel Abramov
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
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.BadPractice {

	public class ConstructorShouldNotCallVirtualMethodsRule : ITypeRule {

		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner)
		{
			// sealed classes are ok
			if (typeDefinition.IsSealed)
				return runner.RuleSuccess;

			// no constructors - ok
			if (typeDefinition.Constructors.Count == 0)
				return runner.RuleSuccess;

			MessageCollection messages = runner.RuleSuccess;
			// check each constructor
			foreach (MethodDefinition constructor in typeDefinition.Constructors) {
				CheckConstructor (constructor, ref messages);
			}
			return messages;
		}

		private static void CheckConstructor (MethodDefinition constructor, ref MessageCollection messages)
		{
			if (!constructor.HasBody)
				return;

			// check constructor for virtual method calls
			foreach (Instruction current in constructor.Body.Instructions) {
				switch (current.OpCode.Code) {
				// not sure what instruction will be used
				// so better check them all
				case Code.Call:
				case Code.Calli:
				case Code.Callvirt:
					MethodDefinition md = (current.Operand as MethodDefinition);
					if (md != null && md.HasThis && md.IsVirtual) {
						Location loc = new Location (constructor, current.Offset);
						Message msg = new Message ("Calling a virtual method from the constructor of a non-sealed class is a bad practice.", loc, MessageType.Error);
						if (messages == null)
							messages = new MessageCollection (msg);
						else
							messages.Add (msg);
					}
					break;
				}
			}
		}
	}
}
