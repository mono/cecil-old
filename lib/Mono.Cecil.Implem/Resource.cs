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
    using Mono.Cecil.Metadata;

    internal abstract class Resource :  IResource {

        private ModuleDefinition m_module;
        private string m_name;
        private ManifestResourceAttributes m_attributes;

        public ModuleDefinition Module {
            get { return m_module; }
        }

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public ManifestResourceAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        protected Resource(string name, ManifestResourceAttributes attributes, ModuleDefinition owner) : this (owner)
        {
            m_name = name;
            m_attributes = attributes;
        }

        private Resource (ModuleDefinition owner)
        {
            m_module = owner;
        }

        public abstract void Accept(IReflectionStructureVisitor visitor);
    }
}

