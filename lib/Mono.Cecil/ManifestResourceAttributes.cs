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

    using System;

    [Flags]
    public enum ManifestResourceAttributes {
        VisibilityMask  = 0x0007,
        Public          = 0x0001,    // The resource is exported from the Assembly
        Private         = 0x0002     // The resource is private to the Assembly
    }
}
