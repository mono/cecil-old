//
// Gendarme.Rules.Ui.SystemWindowsFormsExecutableTargetRule
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

	public class SystemWindowsFormsExecutableTargetRule: ExecutableTargetRule {

		protected override bool CheckReferences (AssemblyDefinition assembly)
		{
			foreach (AssemblyNameReference a in assembly.MainModule.AssemblyReferences) {
				// check name and public key token (but not version or culture)
				if (a.Name == "System.Windows.Forms") {
					byte[] token = a.PublicKeyToken;
					if (token != null) {
						if ((token[0] == 0xb7) && (token[1] == 0x7a) &&
						    (token[2] == 0x5c) && (token[3] == 0x56) &&
						    (token[4] == 0x19) && (token[5] == 0x34) &&
						    (token[6] == 0xe0) && (token[7] == 0x89))
							return true;
					}
				}
			}
			return false;
		}

		protected override string Toolkit {
			get { return "WinForms"; }
		}
	}
}
