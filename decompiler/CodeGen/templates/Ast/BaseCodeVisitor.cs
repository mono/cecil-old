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
using System.Collections;

namespace Cecil.Decompiler.Ast {

	public class BaseCodeVisitor : ICodeVisitor {

		public virtual void Visit (ICodeNode node)
		{
			if (null == node)
				return;

			switch (node.CodeNodeType) {
<%
	for node in model.GetVisitableNodes():
%>			case CodeNodeType.${node.Name}:
				Visit${node.Name} ((${node.Name}) node);
				break;
<%
	end
%>			default:
				throw new ArgumentException ();
			}
		}

		public virtual void Visit (IEnumerable collection)
		{
			foreach (ICodeNode node in collection)
				Visit (node);
		}
<%
	for node in model.GetVisitableNodes():
%>
		public virtual void Visit${node.Name} (${node.Name} node)
		{
<%
		for field in model.GetVisitableFields(node):
%>			Visit (node.${field.Name});
<%
		end
%>		}
<%
	end
%>	}
}
