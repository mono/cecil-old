/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;
	using SR = System.Reflection;

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	internal class CilWorker : ICilWorker {

		private ModuleDefinition m_module;
		private MethodDefinition m_meth;
		private MethodBody m_mbody;

		private InstructionCollection m_instrs;

		public CilWorker (MethodDefinition meth)
		{
			m_meth = meth;
			m_mbody = new MethodBody (meth);
			meth.Body = m_mbody;

			m_module = (meth.DeclaringType as TypeDefinition).Module;

			m_instrs = m_mbody.Instructions as InstructionCollection;
		}

		public IInstruction Create (OpCode opcode)
		{
			if (opcode.OperandType != OperandType.InlineNone)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode);
		}

		public IInstruction Create (OpCode opcode, ITypeReference type)
		{
			if (opcode.OperandType != OperandType.InlineType &&
				opcode.OperandType != OperandType.InlineTok)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, type);
		}

		public IInstruction Create (OpCode opcode, Type type)
		{
			return Create (opcode, m_module.Controller.Helper.RegisterType (type));
		}

		public IInstruction Create (OpCode opcode, IMethodReference meth)
		{
			if (opcode.OperandType != OperandType.InlineMethod &&
				opcode.OperandType != OperandType.InlineTok)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, meth);
		}

		public IInstruction Create (OpCode opcode, SR.MethodInfo meth)
		{
			return Create (opcode, m_module.Controller.Helper.RegisterMethod (meth));
		}

		public IInstruction Create (OpCode opcode, SR.ConstructorInfo ctor)
		{
			return Create (opcode, m_module.Controller.Helper.RegisterConstructor (ctor));
		}

		public IInstruction Create (OpCode opcode, IFieldReference field)
		{
			if (opcode.OperandType != OperandType.InlineField &&
				opcode.OperandType != OperandType.InlineTok)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, field);
		}

		public IInstruction Create (OpCode opcode, SR.FieldInfo field)
		{
			return Create (opcode, m_module.Controller.Helper.RegisterField (field));
		}

		public IInstruction Create (OpCode opcode, string str)
		{
			if (opcode.OperandType != OperandType.InlineString)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, str);
		}

		public IInstruction Create (OpCode opcode, byte b)
		{
			if (opcode.OperandType != OperandType.ShortInlineI)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, b);
		}

		public IInstruction Create (OpCode opcode, int i)
		{
			if (opcode.OperandType != OperandType.InlineI)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, i);
		}

		public IInstruction Create (OpCode opcode, long l)
		{
			if (opcode.OperandType != OperandType.InlineI8)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, l);
		}

		public IInstruction Create (OpCode opcode, float f)
		{
			if (opcode.OperandType != OperandType.ShortInlineR)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, f);
		}

		public IInstruction Create (OpCode opcode, double d)
		{
			if (opcode.OperandType != OperandType.InlineR)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, d);
		}

		public IInstruction Create (OpCode opcode, IInstruction label)
		{
			if (opcode.OperandType != OperandType.InlineBrTarget &&
				opcode.OperandType != OperandType.ShortInlineBrTarget)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, label);
		}

		public IInstruction Create (OpCode opcode, IInstruction [] labels)
		{
			if (opcode.OperandType != OperandType.InlineSwitch)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, labels);
		}

		public IInstruction Create (OpCode opcode, IVariableDefinition var)
		{
			if (opcode.OperandType != OperandType.ShortInlineVar &&
				opcode.OperandType != OperandType.InlineVar)
				throw new ArgumentException ("opcode");

			return FinalCreate (opcode, var);
		}

		private IInstruction FinalCreate (OpCode opcode)
		{
			return FinalCreate (opcode, null);
		}

		private IInstruction FinalCreate (OpCode opcode, object operand)
		{
			return new Instruction (0, opcode, operand);
		}

		public IInstruction Emit (OpCode opcode)
		{
			IInstruction instr = Create (opcode);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, ITypeReference type)
		{
			IInstruction instr = Create (opcode, type);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, Type type)
		{
			return Emit (opcode, m_module.Controller.Helper.RegisterType (type));
		}

		public IInstruction Emit (OpCode opcode, IMethodReference meth)
		{
			IInstruction instr = Create (opcode, meth);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, SR.MethodInfo meth)
		{
			return Emit (opcode, m_module.Controller.Helper.RegisterMethod (meth));
		}

		public IInstruction Emit (OpCode opcode, SR.ConstructorInfo ctor)
		{
			return Emit (opcode, m_module.Controller.Helper.RegisterConstructor (ctor));
		}

		public IInstruction Emit (OpCode opcode, IFieldReference field)
		{
			IInstruction instr = Create (opcode);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, SR.FieldInfo field)
		{
			return Emit (opcode, m_module.Controller.Helper.RegisterField (field));
		}

		public IInstruction Emit (OpCode opcode, string str)
		{
			IInstruction instr = Create (opcode, str);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, byte b)
		{
			IInstruction instr = Create (opcode, b);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, int i)
		{
			IInstruction instr = Create (opcode, i);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, long l)
		{
			IInstruction instr = Create (opcode, l);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, float f)
		{
			IInstruction instr = Create (opcode, f);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, double d)
		{
			IInstruction instr = Create (opcode, d);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, IInstruction target)
		{
			IInstruction instr = Create (opcode, target);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, IInstruction [] targets)
		{
			IInstruction instr = Create (opcode, targets);
			Append (instr);
			return instr;
		}

		public IInstruction Emit (OpCode opcode, IVariableDefinition var)
		{
			IInstruction instr = Create (opcode, var);
			Append (instr);
			return instr;
		}

		public void InsertBefore (IInstruction target, IInstruction instr)
		{
			Instruction trgt = target as Instruction, current = instr as Instruction;
			int index = m_instrs.IndexOf (trgt);
			if (index == -1)
				throw new ReflectionException ("Target instruction not in method body");

			m_instrs.Insert (index, instr);
			current.Previous = trgt.Previous;
			trgt.Previous = current;
			current.Next = trgt;
		}

		public void InsertAfter (IInstruction target, IInstruction instr)
		{
			Instruction trgt = target as Instruction, current = instr as Instruction;
			int index = m_instrs.IndexOf (trgt);
			if (index == -1)
				throw new ReflectionException ("Target instruction not in method body");

			m_instrs.Insert (index + 1, instr);
			current.Next = trgt.Next;
			trgt.Next = current;
			current.Previous = trgt;
		}

		public void Append (IInstruction instr)
		{
			Instruction last = null, current = instr as Instruction;
			if (m_instrs.Count > 0)
				last = m_instrs [m_instrs.Count - 1] as Instruction;

			if (last != null) {
				last.Next = instr;
				current.Previous = last;
			}

			m_instrs.Add (current);
		}
	}
}
