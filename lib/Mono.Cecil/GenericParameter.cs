//
// GenericParameter.cs
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

	public enum GenericInstanceKind : byte {
		Type,
		Method
	}

	public sealed class GenericParameter : TypeReference, IGenericParameter {

		int m_position;
		string m_name;
		GenericInstanceKind m_kind;
		GenericParamAttributes m_attributes;
		IGenericParameterProvider m_owner;
		GenericParameterConstraints m_constraints;

		public int Position {
			get { return m_position; }
			set { m_position = value; }
		}

		public GenericInstanceKind Kind {
			get { return m_kind; }
		}

		public GenericParamAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public IGenericParameterProvider Owner {
			get { return m_owner; }
		}

		public GenericParameterConstraints Constraints {
			get { return m_constraints; }
		}

		public override string Name {
			get {
				string name = m_name != null ? m_name : m_position.ToString ();
				if (m_kind == GenericInstanceKind.Type)
					return string.Concat ("!", name);
				else if (m_kind == GenericInstanceKind.Method)
					return string.Concat ("!!", name);
				else
					throw new InvalidOperationException ();
			}
			set { m_name = value; }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override string FullName {
			get { return Name; }
		}

		public GenericParameter (int pos, GenericInstanceKind kind) :
			base (string.Empty, string.Empty)
		{
			m_position = pos;
			m_constraints = new GenericParameterConstraints ();
		}

		public GenericParameter (int pos, IGenericParameterProvider owner) :
			base (string.Empty, string.Empty)
		{
			m_position = pos;
			m_owner = owner;
			if (owner is TypeReference)
				m_kind = GenericInstanceKind.Type;
			else if (owner is MethodReference)
				m_kind = GenericInstanceKind.Method;
			else
				throw new InvalidOperationException ("Unknown kind of generic parameter");

			m_constraints = new GenericParameterConstraints ();
		}
	}
}
