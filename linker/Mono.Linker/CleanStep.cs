//
// CleanStep.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.Linker {

	using System.Collections;

	using Mono.Cecil;

	public class CleanStep : IStep {

		public void Process (LinkContext context)
		{
			foreach (AssemblyMarker am in context.GetAssemblies())
				CleanAssembly (am.Assembly);
		}

		static void CleanAssembly (AssemblyDefinition asm)
		{
			foreach (TypeDefinition type in asm.MainModule.Types)
				CleanType (type);
		}

		static void CleanType (TypeDefinition type)
		{
			CleanNestedTypes (type);
			CleanProperties (type);
			CleanEvents (type);
		}

		static void CleanNestedTypes (TypeDefinition type)
		{
			foreach (TypeDefinition nested in new ArrayList (type.NestedTypes))
				if (!type.Module.Types.Contains (nested))
					type.NestedTypes.Remove (nested);
		}

		static void CleanEvents (TypeDefinition type)
		{
			ArrayList events = new ArrayList (type.Events);
			foreach (EventDefinition evt in events) {
				if (evt.AddMethod != null && !type.Methods.Contains (evt.AddMethod))
					evt.AddMethod = null;
				if (evt.InvokeMethod != null && !type.Methods.Contains (evt.InvokeMethod))
					evt.InvokeMethod = null;
				if (evt.RemoveMethod != null && !type.Methods.Contains (evt.RemoveMethod))
					evt.RemoveMethod = null;

				if (evt.AddMethod == null && evt.InvokeMethod == null && evt.RemoveMethod == null)
					type.Events.Remove (evt);
			}
		}

		static void CleanProperties (TypeDefinition type)
		{
			ArrayList properties = new ArrayList (type.Properties);
			foreach (PropertyDefinition prop in properties) {
				if (prop.GetMethod != null && !type.Methods.Contains (prop.GetMethod))
					prop.GetMethod = null;
				if (prop.SetMethod != null && !type.Methods.Contains (prop.SetMethod))
					prop.SetMethod = null;

				if (prop.GetMethod == null && prop.SetMethod == null)
					type.Properties.Remove (prop);
			}
		}
	}
}
