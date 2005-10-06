//
// ICilWorker.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil.Cil {

	public interface ICilWorker {

		MethodBody GetBody ();

		Instruction Emit (OpCode opcode);

		Instruction Emit (OpCode opcode, TypeReference type);
		Instruction Emit (OpCode opcode, MethodReference meth);
		Instruction Emit (OpCode opcode, FieldReference field);

		Instruction Emit (OpCode opcode, string str);
		Instruction Emit (OpCode opcode, byte b);
		Instruction Emit (OpCode opcode, int i);
		Instruction Emit (OpCode opcode, long l);
		Instruction Emit (OpCode opcode, float f);
		Instruction Emit (OpCode opcode, double d);

		Instruction Emit (OpCode opcode, Instruction target);
		Instruction Emit (OpCode opcode, Instruction [] targets);

		Instruction Emit (OpCode opcode, VariableDefinition var);
		Instruction Emit (OpCode opcode, ParameterDefinition param);

		Instruction Create (OpCode opcode);

		Instruction Create (OpCode opcode, TypeReference type);

		Instruction Create (OpCode opcode, MethodReference meth);
		Instruction Create (OpCode opcode, FieldReference field);

		Instruction Create (OpCode opcode, string str);
		Instruction Create (OpCode opcode, byte b);
		Instruction Create (OpCode opcode, int i);
		Instruction Create (OpCode opcode, long l);
		Instruction Create (OpCode opcode, float f);
		Instruction Create (OpCode opcode, double d);

		Instruction Create (OpCode opcode, Instruction target);
		Instruction Create (OpCode opcode, Instruction [] targets);

		Instruction Create (OpCode opcode, VariableDefinition var);

		Instruction Create (OpCode opcode, ParameterDefinition param);

		void InsertBefore (Instruction target, Instruction instr);
		void InsertAfter (Instruction target, Instruction instr);
		void Append (Instruction instr);
	}
}
