using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;

namespace Cecil.Decompiler.Tests.Cecil.Decompilation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DecompilationTestAttribute:Attribute
    {
        private string fileName;
        private string resultFilename;

		public Mode Mode
		{
			get;
			set;
		}

        public string MethodName
        { get; set; }

        public string Filename
        {
            get { return fileName; }
            set { fileName = value;
            if (string.IsNullOrEmpty(ResultFilename))
                ResultFilename = string.Concat(value, ".txt");
            }
        }

        public string ResultFilename
        {
            get { return resultFilename; }
            set { resultFilename = value; }
        }

        public MethodBody GetMethodBody(TestCaseType type)
        {
            string assemblyLocation = DecompilationTestFixture.CompileResource(fileName,type);
            var asmDefinition = AssemblyFactory.GetAssembly(assemblyLocation);
            MethodDefinition methodInfo = GetMethodDefinition(asmDefinition);
            if (methodInfo == null)
                return null;

            return methodInfo.Body;
        }

        private MethodDefinition GetMethodDefinition(AssemblyDefinition assemblyDefinition)
        {
            foreach(TypeDefinition typeDef in assemblyDefinition.MainModule.Types)
            {
                MethodDefinition[] methodDefs = typeDef.Methods.GetMethod(MethodName);
                if (methodDefs.Length>0)
                    return methodDefs[0];
            }
            return null;
        }

    }

	public enum Mode
	{
		MethodBody,
		MethodDefinition
	}
}
