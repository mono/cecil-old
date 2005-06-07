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

    using System;

    [Flags]
    public enum GenericParamAttributes : ushort {
        VarianceMask  = 0x0003,
        NonVariant    = 0x0000,
        Covariant     = 0x0001,
        Contravariant = 0x0002
    }
}
