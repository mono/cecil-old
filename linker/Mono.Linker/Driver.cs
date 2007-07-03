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

using System;
using System.Collections;
using System.IO;
using SR = System.Reflection;
using System.Xml.XPath;

using Mono.Linker.Steps;

namespace Mono.Linker {

	public class Driver {

		static readonly string _linker = "Mono CIL Linker";

		public static int Main (string [] args)
		{
			if (args.Length == 0)
				Usage ();

			try {

				Run (new Queue (args));

			} catch (Exception e) {
				Console.WriteLine ("Fatal error in {0}", _linker);
				Console.WriteLine (e);
			}

			return 0;
		}

		static void Run (Queue q)
		{
			Pipeline p = GetStandardPipeline ();
			LinkContext context = GetDefaultContext (p);
			I18nAssemblies assemblies = I18nAssemblies.All;

			bool resolver = false;
			while (q.Count > 0) {
				string token = GetParam (q);
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

				switch (token [1]) {
				case 'd': {
					DirectoryInfo info = new DirectoryInfo (GetParam (q));
					context.Resolver.AddSearchDirectory (info.FullName);
					break;
				}
				case 'o':
					context.OutputDirectory = GetParam (q);
					break;
				case 'c':
					context.CoreAction = ParseCoreAction (GetParam (q));
					break;
				case 'x':
					foreach (string file in GetFiles (GetParam (q)))
						p.PrependStep (new ResolveFromXmlStep (new XPathDocument (file)));
					resolver = true;
					break;
				case 'a':
					foreach (string file in GetFiles (GetParam (q)))
						p.PrependStep (new ResolveFromAssemblyStep (file));
					resolver = true;
					break;
				case 'i':
					foreach (string file in GetFiles (GetParam (q)))
						p.PrependStep (new ResolveFromApiInfoStep (new XPathDocument (file)));
					p.AddStepBefore (typeof (OutputStep), new AdjustVisibilityStep ());
					resolver = true;
					break;
				case 'l':
					assemblies = ParseI18n (GetParam (q));
					break;
				default:
					Usage ();
					break;
				}
			}

			if (!resolver)
				Usage ();

			p.AddStepAfter (typeof (LoadReferencesStep), new LoadI18nAssemblies (assemblies));

			p.Process (context);
		}

		static string [] GetFiles (string param)
		{
			if (param.Length < 1 || param [0] != '@')
				return new string [] {param};

			string file = param.Substring (1);
			return ReadLines (file);
		}

		static string [] ReadLines (string file)
		{
			ArrayList lines = new ArrayList ();
			using (StreamReader reader = new StreamReader (file)) {
				while (!reader.EndOfStream)
					lines.Add (reader.ReadLine ());
			}
			return (string []) lines.ToArray ();
		}

		static I18nAssemblies ParseI18n (string str)
		{
			I18nAssemblies assemblies = I18nAssemblies.None;
			string [] parts = str.Split (',');
			foreach (string part in parts)
				assemblies |= (I18nAssemblies) Enum.Parse (typeof (I18nAssemblies), part.Trim (), true);

			return assemblies;
		}

		static AssemblyAction ParseCoreAction (string s)
		{
			return (AssemblyAction) Enum.Parse (typeof (AssemblyAction), s, true);
		}

		static string GetParam (Queue q)
		{
			return (string) q.Dequeue ();
		}

		static LinkContext GetDefaultContext (Pipeline pipeline)
		{
			LinkContext context = new LinkContext (pipeline);
			context.CoreAction = AssemblyAction.Skip;
			context.OutputDirectory = "output";
			return context;
		}

		static void Usage ()
		{
			Console.WriteLine (_linker);
			Console.WriteLine ("monolinker [options] -x|-a|-i file");

			Console.WriteLine ("   --about     About the {0}", _linker);
			Console.WriteLine ("   --version   Print the version number of the {0}", _linker);
			Console.WriteLine ("   -out        Specify the output directory, default to `output'");
			Console.WriteLine ("   -c          Action on the core assemblies, skip, copy or link, default to skip");
			Console.WriteLine ("   -l          List of i18n assemblies to copy to the output directory");
			Console.WriteLine ("                 separated with a comma: none,all,cjk,mideast,other,rare,west");
			Console.WriteLine ("                 default is all");
			Console.WriteLine ("   -x          Link from an XML descriptor");
			Console.WriteLine ("   -a          Link from a list of assemblies");
			Console.WriteLine ("   -i          Link from an mono-api-info descriptor");
			Console.WriteLine ("");

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
			p.AppendStep (new LoadReferencesStep ());
			p.AppendStep (new BlacklistStep ());
			p.AppendStep (new MarkStep ());
			p.AppendStep (new SweepStep ());
			p.AppendStep (new CleanStep ());
			p.AppendStep (new OutputStep ());
			return p;
		}
	}
}
