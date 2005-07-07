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

namespace Mono.Cecil {

	using Mono.Cecil.Implem;

	public sealed class TypeFactory {

		private TypeFactory ()
		{
		}

		public static IArrayType GetArrayType (ITypeReference type)
		{
			return new ArrayType (type);
		}

		public static IPinnedType GetPinnedType (ITypeReference type)
		{
			return new PinnedType (type);
		}

		public static IPointerType GetPointerType (ITypeReference type)
		{
			return new PointerType (type);
		}

		public static IModifierOptional GetModifierOptional (ITypeReference modifier, ITypeReference type)
		{
			return new ModifierOptional (type, modifier);
		}

		public static IModifierRequired GetModifierRequired (ITypeReference modifier, ITypeReference type)
		{
			return new ModifierRequired (type, modifier);
		}
	}
}
