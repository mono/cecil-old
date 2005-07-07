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

	public interface ICilEmitter {

		IVariableDefinition DefineLocal (string name, ITypeReference type);
		IVariableDefinition DefineLocal (string name, Type type);

		ILabel DefineLabel ();

		void Emit (OpCode opcode);

		void Emit (OpCode opcode, ITypeReference type);
		void Emit (OpCode opcode, Type type);

		void Emit (OpCode opcode, IMethodReference meth);
		void Emit (OpCode opcode, System.Reflection.MethodInfo meth);
		void Emit (OpCode opcode, System.Reflection.ConstructorInfo ctor);

		void Emit (OpCode opcode, IFieldReference field);
		void Emit (OpCode opcode, System.Reflection.FieldInfo field);

		void Emit (OpCode opcode, string str);
		void Emit (OpCode opcode, byte b);
		void Emit (OpCode opcode, int i);
		void Emit (OpCode opcode, long l);
		void Emit (OpCode opcode, float f);
		void Emit (OpCode opcode, double d);

		void Emit (OpCode opcode, ILabel label);
		void Emit (OpCode opcode, ILabel [] labels);

		void Emit (OpCode opcode, IVariableDefinition var);

		void MarkLabel (ILabel label);

		ILabel BeginExceptionBlock ();
		void BeginCatchBlock (ITypeReference type);
		void BeginCatchBlock (Type type);
		void BeginFilterHeadBlock ();
		void BeginFilterBodyBlock ();
		void BeginFaultBlock ();
		void BeginFinallyBlock ();
		void EndExceptionBlock ();
	}
}
