#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
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
#endregion

// Warning: generated do not edit

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cecil.Decompiler.Ast {

	public class MethodAddressExpression : Expression {

		Expression target;
		bool is_virtual;
		MethodReference method;

		public MethodAddressExpression ()
		{
		}
		
		public MethodAddressExpression (MethodReference method)
		{
			this.method = method;
		}

		public Expression Target
		{
			get { return target; }
			set { this.target = value; }
		}

		public MethodReference Method
		{
			get { return method; }
			set { this.method = value; }
		}
		
		public bool IsVirtual
		{
			get { return is_virtual; }
			set { this.is_virtual = value; }
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.MethodAddressExpression; }
		}
	}

	public static partial class CodeNode {

	}
}
