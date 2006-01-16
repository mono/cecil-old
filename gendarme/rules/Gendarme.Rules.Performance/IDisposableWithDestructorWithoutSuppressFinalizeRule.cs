//
// Gendarme.Rules.Performance.IDisposableWithDestructorWithoutSuppressFinalizeRule
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
using System.Collections;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Performance {

	public class IDisposableWithDestructorWithoutSuppressFinalizeRule : ITypeRule {

		private bool MethodMatchNameVoidEmpty (IMethodDefinition md, string methodName)
		{
			if (md.Name != methodName)
				return false;
			if (md.Parameters.Count > 0)
				return false;
			return (md.ReturnType.ReturnType.ToString () == "System.Void");
		}

		private IList Recurse (IMethodDefinition method, int level, Runner runner)
		{
			// some methods have no body (e.g. p/invokes, icalls)
			if (method.Body == null) {
				return runner.RuleFailure;
			}

			foreach (IInstruction ins in method.Body.Instructions) {
				switch (ins.OpCode.Name) {
				case "call":
				case "callvirt":
					// are we calling GC.SuppressFinalize ?
					if (ins.Operand.ToString () == "System.Void System.GC::SuppressFinalize(System.Object)")
						return runner.RuleSuccess;
					else if (level < 3) {
						IMethodDefinition callee = (ins.Operand as IMethodDefinition);
						if (callee != null) {
							if (Recurse (callee, level + 1, runner) == null)
								return runner.RuleSuccess;
						}
					}
					break;
				}
			}
			return runner.RuleFailure;
		}

		public IList CheckType (IAssemblyDefinition assembly, IModuleDefinition module, ITypeDefinition type, Runner runner)
		{
			// #1 - does the type implements System.IDisposable ?
			bool idisposable = false;
			foreach (ITypeReference i in type.Interfaces) {
				if (i.ToString () == "System.IDisposable") {
					idisposable = true;
					break;
				}
			}
			if (!idisposable)
				return runner.RuleSuccess;

			// #2 - look for the Dispose method
			IMethodDefinition dispose = null;
			foreach (IMethodDefinition md in type.Methods) {
				if (MethodMatchNameVoidEmpty (md, "Dispose") || 
					MethodMatchNameVoidEmpty (md, "System.IDisposable.Dispose")) {

					dispose = md;
					break;
				}
			}
			if (dispose == null)
				return runner.RuleSuccess;

			// #3 - look for a destructor
			IMethodDefinition destructor = null;
			foreach (IMethodDefinition md in type.Methods) {
				if (MethodMatchNameVoidEmpty (md, "Finalize")) {
					destructor = md;
					break;
				}
			}
			if (destructor == null)
				return runner.RuleSuccess;

			// #4 - look if GC.SuppressFinalize is being called in the 
			// Dispose method - or one of the method it calls
			return Recurse (dispose, 0, runner);
		}
	}
}
