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

    internal sealed class StructureWriter : IReflectionStructureVisitor {

        private ReflectionWriter m_reflectWriter;
        private ReflectionHelper m_reflectHelper;

        private ModuleDefinition m_module;

        public ReflectionWriter ReflectionWriter {
            get { return m_reflectWriter; }
        }

        public ReflectionHelper ReflectionHelper {
            get { return m_reflectHelper; }
        }

        public StructureWriter (ModuleDefinition module)
        {
            m_module = module;
            m_reflectWriter = new ReflectionWriter ();
            m_reflectHelper = new ReflectionHelper (module);
        }

        public void Visit (IAssemblyDefinition asm)
        {
            // TODO
        }

        public void Visit (IAssemblyNameDefinition name)
        {
            // TODO
        }

        public void Visit (IAssemblyNameReferenceCollection names)
        {
            // TODO
        }

        public void Visit (IAssemblyNameReference name)
        {
            // TODO
        }

        public void Visit (IResourceCollection resources)
        {
            // TODO
        }

        public void Visit (IEmbeddedResource res)
        {
            // TODO
        }

        public void Visit (ILinkedResource res)
        {
            // TODO
        }

        public void Visit (IAssemblyLinkedResource res)
        {
            // TODO
        }

        public void Visit (IModuleDefinition module)
        {
            // TODO
        }

        public void Visit (IModuleDefinitionCollection modules)
        {
            // TODO
        }

        public void Visit (IModuleReference module)
        {
            // TODO
        }

        public void Visit (IModuleReferenceCollection modules)
        {
            // TODO
        }
    }
}
