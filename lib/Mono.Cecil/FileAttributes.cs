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
    internal enum FileAttributes : uint {
        ContainsMetaData    = 0x0000,    // This is not a resource file
        ContainsNoMetaData  = 0x0001,    // This is a resource file or other non-metadata-containing file
    }
}
