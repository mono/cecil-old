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
    using Mono.Cecil.Binary;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Signatures;

    internal sealed class MethodDefinition : MemberDefinition, IMethodDefinition {

        private MethodAttributes m_attributes;
        private MethodImplAttributes m_implAttrs;
        private MethodSemanticsAttributes m_semAttrs;
        private ParameterDefinitionCollection m_parameters;
        private SecurityDeclarationCollection m_secDecls;
        private MethodReturnType m_returnType;

        private bool m_hasThis;
        private bool m_explicitThis;
        private MethodCallingConvention m_callConv;

        private MethodBody m_body;
        private RVA m_rva;
        private OverrideCollection m_overrides;
        private PInvokeInfo m_pinvoke;

        public MethodAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public MethodImplAttributes ImplAttributes {
            get { return m_implAttrs; }
            set { m_implAttrs = value; }
        }

        public MethodSemanticsAttributes SemanticsAttributes {
            get { return m_semAttrs; }
            set { m_semAttrs = value; }
        }

        public bool HasThis {
            get { return m_hasThis; }
            set { m_hasThis = value; }
        }

        public bool ExplicitThis {
            get { return m_explicitThis; }
            set { m_explicitThis = value; }
        }

        public MethodCallingConvention CallingConvention {
            get { return m_callConv; }
            set { m_callConv = value; }
        }

        public IParameterDefinitionCollection Parameters {
            get { return m_parameters; }
        }

        public ISecurityDeclarationCollection SecurityDeclarations {
            get {
                if (m_secDecls == null)
                    m_secDecls = new SecurityDeclarationCollection (this);
                return m_secDecls;
            }
        }

        public IMethodReturnType ReturnType {
            get { return m_returnType; }
            set { m_returnType = value as MethodReturnType; }
        }

        public RVA RVA {
            get { return m_rva; }
            set { m_rva = value; }
        }

        public IMethodBody Body {
            get {
                if (m_body == null && m_rva != RVA.Zero) {
                    m_body = new MethodBody (this);
                    ((TypeDefinition)this.DeclaringType).Module.Loader.CodeReader.Visit (m_body);
                }
                return m_body;
            }
        }

        public IPInvokeInfo PInvokeInfo {
            get { return m_pinvoke; }
            set { m_pinvoke = value as PInvokeInfo; }
        }

        public IOverrideCollection Overrides {
            get {
                if (m_overrides == null)
                    m_overrides = new OverrideCollection (this);
                return m_overrides;
            }
        }

        public MethodDefinition (string name, TypeDefinition decType, RVA rva,
                                 MethodAttributes attrs, MethodImplAttributes implAttrs,
                                 bool hasThis, bool explicitThis, MethodCallingConvention callConv)
        {
            this.Name = name;
            m_rva = rva;
            m_attributes = attrs;
            m_implAttrs = implAttrs;
            SetDeclaringType (decType);
            m_parameters = new ParameterDefinitionCollection (this);
            m_hasThis = hasThis;
            m_explicitThis = explicitThis;
            m_callConv = callConv;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
            m_parameters.Accept (visitor);
            m_overrides.Accept (visitor);
        }

        public override string ToString ()
        {
            return Utilities.FullMethodSignature (this);
        }
    }
}
