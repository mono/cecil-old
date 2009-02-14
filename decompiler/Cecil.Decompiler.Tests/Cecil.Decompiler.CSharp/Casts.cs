using System;

using NUnit.Framework;

using Cecil.Decompiler.Languages;

namespace Cecil.Decompiler.CSharp {

	[TestFixture]
	public class Casts {

		[CSharp]
		public void ToShort ()
		{
		}

		[CSharp]
		public void AsBar ()
		{
		}

		[CSharp (Version = CSharpVersion.V1)]
		public void IsBar ()
		{
		}

		[CSharp]
		public void ToBar ()
		{
		}
	}
}
