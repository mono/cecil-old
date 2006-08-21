//
// SweepStep.cs
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

	public class SweepStep : IStep {

		public void Process (LinkContext context)
		{
			foreach (AssemblyMarker am in context.GetAssemblies ())
				SweepAssembly (am);
		}

		void SweepAssembly(AssemblyMarker am)
		{
			Hashtable typesUsed = new Hashtable ();
			foreach (TypeMarker tm in am.GetTypes ()) {
				SweepType (tm);
				typesUsed.Add (tm.Type.ToString (), tm.Type);
			}

			foreach (TypeDefinition type in new ArrayList (am.Assembly.MainModule.Types))
				if (!typesUsed.Contains (type.ToString ()))
					am.Assembly.MainModule.Types.Remove (type);
		}

		void SweepType (TypeMarker tm)
		{
			SweepMethods (tm);
			SweepFields (tm);
		}

		void SweepFields (TypeMarker tm)
		{
			Hashtable fieldsUsed = new Hashtable ();
			foreach (FieldMarker fm in tm.GetFields())
				fieldsUsed.Add (fm.Field.ToString (), fm.Field);

			foreach (FieldDefinition field in new ArrayList (tm.Type.Fields))
				if (!fieldsUsed.Contains (field.ToString ()))
					tm.Type.Fields.Remove (field);
		}

		void SweepMethods (TypeMarker tm)
		{
			Hashtable methodsUsed = new Hashtable ();
			foreach (MethodMarker mm in tm.GetMethods ())
				methodsUsed.Add (mm.Method.ToString (), mm.Method);

			foreach (MethodDefinition meth in new ArrayList (tm.Type.Methods))
				if (!methodsUsed.Contains (meth.ToString ()))
					tm.Type.Methods.Remove (meth);

			foreach (MethodDefinition ctor in new ArrayList (tm.Type.Constructors))
				if (!methodsUsed.Contains (ctor.ToString ()))
					tm.Type.Constructors.Remove (ctor);
		}
	}
}
