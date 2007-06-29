//
// IlGraphVisualizer.cs
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
using System.Collections.Generic;
using System.Text;

using Gtk;

using Mono.Addins;
using Mono.Cecil;
using Mono.Cecil.Cil;

using Monoxide.Framework.Addins;
using Monoxide.Framework.Dot;

/*
TODO
- give cue about what we're calling (if we know about it)
	e.g. SL-hints: transparent, safe-critical and critical
	e.g. icalls, p/invoke ...
*/

namespace Monoxide.Ilasm {

	[Extension ("/Monoxide/Method")]
	internal class IlGraphVisualizer : IMethodVisualizer {

		public string Name {
			get { return "IL Graph"; }
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			// nothing to do when a new assembly is loaded
		}

		public Widget GetWidget (MethodDefinition method)
		{
			Digraph digraph = GetIlSourceAsDot (method);

			Image image = new Image (DotHelper.BuildDotImage (digraph));

			ScrolledWindow sw = new ScrolledWindow ();
			sw.AddWithViewport (image);
			sw.ShowAll ();
			return sw;
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

			if (method.Body != null) {
				List<IlInstruction> instructions = new List<IlInstruction> ();

				foreach (Instruction instr in method.Body.Instructions) {
					IlInstruction i = new IlInstruction (instr);
					i.Name = instr.OpCode.Name;

					switch (instr.OpCode.OperandType) {
					case OperandType.InlineNone:
						break;
					case OperandType.InlineSwitch:
						int[] brchs = instr.Operand as int[];
						i.Calls = new List<string> (brchs.Length);

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
						i.Calls = new List<string> (1);
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
				
				// first add the nodes
				foreach (IlInstruction i in instructions) {
					Node n = i.GetNode (true);
					dot.Nodes.Add (n);
				}
				// then add the edges
				for (int j = 0; j < instructions.Count; j++) {
					IlInstruction i = instructions[j];
					Node n = i.GetNode (false);

					if (i.Calls != null) {
						foreach (string callee in i.Calls) {
							IlInstruction target = FindCallee (callee, instructions);
							Node t = target.GetNode (false);
							Edge e = new Edge (n, t);

							string label = target.Label;
							if (label.Length > 0)
								e.Attributes["label"] = label;

							dot.Edges.Add (e);
						}
					}
					// by default execution continues to the next instruction
					// unless - we have an unconditional branch or it's the last instruction
					if ((j < instructions.Count - 1) && !i.IsUnconditionalBranch ()) {
						IlInstruction next = (IlInstruction)instructions[j + 1];
						Edge e = new Edge (n, next.GetNode (false));
						string label = next.Label;
						if (label.Length > 0)
							e.Attributes["label"] = label;
						dot.Edges.Add (e);
					}
				}
			}

			return dot;
		}

		IlInstruction FindCallee (string callee, List<IlInstruction> instructions)
		{
			foreach (IlInstruction candidate in instructions) {
				if (callee == candidate.Address)
					return candidate;
			}
			return null;
		}
	}

	internal class IlInstruction {
		private string name;
		private List<string> calls;
		private Instruction instr;
		private string label;

		public IlInstruction (Instruction instr)
		{
			this.instr = instr;
		}

		public string Address {
			get { return String.Format ("IL_{0}", instr.Offset.ToString ("X4")); }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public List<string> Calls {
			get { return calls; }
			set { calls = value; }
		}
		
		public string Label {
			get {
				if (label == null)
					return String.Empty;
				return label;
			}
			set { label = value; }
		}

		public Node GetNode (bool details)
		{
			Node n = new Node ();
			n.Label = String.Format ("{0}: {1}", Address, Name);
			if (!details)
				return n;

			n.Attributes["shape"] = "box";

			// now the fun part
			switch (instr.OpCode.Name) {
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

			MethodDefinition method = (instr.Operand as MethodDefinition);
			if (method != null) {
				// double lines for internal calls
				if ((method.ImplAttributes & MethodImplAttributes.InternalCall) != 0) {
					n.Attributes["peripheries"] = "2";
				}

				// CAS checks - red background for security runtime methods
				switch (method.DeclaringType.FullName) {
				case "System.Security.SecurityManager":
				case "System.Security.CodeAccessPermission":
					n.Attributes["style"] = "filled";
					n.Attributes["fillcolor"] = "red";
					n.Attributes["fontcolor"] = "white";
					break;
				}
				if (method.SecurityDeclarations.Count > 0) {
					AddLabel ("\"HasSecurity\"");
				}
				
				// SL checks
				if (IsCritical (method)) {
					n.Attributes["style"] = "filled";
					n.Attributes["fillcolor"] = "red";
					n.Attributes["fontcolor"] = "white";
					AddLabel ("\"[SecurityCritical]\"");
				} else if (IsSafeCritical (method)) {
					n.Attributes["style"] = "filled";
					n.Attributes["fillcolor"] = "orange";
					n.Attributes["fontcolor"] = "white";
					AddLabel ("\"[SecuritySafeCritical]\"");
				}
			}
			return n;
		}

		private void AddLabel (string s)
		{
			if (label == null)
				label = s;
			else
				label += "\n" + s;
		}

		public bool IsUnconditionalBranch ()
		{
			switch (instr.OpCode.Name) {
			case "br":
			case "br.s":
			case "ret":
			case "throw":
				return true;
			default:
				return false;
			}
		}
		
		private bool CheckForAttribute (string attrName, CustomAttributeCollection cac)
		{
			foreach (CustomAttribute ca in cac) {
				if (ca.Constructor.DeclaringType.FullName == attrName)
					return true;
			}
			return false;
		}

		private bool CheckForAttribute (string attrName, MethodDefinition md)
		{
			if (CheckForAttribute (attrName, md.CustomAttributes))
				return true;
			if (CheckForAttribute (attrName, md.DeclaringType.CustomAttributes))
				return true;
			if (CheckForAttribute (attrName, md.DeclaringType.Module.CustomAttributes))
				return true;
			if (CheckForAttribute (attrName, md.DeclaringType.Module.Assembly.CustomAttributes))
				return true;
			return false;
		}
		
		public bool IsCritical (MethodDefinition md)
		{
			return CheckForAttribute ("System.Security.SecurityCriticalAttribute", md);
		}

		public bool IsSafeCritical (MethodDefinition md)
		{
			return CheckForAttribute ("System.Security.SecuritySafeCriticalAttribute", md);
		}
	}
}
