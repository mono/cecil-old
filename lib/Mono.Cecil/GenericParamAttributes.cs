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

#if NET_2_0

namespace Mono.Cecil {

    using System;

    [Flags]
    internal enum GenericParamAttributes : ushort {
        VarianceMask  = 0x0003,
        NonVariant    = 0x0000,
        Covariant     = 0x0001,
        Contravariant = 0x0002
    }
}

#endif
