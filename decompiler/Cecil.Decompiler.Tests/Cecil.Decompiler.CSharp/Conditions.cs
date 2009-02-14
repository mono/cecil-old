using System;

using NUnit.Framework;

using Cecil.Decompiler.Languages;

namespace Cecil.Decompiler.CSharp {

	[TestFixture]
	public class Conditions {

		[CSharp (Version = CSharpVersion.V1, Mode = CompilationMode.Debug)]
		public void ReturnTernary ()
		{
		}

		[CSharp (Version = CSharpVersion.V1, Mode = CompilationMode.Debug)]
		public void ReturnNestedTernary ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		public void SimpleIf ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		public void SimpleIfElse ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		[Ignore] // doesn't work
		public void SimpleIfOr ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		[Ignore] // doesn't work
		public void SimpleIfAnd ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		public void NullCoalesce ()
		{
		}
	}
}
