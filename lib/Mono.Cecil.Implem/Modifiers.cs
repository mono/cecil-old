/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using Mono.Cecil;

	internal abstract class ModType : TypeReference, IModifierType {

		private ITypeReference m_elementType;
		private ITypeReference m_modifierType;

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

		public ITypeReference ElementType {
			get { return m_elementType; }
			set { m_elementType = value; }
		}

		public ITypeReference ModifierType {
			get { return m_modifierType; }
			set { m_modifierType = value; }
		}

		public override string FullName {
			get { return m_elementType.FullName; }
		}

		public ModType (ITypeReference elemType, ITypeReference modType) : base (string.Empty, string.Empty)
		{
			m_elementType = elemType;
			m_modifierType = modType;
		}
	}

	internal sealed class ModifierOptional : ModType, IModifierOptional {

		public ModifierOptional (ITypeReference elemType, ITypeReference modType) : base (elemType, modType)
		{
		}
	}

	internal sealed class ModifierRequired : ModType, IModifierRequired {

		public ModifierRequired (ITypeReference elemType, ITypeReference modType) : base (elemType, modType)
		{
		}
	}
}
