//
// Gendarme.Rules.Smells.AvoidSpeculativeGeneralityRule class
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
using System.Collections;

using Mono.Cecil;
using Gendarme.Framework;
using Gendarme.Rules.Performance;

namespace Gendarme.Rules.Smells {
	public class AvoidSpeculativeGeneralityRule : ITypeRule {

		private MessageCollection messageCollection;
		
		private void CheckAbstractClassWithoutResponsability (TypeDefinition type) 
		{
			if (type.IsAbstract) {
				ICollection inheritedClasses = Utilities.GetInheritedClassesFrom (type);
				if (inheritedClasses.Count == 1) 
					AddMessage (type.Name, "This abstract class only has one class inheritting from.  The abstract classes without responsability are a sign for the Speculative Generality smell.");
			}
		}

		private void AddMessage (string typeName, string summary) 
		{
			Location location = new Location (typeName, String.Empty, 0);
			Message message = new Message (summary, location, MessageType.Error);
			messageCollection.Add (message);
		}
		
		private int CountClientsFrom (TypeDefinition type) 
		{
			int counter = 0;
			foreach (TypeDefinition client in type.Module.Types) {
				foreach (FieldDefinition field in client.Fields) {
					if (field.FieldType.Equals (type)) 
						counter++;
				}
			}
			return counter;
		}
	
		private void CheckUnnecesaryDelegation (TypeDefinition type) 
		{
			if (CountClientsFrom (type) == 1) 
				AddMessage (type.Name, "This class has only one client.  This unnecesary delegation is a sign for the Speculative Generality smell.");
		}

		private bool ContainsAvoidUnusedParametersRule (Runner runner)
		{
			foreach (IMethodRule rule in runner.Rules.Method) {
				if (rule is AvoidUnusedParametersRule)
					return true;
			}
			return false;
		}

		private void AddExistingMessages (MessageCollection existingMessages) {
			if (existingMessages == null)
				return;

			foreach (Message violation in existingMessages) {
				Message message = new Message ("This method contains unused parameters.  This is a sign for the Speculative Generality smell.",violation.Location, MessageType.Error);
				messageCollection.Add (message);
			}
		}

		private void CheckMethods (IMethodRule rule, ICollection methods, Runner runner) 
		{
			foreach (MethodDefinition method in methods) {
				AddExistingMessages (rule.CheckMethod (method, runner));
			}
		}

		private void CheckUnusedParameters (TypeDefinition type, Runner runner) 
		{
			IMethodRule avoidUnusedParameters = new AvoidUnusedParametersRule ();
			CheckMethods (avoidUnusedParameters, type.Methods, runner);
			CheckMethods (avoidUnusedParameters, type.Constructors, runner);
		}

		public MessageCollection CheckType (TypeDefinition type, Runner runner) 
		{
			messageCollection = new MessageCollection ();
			
			CheckAbstractClassWithoutResponsability (type);
			CheckUnnecesaryDelegation (type);
			if (!ContainsAvoidUnusedParametersRule (runner))
				CheckUnusedParameters (type, runner);

			if (messageCollection.Count != 0)
				return messageCollection;
			return null;
		}
	}
}
