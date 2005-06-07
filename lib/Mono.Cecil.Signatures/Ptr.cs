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

namespace Mono.Cecil.Signatures {

    using Mono.Cecil.Metadata;

    internal sealed class PTR : SigType {

        public CustomMod [] CustomMods;
        public SigType PtrType;
        public bool Void;

        public PTR () : base (ElementType.Ptr)
        {
        }
    }
}
