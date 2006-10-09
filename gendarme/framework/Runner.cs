//
// Gendarme.Framework.Runner class
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
using System.IO;
using System.Collections;
using System.Reflection;

using Mono.Cecil;

namespace Gendarme.Framework {

	public abstract class Runner {

		private Rules rules;
		private Violations violations;
		private static MessageCollection failure = new MessageCollection();
		protected bool debug = false;

		public Rules Rules {
			get {
				if (rules == null)
					rules = new Rules ();
				return rules;
			}
		}

		public Violations Violations {
			get {
				if (violations == null)
					violations = new Violations ();
				return violations;
			}
		}

		public bool Debug {
			get {
				return debug;
			}
		}

		public MessageCollection RuleSuccess {
			get {
				return null;
			}
		}

		public MessageCollection RuleFailure {
			get {
				return failure;
			}
		}

		private IRule CreateRuleFromType (Type type)
		{
			return (IRule) Activator.CreateInstance (type);
		}

		public int LoadRulesFromAssembly (string assembly, string includeMask, string excludeMask)
		{
			if (includeMask != "*")
				throw new NotImplementedException ("includeMask");
			if ((excludeMask != null) && (excludeMask.Length > 0))
				throw new NotImplementedException ("excludeMask");

			int total = 0;
			Assembly a = Assembly.LoadFile (Path.GetFullPath (assembly));
			foreach (Type t in a.GetTypes ()) {
				bool added = false;
				if (t.GetInterface ("Gendarme.Framework.IAssemblyRule") != null) {
					Rules.Assembly.Add (CreateRuleFromType (t));
					added = true;
				}
				if (t.GetInterface ("Gendarme.Framework.IModuleRule") != null) {
					Rules.Module.Add (CreateRuleFromType (t));
					added = true;
				}
				if (t.GetInterface ("Gendarme.Framework.ITypeRule") != null) {
					Rules.Type.Add (CreateRuleFromType (t));
					added = true;
				}
				if (t.GetInterface ("Gendarme.Framework.IMethodRule") != null) {
					Rules.Method.Add (CreateRuleFromType (t));
					added = true;
				}
				if (added)
					total++;
			}
			return total;
		}

		public void Process (AssemblyDefinition assembly)
		{
			MessageCollection messages;
			foreach (IAssemblyRule rule in Rules.Assembly) {
				messages = rule.CheckAssembly(assembly, this);
				if (messages != RuleSuccess)
					Violations.Add (rule, assembly, messages);
			}

			foreach (ModuleDefinition module in assembly.Modules) {

				foreach (IModuleRule rule in Rules.Module) {
					messages = rule.CheckModule(assembly, module, this);
					if (messages != RuleSuccess)
						Violations.Add (rule, module, messages);
				}

				foreach (TypeDefinition type in module.Types) {

					foreach (ITypeRule rule in Rules.Type) {
						messages = rule.CheckType(assembly, module, type, this);
						if (messages != RuleSuccess)
							Violations.Add (rule, type, messages);
					}

					foreach (MethodDefinition method in type.Methods) {

						foreach (IMethodRule rule in Rules.Method) {
							messages = rule.CheckMethod(assembly, module, type, method, this);
							if (messages != RuleSuccess)
								Violations.Add (rule, method, messages);
						}
					}
				}
			}
		}
	}
}
