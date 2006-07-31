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

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	public class Driver {

		public static void Main (string [] args)
		{
			AssemblyDefinition asm = AssemblyFactory.GetAssembly (args [0]);
			LinkContext context = new LinkContext ();
			context.OutputDirectory = "lnk";
			context.PreserveCoreLibraries = true;

			AssemblyMarker marker = new AssemblyMarker (AssemblyAction.Preserve, asm);
			foreach (TypeDefinition type in asm.MainModule.Types) {
				TypeMarker tm = marker.Mark (type);

				foreach (MethodDefinition meth in type.Methods)
					tm.Mark (meth, MethodAction.ForceParse);
				foreach (MethodDefinition ctor in type.Constructors)
					tm.Mark (ctor, MethodAction.ForceParse);
			}

			context.AddMarker (marker);

			Pipeline p = new Pipeline ();
			p.AddStep (new MarkStep ());
			p.AddStep (new PrintStep (Console.Out));
			p.AddStep (new SweepStep ());
			p.AddStep (new OutputStep ());

			p.Process (context);
		}
	}
}