//
// Gendarme.Rules.Performance.AvoidUnusedParameters class
//
// Authors:
//	Néstor Salceda <nestor.salceda@gmail.com>
//
//  (C) 2007 Néstor Salceda
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

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Performance {

	public class AvoidUnusedParametersRule : IMethodRule {
	
		private bool UseParameter (MethodDefinition method, ParameterDefinition parameter) 
		{
			foreach (Instruction instruction in method.Body.Instructions) {
				switch (instruction.OpCode.Code) {
					case Code.Ldarg_0:
						if (method.Parameters.IndexOf (parameter) == 0)
							return true;
						break;
					case Code.Ldarg_1:
						if (method.Parameters.IndexOf (parameter) == (method.IsStatic? 1 : 0))
							return true;
						break;
					case Code.Ldarg_2:
						if (method.Parameters.IndexOf (parameter) == (method.IsStatic? 2 : 1))
							return true;
						break;
					case Code.Ldarg_3:
						if (method.Parameters.IndexOf (parameter) == (method.IsStatic? 3 : 2))
							return true;
						break;
					case Code.Ldarg_S:
						if (instruction.Operand.Equals (parameter))
							return true;
						break;
					default:
						break;
				}
			}
			return false;
		}

		private bool ContainsReferenceDelegateInstructionFor (MethodDefinition method, MethodDefinition delegateMethod) 
		{
			if (method.HasBody) {
				foreach (Instruction instruction in method.Body.Instructions) {
					if (instruction.OpCode.Code == Code.Ldftn) 
						return instruction.Operand.Equals (delegateMethod);
				}
			}
			return false;
		}
		
		private bool IsReferencedByDelegate (MethodDefinition delegateMethod) 
		{
			if (delegateMethod.DeclaringType is TypeDefinition) {
				TypeDefinition type = (TypeDefinition) delegateMethod.DeclaringType;	
				foreach (MethodDefinition method in type.Methods) {
					if (ContainsReferenceDelegateInstructionFor (method, delegateMethod)) 
						return true;
				}

				foreach (MethodDefinition method in type.Constructors) {
					if (ContainsReferenceDelegateInstructionFor (method, delegateMethod))
						return true;
				}
			}
			return false;
		}

		private bool IsExaminable (MethodDefinition method) 
		{
			return !(method.IsAbstract || method.IsVirtual || method.Overrides.Count != 0 
				|| method.PInvokeInfo != null || IsReferencedByDelegate (method));
		}

		private ICollection GetUnusedParameters (MethodDefinition method) 
		{
			ArrayList unusedParameters = new ArrayList ();
			if (IsExaminable (method)) {
				foreach (ParameterDefinition parameter in method.Parameters) {
					if (!UseParameter (method, parameter))
						unusedParameters.Add (parameter);
				}
			}
			return unusedParameters;
		}
	
		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection ();
			foreach (ParameterDefinition parameter in GetUnusedParameters (method)) {
				Location location = new Location (method.DeclaringType.Name, method.Name, 0);
				Message message = new Message (String.Format ("The parameter {0} is never used.", parameter.Name),location, MessageType.Error);
				messageCollection.Add (message);
			}
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}
