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

using System.Text;
using System.Text.RegularExpressions;

namespace Mono.Disassembler {

	using System;
	using Mono.Cecil;
	using Mono.Cecil.Cil;

	class CodeDisassembler : BaseCodeVisitor {

		ReflectionDisassembler m_reflectDis;
		CilWriter m_writer;

		static readonly int [] m_methodVisibilityVals = new int [] {
			(int) MethodAttributes.Private, (int) MethodAttributes.FamANDAssem, (int) MethodAttributes.Assem,
			(int) MethodAttributes.Family, (int) MethodAttributes.FamORAssem, (int) MethodAttributes.Public
		};

		static readonly string [] m_methodVisibilityMap = new string [] {
			"private", "famandassem", "assembly", "family", "famorassem", "public"
		};

		void WriteMethodVisibility (MethodAttributes attributes)
		{
			m_writer.WriteFlags ((int) attributes, (int) (ushort) MethodAttributes.MemberAccessMask,
				m_methodVisibilityVals, m_methodVisibilityMap);
		}

		static readonly int [] m_methodFlagsVals = new int [] {
			(int) MethodAttributes.Static, (int) MethodAttributes.Final, (int) MethodAttributes.Virtual, (int) MethodAttributes.HideBySig,
			(int) MethodAttributes.Abstract, (int) MethodAttributes.SpecialName, (int) MethodAttributes.PInvokeImpl,
			(int) MethodAttributes.UnmanagedExport, (int) MethodAttributes.RTSpecialName, (int) MethodAttributes.RequireSecObject,
			(int) MethodAttributes.NewSlot
		};

		static readonly string [] m_methodFlagsMap = new string [] {
			"static", "final", "virtual", "hidebysig", "abstract",
			"specialname", "pinvokeimpl", "export", "rtspecialname",
			"requiresecobj", "newslot"
		};

		void WriteMethodAttributes (MethodAttributes attributes)
		{
			m_writer.WriteFlags ((int) attributes, m_methodFlagsVals, m_methodFlagsMap);
		}

		static readonly int [] m_methodCallConvVals = new int [] {
			(int) MethodCallingConvention.Default, (int) MethodCallingConvention.C, (int) MethodCallingConvention.StdCall,
			(int) MethodCallingConvention.ThisCall, (int) MethodCallingConvention.FastCall, (int) MethodCallingConvention.VarArg,
			/* ma.Generic */
		};

		static readonly string [] m_methodCallConvMap = new string [] {
			"default", "unmanaged cdecl", "unmanaged stdcall",
			"unmanaged thiscall", "unmanaged fastcall", "vararg"
		};

		void WriteMethodCallingConvention (MethodCallingConvention cc)
		{
			m_writer.WriteFlags ((int) cc, m_methodCallConvVals, m_methodCallConvMap);
		}

		static readonly int [] m_methodImplVals = new int [] {
			(int) MethodImplAttributes.IL, (int) MethodImplAttributes.Native, (int) MethodImplAttributes.OPTIL,
			(int) MethodImplAttributes.Runtime
		};

		static readonly string [] m_methodImplMap = new string [] {
			"cil", "native", "optil", "runtime"
		};

		void WriteMethodCodeType (MethodImplAttributes attributes)
		{
			m_writer.WriteFlags ((int) attributes, (int) MethodImplAttributes.CodeTypeMask, m_methodImplVals, m_methodImplMap);
		}

		static readonly int [] m_methodManagedTypeVals = new int [] {
			(int) MethodImplAttributes.Managed, (int) MethodImplAttributes.Unmanaged
		};

		static readonly string [] m_methodManagedTypeMap = new string [] {
			"managed", "unmanaged"
		};
		
		void WriteMethodManagedType (MethodImplAttributes attributes)
		{
			m_writer.WriteFlags ((int) attributes, (int) MethodImplAttributes.ManagedMask, m_methodManagedTypeVals, m_methodManagedTypeMap);
		}

		public CodeDisassembler (ReflectionDisassembler dis)
		{
			m_reflectDis = dis;
		}

		public CilWriter Writer {
			get { return m_writer; }
			set { m_writer = value; }
		}

		//FIXME: Move to ReflectionDisassembler ?
		public void DisassembleMethod (MethodDefinition method, CilWriter writer)
		{
			m_writer = writer;
			//    .method public hidebysig  specialname
			//               instance default class [mscorlib]System.IO.TextWriter get_BaseWriter ()  cil managed
			//

			// write method header
			m_writer.Write (".method ");

			//emit flags
			WriteMethodVisibility (method.Attributes);
			WriteMethodAttributes (method.Attributes);

			m_writer.WriteLine ();
			m_writer.Indent ();

			if (method.HasThis)
				m_writer.Write ("instance ");

			//call convention
			WriteMethodCallingConvention (method.CallingConvention);

			//return type
			//method.ReturnType.ReturnType.Accept (m_reflectDis);
			//FIXME: another visitor for printing names (refs to methoddef/typedef/typeref etc
			m_writer.Write (Formater.Signature (method.ReturnType.ReturnType, false, true));
			m_writer.Write (method.Name);

			//( params )
			m_writer.Write (" (");
			method.Parameters.Accept (m_reflectDis);
			m_writer.Write (") ");
			//cil managed
			WriteMethodCodeType (method.ImplAttributes);
			WriteMethodManagedType (method.ImplAttributes);

			m_writer.Unindent ();

			m_writer.OpenBlock ();
			m_reflectDis.VisitCustomAttributeCollection (method.CustomAttributes);
			m_reflectDis.VisitSecurityDeclarationCollection (method.SecurityDeclarations);

			if (method.HasBody)
				VisitMethodBody (method.Body);

			m_writer.CloseBlock ();
		}

		public override void VisitMethodBody (MethodBody body)
		{
			m_writer.WriteLine ("// Method begins at RVA 0x{0}",
				body.Method.RVA.Value.ToString ("x4"));
			m_writer.WriteLine ("// Code size {0} (0x{0:x})", body.CodeSize);
			m_writer.WriteLine (".maxstack {0}", body.MaxStack);
			if (body.Method.DeclaringType.Module.Assembly.EntryPoint != null &&
				body.Method.DeclaringType.Module.Assembly.EntryPoint.ToString () == body.Method.ToString ())
				m_writer.WriteLine (".entrypoint");

			VisitVariableDefinitionCollection (body.Variables);
			VisitInstructionCollection (body.Instructions);
		}

		public override void VisitVariableDefinitionCollection (VariableDefinitionCollection variables)
		{
			if (variables.Count == 0)
				return;

			//FIXME: check for 'init' from method header
			m_writer.WriteLine (".locals init (");
			m_writer.Indent ();
			for (int i = 0; i < variables.Count; i++) {
				VisitVariableDefinition (variables [i]);
				m_writer.WriteLine (i < variables.Count - 1 ? "," : string.Empty);
			}
			m_writer.Unindent ();
			m_writer.WriteLine (")");
		}

		public override void VisitVariableDefinition (VariableDefinition var)
		{
			m_writer.Write (Formater.Signature (var.VariableType));
			m_writer.Write (Formater.Escape (var.Name));
		}


		string Label (Instruction instr)
		{
			return string.Concat ("IL_", instr.Offset.ToString ("x4"));
		}

		void CheckExceptionHandlers (Instruction instr)
		{
		}

		//FIXME: Where should this be? move to ReflectionDisassembler ?
		public void VisitMemberReference (MemberReference member)
		{
			if (member == null)
				return;

			MethodReference method = member as MethodReference;
			if (method != null) {
				StringBuilder sb = new StringBuilder (" ");
				if (method.HasThis)
					sb.Append ("instance ");

				if (method.ReturnType.ReturnType == null)
					Console.WriteLine ("method = {0}, rt = {1}", method.Name, method.ReturnType);
				sb.Append (Formater.Signature (method.ReturnType.ReturnType, false, true));
				sb.Append (" ");
				sb.Append (Formater.Signature (method.DeclaringType));
				sb.Append ("::");
				sb.Append (Formater.Escape (method.Name));
				sb.Append ("(");
				for (int i = 0; i < method.Parameters.Count; i++) {
					sb.Append (Formater.Signature (method.Parameters [i].ParameterType, false, true));
					if (i < method.Parameters.Count - 1)
						sb.Append (", ");
				}
				sb.Append (")");

				m_writer.Write (sb.ToString ());

				return;
			}

			FieldReference field = member as FieldReference;
			if (field != null) {
				StringBuilder sb = new StringBuilder (" ");

				sb.Append (Formater.Signature (field.FieldType, false, true));
				sb.Append (" ");
				sb.Append (Formater.Signature (field.DeclaringType, true));
				sb.Append ("::");
				sb.Append (Formater.Escape (field.Name));
				m_writer.Write (sb.ToString ());

				return;
			}
		}

		public override void VisitInstructionCollection (InstructionCollection instructions)
		{
			foreach (Instruction instr in instructions) {
				CheckExceptionHandlers (instr);
				m_writer.Write ("{0}:  {1}",
					Label (instr), instr.OpCode.Name);

				switch (instr.OpCode.OperandType) {
				case OperandType.InlineNone :
					break;
				case OperandType.InlineSwitch :
					m_writer.WriteLine ("(");
					m_writer.Indent ();
					Instruction [] targets = (Instruction []) instr.Operand;
					for (int i = 0; i < targets.Length; i++) {
						m_writer.Write (Label (targets [i]));
						m_writer.WriteLine (i < targets.Length - 1 ? "," : string.Empty);
					}
					m_writer.Unindent ();
					m_writer.Write (")");
					break;
				case OperandType.ShortInlineBrTarget :
				case OperandType.InlineBrTarget :
					m_writer.WriteLine (Label ((Instruction) instr.Operand));
					break;
				case OperandType.ShortInlineVar :
				case OperandType.InlineVar :
					VariableDefinition var = (VariableDefinition) instr.Operand;
					if (var.Name != null && var.Name.Length > 0)
						m_writer.Write (var.Name);
					else
						m_writer.Write (instructions.Container.Variables.IndexOf (var));
					break;
				case OperandType.ShortInlineParam :
				case OperandType.InlineParam :
					ParameterDefinition param = (ParameterDefinition) instr.Operand;
					if (param.Name != null && param.Name.Length > 0)
						m_writer.Write (Formater.Escape (param.Name));
					else
						m_writer.Write (instructions.Container.Method.Parameters.IndexOf (param));
					break;
				case OperandType.InlineI :
				case OperandType.InlineI8 :
				case OperandType.InlineR :
				case OperandType.ShortInlineI :
				case OperandType.ShortInlineR :
					m_writer.Write (instr.Operand.ToString ());
					break;
				case OperandType.InlineString :
					//FIXME: Handle unicode strings with non zero high byte
					StringBuilder sb = new StringBuilder (instr.Operand.ToString ());
					//FIXME: extract to a method
					sb.Replace ("\"", "\\\"");
					sb.Replace ("\t", "\\t");
					sb.Replace ("\r", "\\r");
					sb.Replace ("\n", "\\n");

					m_writer.Write (String.Concat (
						"\"", sb.ToString (), "\""));
					break;
				case OperandType.InlineType :
					m_writer.Write (Formater.Signature ((TypeReference) instr.Operand));
					break;
				case OperandType.InlineMethod :
				case OperandType.InlineField :
					VisitMemberReference (instr.Operand as MemberReference);
					break;
				case OperandType.InlineTok :
					if (instr.Operand is TypeReference) {
						m_writer.Write (Formater.Signature ((TypeReference) instr.Operand, true));
					} else if (instr.Operand is MemberReference) {
						if (instr.Operand is FieldReference)
							m_writer.Write ("field");
						//FIXME: method
						VisitMemberReference ((MemberReference) instr.Operand);
					}
					break;
				}

				m_writer.WriteLine ();
			}
		}
	}
}
