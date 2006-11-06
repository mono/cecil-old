//
// Driver.cs
//
// Author:
//	 Alex Prudkiy (prudkiy@gmail.com)
//
// (C) 2006 Alex Prudkiy
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

using System;
using System.Collections;

using Mono.Cecil;

namespace Mono.Merge {

	public class Driver {

		static readonly string merge_name = "Mono CIL Merge";

		static MergeContext GetDefaultContext ()
		{
			MergeContext context = new MergeContext ();
			return context;
		}

		public static int Main (string [] args)
		{
			if (args.Length == 0)
				Usage ();

			try {
				Run (new Queue (args));
			} catch (Exception e) {
				Console.WriteLine ("Fatal error in {0}", merge_name);
				Console.WriteLine (e);
			}

			return 0;
		}

		static void Run (Queue q)
		{
			MergeContext context = GetDefaultContext ();
			while (q.Count > 0) {
				string token = (string) q.Dequeue ();

				if (token.Length < 2)
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

				if (token [0] == '-' || token [0] == '/') {
					token = token.Substring (1);

					if (token == "o" || token == "out")
						context.OutputPath = (string) q.Dequeue ();
					else
						Usage ();
				} else {
					context.Assemblies.Add (token);
					while (q.Count > 0)
						context.Assemblies.Add ((string) q.Dequeue ());
				}
			}

			if (context.Assemblies.Count < 2)
				Error ("At least two assemblies needed");

			if (context.OutputPath == "")
				Error ("Please set output filename");

			Process (context);
		}

		static void Process (MergeContext context)
		{
			AssemblyDefinition primary = AssemblyFactory.GetAssembly (context.Assemblies [0]);

			for (int i = 1; i < context.Assemblies.Count; i++) {
				AssemblyDefinition asm = AssemblyFactory.GetAssembly (context.Assemblies [i]);
				asm.Accept (new StructureMerger (context, primary, asm));
			}

			FixReflectionAfterMerge fix = new FixReflectionAfterMerge (context, primary, primary);
			fix.Process ();

			AssemblyFactory.SaveAssembly (primary, context.OutputPath);
		}

		static void Usage ()
		{
			Console.WriteLine (merge_name);
			Console.WriteLine ("monomerge [options] -out result_file primary assemly [files...]");

			Console.WriteLine ("   --about     About the {0}", merge_name);
			Console.WriteLine ("   --version   Print the version number of the {0}", merge_name);
			Console.WriteLine ("   -out        Specify the output file");
			Console.WriteLine ("");
			Console.WriteLine ("   Sample: monomerge -out output.exe input.exe input_lib.dll");

			Environment.Exit (1);
		}

		static void Version ()
		{
			Console.WriteLine (
				"{0} Version {1}", merge_name,
				System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version);

			Environment.Exit (1);
		}

		static void About ()
		{
			Console.WriteLine ("For more information, visit the project Web site");
			Console.WriteLine ("	 http://www.mono-project.com/");

			Environment.Exit (1);
		}

		static void Error (string Message)
		{
			Console.WriteLine ("Error : {0}", Message);
			Environment.Exit (1);
		}
	}
}
