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
    internal enum SubSystem : ushort {
        Unknown = 0x0,
        Native = 0x1,
        WindowsGui = 0x2,
        WindowsCui = 0x3,
        PosixCui = 0x7,
        WindowsCeGui = 0x9,
        EfiApplication = 0x10,
        EfiBootServiceDriver = 0x11,
        EfiRuntimeDriver = 0x12,
        EfiRom = 0x13,
        Xbox = 0x14,
        NexusAgent = 0x15
    }
}
