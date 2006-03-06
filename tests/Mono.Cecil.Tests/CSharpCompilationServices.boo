namespace Mono.Cecil.Tests

import System
import System.CodeDom.Compiler
import System.Text
import System.IO
import Useful.IO

def EmitCSharpAssembly(assemblyFileName as string, *code as (string)):
	basePath = Path.GetDirectoryName(assemblyFileName)
	CreateDirectoryIfNeeded(basePath)	
	sourceFiles = WriteSourceFiles(Path.GetTempPath(), code)
	CompileCSharpFiles(assemblyFileName, sourceFiles)

def WriteSourceFiles(basePath as string, code as (string)):
	sourceFiles = array(string, len(code))
	for i in range(len(code)):
		sourceFile = Path.Combine(basePath, "source${i}.cs")
		TextFile.WriteFile(sourceFile, code[i])
		sourceFiles[i] = sourceFile
	return sourceFiles

def GetCSharpCompilerInfo():
	return CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(".cs"))
	
def GetCSharpCodeDomProvider():
	return GetCSharpCompilerInfo().CreateProvider()

def CreateDefaultCompilerParameters():
	return GetCSharpCompilerInfo().CreateDefaultCompilerParameters()
	
def CompileCSharpFiles(assemblyFName as string, files as (string)):
	using provider=GetCSharpCodeDomProvider():
		parameters = CreateDefaultCompilerParameters()
		parameters.IncludeDebugInformation = false
		parameters.OutputAssembly = assemblyFName
		parameters.GenerateExecutable = true
		
		compiler = provider.CreateCompiler()
		results = compiler.CompileAssemblyFromFileBatch(parameters, files)
		if results.Errors.Count > 0:
			raise ApplicationException(join(results.Errors, '\n'))

