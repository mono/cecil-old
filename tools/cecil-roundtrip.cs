//
// cecil-roundtrip.cs
// A program that uses the roundtrip ability of Cecil
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

namespace Cecil.Tools {

	using System;

	using Mono.Cecil;

	class Roundtrip {

		static void Main (string [] args)
		{
			if (args.Length != 2) {
				Console.WriteLine ("usage:mono cecil-roundtrip.exe assembly target");
				return;
			}

			try {
				AssemblyDefinition asm = AssemblyFactory.GetAssembly (args [0]);
				AssemblyFactory.SaveAssembly (asm, args [1]);
				Console.WriteLine ("Assembly {0} succesfully roundtripped to {1}",
					asm.Name.FullName, args [1]);
			} catch (Exception e) {
				Console.WriteLine ("Failed to roundtrip assembly {0}", args [0]);
				Console.WriteLine (e);
			}
		}
	}
}
