using System;

using Cecil.Decompiler.Languages;

namespace Cecil.Decompiler.CSharp {

	public class CSharpAttribute : CompiledTestCaseAttribute {

		CSharpVersion version = CSharpVersion.None;

		public CSharpVersion Version {
			get { return version; }
			set { version = value; }
		}

		public override string LanguageName {
			get { return "CSharp"; }
		}

		public override string SourceFileExtension {
			get { return ".cs"; }
		}

		public override ILanguage GetLanguage ()
		{
			return Cecil.Decompiler.Languages.CSharp.GetLanguage (version);
		}
	}
}
