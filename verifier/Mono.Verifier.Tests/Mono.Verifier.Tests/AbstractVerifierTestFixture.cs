//
// AbstractVerifierTestFixture.cs
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
using Mono.Bat;
using Mono.Cecil;

namespace Mono.Verifier.Tests {

	public abstract class AbstractVerifierTestFixture : AbstractAssemblyTestFixture {

		protected void RunVerifierTestCase (string file)
		{
			string boo = GetBooFile (file);
			string asm = GetTempFileName ();
			WriteAssemblyScript was = CompileBooFile<WriteAssemblyScript> (boo);
			was.DefineAssembly (file);
			was.Run ();
			AssemblyFactory.SaveAssembly (was.ASM, asm);

			IVerifier verifier = new ManagedVerifier (AssemblyFactory.GetAssembly (asm));
			VerifierResult result = verifier.Run ();

			RunAndAssertOutput (boo, delegate
			{
				foreach (ResultItem item in result.GetItems ())
					Console.WriteLine (item);
			});
		}
	}
}
