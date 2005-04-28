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
    using Mono.Cecil.Binary;
    using Mono.Cecil.Metadata;

    internal sealed class ModuleDefinition : IModuleDefinition {

        private string m_name;
        private Guid m_mvid;
        private bool m_main;

        private AssemblyNameReferenceCollection m_asmRefs;
        private ModuleReferenceCollection m_modRefs;
        private ResourceCollection m_res;
        private TypeDefinitionCollection m_types;
        private TypeReferenceCollection m_refs;
        private ExternTypeCollection m_externs;
        private CustomAttributeCollection m_customAttrs;

        private AssemblyDefinition m_asm;
        private ImageReader m_reader;
        private LazyLoader m_loader;
        private StructureWriter m_writer;

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

        public ITypeReferenceCollection TypeReferences {
            get { return m_refs; }
        }

        public IExternTypeCollection ExternTypes {
            get {
                if (m_externs == null)
                    m_externs = new ExternTypeCollection (this, m_loader);
                return m_externs;
            }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null)
                    m_customAttrs = new CustomAttributeCollection (this, m_loader);
                return m_customAttrs;
            }
        }

        public AssemblyDefinition Assembly {
            get { return m_asm; }
        }

        public ImageReader Reader {
            get { return m_reader; }
        }

        public LazyLoader Loader {
            get { return m_loader; }
        }

        public StructureWriter Writer {
            get { return m_writer; }
        }

        public ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader) : this (name, asm, reader, false)
        {
        }

        public ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader, bool main)
        {
            if (asm == null)
                throw new ArgumentException ("asm");
            if (name == null || name.Length == 0)
                throw new ArgumentException ("name");

            m_asm = asm;
            m_name = name;
            m_main = main;
            m_reader = reader;
            m_loader = new LazyLoader (this, asm.LoadingType);
            m_writer = new StructureWriter (this);
            m_mvid = new Guid ();
            m_modRefs = new ModuleReferenceCollection (this);
            m_asmRefs = new AssemblyNameReferenceCollection (this);
            m_res = new ResourceCollection (this);
            m_types = new TypeDefinitionCollection (this, m_loader);
            m_refs = new TypeReferenceCollection (this);
        }

        public void DefineModuleReference (string module)
        {
            m_modRefs.Add (new ModuleReference (module));
        }

        public IEmbeddedResource DefineEmbeddedResource (string name, ManifestResourceAttributes attributes, byte [] data)
        {
            EmbeddedResource res = new EmbeddedResource (name, attributes, this, data);
            m_res [name] = res;
            return res;
        }

        public ILinkedResource DefineLinkedResource (string name, ManifestResourceAttributes attributes, string file)
        {
            LinkedResource res = new LinkedResource (name, attributes, this, file);
            m_res [name] = res;
            return res;
        }

        public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
        {
            CustomAttribute ca = new CustomAttribute(ctor);
            m_customAttrs.Add (ca);
            return ca;
        }

        public ICustomAttribute DefineCustomAttribute (ConstructorInfo ctor)
        {
            CustomAttribute ca = new CustomAttribute (m_writer.ReflectionHelper.RegisterConstructor(ctor));
            m_customAttrs.Add (ca);
            return ca;
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

