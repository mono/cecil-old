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

namespace Mono.Cecil {

    public interface ITypeDefinition : ITypeReference {

        IType BaseType { get; set; }
        IInterfaceCollection Interfaces { get; }
        ITypeDefinitionCollection NestedTypes { get; }

        IMethodDefinitionCollection Methods { get; }
    }
}
