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

namespace Mono.Cecil.Signatures {

    using Mono.Cecil.Metadata;

    internal sealed class CustomMod {

        public enum CMODType : byte {
            None = 0x0,
            OPT = (byte) ElementType.CModOpt,
            REQD = (byte) ElementType.CModReqD
        }

        public CMODType CMOD;
        public MetadataToken TypeDefOrRef;
    }
}
