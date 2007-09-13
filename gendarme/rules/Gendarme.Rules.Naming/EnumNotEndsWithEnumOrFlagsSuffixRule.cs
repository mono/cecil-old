//
// EnumNotEndsWithEnumOrFlagsSuffixRule class
//
// Authors:
//	Néstor Salceda <nestor.salceda@gmail.com>
//
// 	(C) 2007 Néstor Salceda
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

namespace Gendarme.Rules.Naming {
	public class EnumNotEndsWithEnumOrFlagsSuffixRule : ITypeRule {
		
		private bool HasFlagsAttribute (TypeDefinition typeDefinition) 
		{
			foreach (CustomAttribute customAttribute in typeDefinition.CustomAttributes) {
				if (customAttribute.Constructor.DeclaringType.FullName == "System.FlagsAttribute")
					return true;
			}
			return false;
		}
		
		private bool EndsWithEnumSuffix (string typeName) 
		{
			return typeName.EndsWith ("Enum") || typeName.ToLower ().EndsWith ("enum");
		}
		
		private bool EndsWithFlagsSuffix (string typeName) 
		{
			return typeName.EndsWith ("Flags") || typeName.ToLower ().EndsWith ("flags");			
		}
		
		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner)
		{	
			MessageCollection messageCollection = new MessageCollection ();
			if (typeDefinition.IsEnum) {
				if (!HasFlagsAttribute (typeDefinition)) {
					if (EndsWithEnumSuffix (typeDefinition.Name)) {
						Location location = new Location (typeDefinition.FullName, typeDefinition.Name, 0);
						Message message = new Message ("The class name ends with Enum Suffix", location, MessageType.Error);
						messageCollection.Add (message);  
					}
				}
				else {
					if (EndsWithFlagsSuffix (typeDefinition.Name)){
						Location location = new Location (typeDefinition.FullName, typeDefinition.Name, 0);
						Message message = new Message ("The class name ends with Flags Suffix", location, MessageType.Error);
						messageCollection.Add (message);
					}
				}
			}
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}