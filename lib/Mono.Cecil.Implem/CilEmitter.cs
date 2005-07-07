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

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	internal class CilEmitter : ICilEmitter {

		private ModuleDefinition m_module;
		private MethodDefinition m_meth;
		private MethodBody m_mbody;

		private VariableDefinitionCollection m_lvars;
		private InstructionCollection m_instrs;
		private ExceptionHandlerCollection m_ehs;

		private int m_offset = 0;
		private Stack m_exstack;

		public CilEmitter (MethodDefinition meth)
		{
			m_meth = meth;
			m_mbody = new MethodBody (meth);
			meth.Body = m_mbody;

			m_module = (meth.DeclaringType as TypeDefinition).Module;

			m_lvars = m_mbody.Variables as VariableDefinitionCollection;
			m_instrs = m_mbody.Instructions as InstructionCollection;
			m_ehs = m_mbody.ExceptionHandlers as ExceptionHandlerCollection;
		}

		public IVariableDefinition DefineLocal (string name, ITypeReference type)
		{
			VariableDefinition var = new VariableDefinition (name, m_lvars.Count, m_meth, type);
			m_lvars.Add (var);
			return var;
		}

		public IVariableDefinition DefineLocal (string name, Type type)
		{
			return DefineLocal (name, m_module.Controller.Helper.RegisterType (type));
		}

		public ILabel DefineLabel ()
		{
			return new Label ();
		}

		public void Emit (OpCode opcode)
		{
			if (opcode.OperandType != OperandType.InlineNone)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode);
		}

		public void Emit (OpCode opcode, ITypeReference type)
		{
			if (opcode.OperandType != OperandType.InlineType &&
				opcode.OperandType != OperandType.InlineTok)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, type);
			m_offset += 4;
		}

		public void Emit (OpCode opcode, Type type)
		{
			Emit (opcode, m_module.Controller.Helper.RegisterType (type));
		}

		public void Emit (OpCode opcode, IMethodReference meth)
		{
			if (opcode.OperandType != OperandType.InlineMethod &&
				opcode.OperandType != OperandType.InlineTok)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, meth);
			m_offset += 4;
		}

		public void Emit (OpCode opcode, System.Reflection.MethodInfo meth)
		{
			Emit (opcode, m_module.Controller.Helper.RegisterMethod (meth));
		}

		public void Emit (OpCode opcode, System.Reflection.ConstructorInfo ctor)
		{
			Emit (opcode, m_module.Controller.Helper.RegisterConstructor (ctor));
		}

		public void Emit (OpCode opcode, IFieldReference field)
		{
			if (opcode.OperandType != OperandType.InlineField &&
				opcode.OperandType != OperandType.InlineTok)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, field);
			m_offset += 4;
		}

		public void Emit (OpCode opcode, System.Reflection.FieldInfo field)
		{
			Emit (opcode, m_module.Controller.Helper.RegisterField (field));
		}

		public void Emit (OpCode opcode, string str)
		{
			if (opcode.OperandType != OperandType.InlineString)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, str);
			m_offset += 4;
		}

		public void Emit (OpCode opcode, byte b)
		{
			if (opcode.OperandType != OperandType.ShortInlineI)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, b);
			m_offset ++;
		}

		public void Emit (OpCode opcode, int i)
		{
			if (opcode.OperandType != OperandType.InlineI)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, i);
			m_offset += 4;
		}

		public void Emit (OpCode opcode, long l)
		{
			if (opcode.OperandType != OperandType.InlineI8)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, l);
			m_offset += 8;
		}

		public void Emit (OpCode opcode, float f)
		{
			if (opcode.OperandType != OperandType.ShortInlineR)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, f);
			m_offset += 4;
		}

		public void Emit (OpCode opcode, double d)
		{
			if (opcode.OperandType != OperandType.InlineR)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, d);
			m_offset += 8;
		}

		public void Emit (OpCode opcode, ILabel label)
		{
			if (opcode.OperandType != OperandType.InlineBrTarget &&
				opcode.OperandType != OperandType.ShortInlineBrTarget)
				throw new ArgumentException ("opcode");

			Instruction instr = null;
			Label lbl = label as Label;
			if (opcode.OperandType == OperandType.InlineBrTarget) {
				FinalEmit (opcode, lbl.Offset);
				m_offset += 4;
			} else { // ShortInlineBrTarget
				FinalEmit (opcode, Convert.ToByte (lbl.Offset));
				m_offset ++;
			}

			lbl.Owner = instr;
			if (lbl.Marked)
				lbl.Used = true;
		}

		public void Emit (OpCode opcode, ILabel [] labels)
		{
			if (opcode.OperandType != OperandType.InlineSwitch)
				throw new ArgumentException ("opcode");

			int [] table = new int [labels.Length];
			for (int i = 0; i < table.Length; i++) {
				Label lbl = labels [i] as Label;
				lbl.Position = i;
				table [i] = lbl.Offset;
				if (lbl.Marked)
					lbl.Used = true;
			}

			FinalEmit (opcode, table);
			m_offset += 4 * labels.Length + 1;
		}

		public void Emit (OpCode opcode, IVariableDefinition var)
		{
			if (opcode.OperandType != OperandType.ShortInlineVar &&
				opcode.OperandType != OperandType.InlineVar)
				throw new ArgumentException ("opcode");

			FinalEmit (opcode, var);
			m_offset += opcode.OperandType == OperandType.InlineVar ? 4 : 1;
		}

		private void FinalEmit (OpCode opcode)
		{
			// should compute maxstack
			FinalEmit (opcode, null);
		}

		private void FinalEmit (OpCode opcode, object operand)
		{
			m_instrs.Add (new Instruction (m_offset, opcode, operand));
			m_offset += opcode.Size;
		}

		public void MarkLabel (ILabel label)
		{
			Label lbl = label as Label;
			lbl.Marked = true;
			lbl.Offset = m_offset;

			if (!lbl.Used) {
				Instruction ins = lbl.Owner;
				switch (ins.OpCode.OperandType) {
				case OperandType.ShortInlineBrTarget :
					ins.Operand = Convert.ToByte (lbl.Offset);
					break;
				case OperandType.InlineBrTarget :
					ins.Operand = lbl.Offset;
					break;
				case OperandType.InlineSwitch :
					int [] table = (int []) ins.Operand;
					table [lbl.Position] = lbl.Offset;
					break;
				}
				lbl.Used = true;
			}
		}

		private class TryBlock {

			public int Offset;
			public int Length;
			public readonly ArrayList Handlers = new ArrayList ();

			private Label m_lbl;

			public Label Label {
				get { return m_lbl; }
			}

			public TryBlock (int toffset, Label lbl)
			{
				this.Offset = toffset;
				m_lbl = lbl;
			}
		}

		public ILabel BeginExceptionBlock ()
		{
			if (m_exstack == null)
				m_exstack = new Stack ();

			Label lbl = new Label ();
			TryBlock t = new TryBlock (m_offset, lbl);
			m_exstack.Push (t);
			return lbl;
		}

		public void BeginCatchBlock (ITypeReference type)
		{
			TryBlock t = m_exstack.Peek () as TryBlock;
			if (t == null)
				throw new NotSupportedException ("Not in an exception block");

			ExceptionHandler ch = new ExceptionHandler (ExceptionHandlerType.Catch);
			ch.CatchType = type;
			ch.TryOffset = t.Offset;
			ch.HandlerOffset = m_offset;
			t.Handlers.Add (ch);
		}

		public void BeginCatchBlock (Type type)
		{
			BeginCatchBlock (m_module.Controller.Helper.RegisterType (type));
		}

		public void BeginFilterHeadBlock ()
		{
			TryBlock t = m_exstack.Peek () as TryBlock;
			if (t == null)
				throw new NotSupportedException ("Not in an exception block");

			ExceptionHandler fh = new ExceptionHandler (ExceptionHandlerType.Filter);
			fh.TryOffset = t.Offset;
			fh.FilterOffset = m_offset;
			t.Handlers.Add (fh);
		}

		public void BeginFilterBodyBlock ()
		{
			TryBlock t = m_exstack.Peek () as TryBlock;
			if (t == null)
				throw new NotSupportedException ("Not in an exception block");

			if (t.Handlers.Count == 0)
				throw new NotSupportedException ("Not in an exception filter block");

			ExceptionHandler last = t.Handlers [t.Handlers.Count - 1] as ExceptionHandler;
			if (last.Type != ExceptionHandlerType.Filter)
				throw new NotSupportedException ("Not in an exception filter block");

			Emit (OpCodes.Endfilter);
			last.HandlerOffset = m_offset;
		}

		public void BeginFaultBlock ()
		{
			TryBlock t = m_exstack.Peek () as TryBlock;
			if (t == null)
				throw new NotSupportedException ("Not in an exception block");

			ExceptionHandler fh = new ExceptionHandler (ExceptionHandlerType.Fault);
			fh.TryOffset = t.Offset;
			fh.HandlerOffset = m_offset;
			t.Handlers.Add (fh);
		}

		public void BeginFinallyBlock ()
		{
			TryBlock t = m_exstack.Peek () as TryBlock;
			if (t == null)
				throw new NotSupportedException ("Not in an exception block");

			ExceptionHandler fh = new ExceptionHandler (ExceptionHandlerType.Finally);
			fh.TryOffset = t.Offset;
			fh.HandlerOffset = m_offset;
			t.Handlers.Add (fh);
		}

		public void EndExceptionBlock ()
		{
			TryBlock t = m_exstack.Pop () as TryBlock;
			if (t == null)
				throw new NotSupportedException ("Not in an exception block");
			t.Label.Offset = m_offset;
			t.Label.Marked = true;

			for (int i = 0; i < t.Handlers.Count; i++) {
				ExceptionHandler eh = t.Handlers [i] as ExceptionHandler;

				if (i == 0)
					t.Length = eh.HandlerOffset - t.Offset;
				else {
					ExceptionHandler prev = t.Handlers [i - 1] as ExceptionHandler;
					prev.HandlerLength = eh.HandlerOffset - prev.HandlerOffset;
				}
			}

			if (t.Length == 0)
				t.Length = m_offset - t.Offset;
		}
	}
}
