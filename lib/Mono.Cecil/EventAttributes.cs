/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil {

    using System;

    [Flags]
    public enum EventAttributes : ushort {
        SpecialName     = 0x0200,    // Event is special
        RTSpecialName   = 0x0400     // CLI provides 'special' behavior, depending upon the name of the event
    }
}
