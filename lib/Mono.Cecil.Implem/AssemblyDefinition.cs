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

    using System;
    using System.Reflection;

    using Mono.Cecil;

    internal class AssemblyDefinition : IAssemblyDefinition {

        private AssemblyNameDefinition m_asmName;
        private ModuleDefinitionCollection m_modules;
        private SecurityDeclarationCollection m_secDecls;
        private CustomAttributeCollection m_customAttrs;
        private MethodDefinition m_ep;

        private ModuleDefinition m_mainModule;
        private StructureReader m_reader;
        private LoadingType m_loadingType;

        public IAssemblyNameDefinition Name {
            get { return m_asmName; }
        }

        public IModuleDefinitionCollection Modules {
            get { return m_modules; }
        }

        public ISecurityDeclarationCollection SecurityDeclarations {
            get {
                if (m_secDecls == null)
                    m_secDecls = new SecurityDeclarationCollection (this, (this.MainModule as ModuleDefinition).Controller);
                return m_secDecls;
            }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null)
                    m_customAttrs = new CustomAttributeCollection (this, (this.MainModule as ModuleDefinition).Controller);
                return m_customAttrs;
            }
        }

        public IMethodDefinition EntryPoint {
            get { return m_ep; }
            set { m_ep = value as MethodDefinition; }
        }

        public IModuleDefinition MainModule {
            get {
                if (m_mainModule == null)
                    foreach (ModuleDefinition module in m_modules)
                        if (module.Main)
                            m_mainModule = module;

                return m_mainModule;
            }
        }

        public StructureReader Reader {
            get { return m_reader; }
        }

        public LoadingType LoadingType {
            get { return m_loadingType; }
            set { m_loadingType = value; }
        }

        public AssemblyDefinition (AssemblyNameDefinition name)
        {
            if (name == null)
                throw new ArgumentNullException ("name");

            m_asmName = name;
            m_modules = new ModuleDefinitionCollection (this);
        }

        public AssemblyDefinition (AssemblyNameDefinition name, StructureReader reader, LoadingType lt) : this (name)
        {
            m_reader = reader;
            m_loadingType = lt;
        }

        public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
        {
            CustomAttribute ca = new CustomAttribute(ctor);
            m_customAttrs.Add (ca);
            return ca;
        }

        public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
        {
            return DefineCustomAttribute (
                (this.MainModule as ModuleDefinition).Controller.Helper.RegisterConstructor(ctor));
        }

        public ICustomAttribute DefineCustomAttribute (IMethodReference ctor, byte [] data)
        {
            CustomAttribute ca = (this.MainModule as ModuleDefinition).Controller.Reader.GetCustomAttribute (ctor, data);
            m_customAttrs.Add (ca);
            return ca;
        }

        public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor, byte [] data)
        {
            return DefineCustomAttribute (
                (this.MainModule as ModuleDefinition).Controller.Helper.RegisterConstructor(ctor), data);
        }

        public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action)
        {
            SecurityDeclaration dec = new SecurityDeclaration (action);
            m_secDecls.Add (dec);
            return dec;
        }

        public ISecurityDeclaration DefineSecurityDeclaration (SecurityAction action, byte [] declaration)
        {
            SecurityDeclaration dec =
                (this.MainModule as ModuleDefinition).Controller.Reader.BuildSecurityDeclaration (action, declaration);
            m_secDecls.Add (dec);
            return dec;
        }

        public void Accept (IReflectionStructureVisitor visitor)
        {
            visitor.Visit (this);

            m_asmName.Accept (visitor);
            m_modules.Accept (visitor);
        }
    }
}

