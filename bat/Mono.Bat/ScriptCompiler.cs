//
// ScriptCompiler.cs
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

using System.IO;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using NUnit.Framework;

namespace Mono.Bat {

	public class ScriptCompiler<TScript> where TScript : IScript {

		string _file;
		BooCompiler _compiler;
		InstantiateScript<TScript> _instantiater;

		public ScriptCompiler (string file)
		{
			_file = file;
			SetupCompiler (new CompileToFile ());
		}

		public TScript Compile ()
		{
			CompilerContext result = _compiler.Run ();
			AssertCompilerResult (result);
			return _instantiater.Script;
		}

		static void AssertCompilerResult (CompilerContext result)
		{
			Assert.AreEqual (0, result.Errors.Count, result.Errors.ToString (true));
		}

		void SetupCompiler (CompilerPipeline pipeline)
		{
			_compiler = new BooCompiler ();
			_compiler.Parameters.Input.Add (new FileInput (_file));
			_compiler.Parameters.OutputType = CompilerOutputType.Library;
			_compiler.Parameters.OutputAssembly = Path.GetTempFileName ();
			_compiler.Parameters.References.Add (typeof (ScriptCompiler<>).Assembly);
			_compiler.Parameters.References.Add (typeof (Mono.Cecil.AssemblyFactory).Assembly);
			_compiler.Parameters.References.Add (typeof (NUnit.Framework.Assert).Assembly);

			_compiler.Parameters.Pipeline = pipeline;
			pipeline.Insert (1, new PrepareScript<TScript> ());
			_instantiater = new InstantiateScript<TScript> ();
			pipeline.Add (_instantiater);
		}
	}
}
