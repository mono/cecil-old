//
// PrintStep.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.Linker {

	using System.IO;

	class PrintStep : IPipelineStep {

		TextWriter _writer;
		int _inc;
		const string _indent = "  ";

		public PrintStep (TextWriter writer)
		{
			_writer = writer;
		}

		void Inc ()
		{
			_inc++;
		}

		void Dec ()
		{
			_inc--;
		}

		void P (string s)
		{
			for (int i = 0; i < _inc; i++)
				_writer.Write (_indent);
			_writer.WriteLine (s);
		}

		void P (string s, params object [] parameters)
		{
			for (int i = 0; i < _inc; i++)
				_writer.Write (_indent);
			_writer.WriteLine (s, parameters);
		}

		public void Process (LinkContext context)
		{
			P ("PrintStep");
			Inc ();
			foreach (AssemblyMarker am in context.GetAssemblies ()) {
				P ("Assembly: " + am.Assembly);
				P ("Action:   " + am.Action);
				Inc ();
				foreach (TypeMarker tm in am.GetTypes ()) {
					P ("Type: " + tm.Type);
					Inc ();
					foreach (FieldMarker fm in tm.GetFields ())
						P ("Field: " + fm.Field);
					foreach (MethodMarker mm in tm.GetMethods ()) {
						P ("Method:" + mm.Method);
						P ("Action:" + mm.Action);
					}
					Dec ();
				}
				Dec ();
			}
			Dec ();
		}
	}
}