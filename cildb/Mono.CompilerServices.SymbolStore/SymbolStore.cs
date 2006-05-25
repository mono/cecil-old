//
// SymbolStore.cs
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

	public class SymbolStore : ISymbolStoreVisitable {

		internal readonly string Signature = "_ildb_signature\0";
		internal readonly Guid StdVersion = new Guid (new byte [] {
			0x7f, 0x55, 0xe7, 0xf1, 0x3c, 0x42, 0x17, 0x41,
			0x8d, 0xa9, 0xc7, 0xa3, 0xcd, 0x98, 0x8d, 0xf1 });

		Guid m_version;
		Method m_entryPoint;
		MethodCollection m_methods;

		public Guid Version {
			get { return m_version; }
			set { m_version = value; }
		}

		public Method EntryPoint {
			get { return m_entryPoint; }
			set { m_entryPoint = value; }
		}

		public MethodCollection Methods {
			get { return m_methods; }
		}

		internal SymbolStore ()
		{
			m_methods = new MethodCollection (this);
		}

		public void Accept (ISymbolStoreVisitor visitor)
		{
			visitor.VisitSymbolStore (this);
			m_methods.Accept (visitor);
			visitor.TerminateSymbolStore (this);
		}
	}
}
