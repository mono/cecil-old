#region license
//
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

using System;

using Mono.Cecil;

using Cecil.Decompiler.Ast;

namespace Cecil.Decompiler.Steps {

	class RebuildPropertyReferences : BaseCodeTransformer, IDecompilationStep {

		public static readonly IDecompilationStep Instance = new RebuildPropertyReferences ();

		public override ICodeNode VisitMethodInvocationExpression (MethodInvocationExpression node)
		{
			var method_ref = node.Method as MethodReferenceExpression;
			if (method_ref == null)
				return base.VisitMethodInvocationExpression (node);

			if (!HasPropertyPrefix (method_ref.Method))
				return base.VisitMethodInvocationExpression (node);

			//var method = method_ref.Method.Resolve ();
			var method = method_ref.Method as MethodDefinition;
			if (method == null)
				return base.VisitMethodInvocationExpression (node);

			if (method.IsGetter)
				return ProcessGetter (method_ref, method);
			else if (method.IsSetter)
				return ProcessSetter (node, method_ref, method);
			else
				return base.VisitMethodInvocationExpression (node);
		}

		static bool HasPropertyPrefix (MethodReference method)
		{
			return method.Name.StartsWith ("get_", StringComparison.InvariantCulture)
				|| method.Name.StartsWith ("set_", StringComparison.InvariantCulture);
		}

		static PropertyReferenceExpression ProcessGetter (MethodReferenceExpression method_ref, MethodDefinition method)
		{
			return CreatePropertyReferenceFromMethod (method_ref, method);
		}

		static AssignExpression ProcessSetter (MethodInvocationExpression invoke, MethodReferenceExpression method_ref, MethodDefinition method)
		{
			return new AssignExpression (
				CreatePropertyReferenceFromMethod (method_ref, method),
				invoke.Arguments [0]);
		}

		static PropertyReferenceExpression CreatePropertyReferenceFromMethod (MethodReferenceExpression method_ref, MethodDefinition method)
		{
			return new PropertyReferenceExpression (method_ref.Target, GetProperty (method));
		}

		static PropertyDefinition GetProperty (MethodDefinition accessor)
		{
			return GetProperty ((TypeDefinition) accessor.DeclaringType, accessor);
		}

		static PropertyDefinition GetProperty (TypeDefinition type, MethodDefinition accessor)
		{
			if (type == null)
				return null;

			foreach (PropertyDefinition property in type.Properties) {
				if (property.GetMethod == accessor)
					return property;
				if (property.SetMethod == accessor)
					return property;
			}

			if (type.BaseType == null)
				return null;

			return GetProperty (type.BaseType.Resolve (), accessor);
		}

		public void Process (DecompilationContext context, BlockStatement body)
		{
			Visit (body);
		}
	}
}
