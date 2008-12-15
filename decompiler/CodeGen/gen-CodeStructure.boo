#region license
#	(C) 2005 - 2008 db4objects Inc. http://www.db4o.com
#	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
#
#  Permission is hereby granted, free of charge, to any person obtaining
#  a copy of this software and associated documentation files (the
#  "Software"), to deal in the Software without restriction, including
#  without limitation the rights to use, copy, modify, merge, publish,
#  distribute, sublicense, and/or sell copies of the Software, and to
#  permit persons to whom the Software is furnished to do so, subject to
#  the following conditions:
#
#  The above copyright notice and this permission notice shall be
#  included in all copies or substantial portions of the Software.
#
#  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
#  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
#  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
#  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
#  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
#  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
#  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

import System
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Useful.BooTemplate from Boo.Lang.Useful

class Model:

	[getter(Module)]
	_module as Module

	def constructor(module as Module):
		_module = module

	def GetFields(node as TypeDefinition):
		return array(Field, GetMembers(node, NodeType.Field))

	def GetParametersFields(node as TypeDefinition):
		return array(Field, member for member in GetInheritedMembers(node, NodeType.Field) if (member as Field).Initializer is null)

	def GetBaseParametersFields(node as TypeDefinition):
		node = ResolveType(node.BaseTypes[0])
		if node is null:
			return array(Field, 0)

		return array(Field, member for member in GetMembers(node, NodeType.Field) if (member as Field).Initializer is null)

	def GetInheritedMembers(container as TypeDefinition, type as NodeType):
		while container is not null:
			for member in container.Members:
				yield member if member.NodeType == type
			container = ResolveType(container.BaseTypes[0]) as TypeDefinition

	def GetVisitableFields(node as TypeDefinition):
		for field in GetFields(node):
			type = ResolveType(field.Type)
			continue if type is null
			continue if IsEnum(type)
			yield field

	def GetVisitableNodes():
		for node in _module.Members:
			continue if node.NodeType != NodeType.ClassDefinition
			continue if node.IsAbstract
			continue if IsCollection(node)
			yield node

	def GetClasses():
		return array(ClassDefinition, GetMembers(_module, NodeType.ClassDefinition))

	def GetMembers(container as TypeDefinition, type as NodeType):
		for member in container.Members:
			yield member if member.NodeType == type

	def GetEnumFields(container as TypeDefinition):
		return array(EnumMember, GetMembers(container, NodeType.EnumMember))

	def GetCollectionItemType(node as TypeDefinition):
		assert IsCollection(node)
		attribute = node.Attributes.Get("collection")[0]
		reference as ReferenceExpression = attribute.Arguments[0]
		return reference.Name

	def IsCollection(node as TypeDefinition):
		return node.Attributes.Contains("collection")

	def IsEnum(node as TypeDefinition):
		return NodeType.EnumDefinition == node.NodeType

	def ResolveType(typeRef as TypeReference):
		return _module.Members[typeRef.ToString()]

class CodeTemplate(AbstractTemplate):

	static Keywords = {
		"operator" : true,
		"else" : true,
		"try" : true,
		"finally" : true,
	}

	[property(model)]
	_model as Model

	[property(node)]
	_node as TypeDefinition

	def ToCamelCase(s as string):
		return s[:1].ToLower() + s[1:]

	def ToParamName(s as string):
		cc = ToCamelCase(s)
		cc = "@" + cc if cc in Keywords
		return cc

	def ToFieldName(s as string):
		return "${ToParamName(s)}"

	def GetFieldTypeName(field as Field):
		type = model.ResolveType(field.Type)
		return ToGenericTypeName (field.Type) if field.Type isa GenericTypeReference 
		return "${field.Type}" if type is not null
		return field.Type.ToString()

	def ToGenericTypeName (t as TypeReference):
		type = t as GenericTypeReference
		return "${type.Name}<${join(type.GenericArguments, ',')}>"

def parse(fname as string):
	parser = BooCompiler()
	parser.Parameters.Pipeline = Pipelines.Parse()
	parser.Parameters.Input.Add(FileInput(fname))
	result = parser.Run()
	assert 0 == len(result.Errors), result.Errors.ToString()
	return result.CompileUnit.Modules[0]

def loadTemplate(model, fname as string):
	compiler = TemplateCompiler(TemplateBaseClass: CodeTemplate)
	result = compiler.CompileFile(Path.Combine("CodeGen/templates/Ast", fname))
	assert 0 == len(result.Errors), result.Errors.ToString()

	templateType = result.GeneratedAssembly.GetType("Template")
	template as CodeTemplate = templateType()
	template.model = model
	return template

def applyTemplate(node as TypeDefinition, template as CodeTemplate, targetFile as string):
	using writer=StreamWriter(Path.Combine("Cecil.Decompiler", targetFile)):
		template.node = node
		template.Output = writer
		template.Execute()
	print targetFile

def applyModelTemplate(model as Model, templateName as string):
	applyTemplate(null, loadTemplate(model, templateName), "Cecil.Decompiler.Ast/${templateName}")

module = parse("CodeGen/CodeStructureModel.boo")
model = Model(module)
interfaceTemplate = loadTemplate(model, "Interface.cs")
classTemplate = loadTemplate(model, "Class.cs")
enumTemplate = loadTemplate(model, "Enum.cs")
collectionClassTemplate = loadTemplate(model, "CollectionClass.cs")

for node in module.Members:
	if node isa InterfaceDefinition:
		applyTemplate(node, interfaceTemplate, "Cecil.Decompiler.Ast/${node.Name}.cs")
	elif node isa EnumDefinition:
		applyTemplate(node, enumTemplate, "Cecil.Decompiler.Ast/${node.Name}.cs")
	elif node isa ClassDefinition:
		if model.IsCollection(node):
			applyTemplate(node, collectionClassTemplate, "Cecil.Decompiler.Ast/${node.Name}.cs")
		else:
			applyTemplate(node, classTemplate, "Cecil.Decompiler.Ast/${node.Name}.cs")

applyModelTemplate(model, "ICodeVisitor.cs")
applyModelTemplate(model, "BaseCodeVisitor.cs")
applyModelTemplate(model, "ICodeTransformer.cs")
applyModelTemplate(model, "BaseCodeTransformer.cs")
applyModelTemplate(model, "CodeNodeType.cs")
