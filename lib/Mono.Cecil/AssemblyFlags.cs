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
    internal enum AssemblyFlags : uint {
        PublicKey                   = 0x0001,
        SideBySideCompatible        = 0x0000,
        
        EnableJITcompileTracking    = 0x8000,
        DisableJITcompileOptimizer  = 0x4000
    }
}
