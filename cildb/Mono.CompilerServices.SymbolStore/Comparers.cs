//
// Comparers.cs
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

	class Comparers {

		public class MethodComparer : IComparer {

			public static readonly MethodComparer Instance = new MethodComparer ();

			public int Compare (object x, object y)
			{
				Method a = x as Method;
				Method b = y as Method;

				return Comparer.Default.Compare (a.Token, b.Token);
			}
		}

		public class VariableComparer : IComparer {

			public static readonly VariableComparer Instance = new VariableComparer ();

			public int Compare (object x, object y)
			{
				Variable a = x as Variable;
				Variable b = y as Variable;

				return Comparer.Default.Compare (a.Index, b.Index);
			}
		}

		public class SequencePointComparer : IComparer {

			public static readonly SequencePointComparer Instance = new SequencePointComparer ();

			public int Compare (object x, object y)
			{
				SequencePoint a = x as SequencePoint;
				SequencePoint b = y as SequencePoint;

				return Comparer.Default.Compare (a.Offset, b.Offset);
			}
		}

		public class ScopeComparer : IComparer {

			public static readonly ScopeComparer Instance = new ScopeComparer ();

			public int Compare (object x, object y)
			{
				Scope a = x as Scope;
				Scope b = y as Scope;

				return Comparer.Default.Compare (a.StartOffset, b.StartOffset);
			}
		}
	}
}
