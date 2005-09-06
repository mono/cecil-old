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

namespace Mono.Cecil.Cil {

	using System;

	public interface ICilWorker {

		IMethodBody GetBody ();

		IInstruction Emit (OpCode opcode);

		IInstruction Emit (OpCode opcode, ITypeReference type);
		IInstruction Emit (OpCode opcode, Type type);

		IInstruction Emit (OpCode opcode, IMethodReference meth);
		IInstruction Emit (OpCode opcode, System.Reflection.MethodInfo meth);
		IInstruction Emit (OpCode opcode, System.Reflection.ConstructorInfo ctor);

		IInstruction Emit (OpCode opcode, IFieldReference field);
		IInstruction Emit (OpCode opcode, System.Reflection.FieldInfo field);

		IInstruction Emit (OpCode opcode, string str);
		IInstruction Emit (OpCode opcode, byte b);
		IInstruction Emit (OpCode opcode, int i);
		IInstruction Emit (OpCode opcode, long l);
		IInstruction Emit (OpCode opcode, float f);
		IInstruction Emit (OpCode opcode, double d);

		IInstruction Emit (OpCode opcode, IInstruction target);
		IInstruction Emit (OpCode opcode, IInstruction [] targets);

		IInstruction Emit (OpCode opcode, IVariableDefinition var);

		IInstruction Emit (OpCode opcode, IParameterDefinition param);

		IInstruction Create (OpCode opcode);

		IInstruction Create (OpCode opcode, ITypeReference type);
		IInstruction Create (OpCode opcode, Type type);

		IInstruction Create (OpCode opcode, IMethodReference meth);
		IInstruction Create (OpCode opcode, System.Reflection.MethodInfo meth);
		IInstruction Create (OpCode opcode, System.Reflection.ConstructorInfo ctor);

		IInstruction Create (OpCode opcode, IFieldReference field);
		IInstruction Create (OpCode opcode, System.Reflection.FieldInfo field);

		IInstruction Create (OpCode opcode, string str);
		IInstruction Create (OpCode opcode, byte b);
		IInstruction Create (OpCode opcode, int i);
		IInstruction Create (OpCode opcode, long l);
		IInstruction Create (OpCode opcode, float f);
		IInstruction Create (OpCode opcode, double d);

		IInstruction Create (OpCode opcode, IInstruction target);
		IInstruction Create (OpCode opcode, IInstruction [] targets);

		IInstruction Create (OpCode opcode, IVariableDefinition var);

		IInstruction Create (OpCode opcode, IParameterDefinition param);

		void InsertBefore (IInstruction target, IInstruction instr);
		void InsertAfter (IInstruction target, IInstruction instr);
		void Append (IInstruction instr);
	}
}
