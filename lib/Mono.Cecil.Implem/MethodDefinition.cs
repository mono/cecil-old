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

        private MethodBody m_body;
        private MethodDefSig m_signature;
        private RVA m_rva;
        private OverrideCollection m_overrides;

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

        public MethodDefSig Signature {
            get { return m_signature; }
            set { m_signature = value; }
        }

        public RVA RVA {
            get { return m_rva; }
            set { m_rva = value; }
        }

        public IMethodBody Body {
            get {
                if (m_body == null && m_rva != RVA.Zero)
                    m_body = new MethodBody (this);
                return m_body;
            }
        }

        public IOverrideCollection Overrides {
            get {
                if (m_overrides == null)
                    m_overrides = new OverrideCollection (this);
                return m_overrides;
            }
        }

        public MethodDefinition (string name, TypeDefinition decType, MethodDefSig signature,
                                 RVA rva, MethodAttributes attrs, MethodImplAttributes implAttrs)
        {
            this.Name = name;
            m_signature = signature;
            m_rva = rva;
            m_attributes = attrs;
            m_implAttrs = implAttrs;
            SetDeclaringType (decType);
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
            m_overrides.Accept (visitor);
        }
    }
}
