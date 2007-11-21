//
// Gendarme.Rules.Ui.ExecutableTargetRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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
using Gendarme.Framework;

namespace Gendarme.Rules.Ui {

	abstract public class ExecutableTargetRule : IAssemblyRule {

		abstract protected bool CheckReferences (AssemblyDefinition assembly);

		abstract protected string Toolkit { get; }

		public MessageCollection CheckAssembly (AssemblyDefinition assembly, Runner runner)
		{
			// 1. Check entry point, if no entry point then it's not an executable
			if (assembly.EntryPoint == null)
				return runner.RuleSuccess;

			// 2. Check if the assembly references SWF
			if (!CheckReferences (assembly))
				return runner.RuleSuccess;

			// *** ok, the rule applies! ***

			// 3. On Windows a console window will appear if the subsystem isn't Windows
			//    i.e. the assembly wasn't compiled with /target:winexe
			if (assembly.Kind == AssemblyKind.Windows)
				return runner.RuleSuccess;

			string text = String.Format ("This {0} application was compiled without /target:winexe", Toolkit);
			Message msg = new Message (text, null, MessageType.Warning);
			MessageCollection mc = new MessageCollection (msg);
			return mc;
		}
	}
}
