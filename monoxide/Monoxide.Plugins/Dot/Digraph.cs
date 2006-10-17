//
// Digraph.cs
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
using System.IO;
using System.Text;

namespace Monoxide.Framework.Dot {

	public class Digraph {

		private const int autoConcentrateLimit = 50;

		private string _name;
		private bool _concentrate;
		private bool _autoConcentrate;
		private string _label;
		private string _labelloc;
		private string _fontname;
		private int _fontsize;
		private ArrayList _subgraphs;
		private ArrayList _nodes;
		private ArrayList _edges;
		private Node _defaultNode;
		private Edge _defaultEdge;

		public string Name {
			get { return _name; }
			set { _name = value; }
		}

		public bool Concentrate {
			get {
				if (_autoConcentrate) {
					int n = 0;
					if (_edges != null)
						n += _edges.Count;
					if (_subgraphs != null) {
						foreach (Subgraph sg in _subgraphs) {
							n += sg.Edges.Count;
						}
					}
					return (n > autoConcentrateLimit);
				}
				return _concentrate;
			}
			set { _concentrate = value; }
		}

		public bool AutoConcentrate {
			get { return _autoConcentrate; }
			set { _autoConcentrate = value; }
		}

		public string Label {
			get { return _label; }
			set { _label = value; }
		}

		public string LabelLoc {
			get { return _labelloc; }
			set { _labelloc = value; }
		}

		public string FontName {
			get { return _fontname; }
			set { _fontname = value; }
		}

		public int FontSize {
			get { return _fontsize; }
			set { _fontsize = value; }
		}

		public IList Subgraphs {
			get {
				if (_subgraphs == null)
					_subgraphs = new ArrayList ();
				return _subgraphs;
			}
		}

		public IList Edges {
			get {
				if (_edges == null)
					_edges = new ArrayList ();
				return _edges;
			}
		}

		public IList Nodes {
			get {
				if (_nodes == null)
					_nodes = new ArrayList ();
				return _nodes;
			}
		}

		public Node DefaultNode {
			get { return _defaultNode; }
			set { _defaultNode = value; }
		}

		public Edge DefaultEdge {
			get { return _defaultEdge; }
			set { _defaultEdge = value; }
		}

		public override string ToString ()
		{
			StringWriter sw = new StringWriter ();
			sw.WriteLine ("digraph {0} {{", _name);

			// manual or automatic
			if (Concentrate)
				sw.WriteLine ("\tconcentrate=true;");

			if ((_label != null) && (_label.Length > 0)) {
				sw.WriteLine ("\tlabel=\"{0}\";", _label);
				if ((_fontname != null) && (_fontname.Length > 0))
					sw.WriteLine ("\tfontname=\"{0}\";", _fontname);
				if (_fontsize > 0)
					sw.WriteLine ("\tfontsize={0};", _fontsize);
				if ((_labelloc != null) && (_labelloc.Length > 0))
					sw.WriteLine ("\tlabelloc={0};", _labelloc);
			}

			if (_defaultNode != null) {
				string attrs = _defaultNode.GetAttributes ();
				if (attrs.Length > 0)
					sw.WriteLine ("\tnode [{0}];", attrs);
			}

			if (_defaultEdge != null) {
				string attrs = _defaultEdge.GetAttributes ();
				if (attrs.Length > 0)
					sw.WriteLine ("\tedge [{0}];", attrs);
			}

			if (_subgraphs != null) {
				foreach (Subgraph s in _subgraphs) {
					sw.WriteLine (s.ToString ("\t"));
				}
			}

			if (_nodes != null) {
				foreach (Node n in _nodes) {
					sw.WriteLine (n.ToString ("\t"));
				}
			}

			if (_edges != null) {
				foreach (Edge e in _edges) {
					sw.WriteLine (e.ToString ("\t"));
				}
			}

			sw.WriteLine ("}");
			return sw.ToString ();
		}
	}
}
