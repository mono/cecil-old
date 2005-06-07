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

    internal sealed class PropertySig : Signature {

        public bool Property;
        public int ParamCount;
        public SigType Type;
        public Param [] Parameters;

        public PropertySig () : base ()
        {
        }

        public PropertySig (uint blobIndex) : base (blobIndex)
        {
        }

        public override void Accept (ISignatureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
