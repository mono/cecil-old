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

    internal sealed class FunctionPointerType : TypeReference, IFunctionPointerType {

        private bool m_hasThis;
        private bool m_explicitThis;
        private MethodCallingConvention m_callingConv;

        private ParameterDefinitionCollection m_parameters;
        private IMethodReturnType m_retType;

        public bool HasThis {
            get { return m_hasThis; }
            set { m_hasThis = value; }
        }

        public bool ExplicitThis {
            get { return m_explicitThis; }
            set { m_explicitThis = value; }
        }

        public MethodCallingConvention CallingConvention {
            get { return m_callingConv; }
            set { m_callingConv = value; }
        }

        public IParameterDefinitionCollection Parameters {
            get { return m_parameters; }
        }

        public IMethodReturnType ReturnType {
            get { return m_retType; }
            set { m_retType = value; }
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
            get { return string.Concat ("function", Utilities.ParametersSignature(m_parameters)); }
        }

        public FunctionPointerType (bool hasThis, bool explicitThis, MethodCallingConvention callConv,
                                ParameterDefinitionCollection parameters, MethodReturnType retType) : base (string.Empty, string.Empty)
        {
            m_hasThis = hasThis;
            m_explicitThis = explicitThis;
            m_callingConv = callConv;
            m_parameters = parameters;
            m_retType = retType;
        }
    }
}
