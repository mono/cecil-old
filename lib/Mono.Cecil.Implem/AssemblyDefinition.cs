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

    internal class AssemblyDefinition : IAssemblyDefinition {

        private AssemblyNameDefinition m_asmName;
        private ModuleDefinitionCollection m_modules;
        private SecurityDeclarationCollection m_secDecl;
        private CustomAttributeCollection m_customAttrs;

        private StructureReader m_sr;
        private LoadingType m_loadingType;

        public IAssemblyNameDefinition Name {
            get { return m_asmName; }
        }

        public IModuleDefinitionCollection Modules {
            get { return m_modules; }
        }

        public ISecurityDeclarationCollection SecurityDeclarations {
            get {
                if (m_secDecl == null)
                    m_secDecl = new SecurityDeclarationCollection (this, (this.MainModule as ModuleDefinition).Loader);
                return m_secDecl;
            }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null)
                    m_customAttrs = new CustomAttributeCollection (this, (this.MainModule as ModuleDefinition).Loader);
                return m_customAttrs;
            }
        }

        public IModuleDefinition MainModule {
            get {
                foreach (IModuleDefinition mod in m_modules)
                    if (mod.Main)
                        return mod;
                throw new ReflectionException ("No main module defined");
            }
        }

        public StructureReader Reader {
            get { return m_sr; }
        }

        public LoadingType LoadingType {
            get { return m_loadingType; }
            set { m_loadingType = value; }
        }

        public AssemblyDefinition (AssemblyNameDefinition name, StructureReader sr, LoadingType lt)
        {
            if (sr == null)
                throw new ArgumentException ("sr");
            if (name == null)
                throw new ArgumentException ("name");

            m_asmName = name;
            m_sr = sr;
            m_loadingType = lt;
            m_modules = new ModuleDefinitionCollection (this);
        }

        public void Accept (IReflectionStructureVisitor visitor)
        {
            visitor.Visit (this);

            m_asmName.Accept (visitor);
            m_modules.Accept (visitor);
        }
    }
}

