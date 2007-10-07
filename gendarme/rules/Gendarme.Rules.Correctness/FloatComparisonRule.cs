//
// Gendarme.Rules.Correctness.FloatComparisonRule
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
	public class FloatComparisonRule : IMethodRule
	{
		private const string MessageString = "Floating point values should not be compared for equality";

		public MessageCollection CheckMethod(MethodDefinition method, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection();
			
			foreach (Instruction instruction in method.Body.Instructions)
			{
				if (instruction.OpCode == OpCodes.Ceq)
				{
					Instruction precedingInstruction = SkipArithmeticOperations(instruction);
					
					switch (precedingInstruction.OpCode.Code)
					{
						
						case Code.Conv_R4:
						case Code.Conv_R8:
							AddProblem(method, instruction, messageCollection);
							break;
						case Code.Ldc_R4:
						case Code.Ldc_R8:
							CheckFloatConstants(precedingInstruction.Operand, method, instruction, messageCollection);
							break;
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
							VariableReference local = precedingInstruction.Operand as VariableReference;
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
							ParameterReference parameter = precedingInstruction.Operand as ParameterReference;
							CheckTypeReference(parameter.ParameterType, method, instruction, messageCollection);
							break;
						case Code.Call:
						case Code.Calli:
						case Code.Callvirt:
							MethodReference call = precedingInstruction.Operand as MethodReference;
							CheckTypeReference(call.ReturnType.ReturnType, method, instruction, messageCollection);
							break;
						case Code.Ldfld:
						case Code.Ldsfld:
							FieldReference field = precedingInstruction.Operand as FieldReference;
							CheckTypeReference(field.FieldType, method, instruction, messageCollection);
							break;
					}
				}
				else if (instruction.OpCode.Code == Code.Call)
				{
					MemberReference member = instruction.Operand as MemberReference;
					
					if (member.DeclaringType.FullName.Equals("System.Single") &&
						member.Name.Equals("Equals"))
					{
						AddProblem(method, instruction, messageCollection);
					}
				}
			}

			return messageCollection.Count == 0 ? runner.RuleSuccess : messageCollection;
		}

		private Instruction SkipArithmeticOperations(Instruction instruction)
		{
			Instruction prevInstr = instruction.Previous;
			OpCode[] arithOpCodes = new OpCode[] 
				{	
					OpCodes.Mul, 
					OpCodes.Add, 
					OpCodes.Sub, 
					OpCodes.Div 
				};

			while (Array.Exists(arithOpCodes, delegate(OpCode code) { return code == prevInstr.OpCode; }))
			{
				prevInstr = prevInstr.Previous;
			}
			return prevInstr;
		}

		private void AddProblem(MethodDefinition method, Instruction instruction, MessageCollection messageCollection)
		{
			Location location = new Location(method.DeclaringType.Name, method.Name, instruction.Offset);
			Message message = new Message(MessageString, location, MessageType.Error);
			messageCollection.Add(message);
		}

		private void CheckTypeReference(TypeReference type, MethodDefinition method, Instruction instruction, MessageCollection messageCollection)
		{
			if (type.FullName.Equals("System.Single") || type.FullName.Equals("System.Double"))
			{
				AddProblem(method, instruction, messageCollection);
			}

		}

		private void CheckFloatConstants(object operand, MethodDefinition method, Instruction instruction, MessageCollection messageCollection)
		{
			object[] specialValues = new object[] 
			{
				float.PositiveInfinity,
				float.NegativeInfinity,
				float.MinValue,
				float.MaxValue,
				double.PositiveInfinity,
				double.NegativeInfinity,
				double.MinValue,
				double.MaxValue
			};

			if (!Array.Exists(specialValues, delegate(object value) { return value.Equals(operand); }))
			{
				AddProblem(method, instruction, messageCollection);
			}
		}

	}
}
