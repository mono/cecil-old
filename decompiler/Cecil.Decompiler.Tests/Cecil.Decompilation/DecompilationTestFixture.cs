using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using System.IO;
using Cecil.Decompiler.Languages;
using NUnit.Framework;
using Mono.Cecil;

namespace Cecil.Decompiler.Tests.Cecil.Decompilation
{
    public abstract class DecompilationTestFixture
    {
        public void AssertMethod(MethodBody body, string expected, ILanguage language)
        {
            string decompiled = DecompileMethod(body, language);
            decompiled = Normalize(decompiled);
            Assert.AreEqual(expected, decompiled);
        }

		public void AssertMethodDefinition(MethodDefinition definition, string expected, ILanguage language)
		{
			string decompiled = DecompileMethod (definition, language);
			decompiled = Normalize (decompiled);
			Assert.AreEqual (expected, decompiled);
		}

		private string DecompileMethod(MethodDefinition definition, ILanguage language)
		{
			var str = new StringWriter();
			var writer = language.GetWriter(new PlainTextFormatter(str));
			writer.Write (definition);
			return str.ToString ();
		}

        public string Normalize(string decompiled)
        {
            string normalized = decompiled;
            if (decompiled.EndsWith("\r\n\r\n"))
                normalized = normalized.Substring(0, decompiled.Length - "\r\n\r\n".Length);

            return normalized;

        }

        private string DecompileMethod(MethodBody body, ILanguage language)
        {
            var str = new StringWriter();
            var writer = language.GetWriter(new PlainTextFormatter(str));

            var block = body.Decompile(language);
            writer.Write(block);

            return str.ToString();
        }

        public static  string TestCasesDirectory
        {
            get { return Path.GetFullPath(FindTestcasesDirectory()); }
        }

        static string FindTestcasesDirectory()
        {
            string currentPath = Environment.CurrentDirectory;
            while (!Directory.Exists(Path.Combine(currentPath, "TestCases")))
            {
                string oldPath = currentPath;
                currentPath = Path.GetDirectoryName(currentPath);
                Assert.AreNotEqual(oldPath, currentPath);
            }
            return Path.Combine(currentPath, "TestCases");
        }

        public static string CompileResource(string name,TestCaseType type)
        {
            string file = string.Concat(Path.GetTempFileName(),".dll");
           
            
            string path = string.Concat(TestCasesDirectory, Path.DirectorySeparatorChar + name);

            using (var provider = GetProvider(name))
            {
                var parameters = GetDefaultParameters(name);
                parameters.IncludeDebugInformation = false;
                parameters.GenerateExecutable = false;
                parameters.OutputAssembly = file;
                parameters.GenerateInMemory = false;
                if (type == TestCaseType.Release)
                    parameters.CompilerOptions = "/optimize";

                var results = provider.CompileAssemblyFromFile(parameters, path);
                AssertCompilerResults(results);
            }

            return file;
        }

        static CompilerParameters GetDefaultParameters(string name)
        {
            return GetCompilerInfo(name).CreateDefaultCompilerParameters();
        }

        static CodeDomProvider GetProvider(string name)
        {
            return GetCompilerInfo(name).CreateProvider();
        }

        static CompilerInfo GetCompilerInfo(string name)
        {
            return CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(Path.GetExtension(name)));
        }

        static void AssertCompilerResults(CompilerResults results)
        {
            Assert.IsFalse(results.Errors.HasErrors, GetErrorMessage(results));
        }

        static string GetErrorMessage(CompilerResults results)
        {
            if (!results.Errors.HasErrors)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (CompilerError error in results.Errors)
                builder.AppendLine(error.ToString());
            return builder.ToString();
        }
    }
}
