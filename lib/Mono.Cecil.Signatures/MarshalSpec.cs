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

    using Mono.Cecil;

    internal class MarshalSpec {

        public NativeType NativeInstrinsic;
        public MarshalSpecArray SpecArray;

        public bool IsArray {
            get { return this.NativeInstrinsic == NativeType.ARRAY; }
        }

        public MarshalSpec (NativeType nt)
        {
            this.NativeInstrinsic = nt;
        }

        internal struct MarshalSpecArray {

            public NativeType NativeInstrinsic;
            public int ParamNum;
            public int ElemMult;
            public int NumElem;

            public MarshalSpecArray (NativeType nt, int paramNum, int elemMult, int numElem)
            {
                this.NativeInstrinsic = nt;
                this.ParamNum = paramNum;
                this.ElemMult = elemMult;
                this.NumElem = numElem;
            }
        }
    }
}
