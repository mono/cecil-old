//
// CodeDomScript.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
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

using System.CodeDom.Compiler;
using System.Text;
using NUnit.Framework;

namespace Mono.Bat {

	public abstract class CSharpScript : CodeDomScript {

		public override string Extension {
			get { return ".cs"; }
		}
	}

	public abstract class CodeDomScript : IScript {

		string code;

		public string CODE {
			get { return code; }
			set { code = value; }
		}

		public abstract string Extension { get; }

		public abstract void Run ();

		public void CompileTo (string file)
		{
			using (CodeDomProvider provider = GetProvider ()) {
				CompilerParameters parameters = GetDefaultParameters ();
				parameters.IncludeDebugInformation = false;
				parameters.GenerateExecutable = true;
				parameters.OutputAssembly = file;

				CompilerResults results = provider.CompileAssemblyFromSource (parameters, code);
				AssertCompilerResults (results);
			}
		}

		static void AssertCompilerResults (CompilerResults results)
		{
			Assert.AreEqual (0, results.Errors.Count, GetErrorMessage (results));
		}

		static string GetErrorMessage (CompilerResults results)
		{
			if (!results.Errors.HasErrors)
				return string.Empty;

			StringBuilder sb = new StringBuilder ();
			foreach (CompilerError error in results.Errors)
				sb.AppendLine (error.ToString ());
			return sb.ToString ();
		}

		CompilerParameters GetDefaultParameters ()
		{
			return GetCompilerInfo ().CreateDefaultCompilerParameters ();
		}

		CodeDomProvider GetProvider ()
		{
			return GetCompilerInfo ().CreateProvider ();
		}

		CompilerInfo GetCompilerInfo ()
		{
			return CodeDomProvider.GetCompilerInfo (CodeDomProvider.GetLanguageFromExtension (Extension));
		}
	}
}
