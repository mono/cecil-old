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

    public struct OpCode {

        private string m_name;
        private byte m_op1;
        private byte m_op2;
        private byte m_size;

        private FlowControl m_flowControl;
        private OpCodeType m_opCodeType;
        private OperandType m_operandType;
        private StackBehaviour m_stackBehaviourPop;
        private StackBehaviour m_stackBehaviourPush;

        public string Name {
            get { return m_name; }
        }

        public short Value {
            get { return m_size == 1 ? m_op2 : (short) ((m_op1 << 8) | m_op2); }
        }

        public FlowControl FlowControl {
            get { return m_flowControl; }
        }

        public OpCodeType OpCodeType {
            get { return m_opCodeType; }
        }

        public OperandType OperandType {
            get { return m_operandType; }
        }

        public StackBehaviour StackBehaviourPop {
            get { return m_stackBehaviourPop; }
        }

        public StackBehaviour StackBehaviourPush {
            get { return m_stackBehaviourPush; }
        }

        internal OpCode (string name, byte op1, byte op2, int size, FlowControl flowControl,
            OpCodeType opCodeType, OperandType operandType,
            StackBehaviour pop, StackBehaviour push)
        {
            m_name = name;
            m_op1 = op1;
            m_op2 = op2;
            m_size = (byte)size;
            m_flowControl = flowControl;
            m_opCodeType = opCodeType;
            m_operandType = operandType;
            m_stackBehaviourPop = pop;
            m_stackBehaviourPush = push;
        }

        public override int GetHashCode ()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is OpCode))
                return false;
            OpCode v = (OpCode)obj;
            return v.m_op1 == m_op1 && v.m_op2 == m_op2;
        }
    }
}
