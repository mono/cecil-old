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

        private bool m_hasThis;
        private bool m_explicitThis;
        private MethodCallingConvention m_callConv;
        private int m_paramCount;
        private RetType m_retType;
        private Param [] m_parameters;

        public bool HasThis {
            get { return m_hasThis; }
            set { m_hasThis = value; }
        }

        public bool ExplicitThis {
            get { return m_explicitThis; }
            set { m_explicitThis = value; }
        }

        public MethodCallingConvention MethCallConv {
            get { return m_callConv; }
            set { m_callConv = value; }
        }

        public int ParamCount {
            get { return m_paramCount; }
            set { m_paramCount = value; }
        }

        public RetType RetType {
            get { return m_retType; }
            set { m_retType = value; }
        }

        public Param [] Parameters {
            get { return m_parameters; }
            set { m_parameters = value; }
        }

        public MethodSig () : base ()
        {
        }

        public MethodSig (uint blobIndex) : base (blobIndex)
        {
        }
    }
}
