//
// PdbReader.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Cci;
using Microsoft.Cci.Pdb;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Mono.Cecil.Pdb {

	public class PdbCciReader : ISymbolReader {

		Dictionary<string, Document> documents = new Dictionary<string, Document> ();
		Dictionary<uint, PdbFunction> functions = new Dictionary<uint, PdbFunction> ();

		internal PdbCciReader (string file)
		{
			PopulateFunctions (file);
		}

		void PopulateFunctions (string file)
		{
			Stream pdb = File.OpenRead (file);

			foreach (PdbFunction function in PdbFile.LoadFunctions (pdb, true))
				functions.Add (function.token, function);
		}

		public void Read (MethodBody body, IDictionary instructions)
		{
			PdbFunction function;
			if (!functions.TryGetValue (body.Method.MetadataToken.ToUInt (), out function))
				return;

			ReadSequencePoints (function, instructions);
			ReadScopeAndLocals (function.scopes, null, body, instructions);
		}

		static Instruction GetInstruction (MethodBody body, IDictionary instructions, int offset)
		{
			Instruction instr = (Instruction) instructions [offset];
			if (instr != null)
				return instr;

			return body.Instructions.Outside;
		}

		static void ReadScopeAndLocals (PdbScope [] scopes, Scope parent, MethodBody body, IDictionary instructions)
		{
			foreach (PdbScope scope in scopes)
				ReadScopeAndLocals (scope, parent, body, instructions);
		}

		static void ReadScopeAndLocals (PdbScope scope, Scope parent, MethodBody body, IDictionary instructions)
		{
			//Scope s = new Scope ();
			//s.Start = GetInstruction (body, instructions, (int) scope.address);
			//s.End = GetInstruction (body, instructions, (int) scope.length - 1);

			//if (parent != null)
			//	parent.Scopes.Add (s);
			//else
			//	body.Scopes.Add (s);

			foreach (PdbSlot slot in scope.slots) {
				Cil.VariableDefinition variable = body.Variables [(int) slot.slot];
				variable.Name = slot.name;

				//s.Variables.Add (variable);
			}

			ReadScopeAndLocals (scope.scopes, null /* s */, body, instructions);
		}

		void ReadSequencePoints (PdbFunction function, IDictionary instructions)
		{
			foreach (PdbLines lines in function.lines)
				ReadLines (lines, instructions);
		}

		void ReadLines (PdbLines lines, IDictionary instructions)
		{
			Document document = GetDocument (lines.file.name);

			foreach (PdbLine line in lines.lines)
				ReadLines (line, document, instructions);
		}

		void ReadLines (PdbLine line, Document document, IDictionary instructions)
		{
			Instruction instruction = (Instruction) instructions [line.offset];
			if (instruction == null)
				return;

			SequencePoint point = new SequencePoint (document);
			point.StartLine = (int) line.lineBegin;
			point.StartColumn = (int) line.colBegin;
			point.EndLine = (int) line.lineEnd;
			point.EndColumn = (int) line.colEnd;

			instruction.SequencePoint = point;
		}

		Document GetDocument (string name)
		{
			Document document;
			if (documents.TryGetValue (name, out document))
				return document;

			document = new Document (name);
			documents.Add (name, document);
			return document;
		}

		public void Dispose ()
		{
		}
	}
}
