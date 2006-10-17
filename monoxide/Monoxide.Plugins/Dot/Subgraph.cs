//
// Subgraph.cs
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

	public class Subgraph {

		private string _name;
		private string _label;
		private string _labelloc;
		private string _fontname;
		private int _fontsize;
		private ArrayList _nodes;
		private ArrayList _edges;

		public string Name {
			get { return _name; }
			set { _name = value; }
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

		internal string ToString (string prefix)
		{
			StringWriter sw = new StringWriter ();
			string name = (_name != null) ? _name : Math.Abs (this.GetHashCode ()).ToString ();
			sw.WriteLine ("{0}subgraph cluster{1} {{", prefix, name);

			if ((_label != null) && (_label.Length > 0)) {
				sw.WriteLine ("{0}\tlabel=\"{1}\";", prefix, _label);
				if ((_fontname != null) && (_fontname.Length > 0))
					sw.WriteLine ("{0}\tfontname=\"{1}\";", prefix, _fontname);
				if (_fontsize > 0)
					sw.WriteLine ("{0}\tfontsize={1};", prefix, _fontsize);
				if ((_labelloc != null) && (_labelloc.Length > 0))
					sw.WriteLine ("{0}\tlabelloc={1};", prefix, _labelloc);
			}

			if (_nodes != null) {
				foreach (Node n in _nodes) {
					sw.WriteLine (n.ToString ("\t\t"));
				}
			}

			if (_edges != null) {
				foreach (Edge e in _edges) {
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
