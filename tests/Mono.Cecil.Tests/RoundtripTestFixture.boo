namespace Mono.Cecil.Tests

import System
import System.IO
import NUnit.Framework
import Mono.Cecil
import Mono.Cecil.Cil

[TestFixture]
class RoundtripTestFixture:
	
	[Test]
	def GenericEnumerator():
		code = """
using System;
using System.Collections.Generic;

public class Program
{
	public static void Main(string[] args)
	{
		foreach (int i in GetNumbers())
		{
			Console.WriteLine(i);
		}
	}
	
	static IEnumerable<int> GetNumbers()
	{
		for (int i=0; i<3; ++i)
		{
			yield return i+1;
		}
	}
}"""
		InstrumentAndExecute("1\n2\n3", "GenericEnumerator", code) do (assembly as AssemblyDefinition):
			pass // no instrumentation for now
		
		
	[Test]
	def ReplaceByGenericInstanceMethod():
		code = """
using System;

public class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Hello, world!");
	}
	
	public static void print<T>(T value)	
	{
		Console.WriteLine("print<T>");
		Console.WriteLine(value);
	}
}
"""
		InstrumentAndExecute("print<T>\nHello, world!", "ReplaceByGenericInstanceMethod", code) do (assembly as AssemblyDefinition):
			program = assembly.MainModule.Types["Program"]
			
			main as MethodDefinition = program.Methods.GetMethod("Main")[0]
			assert main is not null
			
			print as MethodDefinition = program.Methods.GetMethod("print")[0]
			assert print is not null
			
			worker = main.Body.CilWorker
			worker.Replace(
						FindFirst(main.Body, OpCodes.Call),
						worker.Create(
							OpCodes.Call,
							InstantiateMethod(print, assembly.MainModule.Import(string))))
						
	def InstantiateMethod(method as MethodReference, *typeArguments as (TypeReference)):
		instance = GenericInstanceMethod(method)
		for typeArgument in typeArguments:
			instance.GenericArguments.Add(typeArgument)
		return instance
		
	def FindFirst(body as MethodBody, opcode as OpCode):
		for instr as Instruction in body.Instructions:
			if instr.OpCode.Value == opcode.Value:
				return instr

	def InstrumentAndExecute(expectedOutput as string, assemblyName as string, code as string,
							instrument as callable(AssemblyDefinition)):
		fname = BuildTempPath(Path.Combine("before", "${assemblyName}.exe"))
		EmitCSharpAssembly(fname, code)
		assembly = AssemblyFactory.GetAssembly(fname)
		
		instrument(assembly) if instrument is not null
		
		roundtripped = BuildTempPath(Path.Combine("after", "${assemblyName}.exe"))
		AssemblyFactory.SaveAssembly(assembly, roundtripped)
		
		output = ExecuteAssembly(roundtripped)
		Assert.AreEqual(expectedOutput.Trim(), output.Trim())

