//
// AbstractLinkingTestFixture.cs
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

namespace Mono.Linker.Tests {

	using System.IO;

	using Mono.Cecil;

	using NUnit.Framework;

	[TestFixture]
	public abstract class AbstractLinkingTestFixture : AbstractTestFixture {

		[SetUp]
		public override void SetUp ()
		{
			base.SetUp ();

			TestCasesRoot = "Linker";
		}

		[TearDown]
		public override void TearDown ()
		{
			base.TearDown ();

			string output = GetOutputPath ();
			if (Directory.Exists (output))
				Directory.Delete (output, true);
		}

		protected override Pipeline GetPipeline ()
		{
			Pipeline p = new Pipeline ();
			p.AppendStep (new MarkStep ());
			p.AppendStep (new SweepStep ());
			p.AppendStep (new CleanStep ());
			p.AppendStep (new OutputStep ());
			return p;
		}

		protected override void Test (string testCase)
		{
			base.Test (testCase);

			Prepare ();
		}

		void Prepare ()
		{
			Context = GetContext ();
			string output = GetOutputPath ();
			if (Directory.Exists (output))
				Directory.Delete (output, true);
			Directory.CreateDirectory (output);
		}

		protected override void Run ()
		{
			base.Run ();
			Compare ();
		}

		void Compare ()
		{
			foreach (AssemblyMarker am in Context.GetAssemblies ()) {
				if (am.Action == AssemblyAction.Preserve)
					continue;

				string fileName = GetAssemblyFileName (am.Assembly);

				CompareAssemblies (
					AssemblyFactory.GetAssembly (
						Path.Combine (GetTestCasePath (), fileName)),
					AssemblyFactory.GetAssembly (
						Path.Combine (GetOutputPath (), fileName)));
			}
		}

		void CompareAssemblies (AssemblyDefinition original, AssemblyDefinition linked)
		{
			foreach (TypeDefinition originalType in original.MainModule.Types) {
				TypeDefinition linkedType = linked.MainModule.Types [originalType.FullName];
				if (NotLinked (originalType)) {
					Assert.IsNull (linkedType);
					continue;
				}

				CompareTypes (originalType, linkedType);
			}
		}

		void CompareTypes (TypeDefinition type, TypeDefinition linkedType)
		{
			foreach (FieldDefinition originalField in type.Fields) {
				FieldDefinition linkedField = linkedType.Fields.GetField (originalField.Name);// TODO: also get with the type!
				if (NotLinked (originalField)) {
					Assert.IsNull (linkedField);
					continue;
				}

				Assert.IsNotNull (linkedField);
			}

			foreach (MethodDefinition originalCtor in type.Constructors) {
				MethodDefinition linkedCtor = linkedType.Constructors.GetConstructor (originalCtor.IsStatic, originalCtor.Parameters);
				if (NotLinked (originalCtor)) {
					Assert.IsNull (linkedCtor);
					continue;
				}

				Assert.IsNotNull (linkedCtor);
			}

			foreach (MethodDefinition originalMethod in type.Methods) {
				MethodDefinition linkedMethod = linkedType.Methods.GetMethod (originalMethod.Name, originalMethod.Parameters);
				if (NotLinked (originalMethod)) {
					Assert.IsNull (linkedMethod);
					continue;
				}

				Assert.IsNotNull (linkedMethod);
			}
		}

		bool NotLinked(ICustomAttributeProvider provider)
		{
			foreach (CustomAttribute ca in provider.CustomAttributes)
				if (ca.Constructor.DeclaringType.Name == "NotLinkedAttribute")
					return true;

			return false;
		}
	}
}