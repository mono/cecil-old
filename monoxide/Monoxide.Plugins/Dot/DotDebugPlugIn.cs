//
// DotDebugPlugIn.cs
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
using Gtk;
using Mono.Cecil;
using Monoxide.Framework.PlugIns;

namespace Monoxide.Framework.Dot {

	public class DotDebugPlugIn : IPlugIn {

		static bool _debugDot;
		static string _currentDot;

		static DotDebugPlugIn ()
		{
#if DEBUG
			_debugDot = true;
#else
			string debug = Environment.GetEnvironmentVariable ("MONO_DEBUG");
			_debugDot = ((debug != null) && (debug.Length > 0));
#endif
		}

		IDisplay[] displays;

		public DotDebugPlugIn ()
		{
			if (DebugDotOutput) {
				displays = new IDisplay[1];
				displays[0] = new DotDebugDisplay ();
			} else {
				displays = new IDisplay[0];
			}
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			// nothing do to
		}

		public string Name {
			get { return "Dot Output"; }
		}

		public IDisplay[] Displays {
			get { return displays; }
		}

		//

		static public bool DebugDotOutput {
			get { return _debugDot;	}
		}

		static public string CurrentDotContent {
			get { return _currentDot; }
			set {
				if (DebugDotOutput)
					_currentDot = value;
			}
		}
	}
}
