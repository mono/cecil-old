//
// ImportReflectionTestFixture.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2008 Novell, Inc (http://www.novell.com)
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
using SR = System.Reflection;

using Mono.Cecil;

using NUnit.Framework;

namespace Mono.Cecil.Tests {

	[TestFixture]
	public class ImportReflectionTestFixture {

		[Test]
		public void ImportVoid ()
		{
			TypeReference type = TestImport (typeof (void));
			Assert.AreEqual ("System.Void", type.FullName);
		}

		[Test]
		public void ImportInt32 ()
		{
			TypeReference type = TestImport (typeof (int));
			Assert.AreEqual ("System.Int32", type.FullName);
		}

		static TypeReference TestImport (Type type)
		{
			AssemblyDefinition assembly = GetAssembly();
			TypeReference reference = assembly.MainModule.Import (type);
			assembly.MainModule.Import (type);
			CheckForDuplicates (assembly);

			return reference;
		}

		static MethodReference TestImport (SR.MethodBase method)
		{
			AssemblyDefinition assembly = GetAssembly();
			MethodReference reference = assembly.MainModule.Import (method);
			assembly.MainModule.Import (method);
			CheckForDuplicates (assembly);

			return reference;
		}

		static FieldReference TestImport (SR.FieldInfo field)
		{
			AssemblyDefinition assembly = GetAssembly();
			FieldReference reference = assembly.MainModule.Import (field);
			assembly.MainModule.Import (field);
			CheckForDuplicates (assembly);

			return reference;
		}

		static void CheckForDuplicates (AssemblyDefinition assembly)
		{
			CheckTypeReferenceDuplicates (assembly.MainModule.TypeReferences);
			CheckMemberReferenceDuplicates (assembly.MainModule.MemberReferences);
		}

		static void CheckTypeReferenceDuplicates (TypeReferenceCollection references)
		{
			List<string> names = new List<string> ();
			foreach (TypeReference reference in references) {
				Assert.IsFalse (names.Contains (reference.FullName));
				names.Add (reference.FullName);
			}
		}

		static void CheckMemberReferenceDuplicates (MemberReferenceCollection references)
		{
			List<string> names = new List<string> ();
			foreach (MemberReference reference in references) {
				Assert.IsFalse (names.Contains (reference.ToString ()));
				names.Add (reference.ToString ());
			}
		}

		static AssemblyDefinition GetAssembly ()
		{
			return AssemblyFactory.DefineAssembly ("foo", AssemblyKind.Dll);
		}
	}
}
