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
    internal enum SectionCharacteristics : uint {
        ContainsCode = 0x00000020,
        ContainsInitializedData = 0x00000040,
        ContainsUninitializedData = 0x00000080,
        MemoryExecute = 0x20000000,
        MemoryRead = 0x40000000,
        MemoryWrite = 0x80000000
    }
}
