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

    internal abstract class MethodSig : Signature {

        private bool m_hasThis;
        private bool m_explicitThis;
        private CallingConvention m_callConv;
        private uint m_paramCount;
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

        public CallingConvention CallCon {
            get { return m_callConv; }
            set { m_callConv = value; }
        }

        public uint ParamCount {
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
    }
}
