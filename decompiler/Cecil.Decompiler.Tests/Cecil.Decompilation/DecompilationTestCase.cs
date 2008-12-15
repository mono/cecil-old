using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using System.Text;
using Mono.Cecil.Cil;
using NUnit.Core;
using SR = System.Reflection;
using System.IO;

namespace Cecil.Decompiler.Tests.Cecil.Decompilation
{
    public class DecompilationTestCase:NUnitTestMethod
    {
        private string resultFilename;
        private TestCaseType type;

        public DecompilationTestCase(SR.MethodInfo method, TestCaseType type, string filename, string resultFilename) : base(method)
        {
            this.TestName.Name = method.DeclaringType.FullName + "." + method.Name + "." + type.ToString();
            this.TestName.FullName = this.TestName.Name;
            this.resultFilename=resultFilename;
            this.type = type;
        }

        public override void RunTestMethod(TestCaseResult testResult)
        {
        	DecompilationTestAttribute attribute = DecompilationTestFactory.GetDecompilationTestAttribute (Method);
			var body = attribute.GetMethodBody(this.type);

            string path = string.Concat(DecompilationTestFixture.TestCasesDirectory, Path.DirectorySeparatorChar + resultFilename);
            if(attribute.Mode==Mode.MethodDefinition)
				Reflect.InvokeMethod(Method, Fixture, new object[] { body.Method, File.ReadAllText(path) });
			else
			Reflect.InvokeMethod(Method,Fixture, new object[] {body, File.ReadAllText(path)});
        }
    }
}
