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

namespace Mono.Cecil.Binary {

    using System;

    [Flags]
    internal enum ImageCharacteristics : ushort {
        __flags = 0x0002 | 0x0004 | 0x0008 | 0x0100 | 0x0020,

        CILOnlyDll = 0x2000 | (ushort)__flags,
        CILOnlyExe = __flags
    }
}

