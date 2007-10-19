
// Gendarme.Rules.Correctness.UseValueInPropertySetterRule 
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


namespace Gendarme.Rules.Correctness
{
	public class UseValueInPropertySetterRule : IMethodRule
	{
		private const string MessageString = "Property setter should use the assigned value";
		
			
		public MessageCollection CheckMethod(MethodDefinition method, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection();

			if ((method.Attributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName && method.Name.StartsWith("set_"))
			{
				bool valueAccessed = false;

				//Skip the test, instead of flooding messages 
				//in stubs or empty setters.
				if (!method.HasBody) 
					return null;

				foreach (Instruction instruction in method.Body.Instructions)
				{
					if (instruction.OpCode.Code == Code.Ldarg_1)
					{
						valueAccessed = true;
					}
				}
				if (!valueAccessed)
				{
					Location location = new Location(method.DeclaringType.Name, method.Name, 0);
					Message message = new Message(MessageString, location, MessageType.Error);
					messageCollection.Add(message);
				}
			}
			
			return messageCollection.Count > 0 ? messageCollection : null;
		}

		
	}
}
