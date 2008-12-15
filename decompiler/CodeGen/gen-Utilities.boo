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
import Useful.BooTemplate from Boo.Lang.Useful

class InstructionInfo:

	[getter(OpCodes)]
	_opcodes as (string)

	def constructor(line as string):
		_opcodes = array(formatOpCode(opcode) for opcode in /\s+/.Split(line.Trim()))

class CodeTemplate(AbstractTemplate):

	[property(Instructions)]
	_instructions as (InstructionInfo)

def formatOpCode(opcode as string):
	parts = /\./.Split(opcode)
	return join(capitalize(part) for part in parts, '_')

def capitalize(s as string):
	return s[:1].ToUpper() + s[1:]

def parse(fname as string):
	for line in File.OpenText(fname):
		yield InstructionInfo(line)

def applyTemplate(instructions as (InstructionInfo), fname as string):
	compiler = TemplateCompiler(TemplateBaseClass: CodeTemplate)
	result = compiler.CompileFile(Path.Combine("CodeGen/templates/Cil", fname))
	assert 0 == len(result.Errors), result.Errors.ToString()

	templateType = result.GeneratedAssembly.GetType("Template")
	template as CodeTemplate = templateType()
	template.Instructions = instructions

	print fname
	using writer=StreamWriter(Path.Combine("Cecil.Decompiler/Cecil.Decompiler.Cil", fname)):
		template.Output = writer
		template.Execute()

instructions = array(parse("CodeGen/instructions.txt"))
applyTemplate(instructions, "InstructionDispatcher.cs")
applyTemplate(instructions, "IInstructionVisitor.cs")
applyTemplate(instructions, "BaseInstructionVisitor.cs")
