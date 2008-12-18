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
using System.Collections.Generic;

namespace Cecil.Decompiler.Ast {

	public class BaseCodeTransformer : ICodeTransformer {

		public virtual ICodeNode Visit (ICodeNode node)
		{
			if (node == null)
				return null;

			switch (node.CodeNodeType) {
<%
	for node in model.GetVisitableNodes():
%>			case CodeNodeType.${node.Name}:
				return Visit${node.Name} ((${node.Name}) node);
<%
	end
%>			default:
				throw new ArgumentException ();
			}
		}

		protected virtual TCollection Visit<TCollection, TElement> (TCollection original)
			where TCollection : class, IList<TElement>, new ()
			where TElement : class, ICodeNode
		{
			TCollection collection = null;

			for (int i = 0; i < original.Count; i++) {
				var element = (TElement) Visit (original [i]);

				if (collection != null) {
					collection.Add (element);
					continue;
				}

				if (!EqualityComparer<TElement>.Default.Equals (element, original [i])) {
					collection = new TCollection ();
					for (int j = 0; j < i; j++)
						collection.Add (original [j]);

					if (element != null)
						collection.Add (element);
				}
			}

			return collection ?? original;
		}
<%
	for node in model.GetCollections():
		itemType = model.GetCollectionItemType(node)
%>
		public virtual ICollection<${itemType}> Visit (${node.Name} node)
		{
			return Visit<${node.Name}, ${itemType}> (node); 
		}
<%
	end

	for node in model.GetVisitableNodes():
%>
		public virtual ICodeNode Visit${node.Name} (${node.Name} node)
		{
<%
		for field in model.GetVisitableFields(node):
%>			node.${field.Name} = (${field.Type.ToString ()}) Visit (node.${field.Name});
<%
		end
%>			return node;
		}
<%
	end
%>	}
}
