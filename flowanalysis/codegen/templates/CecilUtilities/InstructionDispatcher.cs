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
using Mono.Cecil.Cil;

namespace Cecil.FlowAnalysis.CecilUtilities {

	public class InstructionDispatcher {

		public static void Dispatch (Instruction instruction, IInstructionVisitor visitor)
		{
			InstructionVisitorDelegate handler = (InstructionVisitorDelegate)_handlers[instruction.OpCode.Value];
			if (null == handler) throw new ArgumentException (CecilFormatter.FormatInstruction (instruction), "instruction");
			handler (visitor, instruction);
		}

		delegate void InstructionVisitorDelegate (IInstructionVisitor visitor, Instruction instruction);

		static IDictionary _handlers = new Hashtable ();

		static InstructionDispatcher ()
		{
<%
	for instr in Instructions:
		opcodes = join("OpCodes.${code}" for code in instr.OpCodes, ", ")
%>			Bind (new InstructionVisitorDelegate (Dispatch${instr.OpCodes[0]}), ${opcodes});
<%
	end
%>		}

		static void Bind (InstructionVisitorDelegate handler, params OpCode[] opcodes)
		{
			foreach (OpCode op in opcodes)
			{
				_handlers.Add (op.Value, handler);
			}
		}
<%
	for instr in Instructions:
%>
		static void Dispatch${instr.OpCodes[0]} (IInstructionVisitor visitor, Instruction instruction)
		{
			visitor.On${instr.OpCodes[0]} (instruction);
		}
<%
	end
%>	}
}

