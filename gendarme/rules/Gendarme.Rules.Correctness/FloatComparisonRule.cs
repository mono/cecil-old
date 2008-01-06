//
// Gendarme.Rules.Correctness.FloatComparisonRule
//
// Authors:
//	Lukasz Knop <lukasz.knop@gmail.com>
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2007 Lukasz Knop
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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

namespace Gendarme.Rules.Correctness {

	public class FloatComparisonRule : IMethodRule {

		private const string MessageString = "Floating point values should not be compared for equality";
		private MessageCollection messageCollection = null;

		private void CheckCeqInstruction (Instruction instruction, Instruction precedingInstruction, MethodDefinition method) 
		{
			bool problem = false;
			switch (precedingInstruction.OpCode.Code) {
			case Code.Conv_R_Un:
			case Code.Conv_R4:
			case Code.Conv_R8:
				problem = true;
				break;
			case Code.Ldc_R4:
				problem = !CheckFloatConstants ((float) precedingInstruction.Operand);
				break;
			case Code.Ldc_R8:
				problem = !CheckDoubleConstants ((double) precedingInstruction.Operand);
				break;
			case Code.Ldelem_R4:
			case Code.Ldelem_R8:
				problem = true;
				break;
			case Code.Ldloc_0:
			case Code.Ldloc_1:
			case Code.Ldloc_2:
			case Code.Ldloc_3:
				int loc_index = (int) (precedingInstruction.OpCode.Code - Code.Ldloc_0);
				problem = IsFloatingPoint (method.Body.Variables [loc_index].VariableType);
				break;
			case Code.Ldloc_S:
				VariableReference local = precedingInstruction.Operand as VariableReference;
				problem = IsFloatingPoint (local.VariableType);
				break;
			case Code.Ldarg_0:
			case Code.Ldarg_1:
			case Code.Ldarg_2:
			case Code.Ldarg_3:
				int arg_index = (int) (precedingInstruction.OpCode.Code - Code.Ldarg_0);
				if (!method.IsStatic)
					arg_index--;
				problem = IsFloatingPoint (method.Parameters [arg_index].ParameterType);
				break;
			case Code.Ldarg:
				ParameterReference parameter = precedingInstruction.Operand as ParameterReference;
				problem = IsFloatingPoint (parameter.ParameterType);
				break;
			case Code.Call:
			case Code.Calli:
			case Code.Callvirt:
				MethodReference call = precedingInstruction.Operand as MethodReference;
				problem = IsFloatingPoint (call.ReturnType.ReturnType);
				break;
			case Code.Ldfld:
			case Code.Ldsfld:
				FieldReference field = precedingInstruction.Operand as FieldReference;
				problem = IsFloatingPoint (field.FieldType);
				break;
			}
			if (problem)
				AddProblem (method, instruction);
		}

		static bool IsFloatingPoint (TypeReference type)
		{
			return ((type.FullName == Mono.Cecil.Constants.Single) ||
				(type.FullName == Mono.Cecil.Constants.Double));
		}

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			//For the rule's lifecycle I should initializate the
			//field to null.
			messageCollection = null;

			if (!method.HasBody)
				return runner.RuleSuccess;

			foreach (Instruction instruction in method.Body.Instructions) {
				switch (instruction.OpCode.Code) {
				case Code.Ceq:
					CheckCeqInstruction (instruction, SkipArithmeticOperations (instruction), method); 
					break;
				case Code.Call:
				case Code.Calli:
				case Code.Callvirt:
					MemberReference member = instruction.Operand as MemberReference;
					if (IsFloatingPoint (member.DeclaringType) && member.Name.Equals ("Equals"))
						AddProblem(method, instruction);
					break;
				}
			}

			return messageCollection == null || messageCollection.Count == 0 ? runner.RuleSuccess : messageCollection;
		}

		static OpCode [] arithOpCodes = new OpCode [] {
			OpCodes.Mul,
			OpCodes.Add,
			OpCodes.Sub,
			OpCodes.Div
		};

		private static Instruction SkipArithmeticOperations (Instruction instruction)
		{
			Instruction prevInstr = instruction.Previous;

			while (Array.Exists (arithOpCodes, 
				delegate (OpCode code) {
					return code == prevInstr.OpCode;
				})) {
				prevInstr = prevInstr.Previous;
			}

			return prevInstr;
		}

		private void AddProblem (MethodDefinition method, Instruction instruction)
		{
			if (messageCollection == null)
				messageCollection = new MessageCollection ();

			Location location = new Location(method, instruction.Offset);
			Message message = new Message(MessageString, location, MessageType.Error);
			messageCollection.Add (message);
		}

		private static bool CheckFloatConstants (float value)
		{
			// IsInfinity covers both positive and negative infinity
			return (Single.IsInfinity (value) || Single.IsNaN (value) ||
				(Single.MinValue.CompareTo (value) == 0) ||
				(Single.MaxValue.CompareTo (value) == 0));
		}

		private static bool CheckDoubleConstants (double value)
		{
			// IsInfinity covers both positive and negative infinity
			return (Double.IsInfinity (value) || Double.IsNaN (value) ||
				(Double.MinValue.CompareTo (value) == 0) || 
				(Double.MaxValue.CompareTo (value) == 0));
		}
	}
}
