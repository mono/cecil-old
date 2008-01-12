//
// UseCorrectCasingRule class
//
// Authors:
//	Daniel Abramov <ex@vingrad.ru>
//
// 	(C) 2007 Daniel Abramov
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
using System.Collections.Generic;

using Mono.Cecil;
using Gendarme.Framework;

namespace Gendarme.Rules.Naming {

	public class UseCorrectCasingRule : ITypeRule, IMethodRule {

		// check if name is PascalCased
		private static bool IsPascalCase (string name)
		{
			if (name == null || name.Length == 0) {
				return true;
			}
			return Char.IsUpper (name [0]);
		}

		// convert name to PascalCase
		private static string PascalCase (string name)
		{
			if (name == null || name.Length == 0) {
				return name;
			}
			if (name.StartsWith ("_"))
				RemoveLeadingUnderlines (ref name);
			return Char.ToUpper (name [0]) + name.Substring (1);
		}

		// check if name is camelCased
		private static bool IsCamelCase (string name)
		{
			if (name == null || name.Length == 0) {
				return true;
			}
			return Char.IsLower (name [0]);
		}

		// convert name to camelCase
		private static string CamelCase (string name)
		{
			if (name == null || name.Length == 0) {
				return name;
			}
			if (name.StartsWith ("_"))
				RemoveLeadingUnderlines (ref name);
			return Char.ToLower (name [0]) + name.Substring (1);
		}

		private static void RemoveLeadingUnderlines (ref string s)
		{
			int idx = 0;
			while ((s [idx] == '_') && (s.Length > 1))
				idx++;
			s = s.Substring (idx);
		}


		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner)
		{
			// types should all be PascalCased
			if (IsPascalCase (typeDefinition.Name))
				return runner.RuleSuccess;

			if (NamingUtils.IsNameGeneratedByCompiler (typeDefinition))
				return runner.RuleSuccess;

			Location location = new Location (typeDefinition);
			string errorMessage = string.Format ("By existing naming conventions, the type names should all be pascal-cased (e.g. MyClass). Rename '{0}' type to '{1}'.",
							     typeDefinition.Name, PascalCase (typeDefinition.Name));
			return new MessageCollection (new Message (errorMessage, location, MessageType.Error));
		}

		public MessageCollection CheckMethod (MethodDefinition methodDefinition, Runner runner)
		{
			string name = methodDefinition.Name;
			MethodSemanticsAttributes attrs = methodDefinition.SemanticsAttributes;
			MethodSemanticsAttributes mask = MethodSemanticsAttributes.Getter | MethodSemanticsAttributes.Setter
				| MethodSemanticsAttributes.AddOn | MethodSemanticsAttributes.RemoveOn;
			if ((attrs & mask) != 0) {
				// it's something special
				int underscore = methodDefinition.Name.IndexOf ('_');
				if (underscore != -1)
					name = name.Substring (underscore + 1);
			} else if (NamingUtils.IsNameGeneratedByCompiler (methodDefinition)) {
				return runner.RuleSuccess;
			}

			MessageCollection messages = new MessageCollection ();
			Location location = new Location (methodDefinition);

			// like types, methods/props should all be PascalCased, too
			if (!IsPascalCase (name)) {
				string errorMessage = string.Format ("By existing naming conventions, all the method and property names should all be pascal-cased (e.g. MyOperation). Rename '{0}' to '{1}'.",
								     name, PascalCase (name));
				messages.Add (new Message (errorMessage, location, MessageType.Error));
			}
			// check parameters
			List<string> parameterNames = new List<string> ();
			foreach (ParameterDefinition paramDefinition in methodDefinition.Parameters) {
				if (!parameterNames.Contains (paramDefinition.Name)) // somewhy they duplicate sometimes
					parameterNames.Add (paramDefinition.Name);
			}
			foreach (string param in parameterNames) {
				// params should all be camelCased
				if (!IsCamelCase (param)) {
					string errorMessage = string.Format ("By existing naming conventions, the parameter names should all be camel-cased (e.g. myParameter). Rename '{0}' parameter to '{1}'.",
										param, CamelCase (param));
					messages.Add (new Message (errorMessage, location, MessageType.Error));
				}
			}
			if (messages.Count == 0)
				return runner.RuleSuccess;

			return messages;
		}
	}
}
