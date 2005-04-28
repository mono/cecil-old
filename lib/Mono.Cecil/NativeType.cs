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
        BOOLEAN = 0x02,
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
        ARRAY = 0x2a,

        // Msft specific
        CURRENCY = 0x0f,
        BSTR = 0x13,
        LPWSTR = 0x15,
        LPTSTR = 0x16,
        FIXEDSYSSTRING = 0x17,
        IUNKNOWN = 0x19,
        IDISPATCH = 0x1a,
        STRUCT = 0x1b,
        INTF = 0x1c,
        SAFEARRAY = 0x1d,
        FIXEDARRAY = 0x1e,
        BYVALSTR = 0x22,
        ANSIBSTR = 0x23,
        TBSTR = 0x24,
        VARIANTBOOL = 0x25,
        ASANY = 0x28,
        LPSTRUCT = 0x2b,
        CUSTOMMARSHALER = 0x2c,
        ERROR = 0x2d,
        MAX = 0x50
    }
}
