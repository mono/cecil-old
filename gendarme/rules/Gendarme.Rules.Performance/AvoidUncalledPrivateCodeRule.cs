//
// Gendarme.Rules.Performance.AvoidUncalledPrivateCodeRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (c) <2007> Nidhi Rawal
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Performance {

	public class AvoidUncalledPrivateCodeRule: IMethodRule {

		// we should move these methods into an helper class (e.g. MethodHelper)
		// and reuse them in other rules

		public bool IsStaticConstructor (MethodDefinition md)
		{
			return (md.IsStatic && md.IsConstructor);
		}

		public bool IsEntryPoint (MethodDefinition md)
		{
			return (md == md.DeclaringType.Module.Assembly.EntryPoint);
		}

		public bool IsSerializationConstructor (MethodDefinition md)
		{
			if (!md.IsConstructor || (md.Parameters.Count != 2))
				return false;

			if (md.Parameters[0].ParameterType.Name != "System.Runtime.Serialization.SerializationInfo")
				return false;

			return (md.Parameters[1].ParameterType.Name == "System.Runtime.Serialization.StreamingContext");
		}

		// static [void|int] Main ()
		// static [void|int] Main (string[] args)
		public bool IsMain (MethodDefinition md)
		{
			// Main must be static
			if (!md.IsStatic)
				return false;

			if (md.Name != "Main")
				return false;

			// Main must return void or int
			switch (md.ReturnType.ReturnType.Name) {
			case "Void":
			case "Int":
				// ok, continue checks
				break;
			default:
				return false;
			}

			switch (md.Parameters.Count) {
			case 0 :
				// Main (void)
				return true;
			case 1:
				// Main (string[] args)
				return (md.Parameters[0].ParameterType.Name == "String[]");
			default:
				return false;
			}
		}

		public bool IsExplicitImplementationOfInterface (MethodDefinition method)
		{
			// quick out if the name doesn't include a .
			if (!method.Name.Contains ("."))
				return false;

			TypeDefinition type = (method.DeclaringType as TypeDefinition);
			foreach (TypeReference intf in type.Interfaces) {
				if (method.Name.StartsWith (intf.FullName))
					return true;
			}
			return false;
		}

		//

		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			// #1 - rule doesn't apply to static ctor
			if (IsStaticConstructor (method))
				return runner.RuleSuccess;

			// #2 - rule doesn't apply if the method is the assembly entry point
			if (IsEntryPoint (method))
				return runner.RuleSuccess;

			if (IsMain (method))
				return runner.RuleSuccess;


			// ok, the rule applies

			// check if the method is private 
			if (method.IsPrivate) {
				// it's ok for have unused private ctor (and common before static class were introduced in 2.0)
				if (method.IsConstructor)
					return runner.RuleSuccess;

				// it's ok (used or not) if it's required to implement explicitely an interface
				if (IsExplicitImplementationOfInterface (method))
					return runner.RuleSuccess;

				// then we must check if this type use the private method
				if (!CheckTypeForMethodUsage ((method.DeclaringType as TypeDefinition), method)) {
					Location location = new Location (method.DeclaringType.FullName, method.Name, 0);
					Message message = new Message ("The private method code is not used in it's declaring type.", location, MessageType.Error);
					return new MessageCollection (message);
				}
			}

			// check if method is internal
			if (method.IsAssembly) {
				// then we must check if something in the assembly is using this method
				if (!CheckAssemblyForMethodUsage (method.DeclaringType.Module.Assembly, method)) {
					Location location = new Location (method.DeclaringType.FullName, method.Name, 0);
					Message message = new Message ("The internal method code is not used in it's declaring assembly.", location, MessageType.Error);
					return new MessageCollection (message);
				}
			}

			// then method is accessible
			return runner.RuleSuccess;
		}

		private bool CheckAssemblyForMethodUsage (AssemblyDefinition ad, MethodDefinition md)
		{
			// scan each module
			foreach (ModuleDefinition module in ad.Modules) {
				// scan each type
				foreach (TypeDefinition type in module.Types) {
					if (CheckTypeForMethodUsage (type, md))
						return true;
				}
			}
			return false;
		}

		private bool CheckTypeForMethodUsage (TypeDefinition td, MethodDefinition md)
		{
			// check every constructor for the type
			foreach (MethodDefinition ctor in td.Constructors) {
				// skip ourself
				if (ctor == md)
					continue;
				if (CheckMethodUsage (ctor, md))
					return true;
			}
			// check every method for the type
			foreach (MethodDefinition method in td.Methods) {
				// skip check ourself (even with recursion if no one call us then it's still unused)
				if (method == md)
					continue;
				if (CheckMethodUsage (method, md))
					return true;
			}
			return false;
		}

		private bool CheckMethodUsage (MethodDefinition method, MethodDefinition md)
		{
			if (!method.HasBody)
				return false;

			foreach (Instruction instruction in method.Body.Instructions) {
				if (instruction.Operand == md)
					return true;
				if (instruction.OpCode.Code == Code.Callvirt) {
					foreach (MethodReference virtmd in md.Overrides) {
						if (instruction.Operand == virtmd)
							return true;
					}
				}
			}
			return false;
		}
	}
}
