//
// AbstractAssemblyTestFixture.cs
//
// Author:
//   Jb Evain (jb@nurv.fr)
//
// (C) 2007 Jb Evain
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
using Boo.Lang.Parser;
using Mono.Cecil;
using NUnit.Framework;

namespace Mono.Bat {

	[TestFixture]
	public abstract class AbstractAssemblyTestFixture {

		IList<string> _tempFiles = new List<string> ();

		[SetUp]
		public virtual void SetUp ()
		{
			_tempFiles.Clear ();
		}

		[TearDown]
		public virtual void TearDown ()
		{
			foreach (string file in _tempFiles) {
				try {
					File.Delete (file);
				} catch {}
			}
		}

		protected string GetTempFileName ()
		{
			string file = Path.GetTempFileName ();
			_tempFiles.Add (file);
			return file;
		}

		protected void RunReadAssemblyTestCase (string file)
		{
			string boo = GetBooFile (file);
			ReadAssemblyScript ras = CompileBooFile<ReadAssemblyScript> (boo);
			ras.ASM = FindCandidateAssembly (file);
			RunAndAssertOutput (boo, delegate { ras.Run (); });
		}

		protected void RunCSharpRoundTripTestCase (string file)
		{
			string boo = GetBooFile (file);
			CSharpScript cs = CompileBooFile<CSharpScript> (boo);
			string asm = GetTempFileName ();
			cs.Run ();
			cs.CompileTo (asm);

			AssemblyDefinition assembly = AssemblyFactory.GetAssembly (asm);
			string roundtrip = GetTempFileName ();
			AssemblyFactory.SaveAssembly (assembly, roundtrip);

			RunAndAssertOutput(boo, delegate { AppDomain.CurrentDomain.ExecuteAssembly (roundtrip); });
		}

		string GetBooFile (string file)
		{
			return GetTestCasePath (file + ".boo");
		}

		protected AssemblyDefinition RunWriteAssemblyTestCase (string file)
		{
			string boo = GetBooFile (file);
			string asm = GetTempFileName ();
			WriteAssemblyScript was = CompileBooFile<WriteAssemblyScript> (boo);
			was.DefineAssembly (file);
			was.Run ();
			AssemblyFactory.SaveAssembly (was.ASM, asm);
			RunAndAssertOutput(boo, delegate { AppDomain.CurrentDomain.ExecuteAssembly (asm); });
			return was.ASM;
		}

		TScript CompileBooFile<TScript> (string file) where TScript : IScript
		{
			ScriptCompiler<TScript> compiler = new ScriptCompiler<TScript> (file, GetTempFileName ());
			return compiler.Compile ();
		}

		static void RunAndAssertOutput (string file, Action a)
		{
			StringWriter sw = new StringWriter ();
			ChangeStandardOutput (sw, a);
			AssertScriptOutput (file, sw.ToString ());
		}

		delegate void Action ();

		static void ChangeStandardOutput (TextWriter writer, Action a)
		{
			TextWriter stdout = Console.Out;
			try {
				Console.SetOut (writer);
				a ();
			} finally {
				Console.SetOut (stdout);
			}
		}

		AssemblyDefinition FindCandidateAssembly (string file)
		{
			string fullpath = GetTestCasePath (file);
			if (File.Exists (fullpath + ".dll"))
				return AssemblyFactory.GetAssembly (fullpath + ".dll");
			else if (File.Exists (fullpath + ".exe"))
				return AssemblyFactory.GetAssembly (fullpath + ".exe");

			throw new FileNotFoundException ("Can not find assembly: " + file);
		}

		static void AssertScriptOutput (string file, string output)
		{
			Assert.AreEqual (
				NormalizeWhitespace (GetExpectedOutput (file)),
				NormalizeWhitespace (output));
		}

		static string GetExpectedOutput (string path)
		{
			return BooParser.ParseFile (path).Modules [0].Documentation;
		}

		static string NormalizeWhitespace (string s)
		{
			return s.Trim ().Replace ("\r\n", "\n");
		}

		protected virtual string GetTestCasePath (string file)
		{
			return Path.Combine (FindTestcasesDirectory (), file);
		}

		static string FindTestcasesDirectory ()
		{
			string currentPath = Environment.CurrentDirectory;
			while (!Directory.Exists (Path.Combine (currentPath, "TestCases"))) {
				string oldPath = currentPath;
				currentPath = Path.GetDirectoryName (currentPath);
				Assert.AreNotEqual (oldPath, currentPath);
			}
			return Path.Combine (currentPath, "TestCases");
		}
	}
}
