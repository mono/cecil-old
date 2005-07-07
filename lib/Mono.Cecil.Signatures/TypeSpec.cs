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

namespace Mono.Cecil.Signatures {

	using Mono.Cecil.Metadata;

	internal class TypeSpec {

		public SigType Type;

		public TypeSpec (SigType type)
		{
			switch (type.ElementType) {
			case ElementType.Ptr :
			case ElementType.FnPtr :
			case ElementType.Array :
			case ElementType.SzArray :
				Type = type;
				return;
			default :
				throw new ReflectionException ("Non valid TypeSpec");
			}
		}
	}
}
