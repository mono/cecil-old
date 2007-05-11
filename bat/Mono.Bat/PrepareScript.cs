//
// PrepareScript.cs
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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Mono.Bat {

	public class PrepareScript<TScript> : AbstractCompilerStep where TScript : IScript {

		public override void Run ()
		{
			Module module = GetModule ();
			CreateImports (module);

			Method run = CreateRunMethod (module);

			ClassDefinition script = CreateScript (module);
			script.Members.Add (run);

			module.Members.Clear();
			module.Members.Add (script);
		}

		static void CreateImports (Module module)
		{
			module.Imports.Add (CreateImport ("Mono.Bat"));
			module.Imports.Add (CreateImport ("Mono.Cecil"));
			module.Imports.Add (CreateImport ("Mono.Cecil.Cil"));
		}

		static Import CreateImport (string ns)
		{
			Import i = new Import ();
			i.Namespace = ns;
			return i;
		}

		static ClassDefinition CreateScript (Module module)
		{
			ClassDefinition script = new ClassDefinition ();
			script.Name = "__Script__";
			script.BaseTypes.Add (new SimpleTypeReference (typeof (TScript).FullName));
			script.Members.Extend (module.Members);
			return script;
		}

		static Method CreateRunMethod (Module module)
		{
			Method run = new Method ("Run");
			run.Modifiers = TypeMemberModifiers.Override;
			run.Body = module.Globals;
			module.Globals = new Block ();
			return run;
		}

		Module GetModule ()
		{
			return CompileUnit.Modules [0];
		}
	}
}
