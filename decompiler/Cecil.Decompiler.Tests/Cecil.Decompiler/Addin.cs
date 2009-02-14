using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;

using Cecil.Decompiler.Languages;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cecil.Decompiler {

	[NUnitAddin]
	public class Addin : IAddin, ITestCaseBuilder {

		public bool Install (IExtensionHost host)
		{
			if (host == null)
				throw new ArgumentNullException ("host");

			var builders = host.GetExtensionPoint ("TestCaseBuilders");

			if (builders == null)
				return false;

			builders.Install (this);

			return true;
		}

		public bool CanBuildFrom (MethodInfo method)
		{
			if (method == null)
				return false;

			return IsTestMethod (method);
		}

		public Test BuildFrom (MethodInfo method)
		{
			if (method == null)
				throw new ArgumentNullException ("method");

			return CreateTestSuite (method);
		}

		static bool IsTestMethod (MethodInfo method)
		{
			return method.IsDefined (typeof (CompiledTestCaseAttribute), false);
		}

		static Test CreateTestSuite (MethodInfo method)
		{
			var suite = new DecompilerTestSuite (method);
			NUnitFramework.ApplyCommonAttributes (method, suite);
			PopulateTestSuite (method, suite);
			return suite;
		}

		static void PopulateTestSuite (MethodInfo method, DecompilerTestSuite suite)
		{
			var attribute = GetTestAttribute (method);

			if (attribute.TestDebug)
				suite.Add (CreateTestCase (method, attribute, CompilationMode.Debug));

			if (attribute.TestRelease)
				suite.Add (CreateTestCase (method, attribute, CompilationMode.Release));
		}

		static DecompilerTestCase CreateTestCase (MethodInfo method, CompiledTestCaseAttribute attribute, CompilationMode mode)
		{
			var test_case = new DecompilerTestCase (method, mode);
			test_case.Language = attribute.GetLanguage ();
			test_case.Compiler = attribute.CreateCompiler ();
			test_case.CompilerParameters = attribute.CreateParamaters ();

			if (mode == CompilationMode.Release)
				test_case.CompilerParameters.CompilerOptions = "/optimize+";

			test_case.SourceFile = GetTestCaseFile (attribute.SourceFile ?? GetDefaultSourceFile (method, attribute));
			test_case.ExpectedResultFile = GetTestCaseFile (attribute.ExpectedResultFile ?? GetDefaultExpectedResultFile (method, attribute));
			test_case.MethodName = attribute.MethodName ?? GetDefaultMethodName (method);

			test_case.CompilerParameters.OutputAssembly = test_case.ExpectedResultFile + ".dll";

			return test_case;
		}

		static CompiledTestCaseAttribute GetTestAttribute (MethodInfo method)
		{
			return (CompiledTestCaseAttribute) method.GetCustomAttributes (
				typeof (CompiledTestCaseAttribute), false) [0];
		}

		public static string TestCasesDirectory
		{
			get { return Path.GetFullPath (FindTestCasesDirectory ()); }
		}

		static string GetTestCaseFile (string file)
		{
			return Path.Combine (TestCasesDirectory, file);
		}

		static string FindTestCasesDirectory ()
		{
			string currentPath = new Uri (Assembly.GetExecutingAssembly ().CodeBase).LocalPath;
			while (!Directory.Exists (Path.Combine (currentPath, "TestCases"))) {
				string oldPath = currentPath;
				currentPath = Path.GetDirectoryName (currentPath);
				Assert.AreNotEqual (oldPath, currentPath);
			}
			return Path.Combine (currentPath, "TestCases");
		}

		static string GetDefaultSourceFile (MethodInfo method, CompiledTestCaseAttribute attribute)
		{
			return Path.Combine (
				attribute.LanguageName,
				method.DeclaringType.Name + attribute.SourceFileExtension);
		}

		static string GetDefaultExpectedResultFile (MethodInfo method, CompiledTestCaseAttribute attribute)
		{
			return Path.Combine (
				attribute.LanguageName,
				method.DeclaringType.Name + "." + method.Name + ".txt");
		}

		static string GetDefaultMethodName (MethodInfo method)
		{
			return method.Name;
		}
	}

	class DecompilerTestSuite : TestSuite {

		public DecompilerTestSuite (MethodInfo method)
			: base (method.DeclaringType.FullName, method.Name)
        {
        }

        public override TestResult Run (EventListener listener, ITestFilter filter)
        {
            if (this.Parent != null)
                this.Fixture = this.Parent.Fixture;

            return base.Run (listener, filter);
        }
	}

	class DecompilerTestCase : NUnitTestMethod {

		CompilationMode mode;

		string method_name;
		string source_file;
		string expected_result_file;

		CodeDomProvider provider;
		CompilerParameters parameters;

		ILanguage language;

		public CompilationMode CompilationMode {
			get { return mode; }
			set { mode = value; }
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

		public CodeDomProvider Compiler {
			get { return provider; }
			set { provider = value; }
		}

		public CompilerParameters CompilerParameters {
			get { return parameters; }
			set { parameters = value; }
		}

		public ILanguage Language {
			get { return language; }
			set { language = value; }
		}

		public DecompilerTestCase (MethodInfo method, CompilationMode mode)
			: base (method)
		{
			this.TestName.Name = method.Name + "." + mode;
			this.TestName.FullName = this.TestName.Name;
		}

		public override void RunTestMethod (TestCaseResult testResult)
		{
			var result = provider.CompileAssemblyFromFile (parameters, SourceFile);

			AssertCompilerResults (result);

			var assembly = GetAssembly (result);

			var method = GetMethod (assembly);

			var decompiled = Normalize (DecompileMethod (method));
			var expected = Normalize (File.ReadAllText (expected_result_file));

			Assert.AreEqual (expected, decompiled);
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

		static string Normalize (string s)
		{
			return s.Trim ().Replace ("\r\n", "\n");
		}

		string DecompileMethod (MethodDefinition method)
		{
			var str = new StringWriter ();
			var writer = language.GetWriter (new PlainTextFormatter (str));

			writer.Write (method);

			return str.ToString ();
		}

		AssemblyDefinition GetAssembly (CompilerResults result)
		{
			return AssemblyFactory.GetAssembly (result.PathToAssembly);
		}

		MethodDefinition GetMethod (AssemblyDefinition assembly)
		{
			foreach (TypeDefinition type in assembly.MainModule.Types)
				foreach (MethodDefinition method in type.Methods)
					if (method.Name == method_name)
						return method;

			return null;
		}
	}
}
