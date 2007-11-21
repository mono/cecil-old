//
// Gendarme.Rules.Concurrency.WriteStaticFieldFromInstanceMethodRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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
using System.Globalization;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Concurrency {

	public class WriteStaticFieldFromInstanceMethodRule : IMethodRule {

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			// #1 - rule apply only if the method has a body (e.g. p/invokes, icalls don't)
			if (method.Body == null)
				return runner.RuleSuccess;

			// #2 - rule apply only is the method isn't static (static method can modify static fields)
			if (method.IsStatic)
				return runner.RuleSuccess;

			// *** ok, the rule applies! ***

			MessageCollection results = new MessageCollection ();
			string fullname = method.DeclaringType.FullName;

			// #2 - look for stsfld instructions on static fields
			foreach (Instruction ins in method.Body.Instructions) {
				switch (ins.OpCode.Name) {
				case "stsfld":
					FieldDefinition fd = (ins.Operand as FieldDefinition);
					if ((fd != null) && fd.IsStatic) {
						Location loc = new Location (fullname, method.Name, ins.Offset);
						string text = String.Format ("The field '{0}' ({1}) is being set in an instance method.", fd.Name, fd.Attributes);
						Message msg = new Message (text, loc, MessageType.Warning);
						results.Add (msg);
					}
					break;
				}
			}

			if (results.Count == 0)
				return runner.RuleSuccess;
			return results;
		}
	}
}
