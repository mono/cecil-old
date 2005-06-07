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
    public enum MethodSemanticsAttributes : ushort {
        Setter      = 0x0001,    // Setter for property
        Getter      = 0x0002,    // Getter for property
        Other       = 0x0004,    // Other method for property or event
        AddOn       = 0x0008,    // AddOn method for event
        RemoveOn    = 0x0010,    // RemoveOn method for event
        Fire        = 0x0020     // Fire method for event
    }
}
