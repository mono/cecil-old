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

    internal interface ISignatureVisitor {
        void Visit (MethodDefSig methodDef);
        void Visit (MethodRefSig methodRef);
        void Visit (FieldSig field);
        void Visit (PropertySig property);
        void Visit (LocalVarSig localvar);
        void Visit (LocalVarSig.LocalVarSigType localvarType);
    }
}
