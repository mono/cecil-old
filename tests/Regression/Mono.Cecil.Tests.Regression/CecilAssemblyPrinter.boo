namespace Mono.Cecil.Tests.Regression

import Mono.Cecil

class CecilAssemblyPrinter(AbstractPrinter):
	static def ToString(asm as IAssemblyDefinition):
		return CecilAssemblyPrinter(asm).Writer.ToString()
		
	def constructor(asm as IAssemblyDefinition):
		for type as ITypeDefinition in asm.MainModule.Types:
			continue if type.FullName == "<Module>"
			WriteType(type)
			
	def WriteType(type as ITypeDefinition):
		WriteCustomAttributes(type)
		WriteTypeAttributes(type)
		WriteLine("class ${type.FullName}(${join(GetBaseTypes(type), ', ')}):")
		
	def GetBaseTypes(type as ITypeDefinition):
		yield type.BaseType.FullName
		for i as ITypeReference in type.Interfaces:
			yield i.FullName
		
	def WriteCustomAttributes(type as ITypeDefinition):
		pass
		
	def WriteTypeAttributes(type as ITypeDefinition):
		attributes = type.Attributes
		if TypeAttributes.Public == attributes & TypeAttributes.Public:
			Write("public ")
		else:
			Write("internal ")

