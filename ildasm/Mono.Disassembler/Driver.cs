//
// Driver.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
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

namespace Mono.Disassembler {

	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;

	using Mono.Cecil;

	class Driver {

		enum Output {
			Gui,
			File,
			Console
		}

		enum OutputEncoding {
			ASCII,
			UTF8,
			Unicode
		}

		static void Main (string [] args)
		{
			Driver drv = new Driver (args);
			drv.Run ();
			Environment.Exit (0);
		}

		string [] m_args;
		Output m_output = Output.Console;
		OutputEncoding m_encoding = OutputEncoding.ASCII;
		string m_assembly;
		string m_outputFile;
		string m_error;

		Driver (string [] args)
		{
			m_args = args;
		}

		void Run ()
		{
			try {
				Parse (m_args);
				switch (m_output) {
				case Output.Gui:
					throw new NotImplementedException ("GUI is not implemented");
				case Output.Console:
					WriteDisassembly (Console.Out);
					break;
				case Output.File:
					FileStream fs = new FileStream (m_outputFile, FileMode.Create, FileAccess.Write, FileShare.None);
					Encoding enc = null;
					if (m_encoding == OutputEncoding.Unicode)
						enc = Encoding.Unicode;
					else if (m_encoding == OutputEncoding.UTF8)
						enc = Encoding.UTF8;
					else
						enc = Encoding.ASCII;

					using (StreamWriter sw = new StreamWriter (fs, enc))
						WriteDisassembly (sw);

					break;
				}
			} catch (Exception e) {
				m_error = e.ToString ();
				Usage ();
			}
		}

		void WriteDisassembly (TextWriter writer)
		{
			CilWriter cw = new CilWriter (writer);
			StructureDisassembler sd = new StructureDisassembler ();
			sd.DisassembleAssembly (AssemblyFactory.GetAssembly (m_assembly), cw);
		}

		string AssemblyName {
			get { return m_assembly; }
			set {
				if (m_assembly != null) {
					m_error = "Multiple input files specified !";
					return;
				}
					
				m_assembly = value;
				if (!File.Exists (m_assembly)) {
					m_error = "Specified file does not exists !";
				}
			}
		}

		void Parse (string [] args)
		{
			string cmd_arg;
			foreach (string cmd in args) {
				if (cmd [0] != '-' && cmd [0] != '/') {
					AssemblyName = cmd;
					continue;
				}

				switch (GetCommand (cmd, out cmd_arg)) {
				case "text":
					m_output = Output.Console;
					break;
				case "output":
					m_output = Output.File;
					m_outputFile = cmd_arg;
					break;
				case "utf8":
					m_encoding = OutputEncoding.UTF8;
					break;
				case "unicode":
					m_encoding = OutputEncoding.Unicode;
					break;
				case "-about":
					About ();
					break;
				case "-version":
					Version ();
					break;
				default:
					if (cmd [0] == '-')
						break;
					break;	
				}
			}

			if (args.Length == 0 || m_error != null)
				Usage ();
		}

		string GetCommand (string cmd, out string arg)
		{
			int sep = cmd.IndexOfAny (new char [] {':', '='}, 1);
			if (sep == -1) {
				arg = null;
				return cmd.Substring (1);
			}

			string command = cmd.Substring (1, sep - 1);
			arg = cmd.Substring (sep + 1);
			return command.ToLower ();
		}

		void Usage ()
		{
			Console.WriteLine (
				"Mono CIL Disassembler");
			if (m_error != null)
				Console.WriteLine ("\n{0}\n", m_error);
			Console.WriteLine (
				"ildasm [options] assembly\n" +
				"   --about\t\t\tabout Mono CIL Disassembler\n" +
				"   --version\t\t\tprint version of the Mono CIL Disassembler\n" +
				"   -output:filename\t\tprint disassembly into filename\n" +
				"   -text\t\t\tprint disassembly into the console\n" +
				"   -utf8\t\t\tuse utf8 encoding instead of ASCII\n" +
				"   -unicode\t\t\tuse unicode encoding instead of ASCII\n" +
				"");
			Environment.Exit (1);
		}

		void Version ()
		{
			Console.WriteLine ("Mono CIL Disassembler version {0}",
				Assembly.GetExecutingAssembly ().GetName ().Version.ToString ());
			Environment.Exit (0);
		}

		void About ()
		{
			Console.WriteLine (
				"Mono CIL Disassembler" +
				"For more information on Mono, visit\n" +
				"   http://www.mono-project.com\n" +
				"");
			Environment.Exit (0);
		}
	}
}
