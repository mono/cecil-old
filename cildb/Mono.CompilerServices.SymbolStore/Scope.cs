//
// Scope.cs
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

	public class Scope : IScopeContainer, ISymbolStoreVisitable {

		Scope m_parent;
		int m_index = -1;

		int m_start;
		int m_end;

		ScopeCollection m_scopes;
		UsingCollection m_usings;
		ConstantCollection m_constants;
		VariableCollection m_variables;

		public Scope Parent {
			get { return m_parent; }
			set { m_parent = value; }
		}

		internal int Index {
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

		public ScopeCollection Scopes {
			get { return m_scopes; }
		}

		public UsingCollection Usings {
			get { return m_usings; }
		}

		public ConstantCollection Constants {
			get { return m_constants; }
		}

		public VariableCollection Variables {
			get { return m_variables; }
		}

		public Scope ()
		{
			m_scopes = new ScopeCollection (this);
			m_usings = new UsingCollection (this);
			m_constants = new ConstantCollection (this);
			m_variables = new VariableCollection (this);
		}

		public void Accept (ISymbolStoreVisitor visitor)
		{
			visitor.VisitScope (this);
			m_usings.Accept (visitor);
			m_constants.Accept (visitor);
			m_variables.Accept (visitor);
			m_scopes.Accept (visitor);
		}
	}
}
