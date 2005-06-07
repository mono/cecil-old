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

    public interface ITypeReference : IType, IMemberReference, ICustomAttributeProvider {

        string Namespace { get; set; }
        IMetadataScope Scope { get; }

        string FullName { get; }
    }
}
