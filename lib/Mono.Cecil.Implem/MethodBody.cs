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

    internal sealed class MethodBody : IMethodBody {

        private int m_maxStack;
        private RVA m_rva;

        private InstructionCollection m_instructions;
        private ExceptionHandlerCollection m_exceptions;
        private VariableDefinitionCollection m_variables;

        public int MaxStack {
            get { return m_maxStack; }
            set { m_maxStack = value; }
        }

        public RVA RVA {
            get { return m_rva; }
            set { m_rva = value; }
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

        public MethodBody ()
        {
        }

        public MethodBody (RVA rva)
        {
            m_rva = rva;
        }

        public void Accept (ICodeVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
