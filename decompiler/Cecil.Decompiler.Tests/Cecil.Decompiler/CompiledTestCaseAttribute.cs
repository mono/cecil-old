
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

using NUnit.Framework;

using Cecil.Decompiler.Languages;

namespace Cecil.Decompiler {

	[Flags]
	public enum CompilationMode {
		Release = 0x1,
		Debug = 0x2,
	}

	public abstract class CompiledTestCaseAttribute : Attribute {

		CompilationMode mode = CompilationMode.Debug | CompilationMode.Release;
		string method_name;
		string source_file;
		string expected_result_file;

		public CompilationMode Mode {
			get { return mode; }
			set { mode = value; }
		}

		public bool TestRelease {
			get { return (mode & CompilationMode.Release) != 0; }
		}

		public bool TestDebug {
			get { return (mode & CompilationMode.Debug) != 0; }
		}

		public string MethodName {
			get { return method_name; }
			set { method_name = value; }
		}

		public string SourceFile {
			get { return source_file; }
			set { source_file = value; }
		}

		public string ExpectedResultFile {
			get { return expected_result_file; }
			set { expected_result_file = value; }
		}

		public abstract string SourceFileExtension { get; }
		public abstract string LanguageName { get; }

		public CodeDomProvider CreateCompiler ()
		{
			return GetProvider (SourceFileExtension);	
		}

		public CompilerParameters CreateParamaters ()
		{
			var parameters = GetDefaultParameters (SourceFileExtension);
			parameters.GenerateExecutable = false;
			parameters.GenerateInMemory = false;
			return parameters;
		}

		static CompilerParameters GetDefaultParameters (string extension)
		{
			return GetCompilerInfo (extension).CreateDefaultCompilerParameters ();
		}

		static CodeDomProvider GetProvider (string extension)
		{
			return GetCompilerInfo (extension).CreateProvider ();
		}

		static CompilerInfo GetCompilerInfo (string extension)
		{
			return CodeDomProvider.GetCompilerInfo (CodeDomProvider.GetLanguageFromExtension (extension));
		}

		static void AssertCompilerResults (CompilerResults results)
		{
			Assert.IsFalse (results.Errors.HasErrors, GetErrorMessage (results));
		}

		static string GetErrorMessage (CompilerResults results)
		{
			if (!results.Errors.HasErrors)
				return string.Empty;

			var builder = new StringBuilder ();

			foreach (CompilerError error in results.Errors)
				builder.AppendLine (error.ToString ());

			return builder.ToString ();
		}

		public abstract ILanguage GetLanguage ();
	}
}
