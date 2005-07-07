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

	internal abstract class MemberDefinition : MemberReference, IMemberDefinition {

		public readonly TypeDefinition DecTypeDef;

		public MemberDefinition (string name, TypeDefinition declaringType) : base (name, declaringType)
		{
			this.DecTypeDef = declaringType;
		}
	}
}
