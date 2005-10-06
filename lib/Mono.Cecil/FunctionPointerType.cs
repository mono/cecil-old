//
// FunctionPointerType.cs
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

	using System;

	using Mono.Cecil.Signatures;

	public sealed class FunctionPointerType : TypeReference, IFunctionPointerType {

		MethodReference m_function;

		public bool HasThis {
			get { return m_function.HasThis; }
			set { m_function.HasThis = value; }
		}

		public bool ExplicitThis {
			get { return m_function.ExplicitThis; }
			set { m_function.ExplicitThis = value; }
		}

		public MethodCallingConvention CallingConvention {
			get { return m_function.CallingConvention; }
			set { m_function.CallingConvention = value; }
		}

		public ParameterDefinitionCollection Parameters {
			get { return m_function.Parameters; }
		}

		public MethodReturnType ReturnType {
			get { return m_function.ReturnType; }
			set { m_function.ReturnType = value; }
		}

		public override string Name {
			get { return m_function.Name; }
			set { throw new InvalidOperationException (); }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override IMetadataScope Scope {
			get { return m_function.DeclaringType.Scope; }
		}

		public override string FullName {
			get { return m_function.ToString (); }
		}

		public FunctionPointerType (bool hasThis, bool explicitThis, MethodCallingConvention callConv,
			ParameterDefinitionCollection parameters, MethodReturnType retType) :
			base (string.Empty, string.Empty)
		{
			m_function = new MethodReference ("function", hasThis, explicitThis, callConv);
			m_function.DeclaringType = this;
			m_function.ReturnType = retType;
			foreach (ParameterDefinition param in parameters)
				(m_function.Parameters as ParameterDefinitionCollection).Add (param);
		}
	}
}
