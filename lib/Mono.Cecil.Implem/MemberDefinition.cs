/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

    using Mono.Cecil;

    internal abstract class MemberDefinition : MemberReference, IMemberDefinition {

        public MemberDefinition (string name, TypeDefinition declaringType) : base (name, declaringType)
        {
        }
    }
}
