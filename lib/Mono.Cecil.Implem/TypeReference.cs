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
    using System.Reflection;

    using Mono.Cecil;

    internal class TypeReference : ITypeReference {

        private string m_name;
        private string m_namespace;
        private ITypeReference m_decType;
        private IMetadataScope m_scope;

        private CustomAttributeCollection m_customAttrs;

        protected ModuleDefinition m_module;

        public virtual string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public virtual string Namespace {
            get { return m_namespace; }
            set { m_namespace = value; }
        }

        public virtual ITypeReference DeclaringType {
            get { return m_decType; }
            set { m_decType = value; }
        }

        public ModuleDefinition Module {
            get { return m_module; }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null && m_module != null)
                    m_customAttrs = new CustomAttributeCollection (this, m_module.Loader);
                else if (m_customAttrs == null && m_module == null)
                    m_customAttrs = new CustomAttributeCollection (this);
                return m_customAttrs;
            }
        }

        public virtual IMetadataScope Scope {
            get {
                if (m_decType != null)
                    return m_decType.Scope;
                return m_scope;
            }
        }

        public virtual string FullName {
            get {
                if (m_decType != null)
                    return string.Concat (m_decType.FullName, "/", m_name);

                if (m_namespace == null || m_namespace.Length == 0)
                    return m_name;

                return string.Concat (m_namespace, ".", m_name);
            }
        }

        public TypeReference (string name, string ns)
        {
            m_name = name;
            m_namespace = ns;
        }

        public TypeReference (string name, string ns, ModuleDefinition module, IMetadataScope scope) : this (name, ns)
        {
            m_module = module;
            m_scope = scope;
        }

        public virtual ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
        {
            CustomAttribute ca = new CustomAttribute(ctor);
            m_customAttrs.Add (ca);
            return ca;
        }

        public virtual ICustomAttribute DefineCustomAttribute (ConstructorInfo ctor)
        {
            //TODO: implement this
            return null;
        }

        public virtual void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }

        public override string ToString ()
        {
            return this.FullName;
        }
    }
}
