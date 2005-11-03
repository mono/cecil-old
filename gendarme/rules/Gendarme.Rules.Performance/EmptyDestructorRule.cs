//
// Gendarme.Rules.Performance.EmptyDestructorRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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

	public class EmptyDestructorRule : ITypeRule {

		public bool CheckType (IAssemblyDefinition assembly, IModuleDefinition module, ITypeDefinition type)
		{
			IMethodDefinition destructor = null;
			// #1 - look for a destructor
			foreach (IMethodDefinition md in type.Methods) {
				if (md.Name == "Finalize") {
					destructor = md;
					break;
				}
			}
			if (destructor == null)
				return true;

			// #2 - destructor is present, look if it has any code within it
			// i.e. look if is does anything else than calling it's base class
			foreach (IInstruction ins in destructor.Body.Instructions) {
				switch (ins.OpCode.Name) {
				case "call":
					// it's empty if we're calling the base class destructor
					IMethodReference mr = (ins.Operand as IMethodReference);
					if ((mr == null) || (mr.Name != "Finalize"))
						return true;
					break;
				case "nop":
				case "leave.s":
				case "ldarg.0":
				case "endfinally":
				case "ret":
					// ignore
					break;
				default:
					// destructor isn't empty (normal)
					return true;
				}
			}
			// destructor is empty (bad / useless)
			return false;
		}
	}
}
