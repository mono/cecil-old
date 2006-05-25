//
// Variable.cs
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

	public class Variable : ISymbolStoreVisitable {

		string m_name;
		VariableAttributes m_attributes;
		byte [] m_signature;
		int m_index;
		int m_start;
		int m_end;
		bool m_hide;

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public VariableAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public byte [] Signature {
			get { return m_signature; }
			set { m_signature = value; }
		}

		public int Index {
			get { return m_index; }
			set { m_index = value; }
		}

		public int StartOffset {
			get { return m_start; }
			set { m_start = value; }
		}

		public int EndOffset {
			get { return m_end; }
			set { m_end = value; }
		}

		public bool HiddenFromDebugger {
			get { return m_hide; }
			set { m_hide = value; }
		}

		public Variable (string name, VariableAttributes attributes)
		{
			m_name = name;
			m_attributes = attributes;
		}

		public void Accept (ISymbolStoreVisitor visitor)
		{
			visitor.VisitVariable (this);
		}
	}
}
