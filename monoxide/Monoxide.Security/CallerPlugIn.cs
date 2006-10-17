//
// CallerPlugIn.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
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
using Gtk;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Monoxide.Framework.PlugIns;

namespace Monoxide.Security {

	public class CallerPlugIn : IPlugIn {

		static internal Hashtable callsInto = new Hashtable ();
		static internal Hashtable calledFrom = new Hashtable ();

		private IDisplay[] displays;

		public CallerPlugIn ()
		{
			displays = new IDisplay[1];
			displays[0] = new CallerAnalysisView ();
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			foreach (ModuleDefinition module in assembly.Modules) {
				foreach (TypeDefinition type in module.Types) {
					foreach (MethodDefinition md in type.Methods) {
						Process (md);
					}
				}
			}
		}

		public string Name {
			get { return "Security Views"; }
		}

		public IDisplay[] Displays {
			get { return displays; }
		}

		// internal stuff

		private bool Process (MethodDefinition md)
		{
			if (md.Body == null)
				return false;

			foreach (Instruction ins in md.Body.Instructions) {
				MethodDefinition operand = (ins.Operand as MethodDefinition);
				if (operand != null) {
					ProcessCall (md, operand, ins.OpCode.Name);
				}
			}
			return true;
		}

		private void ProcessCall (MethodDefinition caller, MethodDefinition callee, string how)
		{
			// avoid recursion
			if (caller == callee)
				return;

			ArrayList list = (callsInto[caller] as ArrayList);
			if (list == null) {
				list = new ArrayList ();
				callsInto.Add (caller, list);
				list.Add (new Calls (callee, how));
			} else {
				// check if it's already processed
				bool present = false;
				foreach (Calls c in list) {
					if (c.Callee == callee) {
						present = true;
						break;
					}
				}
				if (!present)
					list.Add (new Calls (callee, how));
			}

			list = (calledFrom[callee] as ArrayList);
			if (list == null) {
				list = new ArrayList ();
				calledFrom.Add (callee, list);
				list.Add (new Calls (caller, how));
			} else {
				// check if it's already processed
				bool present = false;
				foreach (Calls c in list) {
					if (c.Callee == caller) {
						present = true;
						break;
					}
				}
				if (!present)
					list.Add (new Calls (caller, how));
			}
		}
	}
}
