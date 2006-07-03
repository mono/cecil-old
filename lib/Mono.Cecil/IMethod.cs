//
// IMethod.cs
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

	using Mono.Cecil.Cil;

	public interface IMethodReference : IMethodSignature, IMemberReference,
		IGenericParameterProvider, IReflectionVisitable {
	}

	public interface IMethodDefinition : IMemberDefinition, IMethodReference,
		IHasSecurity, ICustomAttributeProvider {

		MethodAttributes Attributes { get; set; }
		MethodImplAttributes ImplAttributes { get; set; }
		MethodSemanticsAttributes SemanticsAttributes { get; set; }

		bool IsAbstract { get; set; }
		bool IsFinal { get; set; }
		bool IsHideBySignature { get; set; }
		bool IsNewSlot { get; set; }
		bool IsRuntimeSpecialName { get; set; }
		bool IsSpecialName { get; set; }
		bool IsStatic { get; set; }
		bool IsVirtual { get; set; }
		bool IsConstructor { get; }
		bool HasBody { get; }
		bool IsRuntime { get; set; }
		bool IsInternalCall { get; set; }

		OverrideCollection Overrides { get; }
		MethodBody Body { get; }
		PInvokeInfo PInvokeInfo { get; }

		ParameterDefinition This { get; }

		MethodDefinition Clone ();
	}
}
