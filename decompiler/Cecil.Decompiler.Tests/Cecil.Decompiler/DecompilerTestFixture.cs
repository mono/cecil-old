#region license
//
// (C) db4objects Inc. http://www.db4o.com
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
#endregion

using System;
using System.Diagnostics;
using System.IO;
using Mono.Cecil;
using NUnit.Framework;

namespace Cecil.Decompiler.Tests {

	public abstract class DecompilerTestFixture {

		protected static string Normalize (string s)
		{
			return s.Trim ().Replace ("\r\n", "\n");
		}

		private void CompileTestCase (string name)
		{
			string sourceFile = MapTestCasePath (name + ".il");
			Assert.IsTrue (File.Exists (sourceFile), sourceFile + " not found!");
			ilasm (string.Format ("/DLL \"/OUTPUT:{0}\" {1}", TestAssemblyPath, sourceFile));
		}

		protected string LoadTestCaseFile (string fname)
		{
			using (StreamReader reader=File.OpenText (MapTestCasePath (fname))) {
				return reader.ReadToEnd ();
			}
		}

		protected string MapTestCasePath (string name)
		{
			return Path.Combine (TestCasesDirectory, name);
		}

		static void ilasm (string arguments)
		{
			Process p = new Process ();
			p.StartInfo.Arguments = arguments;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = "ilasm";
			p.Start ();
			string output = p.StandardOutput.ReadToEnd ();
			string error = p.StandardError.ReadToEnd ();
			p.WaitForExit ();
			Assert.AreEqual (0, p.ExitCode, output + error);
		}

		protected MethodDefinition LoadTestCaseMethod (string testCaseName)
		{
			CompileTestCase (testCaseName);

			AssemblyDefinition assembly = AssemblyFactory.GetAssembly (TestAssemblyPath);
			TypeDefinition type = assembly.MainModule.Types ["TestCase"];
			Assert.IsNotNull (type, "Type TestCase not found!");
			MethodDefinition[] found = type.Methods.GetMethod ("Main");
			Assert.AreEqual (1, found.Length, "Method TestCase.Main not found!");
			return found [0];
		}

		public virtual string TestCasesDirectory {
			get { return Path.GetFullPath (FindTestcasesDirectory ()); }
		}

		static string FindTestcasesDirectory()
		{
			string currentPath = Environment.CurrentDirectory;
			while (!Directory.Exists(Path.Combine(currentPath, "TestCases")))
			{
				string oldPath = currentPath;
				currentPath = Path.GetDirectoryName(currentPath);
				Assert.AreNotEqual(oldPath, currentPath);
			}
			return Path.Combine(currentPath, "TestCases");
		}

		public string TestAssemblyPath {
			get {
				return Path.Combine (Path.GetTempPath (), "TestCase.dll");
			}
		}
	}
}
