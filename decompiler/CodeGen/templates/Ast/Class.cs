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

<%

fields = model.GetFields(node)

%>using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cecil.Decompiler.Ast {

	public <% if node.IsAbstract: %>abstract <% end %>class ${node.Name} : ${join(node.BaseTypes, ', ')} {
<%
parameters_fields = model.GetParametersFields(node)

if len (fields) > 0 or len (parameters_fields) > 0:
	for field in fields: %>
		${GetFieldTypeName(field)} ${ToFieldName(field.Name)}<% if field.Initializer is not null: %> = new ${GetFieldTypeName(field)} ()<% end %>;<%end %>

		public ${node.Name} ()
		{
		}
<%
	if parameters_fields.Length > 0:
		args = join("${GetFieldTypeName(field)} ${ToParamName(field.Name)}" for field in parameters_fields, ", ")
		base_args = join("${ToParamName(field.Name)}" for field in model.GetBaseParametersFields(node), ", ")
%>
		public ${node.Name} (${args})<% if base_args.Length > 0: %> : base (${base_args})<% end %>
		{
<%		for field in (field for field in fields if field.Initializer is null):
%>			this.${ToFieldName(field.Name)} = ${ToParamName(field.Name)};
<%		end
%>		}
<%
	end

	for field in fields:
%>
		public ${GetFieldTypeName(field)} ${field.Name}
		{
			get { return ${ToFieldName(field.Name)}; }
			set { this.${ToFieldName(field.Name)} = value; }
		}
<%	end
end %>
<% if node.IsAbstract: %>		public abstract CodeNodeType CodeNodeType {
			get;
		}
<% else: %>		public<% if model.ResolveType(node.BaseTypes[0]): %> override<% end %> CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.${node.Name}; }
		}
<% end %>	}

	public static partial class CodeNode {

	}
}
