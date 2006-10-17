//
// DotDebugDisplay.cs
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
using System.Text;

using Mono.Cecil;
using Monoxide.Framework.PlugIns;

using Gtk;
using Pango;

namespace Monoxide.Framework.Dot {

	internal class DotDebugDisplay : ICustomDisplay, IAssemblyView, ITypeView, IMethodView {

		private ScrolledWindow sw;
		private TextView dot;
		private bool _display;

		public bool Display {
			get { return _display; }
			set { _display = value; }
		}

		public string Name {
			get { return "Dot"; }
		}

		public void SetUp (Notebook notebook)
		{
			FontDescription fd = FontDescription.FromString ("Courier 10 Pitch 10");

			dot = new TextView ();
			dot.ModifyFont (fd);
			dot.Editable = false;

			sw = new ScrolledWindow ();
			sw.Add (dot);

			notebook.AppendPage (sw, new Label (Name));
		}
		
		private void Render ()
		{
			string content = DotDebugPlugIn.CurrentDotContent;
			dot.Buffer.Text = content;
			if (content != null) {
				sw.ShowAll ();
			} else {
				sw.HideAll ();
			}
		}

		public void Render (AssemblyDefinition assembly)
		{
			if (assembly == null)
				return;
			Render ();
		}

		public void Render (TypeDefinition type)
		{
			if (type == null)
				return;
			Render ();
		}

		public void Render (MethodDefinition method)
		{
			if (method == null)
				return;
			Render ();
		}
	}
}
