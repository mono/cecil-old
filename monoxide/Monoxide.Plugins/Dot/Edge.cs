//
// Edge.cs
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

	public class Edge {
		private string from;
		private string to;
		private Dictionary<string,string> attributes;

		public Edge ()
		{
		}

		public Edge (string from, string to)
		{
			if (from == null)
				throw new ArgumentNullException ("form");
			if (to == null)
				throw new ArgumentNullException ("to");

			this.from = from;
			this.to = to;
		}

		public Edge (Node from, Node to)
		{
			if (from == null)
				throw new ArgumentNullException ("form");
			if (to == null)
				throw new ArgumentNullException ("to");

			this.from = from.Label;
			this.to = to.Label;
		}

		public string FromLabel {
			get { return from; }
			set { from = value; }
		}

		public string ToLabel {
			get { return to; }
			set { to = value; }
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
			if (prefix.Length > 0)
				sw.Write (prefix);
			
			sw.Write ("\"{0}\" -> \"{1}\"", from, to);
			WriteAttributes (sw);
			sw.WriteLine (";");
			return sw.ToString ();
		}

		public override string ToString ()
		{
			return ToString (String.Empty);
		}
	}
}
