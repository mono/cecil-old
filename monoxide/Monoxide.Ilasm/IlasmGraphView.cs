//
// IlasmGraphView.cs
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
using System.IO;
using System.Collections;
using System.Text;

using Gtk;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Monoxide.Framework.PlugIns;
using Monoxide.Framework.Dot;

namespace Monoxide.Ilasm {

	internal class IlGraphDisplay : IGraphicDisplay, IMethodView {

		private Image image;
		private bool display;

		public void Render (MethodDefinition method)
		{
			if (display && (method != null)) {
				Digraph digraph = GetIlSourceAsDot (method);
				image.FromFile = DotHelper.BuildDotImage (digraph);
				image.Visible = true;
			}
		}

		public void SetUp (Image image)
		{
			this.image = image;
		}

		public bool Display {
			get { return display; }
			set { display = value; }
		}

		public string Name {
			get { return "IL Graph"; }
		}

		// internal stuff

		private Digraph GetIlSourceAsDot (MethodDefinition method)
		{
			Digraph dot = new Digraph ();
			dot.Name = "IL";
			dot.Label = method.ToString ();
			dot.LabelLoc = "t";
			dot.FontName = "tahoma";
			dot.FontSize = 10;

			dot.DefaultNode = new Node ();
			dot.DefaultNode.Attributes["fontname"] = "tahoma";
			dot.DefaultNode.Attributes["fontsize"] = 8;

			dot.DefaultEdge = new Edge ();
			dot.DefaultNode.Attributes["fontname"] = "tahoma";
			dot.DefaultNode.Attributes["fontsize"] = 8;
			dot.DefaultNode.Attributes["labelfontname"] = "tahoma";
			dot.DefaultNode.Attributes["labelfontsize"] = 8;

			if (method.Body != null) {
				ArrayList instructions = new ArrayList ();

				foreach (Instruction instr in method.Body.Instructions) {
					IlInstruction i = new IlInstruction (instr);
					i.Name = instr.OpCode.Name;

					switch (instr.OpCode.OperandType) {
					case OperandType.InlineNone:
						break;
					case OperandType.InlineSwitch:
						int[] brchs = instr.Operand as int[];
						i.Calls = new ArrayList (brchs.Length);

						for (int j = 0; j < brchs.Length; j++) {
							string switchtarget = String.Format ("IL_{0}", brchs[j].ToString ("X4"));
							i.Name += String.Format ("\t{0}{1}", (j > 0) ? ", " : String.Empty, switchtarget);
							i.Calls.Add (switchtarget);
						}
						break;
					case OperandType.ShortInlineBrTarget:
					case OperandType.InlineBrTarget:
						Instruction ins = (instr.Operand as Instruction);
						string target = String.Format ("IL_{0}", ins.Offset.ToString ("X4"));
						i.Name = String.Format ("{0} {1}", i.Name, target);
						i.Calls = new ArrayList (1);
						i.Calls.Add (target);
						break;
					case OperandType.InlineString:
						i.Name = String.Format ("{0} '{1}'", i.Name, instr.Operand);
						break;
					default:
						i.Name = String.Format ("{0} {1}", i.Name, instr.Operand);
						break;
					}

					// add instruction
					instructions.Add (i);
				}

				// build dot for each instructions
				for (int j = 0; j < instructions.Count; j++) {
					IlInstruction i = (IlInstruction)instructions[j];

					Node n = i.GetNode (true);
					dot.Nodes.Add (n);
					if (i.Calls != null) {
						foreach (string callee in i.Calls) {
							IlInstruction target = FindCallee (callee, instructions);
							Node t = target.GetNode (false);
							Edge e = new Edge (n, t);

							if (target.HasSecurity ()) {
								e.Attributes["label"] = target.GetSecurity ();
							}

							dot.Edges.Add (e);
						}
					}
					// by default execution continues to the next instruction
					// unless - we have an unconditional branch or it's the last instruction
					if ((j < instructions.Count - 1) && !i.IsUnconditionalBranch ()) {
						IlInstruction next = (IlInstruction)instructions[j + 1];
						Edge e = new Edge (n, next.GetNode (false));
						dot.Edges.Add (e);
					}
				}
			}

			return dot;
		}

		IlInstruction FindCallee (string callee, ArrayList instructions)
		{
			foreach (IlInstruction candidate in instructions) {
				if (callee == candidate.Address)
					return candidate;
			}
			return null;
		}
	}

	internal class IlInstruction {
		public string Name;
		public ArrayList Calls;

		private Instruction _instr;

		public IlInstruction (Instruction instr)
		{
			_instr = instr;
		}

		public string Address {
			get { return String.Format ("IL_{0}", _instr.Offset.ToString ("X4")); }
		}

		public Node GetNode (bool details)
		{
			Node n = new Node ();
			n.Label = String.Format ("\"{0}: {1}\"", Address, Name);
			if (!details)
				return n;

			n.Attributes["shape"] = "box";

			// now the fun part
			switch (_instr.OpCode.Name) {
			case "throw":
				n.Attributes["style"] = "filled";
				n.Attributes["fillcolor"] = "orange";
				n.Attributes["fontcolor"] = "white";
				break;
			case "ret":
				n.Attributes["style"] = "filled";
				n.Attributes["fillcolor"] = "blue";
				n.Attributes["fontcolor"] = "white";
				break;
			}

			MethodDefinition method = (_instr.Operand as MethodDefinition);
			if (method != null) {
				// double lines for internal calls
				if ((method.ImplAttributes & MethodImplAttributes.InternalCall) != 0) {
					n.Attributes["peripheries"] = "2";
				}

				// red background for security runtime methods
				switch (method.DeclaringType.FullName) {
				case "System.Security.SecurityManager":
				case "System.Security.CodeAccessPermission":
					n.Attributes["style"] = "filled";
					n.Attributes["fillcolor"] = "red";
					n.Attributes["fontcolor"] = "white";
					break;
				}
			}
			return n;
		}

		public bool HasSecurity ()
		{
			MethodDefinition method = (_instr.Operand as MethodDefinition);
			if (method == null)
				return false;
			return (method.SecurityDeclarations.Count > 0);
		}

		public string GetSecurity ()
		{
			// TODO
			return "*** here ***";
		}

		public bool IsUnconditionalBranch ()
		{
			switch (_instr.OpCode.Name) {
			case "br":
			case "br.s":
			case "ret":
			case "throw":
				return true;
			default:
				return false;
			}
		}
	}
}
