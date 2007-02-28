//
// Gendarme.Rules.Correctness.MethodCanBeMadeStaticRule
//
// Authors:
//	Jb Evain <jbevain@gmail.com>
//
// Copyright (C) 2007 Jb Evain
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

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;

namespace Gendarme.Rules.Correctness {

	public class MethodCanBeMadeStaticRule : IMethodRule {

		public MessageCollection CheckMethod (AssemblyDefinition assembly, ModuleDefinition module, TypeDefinition type, MethodDefinition method, Runner runner)
		{
			// we only check non static and non virtual methods
			if (method.IsStatic || method.IsVirtual)
				return runner.RuleSuccess;

			// we only check methods with a body
			if (!method.HasBody)
				return runner.RuleSuccess;

			// if we find a use of the "this" reference, it's ok
			foreach (Instruction instr in method.Body.Instructions)
				if (instr.OpCode == OpCodes.Ldarg_0 ||
					(instr.OpCode == OpCodes.Ldarg && (int) instr.Operand == 0))
					return runner.RuleSuccess;

			return runner.RuleFailure;
		}
	}
}
