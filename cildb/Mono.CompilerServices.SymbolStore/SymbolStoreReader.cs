//
// SymbolStoreReader.cs
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

namespace Mono.CompilerServices.SymbolStore {

	using System;
	using System.Collections;
	using System.IO;
	using System.Text;

	class SymbolStoreReader {

		SymbolStore m_store;

		int m_entryPointToken;
		int m_methodCount;
		Scope [] m_scopes;
		int m_variableCount;
		int m_usingCount;
		int m_constantCount;
		Document [] m_documents;
		SequencePoint [] m_sequencePoints;

		long m_tablesPointer;
		long m_miscHeapPointer;
		IDictionary m_blobCache;
		long m_stringHeapPointer;
		IDictionary m_stringCache;

		BinaryReader m_binaryReader;

		public SymbolStoreReader (BinaryReader reader)
		{
			m_binaryReader = reader;
			m_blobCache = new Hashtable ();
			m_stringCache = new Hashtable ();
		}

		int Int ()
		{
			return m_binaryReader.ReadInt32 ();
		}

		void Pop1 ()
		{
			Int ();
		}

		void Pop2 ()
		{
			Int ();
			Int ();
		}

		void Pop (int n)
		{
			for (int i = 0; i < n; i++)
				Int ();
		}

		string String ()
		{
			int pos = Int ();
			if (pos == 0)
				return string.Empty;

			if (m_stringCache.Contains (pos))
				return (string) m_stringCache [pos];

			return string.Empty;
		}

		byte [] Misc (int pos, int size)
		{
			if (pos == 0 || size == 0)
				return new byte [0];

			if (m_blobCache.Contains (pos))
				return (byte []) m_blobCache [pos];

			long oldpos = m_binaryReader.BaseStream.Position;
			m_binaryReader.BaseStream.Position = m_miscHeapPointer + pos;
			byte [] blob = m_binaryReader.ReadBytes (size);
			m_blobCache [pos] = blob;
			m_binaryReader.BaseStream.Position = oldpos;
			return blob;
		}

		byte [] MiscPosSize ()
		{
			return Misc (Int (), Int ());
		}

		byte [] MiscSizePosFuck ()
		{
			int size = Int ();
			int pos = Int ();
			return Misc (pos, size);
		}

		Guid Guid ()
		{
			return new Guid (m_binaryReader.ReadBytes (16));
		}

		int Enum (Type enumeration)
		{
			return GuidAttribute.GetValueFromGuid (Guid (), enumeration);
		}

		Scope Parent ()
		{
			return m_scopes [Int ()];
		}

		void Pos (int pos)
		{
			m_binaryReader.BaseStream.Position = pos;
		}

		public void ReadSymbolStore (SymbolStore store)
		{
			if (Encoding.ASCII.GetString (m_binaryReader.ReadBytes (16)) != store.Signature)
				throw new FormatException ("Invalid file signature");

			m_store = store;

			store.Version = new Guid (m_binaryReader.ReadBytes (16));

			m_entryPointToken = Int ();
			m_methodCount = Int ();
			m_scopes = new Scope [Int ()];
			m_variableCount = Int ();
			m_usingCount = Int ();
			m_constantCount = Int ();
			m_documents = new Document [Int ()];
			m_sequencePoints = new SequencePoint [Int ()];

			int miscHeapSize = Int ();
			int stringHeapSize = Int ();

			m_tablesPointer = m_binaryReader.BaseStream.Position;

			int constantPointer = (int) m_tablesPointer;
			int methodPointer = constantPointer + m_constantCount * 40;
			int scopePointer = methodPointer + m_methodCount * 52;
			int variablePointer = scopePointer + m_scopes.Length * 20;
			int usingPointer = variablePointer + m_variableCount * 56;
			int spPointer = usingPointer + m_usingCount * 8;
			int docPointer = spPointer + m_sequencePoints.Length * 24;
			m_miscHeapPointer = docPointer + m_documents.Length * 84;
			m_stringHeapPointer = m_miscHeapPointer + miscHeapSize;

			int size = (int) m_stringHeapPointer + stringHeapSize;

			if (size != m_binaryReader.BaseStream.Length)
			    throw new FormatException ("Non valid file");

			Pos ((int) m_stringHeapPointer);
			ReadStrings ();
			Pos (scopePointer);
			ReadScopes ();
			Pos (variablePointer);
			ReadVariables ();
			Pos (usingPointer);
			ReadUsings ();
			Pos (constantPointer);
			ReadConstants ();
			Pos (docPointer);
			ReadDocuments ();
			Pos (spPointer);
			ReadSequencePoints ();
			Pos (methodPointer);
			ReadMethods ();
		}

		void ReadStrings ()
		{
			BinaryReader br = new BinaryReader (m_binaryReader.BaseStream, Encoding.UTF8);
			br.BaseStream.Position = m_stringHeapPointer;
			if (br.ReadChar () != '\0')
				throw new FormatException ("Malformed String heap");

			StringBuilder buffer = new StringBuilder ();
			for (long index = br.BaseStream.Position; br.BaseStream.Position < br.BaseStream.Length; ) {

				char cur = br.ReadChar ();
				if (cur == '\0' && buffer.Length > 0) {
					m_stringCache [(int) (index - m_stringHeapPointer)] = buffer.ToString ();
					buffer = new StringBuilder ();
					index = br.BaseStream.Position;
				} else
					buffer.Append (cur);
			}
		}

		void ReadScopes ()
		{
			for (int i = 0, len = m_scopes.Length; i < len; i++) {
				Scope s = new Scope ();
				int parent = Int ();
				s.StartOffset = Int ();
				s.EndOffset = Int ();
				Pop2 ();

				if (parent != -1) {
					s.Parent = m_scopes [parent];
					s.Parent.Scopes.Add (s);
				}

				m_scopes [i] = s;
			}
		}

		void ReadVariables ()
		{
			for (int i = 0; i < m_variableCount; i++) {
				Scope parent = Parent ();

				Variable v = new Variable (
					String (),
					(VariableAttributes) Int ());

				v.Signature = MiscPosSize ();
				Pop1 ();
				v.Index = Int ();
				Pop2 ();
				v.StartOffset = Int ();
				v.EndOffset = Int ();
				Pop2 ();
				v.HiddenFromDebugger = Int () == 1;

				parent.Variables.Add (v);
			}
		}

		void ReadUsings ()
		{
			for (int i = 0; i < m_usingCount; i++)
				Parent ().Usings.Add (new Using (String ().Substring (1)));
		}

		void ReadConstants ()
		{
			m_binaryReader.ReadBytes (m_constantCount * 40);
			//for (int i = 0; i < m_constantCount; i++) {
			//    Scope parent = Parent ();

			//    Constant cs = new Constant (String ());
			//    cs.Signature = MiscPosSize ();
			//    cs.Value = MiscPosSize ();

			//    parent.Constants.Add (cs);
			//}
		}

		void ReadDocuments ()
		{
			for (int i = 0, len = m_documents.Length; i < len; i++) {
				Document doc = new Document ();
				doc.Language = (Language) Enum (typeof (Language));
				doc.LanguageVendor = (LanguageVendor) Enum (typeof (LanguageVendor));
				doc.DocumentType = (DocumentType) Enum (typeof (DocumentType));
				doc.HashAlgorithm = (HashAlgorithm) Enum (typeof (HashAlgorithm));

				doc.CheckSum = MiscSizePosFuck ();
				doc.Source = MiscSizePosFuck ();
				doc.Url = String ();

				m_documents [i] = doc;
			}
		}

		void ReadSequencePoints ()
		{
			for (int i = 0, len = m_sequencePoints.Length; i < len; i++) {
				SequencePoint sp = new SequencePoint ();
				sp.Offset = Int ();
				sp.StartLine = Int ();
				sp.StartColumn = Int ();
				sp.EndLine = Int ();
				sp.EndColumn = Int ();
				int doc = Int ();
				sp.Document = m_documents [doc];

				m_sequencePoints [i] = sp;
			}
		}

		void ReadMethods ()
		{
			for (int i = 0; i < m_methodCount; i++) {
				Method meth = new Method (Int ());
				if (meth.Token == m_entryPointToken)
					m_store.EntryPoint = meth;

				for (int j = Int (), k = Int (); j < k; j++)
					meth.Scopes.Add (m_scopes [j]);
				Pop (6);
				for (int j = Int (), k = Int (); j < k; j++)
					meth.Documents.Add (m_documents [j]);
				for (int j = Int (), k = Int (); j < k; j++)
					meth.SequencePoints.Add (m_sequencePoints [j]);

				m_store.Methods.Add (meth);
			}
		}
	}
}
