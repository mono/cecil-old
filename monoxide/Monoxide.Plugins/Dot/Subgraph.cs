//
// Subgraph.cs
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

	public class Subgraph {

		private string name;
		private string color;
		private string label;
		private string labelloc;
		private string labeljust;
		private string fontname;
		private int fontsize;
		private List<Node> nodes;
		private List<Edge> edges;
		private Dictionary<string,string> attributes;

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public string Color {
			get { return color; }
			set { color = value; }
		}

		public string Label {
			get { return label; }
			set { label = value; }
		}

		public string LabelLoc {
			get { return labelloc; }
			set { labelloc = value; }
		}

		public string LabelJust {
			get { return labeljust; }
			set { labeljust = value; }
		}

		public string FontName {
			get { return fontname; }
			set { fontname = value; }
		}

		public int FontSize {
			get { return fontsize; }
			set { fontsize = value; }
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

		public Dictionary<string,string> Attributes {
			get {
				if (attributes == null)
					attributes = new Dictionary<string,string> ();
				return attributes;
			}
		}

		public bool HasAttributes {
			get {
				if (attributes == null)
					return false;
				return (attributes.Count > 0);
			}
		}

		internal void WriteAttributes (StringWriter sw)
		{
			if (attributes == null)
				return;

			sw.Write (" [");
			bool first = true;
			foreach (KeyValuePair<string,string> kvp in attributes) {
				if (first) {
					first = false;
				} else {
					sw.Write (",");
				}
				sw.Write ("{0}={1}", kvp.Key, kvp.Value);
			}
			sw.Write ("]");
		}

		internal string ToString (string prefix)
		{
			StringWriter sw = new StringWriter ();
			string name = (this.name != null) ? this.name : Math.Abs (this.GetHashCode ()).ToString ();
			sw.WriteLine ("{0}subgraph cluster{1} {{", prefix, name);

			if ((label != null) && (label.Length > 0)) {
				sw.WriteLine ("{0}\tlabel=\"{1}\";", prefix, label);
				if ((fontname != null) && (fontname.Length > 0))
					sw.WriteLine ("{0}\tfontname=\"{1}\";", prefix, fontname);
				if (fontsize > 0)
					sw.WriteLine ("{0}\tfontsize={1};", prefix, fontsize);
				if ((labelloc != null) && (labelloc.Length > 0))
					sw.WriteLine ("{0}\tlabelloc={1};", prefix, labelloc);
				if ((labeljust != null) && (labeljust.Length > 0))
					sw.WriteLine ("{0}\tlabeljust={1};", prefix, labeljust);
			}

			if ((color != null) && (color.Length > 0))
				sw.WriteLine ("{0}\tcolor={1};", prefix, color);

			WriteAttributes (sw);

			if (nodes != null) {
				foreach (Node n in nodes) {
					sw.WriteLine (n.ToString ("\t\t"));
				}
			}

			if (edges != null) {
				foreach (Edge e in edges) {
					sw.WriteLine (e.ToString ("\t\t"));
				}
			}

			sw.WriteLine ("{0}}}", prefix);
			return sw.ToString ();
		}

		public override string ToString ()
		{
			return ToString (String.Empty);
		}
	}
}
