using System;

using NUnit.Framework;

using Cecil.Decompiler.Languages;

namespace Cecil.Decompiler.CSharp {

	[TestFixture]
	public class Exceptions {

		[CSharp (Version = CSharpVersion.V1)]
		public void TryCatchFinally ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		public void NestedTryCatch ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		public void TryInWhileInTry ()
		{
		}
	}
}
