//
// MarkStep.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.Linker {

	using System.Collections;

	using Mono.Cecil;
	using Mono.Cecil.Cil;

	public class MarkStep : IStep {

		LinkContext _context;
		Queue _queue;

		public MarkStep ()
		{
			_queue = new Queue ();
		}

		public void Process (LinkContext context)
		{
			_context = context;

			InitializeQueue ();
			Process ();
		}

		void InitializeQueue ()
		{
			foreach (AssemblyMarker ac in _context.GetAssemblies ())
				foreach (TypeMarker type in ac.GetTypes ())
					foreach (MethodMarker meth in type.GetMethods ())
						_queue.Enqueue (meth);
		}

		void Process ()
		{
			if (_queue.Count == 0)
				throw new LinkException ("No entry methods");

			while (_queue.Count > 0) {
				MethodMarker method = (MethodMarker) _queue.Dequeue ();
				ProcessMethod (method);
			}
		}

		void MarkMethodBody (MethodBody body)
		{
			foreach (VariableDefinition var in body.Variables)
				MarkType (var.VariableType);

			foreach (ExceptionHandler eh in body.ExceptionHandlers)
				if (eh.Type == ExceptionHandlerType.Catch)
					MarkType (eh.CatchType);

			foreach (Instruction instruction in body.Instructions)
				MarkInstruction (instruction);
		}

		void MarkCustomAttributes (ICustomAttributeProvider provider)
		{
			foreach (CustomAttribute ca in provider.CustomAttributes)
				MarkMethod (ca.Constructor);
		}

		void MarkField (FieldReference field)
		{
			AssemblyMarker am = _context.Resolve (field.DeclaringType.Scope);
			FieldDefinition fd = field as FieldDefinition;
			if (fd == null)
				fd = am.Resolve (field);

			TypeMarker tm = am.Mark ((TypeDefinition) fd.DeclaringType);
			FieldMarker fm = tm.Mark (fd);
			if (fm.Processed)
				return;
			else
				fm.Processed = true;

			MarkType (fd.DeclaringType);
			MarkType (fd.FieldType);
			MarkCustomAttributes (fd);
		}

		void MarkType (TypeReference type)
		{
			if (type == null)
				return;

			while (type is TypeSpecification)
				type = ((TypeSpecification) type).ElementType;

			AssemblyMarker am = _context.Resolve (type.Scope);
			TypeDefinition td = type as TypeDefinition;
			if (td == null)
				td = am.Resolve (type);

			TypeMarker tm = am.Mark (td);
			if (tm.Processed)
				return;
			else
				tm.Processed = true;

			MarkType (td.BaseType);
			MarkCustomAttributes (td);

			am.Mark (td);
		}

		void MarkMethod (MethodReference method)
		{
			while (method is MethodSpecification)
				method = ((MethodSpecification) method).ElementMethod;

			AssemblyMarker am = _context.Resolve (method.DeclaringType.Scope);
			MethodDefinition md = method as MethodDefinition;
			if (md == null)
				md = am.Resolve (method);

			TypeMarker tm = am.Mark ((TypeDefinition) md.DeclaringType);
			MethodMarker mm = tm.Mark (md);

			_queue.Enqueue (mm);
		}

		void ProcessMethod (MethodMarker mm)
		{
			MethodDefinition md = mm.Method;
			AssemblyMarker am = _context.Resolve (md.DeclaringType.Scope);

			if (mm.Processed)
				return;
			else
				mm.Processed = true;

			MarkType (md.DeclaringType);
			MarkCustomAttributes (md);

			foreach (ParameterDefinition pd in md.Parameters) {
				MarkType (pd.ParameterType);
				MarkCustomAttributes (pd);
			}

			MarkType (md.ReturnType.ReturnType);
			MarkCustomAttributes (md.ReturnType);

			if (md.HasBody && (mm.Action == MethodAction.ForceParse ||
				(am.Action == AssemblyAction.Link && mm.Action == MethodAction.ParseIfLinked))) {

				MarkMethodBody (md.Body);
			}
		}

		void MarkInstruction (Instruction instruction)
		{
			switch (instruction.OpCode.OperandType) {
			case OperandType.InlineField:
				MarkField ((FieldReference) instruction.Operand);
				break;
			case OperandType.InlineMethod:
				MarkMethod ((MethodReference) instruction.Operand);
				break;
			case OperandType.InlineTok:
				object token = instruction.Operand;
				if (token is TypeReference)
					MarkType ((TypeReference) token);
				else if (token is MethodReference)
					MarkMethod ((MethodReference) token);
				else
					MarkField ((FieldReference) token);
				break;
			case OperandType.InlineType:
				MarkType ((TypeReference) instruction.Operand);
				break;
			default:
				break;
			}
		}
	}
}
