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

    internal enum RuntimeImage : uint {
        ILOnly = 0x0000001,
        F32BitsRequired = 0x0000002,
        StrongNameSigned = 0x0000008,
        TrackDebugData = 0x00010000
    }
}
