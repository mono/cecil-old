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
        RelocsStripped = 0x0001,
        ExecutableImage = 0x0002,
        LineNumsStripped = 0x0004,
        LocalSymsStripped = 0x0008,
        AggressiveWSTrim = 0x0010,
        LargeAddressAware = 0x0020,
        ReservedForFutureUse = 0x0040,
        BytesReversedLo = 0x0080,
        _32BitsMachine = 0x0100,
        DebugStripped = 0x0200,
        RemovableRunFromSwap = 0x0400,
        NetRunFromSwap = 0x0800,
        System = 0x1000,
        Dll = 0x2000,
        UPSystemOnly = 0x4000,
        BytesReversedHI = 0x8000,

        __flags = 0x0002 | 0x0004 | 0x0008 | 0x0100 | 0x0020,

        CILOnlyDll = 0x2000 | (ushort) __flags,
        CILOnlyExe = __flags
    }
}
