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

namespace Mono.Cecil.Cil {

    internal enum MethodDataSection : ushort {
        EHTable = 0x1,
        OptILTable = 0x2,
        FatFormat = 0x40,
        MoreSects = 0x80
    }
}
