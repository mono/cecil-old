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

namespace Mono.Cecil.Implem  {

    using System;

    public class ReflectionException : Exception {

        internal ReflectionException() : base() {}
        internal ReflectionException(string message) : base(message) {}
        internal ReflectionException(string message, params string[] parameters) : base(string.Format(message, parameters)) {}
        internal ReflectionException(string message, Exception inner) : base(message, inner) {}
    }
}

