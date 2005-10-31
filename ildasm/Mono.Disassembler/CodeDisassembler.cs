//
// CodeDisassembler.cs
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

namespace Mono.Disassembler {

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	class CodeDisassembler : BaseCodeVisitor {

		ReflectionDisassembler m_reflectDis;
		CilWriter m_writer;

		public CodeDisassembler (ReflectionDisassembler dis)
		{
			m_reflectDis = dis;
		}

		public void DisassembleMethod (MethodDefinition method, CilWriter writer)
		{
			m_writer = writer;

			// write method header

			m_writer.OpenBlock ();
			m_reflectDis.VisitCustomAttributeCollection (method.CustomAttributes);
			m_reflectDis.VisitSecurityDeclarationCollection (method.SecurityDeclarations);
			VisitMethodBody (method.Body);
			m_writer.CloseBlock ();
		}

		public override void VisitMethodBody (MethodBody body)
		{
			m_writer.WriteLine ("// methods begin at RVA 0x{0}",
				body.Method.RVA.Value.ToString ("x4"));
			m_writer.WriteLine ("// codesize: {0}", body.CodeSize);
			m_writer.WriteLine (".maxstack {0}", body.MaxStack);

			VisitVariableDefinitionCollection (body.Variables);
			VisitInstructionCollection (body.Instructions);
		}

		public override void VisitVariableDefinitionCollection (VariableDefinitionCollection variables)
		{
			if (variables.Count == 0)
				return;

			m_writer.WriteLine (".local init (");
			m_writer.Indent ();
			for (int i = 0; i < variables.Count; i++) {
				VisitVariableDefinition (variables [i]);
				m_writer.BaseWriter.WriteLine (i > 0 ? string.Empty : ",");
			}
			m_writer.Unindent ();
			m_writer.WriteLine (")");
		}

		public override void VisitVariableDefinition (VariableDefinition var)
		{
			m_writer.Write (Formater.Signature (var.Variable));
			m_writer.BaseWriter.Write (" ");
			m_writer.BaseWriter.Write (Formater.Escape (var.Name));
		}

		string Label (Instruction instr)
		{
			return string.Concat ("IL_", instr.Offset.ToString ("x4"));
		}

		void CheckExceptionHandlers (Instruction instr)
		{
		}

		public override void VisitInstructionCollection (InstructionCollection instructions)
		{
			foreach (Instruction instr in instructions) {
				CheckExceptionHandlers (instr);
				m_writer.Write ("{0}:  {1}",
					Label (instr), instr.OpCode.Name);

				if (instr.OpCode.OperandType != OperandType.InlineNone)
					m_writer.BaseWriter.Write ("  ");

				switch (instr.OpCode.OperandType) {
				case OperandType.InlineNone :
					break;
				case OperandType.InlineSwitch :
					m_writer.BaseWriter.WriteLine ("(");
					m_writer.Indent ();
					Instruction [] targets = (Instruction []) instr.Operand;
					for (int i = 0; i < targets.Length; i++) {
						m_writer.Write (Label (targets [i]));
						m_writer.BaseWriter.WriteLine (i > 0 ? string.Empty : ",");
					}
					m_writer.Unindent ();
					m_writer.Write (")");
					break;
				case OperandType.ShortInlineBrTarget :
				case OperandType.InlineBrTarget :
					m_writer.BaseWriter.Write (Label ((Instruction) instr.Operand));
					break;
				case OperandType.ShortInlineVar :
				case OperandType.InlineVar :
					VariableDefinition var = (VariableDefinition) instr.Operand;
					if (var.Name != null && var.Name.Length > 0)
						m_writer.BaseWriter.Write (var.Name);
					else
						m_writer.BaseWriter.Write (instructions.Container.Variables.IndexOf (var));
					break;
				case OperandType.ShortInlineParam :
				case OperandType.InlineParam :
					ParameterDefinition param = (ParameterDefinition) instr.Operand;
					if (param.Name != null && param.Name.Length > 0)
						m_writer.BaseWriter.Write (param.Name);
					else
						m_writer.BaseWriter.Write (instructions.Container.Method.Parameters.IndexOf (param));
					break;
				case OperandType.InlineI :
				case OperandType.InlineI8 :
				case OperandType.InlineR :
				case OperandType.InlineString :
				case OperandType.ShortInlineI :
				case OperandType.ShortInlineR :
					m_writer.BaseWriter.Write (instr.Operand.ToString ());
					break;
				case OperandType.InlineType :
					m_writer.BaseWriter.Write ((TypeReference) instr.Operand);
					break;
				case OperandType.InlineField :
					break;
				case OperandType.InlineMethod :
					break;
				case OperandType.InlineTok :
					if (instr.Operand is TypeReference)
						m_writer.BaseWriter.Write ((TypeReference) instr.Operand);
					break;
				}

				m_writer.BaseWriter.WriteLine ();
			}
		}
	}
}
