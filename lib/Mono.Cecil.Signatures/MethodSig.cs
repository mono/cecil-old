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

using Mono.Cecil;

namespace Mono.Cecil.Signatures {

    internal abstract class MethodSig : Signature {

        public bool HasThis;
        public bool ExplicitThis;
        public MethodCallingConvention MethCallConv;
        public int ParamCount;
        public RetType RetType;
        public Param [] Parameters;

        public MethodSig () : base ()
        {
        }

        public MethodSig (uint blobIndex) : base (blobIndex)
        {
        }
    }
}
