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

    public enum NativeType {
        Boolean = 0x02,
        I1 = 0x03,
        U1 = 0x04,
        I2 = 0x05,
        U2 = 0x06,
        I4 = 0x07,
        U4 = 0x08,
        I8 = 0x09,
        U8 = 0x0a,
        R4 = 0x0b,
        R8 = 0x0c,
        LPSTR = 0x14,
        INT = 0x1f,
        UINT = 0x20,
        FUNC = 0x26,
        ARRAY = 0x2a
    }
}
