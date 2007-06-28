//
// Digraph.cs
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
using System.IO;

namespace Monoxide.Framework.Dot {

	public class Digraph {

		private const int autoConcentrateLimit = 50;

		private string name;
		private bool concentrate;
		private bool auto_concentrate;
		private string label;
		private string label_loc;
		private string font_name;
		private int font_size;
		
		private List<Subgraph> subgraphs;
		private List<Node> nodes;
		private List<Edge> edges;
		
		private Node defaultNode;
		private Edge defaultEdge;

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public bool Concentrate {
			get {
				if (auto_concentrate) {
					int n = 0;
					if (edges != null)
						n += edges.Count;
					if (subgraphs != null) {
						foreach (Subgraph sg in subgraphs) {
							n += sg.Edges.Count;
						}
					}
					return (n > autoConcentrateLimit);
				}
				return concentrate;
			}
			set { concentrate = value; }
		}

		public bool AutoConcentrate {
			get { return auto_concentrate; }
			set { auto_concentrate = value; }
		}

		public string Label {
			get { return label; }
			set { label = value; }
		}

		public string LabelLoc {
			get { return label_loc; }
			set { label_loc = value; }
		}

		public string FontName {
			get { return font_name; }
			set { font_name = value; }
		}

		public int FontSize {
			get { return font_size; }
			set { font_size = value; }
		}

		public List<Subgraph> Subgraphs {
			get {
				if (subgraphs == null)
					subgraphs = new List<Subgraph> ();
				return subgraphs;
			}
		}

		public List<Edge> Edges {
			get {
				if (edges == null)
					edges = new List<Edge> ();
				return edges;
			}
		}

		public List<Node> Nodes {
			get {
				if (nodes == null)
					nodes = new List<Node> ();
				return nodes;
			}
		}

		public Node DefaultNode {
			get {
				if (defaultNode == null) {
					defaultNode = new Node ();
					defaultNode.Attributes["fontname"] = "tahoma";
					defaultNode.Attributes["fontsize"] = "8";
				}
				return defaultNode;
			}
			set { defaultNode = value; }
		}

		public Edge DefaultEdge {
			get {
				if (defaultEdge == null) {
					defaultEdge = new Edge ();
					defaultEdge.Attributes["fontname"] = "tahoma";
					defaultEdge.Attributes["fontsize"] = "8";
					defaultEdge.Attributes["labelfontname"] = "tahoma";
					defaultEdge.Attributes["labelfontsize"] = "8";
				}
				return defaultEdge;
			}
			set { defaultEdge = value; }
		}

		public override string ToString ()
		{
			StringWriter sw = new StringWriter ();
			sw.WriteLine ("digraph {0} {{", name);

			// manual or automatic
			if (Concentrate)
				sw.WriteLine ("\tconcentrate=true;");

			if ((label != null) && (label.Length > 0)) {
				sw.WriteLine ("\tlabel=\"{0}\";", label);
				if ((font_name != null) && (font_name.Length > 0))
					sw.WriteLine ("\tfontname=\"{0}\";", font_name);
				if (font_size > 0)
					sw.WriteLine ("\tfontsize={0};", font_size);
				if ((label_loc != null) && (label_loc.Length > 0))
					sw.WriteLine ("\tlabelloc={0};", label_loc);
			}

			if (DefaultNode.HasAttributes) {
				sw.Write ("\tnode");
				defaultNode.WriteAttributes (sw);
				sw.WriteLine (";");
			}

			if (DefaultEdge.HasAttributes) {
				sw.Write ("\tedge");
				defaultEdge.WriteAttributes (sw);
				sw.WriteLine (";");
			}

			if (subgraphs != null) {
				foreach (Subgraph s in subgraphs) {
					sw.WriteLine (s.ToString ("\t"));
				}
			}

			if (nodes != null) {
				foreach (Node n in nodes) {
					sw.WriteLine (n.ToString ("\t"));
				}
			}

			if (edges != null) {
				foreach (Edge e in edges) {
					sw.WriteLine (e.ToString ("\t"));
				}
			}

			sw.WriteLine ("}");
			return sw.ToString ();
		}
	}
}
