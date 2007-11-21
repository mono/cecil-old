//
// Gendarme.Rules.Performance.AvoidUninstantiatedInternalClassesRule
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

namespace Gendarme.Rules.Performance
{
	public class AvoidUninstantiatedInternalClassesRule: ITypeRule
	{
		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			// rule apply to non-public types
			if (type.IsPublic)
				return runner.RuleSuccess;

			// rule doesn't apply if the assembly open up itself to others using [InternalsVisibleTo]
			if (ImplementsInternalsVisibleToAttribute (type))
				return runner.RuleSuccess;

			if (IsInstantiable (type) && !IsInstantiated (type, type)) {
				Location location = new Location (type.FullName, type.Name, 0);
				Message message = new Message ("There is no call for any of the types constructor found", location, MessageType.Error);
				return new MessageCollection (message);
			}

			return runner.RuleSuccess;
		}

		// FIXME: we should have a common method to check attributes
		private bool ImplementsInternalsVisibleToAttribute (TypeDefinition type)
		{
			foreach (CustomAttribute ca in type.Module.Assembly.CustomAttributes) {
				if (ca.Constructor.DeclaringType.FullName == "System.Runtime.CompilerServices.InternalsVisibleToAttribute")
					return true;
			}
			return false;
		}

		private bool IsInstantiable (TypeDefinition type)
		{
			// note: that also excludes static types (2.0)
			return !type.IsAbstract;
		}

		private bool IsInstantiated (TypeDefinition type, TypeDefinition nestedType)
		{
			bool instantiated = false;

			foreach (MethodDefinition method in nestedType.Methods)
				foreach (Instruction ins in method.Body.Instructions)
					if (ins.OpCode == OpCodes.Newobj) {
						string [] typeName = ins.Operand.ToString ().Split (':');
						if (("System.Void "+ type.FullName) == typeName [0])
							if (MethodIsCalled (nestedType, method))
								instantiated = true;
					}

			if (nestedType.NestedTypes.Count > 0)
				foreach (TypeDefinition nested in type.NestedTypes)
					instantiated = IsInstantiated (type, nested);
			return instantiated;
		}

		private bool MethodIsCalled (TypeDefinition type, MethodDefinition callingMethod)
		{
			string strCallingMethod = String.Empty;

			if (callingMethod.Name == "Main" && callingMethod.IsStatic && callingMethod.ReturnType.ReturnType.FullName == "System.Void" && callingMethod.Parameters.Count == 1 && callingMethod.Parameters [0].ParameterType.FullName == "System.String[]")
				return true;
			else
			{
				strCallingMethod = callingMethod.ReturnType.ReturnType.FullName + " " + callingMethod.DeclaringType + "::" + callingMethod.Name + "(";
				foreach (ParameterDefinition parameter in callingMethod.Parameters)
					strCallingMethod += parameter.ParameterType.FullName + ",";

				strCallingMethod = strCallingMethod.Remove (strCallingMethod.Length-1, 1) + ")";

				foreach (MethodDefinition method in type.Methods)
					foreach (Instruction ins in method.Body.Instructions)
						if (ins.OpCode == OpCodes.Call || ins.OpCode == OpCodes.Calli || ins.OpCode == OpCodes.Callvirt)
							if (strCallingMethod == ins.Operand.ToString ())
								return true;
			}
			return false;
		}
	}
}
