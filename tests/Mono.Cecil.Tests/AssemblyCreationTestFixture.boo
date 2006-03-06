namespace Mono.Cecil.Tests

import System
import System.IO
import NUnit.Framework
import Mono.Cecil
import Mono.Cecil.Cil

[TestFixture]
class AssemblyCreationTestFixture:
	
	_assembly as AssemblyDefinition
	_method as MethodDefinition
	
	[Test]
	def HelloWorld():
		
		SetUpAssembly("HelloWorld")
		
		worker = _method.Body.CilWorker		
		worker.Emit(OpCodes.Ldstr, "Hello, world!")
		worker.Emit(OpCodes.Call, ImportConsoleWriteLine())
		worker.Emit(OpCodes.Ret)	
		
		ExecuteAssemblyAndExpect("Hello, world!")
		
	[Test]
	def HelloGenericWorld():
		
		SetUpAssembly("HelloGenericWorld")
		
		worker = _method.Body.CilWorker
		
		genericListType = Type.GetType("System.Collections.Generic.List`1[System.String]")
		
		worker.Emit(OpCodes.Newobj, Import(genericListType.GetConstructor(array(Type, 0))))
			
		worker.Emit(OpCodes.Dup)
		worker.Emit(OpCodes.Ldstr, "Foo")
		worker.Emit(OpCodes.Callvirt, Import(genericListType.GetMethod("Add")))
		
		worker.Emit(OpCodes.Ldc_I4_0)
		worker.Emit(OpCodes.Callvirt, Import(genericListType.GetMethod("get_Item")))
		worker.Emit(OpCodes.Call, ImportConsoleWriteLine())
		
		worker.Emit(OpCodes.Ret)
		
		ExecuteAssemblyAndExpect("Foo")
		
		
	def SetUpAssembly(assemblyName as string):
		_assembly = AssemblyFactory.DefineAssembly(assemblyName, "${assemblyName}.exe", TargetRuntime.NET_2_0, AssemblyKind.Console)		
		assert _assembly is not null
		assert _assembly.MainModule is not null
		
		_method = MethodDefinition("Main", MethodAttributes.Static|MethodAttributes.Public, Import(void))		
		_method.Parameters.Add(ParameterDefinition(Import(typeof((string)))))		
		_assembly.EntryPoint = _method		
		_method.CreateBody()
		
		td = TypeDefinition("Program", "", TypeAttributes.AutoClass|TypeAttributes.Public, Import(object))
		td.Methods.Add(_method)
		
		_assembly.MainModule.Types.Add(td)		
		
	def ImportConsoleWriteLine():
		return Import(typeof(Console).GetMethod("WriteLine", (string,)))
		
	def ExecuteAssemblyAndExpect(expected as string):
		fname = BuildTempPath("${_assembly.Name.Name}.exe")
		AssemblyFactory.SaveAssembly(_assembly, fname)
		output = ExecuteAssembly(fname)
		Assert.AreEqual(expected.Trim(), output.Trim())
		
	def Import(type as System.Type):
		return _assembly.MainModule.Import(type)
		
	def Import(method as System.Reflection.MethodBase):
		return _assembly.MainModule.Import(method)
		
		

