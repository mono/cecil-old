//
// Document.cs
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

	public class Document : ISymbolStoreVisitable {

		private Language m_language;
		private LanguageVendor m_languageVendor;
		private DocumentType m_documentType;
		private HashAlgorithm m_algorithm;

		private byte [] m_checksum;
		private byte [] m_source;

		private string m_url;

		public Language Language {
			get { return m_language; }
			set { m_language = value; }
		}

		public LanguageVendor LanguageVendor {
			get { return m_languageVendor; }
			set { m_languageVendor = value; }
		}

		public DocumentType DocumentType {
			get { return m_documentType; }
			set { m_documentType = value; }
		}

		public HashAlgorithm HashAlgorithm {
			get { return m_algorithm; }
			set { m_algorithm = value; }
		}

		public byte [] CheckSum {
			get { return m_checksum; }
			set { m_checksum = value; }
		}

		public byte [] Source {
			get { return m_source; }
			set { m_source = value; }
		}

		public string Url {
			get { return m_url; }
			set { m_url = value; }
		}

		public void Accept (ISymbolStoreVisitor visitor)
		{
			visitor.VisitDocument (this);
		}
	}
}
