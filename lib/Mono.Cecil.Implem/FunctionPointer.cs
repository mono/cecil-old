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

    using System;

    using Mono.Cecil;
    using Mono.Cecil.Signatures;

    internal sealed class FunctionPointer : TypeReference, IFunctionPointer {

        private IMethodReference m_pointedFunc;

        public bool HasThis {
            get { return m_pointedFunc.HasThis; }
            set { m_pointedFunc.HasThis = value; }
        }

        public bool ExplicitThis {
            get { return m_pointedFunc.ExplicitThis; }
            set { m_pointedFunc.ExplicitThis = value; }
        }

        public MethodCallingConvention CallingConvention {
            get { return m_pointedFunc.CallingConvention; }
            set { m_pointedFunc.CallingConvention = value; }
        }

        public IParameterDefinitionCollection Parameters {
            get { return m_pointedFunc.Parameters; }
        }

        public IMethodReturnType ReturnType {
            get { return m_pointedFunc.ReturnType; }
            set { m_pointedFunc.ReturnType = value; }
        }

        public override string Name {
            get { return string.Empty; }
            set { throw new InvalidOperationException (); }
        }

        public override string Namespace {
            get { return string.Empty; }
            set { throw new InvalidOperationException (); }
        }

        public override string FullName {
            get { return Utilities.MethodSignature(m_pointedFunc); }
        }

        public FunctionPointer (IMethodReference pFunc) : base (string.Empty, string.Empty)
        {
            m_pointedFunc = pFunc;
        }
    }
}
