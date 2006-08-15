//
// Driver.cs
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

	using System;
	using System.Collections;
	using System.Xml.XPath;

	public class Driver {

		static readonly string _linker = "Mono CIL Linker";

		public static int Main (string [] args)
		{
			if (args.Length == 0)
				Usage ();

			try {

				Run(new Queue(args));

			} catch (Exception e) {
				Console.WriteLine ("Fatal error in {0}", _linker);
				Console.WriteLine (e);
			}

			return 0;
		}

		static void Run (Queue q)
		{
			LinkContext context = GetDefaultContext ();
			Pipeline p = GetStandardPipeline ();

			bool resolver = false;
			while (q.Count > 0) {
				string token = (string) q.Dequeue ();
				if (token.Length < 2)
					Usage ();

				if (! (token [0] == '-' || token [1] == '/'))
					Usage ();

				if (token [0] == '-' && token [1] == '-') {

					if (token.Length < 3)
						Usage ();

					switch (token [2]) {
					case 'v':
						Version ();
						break;
					case 'a':
						About ();
						break;
					default:
						Usage ();
						break;
					}
				}

				string param = (string) q.Dequeue ();
				switch (token [1]) {
				case 'o':
					context.OutputDirectory = param;
					break;
				case 'p':
					context.PreserveCoreLibraries = bool.Parse (param);
					break;
				case 'x':
					if (resolver)
						Usage ();

					p.PrependStep (new ResolveFromXmlStep (new XPathDocument (param)));
					resolver = true;
					break;
				case 'a':
					if (resolver)
						Usage ();

					p.PrependStep (new ResolveFromAssemblyStep (param));
					resolver = true;
					break;
				default:
					Usage ();
					break;
				}
			}

			if (!resolver)
				Usage ();

			p.Process(context);
		}

		static LinkContext GetDefaultContext ()
		{
			LinkContext context = new LinkContext ();
			context.PreserveCoreLibraries = true;
			context.OutputDirectory = ".";
			return context;
		}

		static void Usage ()
		{
			Console.WriteLine (_linker);
			Console.WriteLine ("linker [options] -x|-a file");

			Console.WriteLine ("   --about     About the {0}", _linker);
			Console.WriteLine ("   --version   Print the version number of the {0}", _linker);
			Console.WriteLine ("   -out        Specify the output directory, default to .");
			Console.WriteLine ("   -p          Preserve the core libraries, true or false, default to true");
			Console.WriteLine ("   -x          Link from an XML descriptor");
			Console.WriteLine ("   -a          Link from an assembly");
			Console.WriteLine ("");
			Console.WriteLine ("   you have to choose one from -x and -a but not both");

			Environment.Exit (1);
		}

		static void Version ()
		{
			Console.WriteLine ("{0} Version {1}",
				_linker,
			    System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version);

			Environment.Exit(1);
		}

		static void About ()
		{
			Console.WriteLine ("For more information, visit the project Web site");
			Console.WriteLine ("   http://www.mono-project.com/");

			Environment.Exit(1);
		}

		static Pipeline GetStandardPipeline ()
		{
			Pipeline p = new Pipeline ();
			p.AppendStep (new MarkStep ());
			p.AppendStep (new SweepStep ());
			p.AppendStep (new CleanStep ());
			p.AppendStep (new OutputStep ());
			return p;
		}
	}
}
