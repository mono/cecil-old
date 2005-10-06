//
// Modifiers.cs
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

	using Mono.Cecil;

	public abstract class ModType : TypeReference, IModifierType {

		TypeReference m_elementType;
		TypeReference m_modifierType;

		public override string Name {
			get { return m_elementType.Name; }
			set { m_elementType.Name = value; }
		}

		public override string Namespace {
			get { return m_elementType.Namespace; }
			set { m_elementType.Namespace = value; }
		}

		public override IMetadataScope Scope {
			get { return m_elementType.Scope; }
		}

		public TypeReference ElementType {
			get { return m_elementType; }
			set { m_elementType = value; }
		}

		public TypeReference ModifierType {
			get { return m_modifierType; }
			set { m_modifierType = value; }
		}

		public override string FullName {
			get { return m_elementType.FullName; }
		}

		public ModType (TypeReference elemType, TypeReference modType) : base (string.Empty, string.Empty)
		{
			m_elementType = elemType;
			m_modifierType = modType;
		}
	}

	public sealed class ModifierOptional : ModType, IModifierOptional {

		public ModifierOptional (TypeReference elemType, TypeReference modType) : base (elemType, modType)
		{
		}
	}

	public sealed class ModifierRequired : ModType, IModifierRequired {

		public ModifierRequired (TypeReference elemType, TypeReference modType) : base (elemType, modType)
		{
		}
	}
}
