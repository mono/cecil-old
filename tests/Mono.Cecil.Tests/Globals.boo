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
	return shellm(fname, (of string:,))	

def CopyBooLangTo(path as string):
	fname = typeof(List).Module.FullyQualifiedName
	target = Path.Combine(path, Path.GetFileName(fname))
	File.Copy(fname, target) unless File.Exists(target)
