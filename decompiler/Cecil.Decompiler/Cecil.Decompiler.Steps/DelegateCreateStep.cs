#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//  (C) 2010 Alexander Corrado http://nirvanai.com
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

using System;

using Mono.Cecil.Cil;

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps {

	public class DelegateCreateStep : BaseCodeTransformer, IDecompilationStep {

		public static readonly IDecompilationStep Instance = new DelegateCreateStep ();
		
		public override ICodeNode VisitObjectCreationExpression (ObjectCreationExpression node)
		{
			if (node.Constructor == null)
				goto skip;
			
			var type = node.Constructor.DeclaringType;
			
			if (!type.Resolve ().IsDelegate ())
				goto skip;
			
			var target = node.Arguments [0];
			var method = node.Arguments [1] as MethodAddressExpression;
			
			if (method == null)
				goto skip;
			
			return new DelegateCreationExpression (type, method.Method, target);

		skip:
			return base.VisitObjectCreationExpression (node);
		}
		
		public BlockStatement Process (DecompilationContext context, BlockStatement body)
		{
			return (BlockStatement) VisitBlockStatement (body);
		}
	}
}
