// Gendarme.Rules.Performance.AvoidToStringOnStringsRule
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

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;

namespace Gendarme.Rules.Performance
{
	public class AvoidToStringOnStringsRule : IMethodRule
	{

		private const string MessageString = "No need to call ToString on a System.String instance";

		#region IMethodRule Members

		public MessageCollection CheckMethod(MethodDefinition method, Runner runner)
		{
			// rule apply only if the method has a body (e.g. p/invokes, icalls don't)
			if (!method.HasBody)
				return runner.RuleSuccess;

			MessageCollection messageCollection = new MessageCollection();

			foreach (Instruction instruction in method.Body.Instructions)
			{
				if (instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt)
				{
					MemberReference member = instruction.Operand as MemberReference;
					if (member.Name.Equals("ToString"))
					{
						CheckStack(instruction.Previous, method, messageCollection);
					}
				}
			}


			return messageCollection.Count == 0 ? runner.RuleSuccess : messageCollection;
		}

		private void CheckStack(Instruction instruction, MethodDefinition method, MessageCollection messageCollection)
		{

			switch (instruction.OpCode.Code)
			{

				case Code.Ldloc_0:
					CheckTypeReference(method.Body.Variables[0].VariableType, method, instruction, messageCollection);
					break;
				case Code.Ldloc_1:
					CheckTypeReference(method.Body.Variables[1].VariableType, method, instruction, messageCollection);
					break;
				case Code.Ldloc_2:
					CheckTypeReference(method.Body.Variables[2].VariableType, method, instruction, messageCollection);
					break;
				case Code.Ldloc_3:
					CheckTypeReference(method.Body.Variables[3].VariableType, method, instruction, messageCollection);
					break;
				case Code.Ldloc_S:
					VariableReference local = instruction.Operand as VariableReference;
					CheckTypeReference(local.VariableType, method, instruction, messageCollection);
					break;
				case Code.Ldarg_1:
					CheckTypeReference(method.Parameters[0].ParameterType, method, instruction, messageCollection);
					break;
				case Code.Ldarg_2:
					CheckTypeReference(method.Parameters[1].ParameterType, method, instruction, messageCollection);
					break;
				case Code.Ldarg_3:
					CheckTypeReference(method.Parameters[2].ParameterType, method, instruction, messageCollection);
					break;
				case Code.Ldarg:
					ParameterReference parameter = instruction.Operand as ParameterReference;
					CheckTypeReference(parameter.ParameterType, method, instruction, messageCollection);
					break;
				case Code.Call:
				case Code.Calli:
				case Code.Callvirt:
					MethodReference call = instruction.Operand as MethodReference;
					CheckTypeReference(call.ReturnType.ReturnType, method, instruction, messageCollection);
					break;
				case Code.Ldfld:
				case Code.Ldsfld:
					FieldReference field = instruction.Operand as FieldReference;
					CheckTypeReference(field.FieldType, method, instruction, messageCollection);
					break;
			}

		}

		private void AddProblem(MethodDefinition method, Instruction instruction, MessageCollection messageCollection)
		{
			Location location = new Location(method.DeclaringType.Name, method.Name, instruction.Offset);
			Message message = new Message(MessageString, location, MessageType.Error);
			messageCollection.Add(message);
		}

		private void CheckTypeReference(TypeReference type, MethodDefinition method, Instruction instruction, MessageCollection messageCollection)
		{
			if (type.FullName.Equals("System.String"))
			{
				AddProblem(method, instruction, messageCollection);
			}

		}

		#endregion
	}
}



