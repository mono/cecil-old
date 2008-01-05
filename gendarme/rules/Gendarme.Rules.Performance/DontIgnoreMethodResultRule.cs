//
// Gendarme.Rules.Performance.DontIgnoreMethodResultRule
//
// Authors:
//	Lukasz Knop <lukasz.knop@gmail.com>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2007 Lukasz Knop
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
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
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Performance {

	public class DontIgnoreMethodResultRule : IMethodRule {

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			// rule only applies if the method has a body
			if (!method.HasBody)
				return runner.RuleSuccess;

			MessageCollection mc = null;
			foreach (Instruction instruction in method.Body.Instructions) {
				if (instruction.OpCode.Code == Code.Pop) {
					Message message = CheckForViolation(instruction.Previous);
					if (message != null) {
						if (mc == null)
							mc = new MessageCollection (message);
						else
							mc.Add (message);
					}
				}
			}
			return mc;
		}

		// some method return stuff that isn't required in most cases
		// the rule ignores them
		private static bool IsException (MethodReference method)
		{
			if (method == null)
				return true;

			switch (method.DeclaringType.FullName) {
			case "System.IO.Directory":
				// the returned DirectoryInfo is optional (since the directory is created)
				return (method.Name == "CreateDirectory");
			case "System.Text.StringBuilder":
				// StringBuilder Append* methods return the StringBuilder itself so
				// the calls can be chained
				return method.Name.StartsWith ("Append");
			case "System.Security.PermissionSet":
				// PermissionSet return the permission (modified or unchanged) when
				// IPermission are added or removed
				return (method.Name == "AddPermission" || method.Name == "RemovePermission");
			default:
				return false;
			}
		}

		private static Message CheckForViolation (Instruction instruction)
		{
			if ((instruction.OpCode.Code == Code.Newobj || instruction.OpCode.Code == Code.Newarr))
				return new Message("Unused object created", null, MessageType.Warning);

			if (instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt) {
				MethodReference method = instruction.Operand as MethodReference;
				if (method != null && !method.ReturnType.ReturnType.IsValueType) {
					// check for some common exceptions (to reduce false positive)
					if (!IsException (method)) {
						Location loc = new Location (method.DeclaringType.Name, method.Name, instruction.Offset);
						// most common case is something like: s.ToLower ();
						MessageType messageType = (method.DeclaringType.FullName == "System.String") ? MessageType.Error : MessageType.Warning;
						return new Message ("Do not ignore method results", loc, messageType);
					}
				}
			}
			return null;
		}
	}
}
