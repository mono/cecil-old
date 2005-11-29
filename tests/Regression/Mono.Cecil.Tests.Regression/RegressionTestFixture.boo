namespace Mono.Cecil.Tests.Regression

import System.IO
import System.Reflection
import NUnit.Framework
import Mono.Cecil

[TestFixture]
class RegressionTestFixture:
	
	[Test]
	def OddInt64Initializer():
		AssertAssemblyDefinition("OddInt64Initializer.dll")

	[Test]
	def SingleClass():
		AssertAssemblyDefinition("SingleClass.exe")
		
	[Test]
	def SingleGenericClass():
		AssertAssemblyDefinition("SingleGenericClass.dll")
		
	def AssertAssemblyDefinition(name as string):
		location = Path.Combine(GetTestCasesLocation(), name)
		
		reflectionAssembly = Assembly.ReflectionOnlyLoadFrom(location)
		assert reflectionAssembly is not null
		
		cecilAssembly = AssemblyFactory.GetAssembly(location)
		assert cecilAssembly is not null
		
		Assert.AreEqual(
			ToString(reflectionAssembly),
			ToString(cecilAssembly))
			
	def ToString(asm as System.Reflection.Assembly):
		return ReflectionAssemblyPrinter.ToString(asm)
		
	def ToString(asm as Mono.Cecil.IAssemblyDefinition):
		return CecilAssemblyPrinter.ToString(asm)
		
	static def GetTestCasesLocation():
		return Path.Combine(
			Path.GetDirectoryName(System.Uri(typeof(RegressionTestFixture).Assembly.CodeBase).LocalPath),
			"../TestCases")
	
