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
    using Mono.Cecil.Metadata;

    internal sealed class ModuleDefinition : IModuleDefinition {

        private string m_name;
        private Guid m_mvid;
        private bool m_main;

        private AssemblyNameReferenceCollection m_asmRefs;
        private ModuleReferenceCollection m_modRefs;
        private ResourceCollection m_res;
        private TypeDefinitionCollection m_types;

        private AssemblyDefinition m_asm;

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public Guid Mvid {
            get { return m_mvid; }
            set { m_mvid = value; }
        }

        public bool Main {
            get { return m_main; }
            set { m_main = value; }
        }

        public IAssemblyNameReferenceCollection AssemblyReferences {
            get { return m_asmRefs; }
        }

        public IModuleReferenceCollection ModuleReferences {
            get { return m_modRefs; }
        }

        public IResourceCollection Resources {
            get { return m_res; }
        }

        public ITypeDefinitionCollection Types {
            get { return m_types; }
        }

        public AssemblyDefinition Assembly {
            get { return m_asm; }
        }

        public ModuleDefinition (string name, AssemblyDefinition asm) : this (name, true, asm)
        {}

        public ModuleDefinition (string name, bool main, AssemblyDefinition asm)
        {
            if (asm == null)
                throw new ArgumentException ("asm");
            if (name == null || name.Length == 0)
                throw new ArgumentException ("name");

            m_asm = asm;
            m_name = name;
            m_main = main;
            m_mvid = new Guid ();
            m_modRefs = new ModuleReferenceCollection (this);
            m_asmRefs = new AssemblyNameReferenceCollection (this);
            m_res = new ResourceCollection (this);
            m_types = new TypeDefinitionCollection (this);
        }

        public void DefineModuleReference (string module)
        {
            m_modRefs [module] = new ModuleReference (module);
        }

        public void DefineEmbeddedResource (string name, ManifestResourceAttributes attributes, byte [] data)
        {
            m_res [name] = new EmbeddedResource (name, attributes, data);
        }

        public void DefineLinkedResource (string name, ManifestResourceAttributes attributes, string file)
        {
            m_res [name] = new LinkedResource (name, attributes, file);
        }

        public void Accept (IReflectionStructureVisitor visitor)
        {
            visitor.Visit (this);

            m_asmRefs.Accept (visitor);
            m_modRefs.Accept (visitor);
            m_res.Accept (visitor);
        }
    }
}

