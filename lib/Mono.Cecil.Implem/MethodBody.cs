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

namespace Mono.Cecil.Implem {

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Signatures;

    internal sealed class MethodBody : IMethodBody {

        private MethodDefinition m_method;
        private int m_maxStack;
        private int m_codeSize;

        private InstructionCollection m_instructions;
        private ExceptionHandlerCollection m_exceptions;
        private VariableDefinitionCollection m_variables;

        public IMethodDefinition Method {
            get { return m_method; }
        }

        public int MaxStack {
            get { return m_maxStack; }
            set { m_maxStack = value; }
        }

        public int CodeSize {
            get { return m_codeSize; }
            set { m_codeSize = value; }
        }

        public IInstructionCollection Instructions {
            get {
                if (m_instructions == null)
                    m_instructions = new InstructionCollection (this);
                return m_instructions;
            }
        }

        public IExceptionHandlerCollection ExceptionHandlers {
            get {
                if (m_exceptions == null)
                    m_exceptions = new ExceptionHandlerCollection (this);
                return m_exceptions;
            }
        }

        public IVariableDefinitionCollection Variables {
            get {
                if (m_variables == null)
                    m_variables = new VariableDefinitionCollection (this);
                return m_variables;
            }
        }

        public MethodBody (MethodDefinition meth)
        {
            m_method = meth;
        }

        public void Accept (ICodeVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
