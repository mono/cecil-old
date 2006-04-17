namespace Mono.Cecil.Tests

import System.IO

def BuildTempPath(fname as string):
	path = Path.Combine(Path.Combine(Path.GetTempPath(), "Mono.Cecil.Tests"), fname)
	CreateDirectoryIfNeeded(Path.GetDirectoryName(path))
	return path
	
def CreateDirectoryIfNeeded(directory as string):
	return if Directory.Exists(directory)
	Directory.CreateDirectory(directory)
	
def ExecuteAssembly(fname as string):
	CopyBooLangTo(Path.GetDirectoryName(fname))
	return shell(fname, "")
	
def ExecuteAssemblyAndExpect(fname as string, expected as string):
	output = ExecuteAssembly(fname)
	NUnit.Framework.Assert.AreEqual(expected.Trim(), output.Trim().Replace("\r\n", "\n"))
	VerifyAssembly(fname)

def CopyBooLangTo(path as string):
	fname = typeof(List).Module.FullyQualifiedName
	target = Path.Combine(path, Path.GetFileName(fname))
	File.Copy(fname, target) unless File.Exists(target)
	
def VerifyAssembly(location as string):
	try:
		process = shellp("peverify.exe", "\"${location}\"")
	except x:
		raise "Failed to run peverify.exe, maybe it's not on your path?\n${x}"
	output = process.StandardOutput.ReadToEnd()
	process.WaitForExit()
	if 0 != process.ExitCode: raise output
