//
// Method.cs
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

	public class Method : IScopeContainer, ISymbolStoreVisitable {

		int m_token;

		ScopeCollection m_scopes;
		DocumentCollection m_documents;
		SequencePointCollection m_sequencePoints;

		public int Token {
			get { return m_token; }
			set { m_token = value; }
		}

		public ScopeCollection Scopes {
			get { return m_scopes; }
		}

		public DocumentCollection Documents {
			get { return m_documents; }
		}

		public SequencePointCollection SequencePoints {
			get { return m_sequencePoints; }
		}

		public Method ()
		{
			m_scopes = new ScopeCollection (this);
			m_documents = new DocumentCollection (this);
			m_sequencePoints = new SequencePointCollection (this);
		}

		public Method (int token) : this ()
		{
			m_token = token;
		}

		public void Accept (ISymbolStoreVisitor visitor)
		{
			visitor.VisitMethod (this);
			m_scopes.Accept (visitor);
			m_documents.Accept (visitor);
			m_sequencePoints.Accept (visitor);
		}
	}
}
