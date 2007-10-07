// 
// Gendarme.Rules.Naming.DetectNonAlphaNumericsInTypeNamesRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//
// Copyright (c) <2007> Nidhi Rawal
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
using System.Collections;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Naming {

	public class DetectNonAlphaNumericsInTypeNamesRule: IMethodRule, ITypeRule {
		
		private bool IsPropertyEventOrOperator (MethodDefinition method)
		{
			if (method.SemanticsAttributes.ToString () == "Getter" || method.SemanticsAttributes.ToString () == "Setter" || method.SemanticsAttributes.ToString() == "AddOn" || method.SemanticsAttributes.ToString () == "RemoveOn" || method.Name == "op_Implicit" || method.Name == "op_Explicit")
				return true;
			else
				return false;
		}
			
		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			string [] access_spec  = type.Attributes.ToString ().Split(',');
			MessageCollection messageCollection = new MessageCollection ();
			
			// Compiler generates an error for any other non alpha-numerics than underscore ('_'), so we just need to check the presence of underscore in type names 
			
			if (access_spec[0] != "NestedPrivate" && type.Name.IndexOf ("_") != -1)
			{
				Location location = new Location (type.FullName, type.Name, 0);
				Message message = new Message ("Type name contains underscore", location, MessageType.Error);
				messageCollection.Add (message);
			}
			
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
		
		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			string [] access_spec  = method.Attributes.ToString ().Split(',');
			MessageCollection messageCollection = new MessageCollection ();
			
			if(!IsPropertyEventOrOperator(method))
			{
				// Compiler generates an error for any other non alpha-numerics than underscore ('_'), so we just need to check the presence of underscore in method names
				if (access_spec[0] != "Private" && method.Name.IndexOf("_") != -1)
				{
					Location location = new Location (method.DeclaringType.ToString(), method.Name, 0);
					Message message = new Message ("Method name contains underscore", location, MessageType.Error);
					messageCollection.Add (message);
				}
			}
			
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}
