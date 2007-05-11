namespace Mono.Cecil.Tests

import System
import System.Reflection

class ReflectionAssemblyPrinter(AbstractPrinter):
	static def ToString(asm as Assembly):
		return ReflectionAssemblyPrinter(asm).Writer.ToString()
		
	def constructor(asm as Assembly):
		for type in asm.GetTypes():
			WriteType(type)
			
	def WriteType(type as Type):
		WriteCustomAttributes(type)
		WriteTypeAttributes(type)
		WriteLine("class ${type.FullName}(${join(GetBaseTypes(type), ', ')}):")
		for member in type.GetMembers(BindingFlags.DeclaredOnly):
			(self as duck).WriteMember(member)
			
	def GetBaseTypes(type as Type):
		yield type.BaseType.FullName
		for i in type.GetInterfaces():
			yield i.FullName
			
	def WriteTypeAttributes(type as Type):
		if type.IsPublic:
			Write("public ")
		else:
			Write("internal ")
		Write("sealed ") if type.IsSealed
		
	def WriteCustomAttributes(mi as MemberInfo):
		for data as CustomAttributeData in CustomAttributeData.GetCustomAttributes(mi):
			WriteLine("[${data.Constructor.DeclaringType.FullName}]")
			
	def WriteMember(pi as PropertyInfo):
		pass
		
	def WriteMember(mi as MethodInfo):
		pass
		
	def WriteMember(fi as FieldInfo):
		pass
		
	def WriteMember(ci as ConstructorInfo):
		pass		
	
			
			
		 
		
