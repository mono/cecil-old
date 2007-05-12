//
// WriteAssemblyScript.cs
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
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mono.Bat {

	public abstract class WriteAssemblyScript : ReadAssemblyScript {

		MethodBody _main;

		public MethodBody Main {
			get { return _main; }
			set { _main = value; }
		}

		public void DefineAssembly (string name)
		{
			ASM = AssemblyFactory.DefineAssembly (name, AssemblyKind.Console);

			TypeDefinition prog = new TypeDefinition ("Program", string.Empty, TypeAttributes.NotPublic, Import (typeof (object)));

			MethodDefinition main = new MethodDefinition ("Main", MethodAttributes.Static | MethodAttributes.Private, Import (typeof (void)));
			main.Parameters.Add (new ParameterDefinition (Import (typeof (string []))));

			prog.Methods.Add (main);

			ASM.MainModule.Types.Add(prog);
			ASM.EntryPoint = main;
			_main = main.Body;
		}

		protected MethodReference ImportConsoleWriteLine ()
		{
			return Import (typeof (Console).GetMethod ("WriteLine", new Type [] { typeof (string) }));
		}

		protected TypeReference Import (Type type)
		{
			return ASM.MainModule.Import(type);
		}

		protected MethodReference Import (System.Reflection.MethodBase method)
		{
			return ASM.MainModule.Import(method);
		}

		protected FieldReference Import (System.Reflection.FieldInfo field)
		{
			return ASM.MainModule.Import(field);
		}
	}
}
