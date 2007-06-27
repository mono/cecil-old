//
// DotHelper.cs
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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Monoxide.Framework.Dot {

	public class DotHelper {

		static public string BuildDotImage (Digraph graph)
		{
			if (graph == null)
				return null;

			string dotfile = Path.GetTempFileName ();
			string pngfile = Path.ChangeExtension (dotfile, "png");

			using (StreamWriter sw = new StreamWriter (dotfile)) {
				string dot = graph.ToString ();
				sw.WriteLine (dot);
				sw.Close ();
			}

			string dotexe = null;
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				dotexe = "dot";
			} else {
				// bad but not really supported so...
				dotexe = @"C:\Program Files\ATT\Graphviz\bin\dot.exe";
				// -Tcmap 
			}
			string options = String.Format ("-Tpng \"{0}\" -o \"{1}\"", dotfile, pngfile);

			// process output dot file to get a PNG image
			ProcessStartInfo psi = new ProcessStartInfo (dotexe, options);
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			Process p = System.Diagnostics.Process.Start (psi);
			Console.WriteLine ("{0} {1}", psi.FileName, psi.Arguments);
			p.WaitForExit ();
			// return image filename
			return pngfile;
		}
	}
}
