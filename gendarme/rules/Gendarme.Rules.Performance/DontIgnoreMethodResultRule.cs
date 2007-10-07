// Gendarme.Rules.Performance.DontIgnoreMethodResultRule
//
// Authors:
//	Lukasz Knop <lukasz.knop@gmail.com>
//
// Copyright (C) 2007 Lukasz Knop
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
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Performance
{
	public class DontIgnoreMethodResultRule : IMethodRule
	{
		
		public MessageCollection CheckMethod(MethodDefinition method, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection();

			if (method.Body == null || method.Body.Instructions == null)
			{
				return null;
			}

			foreach (Instruction instruction in method.Body.Instructions)
			{
				if (instruction.OpCode.Code == Code.Pop)
				{
					Message message = CheckForViolation(instruction.Previous);

					if (message != null)
					{
						messageCollection.Add(message);
					}
				}
			}
			

			return messageCollection.Count == 0 ? runner.RuleSuccess : messageCollection;
		}

		private Message CheckForViolation(Instruction instruction)
		{
			MessageType messageType;
			Message message = null;

			if (instruction.OpCode.Code == Code.Newobj || instruction.OpCode.Code == Code.Newarr)
			{
				messageType = MessageType.Warning;
				message = new Message("Unused object created", null, messageType);
			}
			else if (instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt)
			{
				MethodReference method = instruction.Operand as MethodReference;

				if (method != null && !method.ReturnType.ReturnType.IsValueType)
				{
					if (method.DeclaringType.FullName.Equals("System.String"))
					{
						messageType = MessageType.Error;
					}
					else 
					{
						messageType = MessageType.Warning;
					}
					
					if (!(method.Name.Equals("Append") && method.DeclaringType.FullName.Equals("System.Text.StringBuilder")))
					{
						message = new Message("Do not ignore method results", null, messageType);
					}
				}
			}
			return message;
		}

	}
}
