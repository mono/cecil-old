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
    internal enum PropertyAttributes : ushort {
        SpecialName     = 0x0200,    // Property is special
        RTSpecialName   = 0x0400,    // Runtime(metadata internal APIs) should check name encoding
        HasDefault      = 0x1000,    // Property has default
        Unused          = 0xe9ff     // Reserved: shall be zero in a conforming implementation
    }
}
