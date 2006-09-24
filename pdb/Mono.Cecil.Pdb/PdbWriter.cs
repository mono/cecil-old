//
// PdbWriter.cs
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

using Mono.Cecil.Cil;

namespace Mono.Cecil.Pdb {

	using System;
	using System.Collections;
	using System.Diagnostics.SymbolStore;

	public class PdbWriter : Cil.ISymbolWriter {

		static readonly Guid Zero = new Guid ();

		ISymbolWriter m_writer;
		Hashtable m_documents;

		internal PdbWriter (ISymbolWriter writer)
		{
			m_writer = writer;
			m_documents = new Hashtable ();
		}

		public void Write (Cil.MethodBody body)
		{
			CreateDocuments (body);
			m_writer.OpenMethod (new SymbolToken ((int) body.Method.MetadataToken.ToUInt ()));
			CreateScopes (body, body.Scopes);
			m_writer.CloseMethod ();
		}

		void CreateScopes (MethodBody body, ScopeCollection scopes)
		{
			foreach (Scope s in scopes) {
				m_writer.OpenScope (s.Start.Offset);

				int start = body.Instructions.IndexOf (s.Start);
				int end = s.End == body.Instructions.Outside ?
					body.Instructions.Count - 1 :
					body.Instructions.IndexOf (s.End);

				ArrayList instructions = new ArrayList ();
				for (int i = start; i <= end; i++)
					instructions.Add (body.Instructions [i]);

				Document doc = null;

				int [] offsets = new int [instructions.Count];
				int [] startRows = new int [instructions.Count];
				int [] startCols = new int [instructions.Count];
				int [] endRows = new int [instructions.Count];
				int [] endCols = new int [instructions.Count];

				for (int i = 0; i < instructions.Count; i++) {
					Instruction instr = (Instruction) instructions [i];
					offsets [i] = instr.Offset;
					if (instr.SequencePoint == null)
						continue;

					if (doc == null)
						doc = instr.SequencePoint.Document;

					startRows [i] = instr.SequencePoint.StartLine;
					startCols [i] = instr.SequencePoint.StartColumn;
					endRows [i] = instr.SequencePoint.EndLine;
					endCols [i] = instr.SequencePoint.EndColumn;
				}

				m_writer.DefineSequencePoints (GetDocument (doc),
					offsets, startRows, startCols, endRows, endCols);

				// TODO: local variables

				CreateScopes (body, s.Scopes);
				m_writer.CloseScope (s.End.Offset);
			}
		}

		void CreateDocuments (MethodBody body)
		{
			foreach (Instruction instr in body.Instructions) {
				if (instr.SequencePoint == null)
					continue;

				GetDocument (instr.SequencePoint.Document);
			}
		}

		ISymbolDocumentWriter GetDocument (Document document)
		{
			ISymbolDocumentWriter docWriter = m_documents [document.Url] as ISymbolDocumentWriter;
			if (docWriter != null)
				return docWriter;

			docWriter = m_writer.DefineDocument (document.Url, Zero, Zero, Zero);
			m_documents [document.Url] = docWriter;
			return docWriter;
		}

		public void Dispose ()
		{
			m_writer.Close ();
		}
	}
}
