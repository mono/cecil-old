// 
// Gendarme.Rules.BadPractice.ImplementingEqualsButNotGetHashCodeAndViceVersaRule
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

namespace Gendarme.Rules.BadPractice {

	public class ImplementingEqualsButNotGetHashCodeAndViceVersaRule: ITypeRule
	{
		bool equals = false;
		bool getHashCode = false;
		
		private void EqualsOrGetHashCode (TypeDefinition type)
		{
			equals = false;
			getHashCode = false;
			
			foreach (MethodDefinition method in type.Methods) {
				if (method.Name == "Equals" && method.Parameters.Count == 1)
					equals = true;
				else if (method.Name == "GetHashCode" && method.Parameters.Count == 0)
					getHashCode = true;
			}
		}
		
		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection ();
			EqualsOrGetHashCode (type);
			
			if (equals == true && getHashCode == false)
			{
				Location location = new Location (type.FullName, type.Name, 0);
				Message message = new Message ("Implements Object.Equals (Object) but does not implement Object.GetHashCode ()", location, MessageType.Error);
				messageCollection.Add (message);
			}
			else if (equals == false && getHashCode == true)
			{
				Location location = new Location (type.FullName, type.Name, 0);
				Message message = new Message ("Implements Object.GetHashCode () but does not implement Object.Equals (Object)", location, MessageType.Error);
				messageCollection.Add (message);
			}
			else
				return runner.RuleSuccess;
			
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}
