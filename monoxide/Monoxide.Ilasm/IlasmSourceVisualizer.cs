//
// IlSourceVisualizer.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005-2007 Novell, Inc (http://www.novell.com)
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

using Gtk;
using Pango;

using Mono.Addins;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Monoxide.Framework.Addins;

namespace Monoxide.Ilasm {

	[Extension ("/Monoxide/Method")]
	internal class IlSourceVisualizer : IMethodVisualizer {

		public string Name {
			get { return "IL Source"; }
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			// nothing to do when a new assembly is loaded
		}

		public Widget GetWidget (MethodDefinition method)
		{
			FontDescription fd = FontDescription.FromString ("Courier 10 Pitch 10");

			TextView ilTextView = new TextView ();
			ilTextView.ModifyFont (fd);
			ilTextView.Editable = false;
			ilTextView.Buffer.Text = GetIlSource (method);

			ScrolledWindow sw = new ScrolledWindow ();
			sw.Add (ilTextView);
			sw.ShowAll ();
			return sw;
		}
		
		// internal stuff

		// shamelessly copied from http://evain.net/public/cecil_il_sample_source.html
		// thanks again Jb!
		private string GetIlSource (MethodDefinition method)
		{
			if (method.Body == null) {
				// it can either be abstract or extern (e.g. icall)
				if (method.IsAbstract)
					return "abstract " + method.ToString ();
				else
					return "extern " + method.ToString ();
			}

			StringBuilder il = new StringBuilder ();
			il.AppendFormat ("{0}{1}{{{1}", method.ToString (), Environment.NewLine);
			il.AppendFormat ("\t// code size : {0}{1}", method.Body.CodeSize, Environment.NewLine);
			il.AppendFormat ("\t.maxstack {0}{1}", method.Body.MaxStack, Environment.NewLine);
			il.Append ("\t.locals (");
			for (int i = 0; i < method.Body.Variables.Count; i++) {
				if (i > 0)
					il.Append (", ");
				VariableDefinition var = method.Body.Variables[i];
				il.Append (string.Concat (var.VariableType.FullName, " ", var.Name));
			}
			il.AppendFormat ("){0}", Environment.NewLine);

			foreach (Instruction instr in method.Body.Instructions) {
				il.AppendFormat ("\tIL_{0}: {1} ", instr.Offset.ToString ("X4"), instr.OpCode.Name);
				switch (instr.OpCode.OperandType) {
				case OperandType.InlineNone:
					break;
				case OperandType.InlineSwitch:
					int[] brchs = instr.Operand as int[];
					for (int i = 0; i < brchs.Length; i++) {
						if (i > 0)
							il.Append (", ");
						il.AppendFormat ("\tIL_{0}", brchs[i].ToString ("X4"));
					}
					il.Append (Environment.NewLine);
					break;
				case OperandType.ShortInlineBrTarget:
				case OperandType.InlineBrTarget:
					Mono.Cecil.Cil.Instruction ins = (instr.Operand as Mono.Cecil.Cil.Instruction);
					il.AppendFormat ("\tIL_{0}", ins.Offset.ToString ("X4"));
					break;
				case OperandType.InlineString:
					il.AppendFormat ("\"{0}\"", instr.Operand);
					break;
				default:
					il.Append (instr.Operand);
					break;
				}
				il.Append (Environment.NewLine);
			}
			il.AppendFormat ("}}{0}", Environment.NewLine);
			return il.ToString ();
		}
	}
}
