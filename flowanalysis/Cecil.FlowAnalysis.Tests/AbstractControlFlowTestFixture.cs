#region license
//
// (C) db4objects Inc. http://www.db4o.com
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
#endregion

using System;
using System.Collections;
using System.IO;
using Cecil.FlowAnalysis.CecilUtilities;
using Cecil.FlowAnalysis.ControlFlow;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Cecil.FlowAnalysis.Tests {
	public class AbstractControlFlowTestFixture : AbstractFlowAnalysisTestFixture {
		protected void RunTestCase (string name)
		{
			MethodDefinition method = LoadTestCaseMethod (name);
			IControlFlowGraph cfg = FlowGraphFactory.CreateControlFlowGraph (method);
			Assert.AreEqual (normalize (LoadExpectedControlFlowString (name)), normalize (ToString (cfg)));
		}

		public static string ToString (IControlFlowGraph cfg)
		{
			StringWriter writer = new StringWriter ();
			FormatControlFlowGraph (writer, cfg);
			return writer.ToString ();
		}

		public static void FormatControlFlowGraph (TextWriter writer, IControlFlowGraph cfg)
		{
			int id = 1;
			foreach (IInstructionBlock block in cfg.Blocks) {
				writer.WriteLine ("block {0}:", id);
				writer.WriteLine ("\tbody:");
				foreach (Instruction instruction in block) {
					writer.Write ("\t\t");
					CecilFormatter.WriteInstruction (writer, instruction);
					writer.WriteLine ();
				}
				IInstructionBlock[] successors = block.Successors;
				if (successors.Length > 0) {
					writer.WriteLine ("\tsuccessors:");
					foreach (IInstructionBlock successor in successors) {
						writer.WriteLine ("\t\tblock {0}", GetBlockId (cfg, successor));
					}
				}

				++id;
			}
		}

		private static int GetBlockId (IControlFlowGraph cfg, IInstructionBlock block)
		{
			return ((IList)cfg.Blocks).IndexOf (block) + 1;
		}

		private string LoadExpectedControlFlowString (string name)
		{
			return LoadTestCaseFile (name + "-cfg.txt");
		}
	}
}
