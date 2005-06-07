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

namespace Mono.Cecil.Implem {

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    internal sealed class ExceptionHandler : IExceptionHandler {

        private int m_tryOffset;
        private int m_tryLength;
        private int m_handlerOffset;
        private int m_handlerLength;
        private int m_filterOffset;

        private ITypeReference m_catchType;
        private ExceptionHandlerType m_type;

        public int TryOffset {
            get { return m_tryOffset; }
            set { m_tryOffset = value; }
        }

        public int TryLength {
            get { return m_tryLength; }
            set { m_tryLength = value; }
        }

        public int HandlerOffset {
            get { return m_handlerOffset; }
            set { m_handlerOffset = value; }
        }

        public int HandlerLength {
            get { return m_handlerLength; }
            set { m_handlerLength = value; }
        }

        public int FilterOffset {
            get { return m_filterOffset; }
            set { m_filterOffset = value; }
        }

        public ITypeReference CatchType {
            get { return m_catchType; }
            set { m_catchType = value; }
        }

        public ExceptionHandlerType Type {
            get { return m_type; }
            set { m_type = value; }
        }

        public ExceptionHandler (ExceptionHandlerType type)
        {
            m_type = type;
        }

        public void Accept (ICodeVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
