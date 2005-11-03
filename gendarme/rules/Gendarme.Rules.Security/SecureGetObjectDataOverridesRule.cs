//
// Gendarme.Rules.Security.SecureGetObjectDataOverridesRule
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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
using System.Security;
using System.Security.Permissions;
using System.Text;

using Mono.Cecil;
using Gendarme.Framework;

namespace Gendarme.Rules.Security {

	public class SecureGetObjectDataOverridesRule: IMethodRule {

		static PermissionSet _ruleSet;

		static PermissionSet RuleSet {
			get {
				if (_ruleSet == null) {
					SecurityPermission sp = new SecurityPermission (SecurityPermissionFlag.SerializationFormatter);
					_ruleSet = new PermissionSet (PermissionState.None);
					_ruleSet.AddPermission (sp);
				}
				return _ruleSet;
			}
		}

		public bool CheckMethod (IAssemblyDefinition assembly, IModuleDefinition module, ITypeDefinition type, IMethodDefinition method)
		{
			// check that the method is called "GetObjectData"
			if (method.Name != "GetObjectData")
				return true;

			// check parameters
			if (method.Parameters.Count != 2)
				return true;
			if (method.Parameters[0].ParameterType.ToString () != "System.Runtime.Serialization.SerializationInfo")
				return true;
			if (method.Parameters[1].ParameterType.ToString () != "System.Runtime.Serialization.StreamingContext")
				return true;

			// check for ISerializable
			bool iserialize = true; // FIXME
			if (type.Interfaces.Count > 0) {
				// check if the type implements the "ISerializable" interface
			}
			if (!iserialize) {
				// then it's (recursively) base type may implements the "ISerializable" interface
			}

			// *** ok, the rule applies! ***

			// is there any security applied ?
			if (method.SecurityDeclarations.Count < 1)
				return false;

			// the SerializationFormatter must be a subset of the one (of the) demand(s)
			foreach (ISecurityDeclaration declsec in method.SecurityDeclarations) {
				switch (declsec.Action) {
				case Mono.Cecil.SecurityAction.Demand:
				case Mono.Cecil.SecurityAction.NonCasDemand:
				case Mono.Cecil.SecurityAction.LinkDemand:
				case Mono.Cecil.SecurityAction.NonCasLinkDemand:
					if (RuleSet.IsSubsetOf (declsec.PermissionSet))
						return true;
					break;
				}
			}

			return false;
		}
	}
}
