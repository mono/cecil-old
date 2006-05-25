//
// BaseSymbolStoreVisitor.cs
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

	using System.Collections;

	public abstract class BaseSymbolStoreVisitor : ISymbolStoreVisitor {

		public virtual void VisitSymbolStore (SymbolStore store)
		{
		}

		public virtual void VisitMethod (Method meth)
		{
		}

		public virtual void VisitMethodCollection (MethodCollection methods)
		{
		}

		public virtual void VisitScope (Scope scope)
		{
		}

		public virtual void VisitScopeCollection (ScopeCollection scopes)
		{
		}

		public virtual void VisitDocument (Document doc)
		{
		}

		public virtual void VisitDocumentCollection (DocumentCollection docs)
		{
		}

		public virtual void VisitUsing (Using ug)
		{
		}

		public virtual void VisitUsingCollection (UsingCollection usings)
		{
		}

		public virtual void VisitSequencePoint (SequencePoint sp)
		{
		}

		public virtual void VisitSequencePointCollection (SequencePointCollection sps)
		{
		}

		public virtual void VisitConstant (Constant cons)
		{
		}

		public virtual void VisitConstantCollection (ConstantCollection constants)
		{
		}

		public virtual void VisitVariable (Variable variable)
		{
		}

		public virtual void VisitVariableCollection (VariableCollection variables)
		{
		}

		protected void VisitCollection (ICollection collection)
		{
			foreach (ISymbolStoreVisitable visitable in collection)
				if (visitable != null)
					visitable.Accept (this);
		}

		public virtual void TerminateSymbolStore (SymbolStore store)
		{
		}
	}
}
