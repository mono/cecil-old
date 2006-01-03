//
// GenericContext.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
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

namespace Mono.Cecil {

	using System.Text;

	internal class GenericContext {

		GenericArgumentCollection m_typeArgs;
		GenericArgumentCollection m_methodArgs;

		public GenericArgumentCollection TypeArguments {
			get { return m_typeArgs; }
			set { m_typeArgs = value; }
		}

		public GenericArgumentCollection MethodArguments {
			get { return m_methodArgs; }
			set { m_methodArgs = value; }
		}

		public bool Null {
			get { return m_typeArgs == null && m_methodArgs == null; }
		}

		public GenericContext ()
		{
		}

		public GenericContext (TypeReference type, MethodReference meth)
		{
			m_typeArgs = type.GenericArguments;
			m_methodArgs = meth.GenericArguments;
		}

		public GenericContext (IGenericParameterProvider provider)
		{
			if (provider is TypeReference)
				m_typeArgs = (provider as TypeReference).GenericArguments;
			else {
				MethodReference meth = provider as MethodReference;
				m_methodArgs = meth.GenericArguments;
				m_typeArgs = meth.DeclaringType.GenericArguments;
			}
		}

		public GenericContext Clone ()
		{
			GenericContext ctx = new GenericContext ();
			ctx.TypeArguments = m_typeArgs;
			ctx.MethodArguments = m_methodArgs;
			return ctx;
		}
	}
}
