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

namespace Mono.Cecil.Cil {

    public struct Instruction {

        private uint m_offset;
        private OpCode m_opCode;
        private object m_operand;

        public uint Offset {
            get { return m_offset; }
        }

        public OpCode OpCode {
            get { return m_opCode; }
        }

        public object Operand {
            get { return m_operand; }
        }

        internal Instruction (uint offset, OpCode opCode, object operand)
        {
            m_offset = offset;
            m_opCode = opCode;
            m_operand = operand;
        }
    }
}
