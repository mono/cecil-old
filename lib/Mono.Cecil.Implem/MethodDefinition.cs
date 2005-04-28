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

    internal sealed class MethodDefinition : MethodReference, IMethodDefinition {

        private MethodAttributes m_attributes;
        private MethodImplAttributes m_implAttrs;
        private MethodSemanticsAttributes m_semAttrs;
        private SecurityDeclarationCollection m_secDecls;
        private CustomAttributeCollection m_customAttrs;

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

        public ISecurityDeclarationCollection SecurityDeclarations {
            get {
                if (m_secDecls == null)
                    m_secDecls = new SecurityDeclarationCollection (this, (this.DeclaringType as TypeDefinition).Module.Loader);
                return m_secDecls;
            }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null)
                    m_customAttrs = new CustomAttributeCollection (this, (this.DeclaringType as TypeDefinition).Module.Loader);
                return m_customAttrs;
            }
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
            get {
                if (m_pinvoke == null && (m_attributes & MethodAttributes.PInvokeImpl) != 0) {
                    m_pinvoke = new PInvokeInfo (this);
                    ((TypeDefinition)this.DeclaringType).Module.Loader.ReflectionReader.Visit (m_pinvoke);
                }
                return m_pinvoke;
            }
            set { m_pinvoke = value as PInvokeInfo; }
        }

        public IOverrideCollection Overrides {
            get {
                if (m_overrides == null)
                    m_overrides = new OverrideCollection (this, (this.DeclaringType as TypeDefinition).Module.Loader);
                return m_overrides;
            }
        }

        public MethodDefinition (string name, TypeDefinition decType, RVA rva,
                                 MethodAttributes attrs, MethodImplAttributes implAttrs,
                                 bool hasThis, bool explicitThis, MethodCallingConvention callConv) : base (name, decType, hasThis, explicitThis, callConv)
        {
            m_rva = rva;
            m_attributes = attrs;
            m_implAttrs = implAttrs;
        }

        public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
        {
            CustomAttribute ca = new CustomAttribute(ctor);
            m_customAttrs.Add (ca);
            return ca;
        }

        public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
        {
            //TODO: implement this
            return null;
        }

        public override void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
            m_secDecls.Accept (visitor);
            m_overrides.Accept (visitor);
            this.Parameters.Accept (visitor);
        }
    }
}
