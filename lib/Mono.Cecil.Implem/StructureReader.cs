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
    using System.Collections;
    using System.IO;

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Metadata;

    internal sealed class StructureReader : IReflectionStructureVisitor {

        private ImageReader m_ir;
        private Image m_img;
        private AssemblyDefinition m_asmDef;
        private ModuleDefinition m_module;

        public ImageReader ImageReader {
            get { return m_ir; }
        }

        public Image Image {
            get { return m_img; }
        }

        public StructureReader (ImageReader ir)
        {
            m_ir = ir;
            m_img = ir.Image;
        }

        public void Visit (IAssemblyDefinition asm)
        {
            m_asmDef = asm as AssemblyDefinition;
            AssemblyTable atable = m_img.MetadataRoot.Streams.TablesHeap [typeof(AssemblyTable)] as AssemblyTable;
            if (atable == null || atable.Rows == null || atable.Rows [0] == null)
                throw new ReflectionException ("Can not create an AssemblyDefinition");
        }

        public void Visit (IAssemblyNameDefinition name)
        {
            AssemblyTable atable = m_img.MetadataRoot.Streams.TablesHeap [typeof(AssemblyTable)] as AssemblyTable;
            AssemblyRow arow = atable [0];
            name.Name = m_img.MetadataRoot.Streams.StringsHeap [arow.Name];
            if (arow.PublicKey != 0)
                name.PublicKey = m_img.MetadataRoot.Streams.BlobHeap.Read (arow.PublicKey);

            name.Culture = m_img.MetadataRoot.Streams.StringsHeap [arow.Culture];
            name.Version = new Version (
                arow.MajorVersion, arow.MinorVersion,
                arow.BuildNumber, arow.RevisionNumber);
            name.HashAlgorithm = arow.HashAlgId;
        }

        public void Visit (IAssemblyNameReferenceCollection names)
        {
            AssemblyRefTable aref = m_img.MetadataRoot.Streams.TablesHeap [typeof(AssemblyRefTable)] as AssemblyRefTable;
            if (aref != null && aref.Rows.Count > 0) {
                foreach (AssemblyRefRow arefrow in aref.Rows) {
                    AssemblyNameReference aname = new AssemblyNameReference (
                        m_img.MetadataRoot.Streams.StringsHeap [arefrow.Name],
                        m_img.MetadataRoot.Streams.StringsHeap [arefrow.Culture],
                        new Version (arefrow.MajorVersion, arefrow.MinorVersion,
                                     arefrow.BuildNumber, arefrow.RevisionNumber));
                    aname.PublicKeyToken = m_img.MetadataRoot.Streams.BlobHeap.Read (arefrow.PublicKeyOrToken);
                    names [aname.Name] = aname;
                }
            }
        }

        public void Visit (IAssemblyNameReference name)
        {
        }

        public void Visit (IResourceCollection resources)
        {
            ManifestResourceTable mrt = m_img.MetadataRoot.Streams.TablesHeap [typeof(ManifestResourceTable)] as ManifestResourceTable;
            if (mrt != null) {
                foreach (ManifestResourceRow mrr in mrt.Rows) {
                    EmbeddedResource er = new EmbeddedResource (
                        m_img.MetadataRoot.Streams.StringsHeap [mrr.Name], mrr.Flags);

                    resources [er.Name] = er;
                }
            }

            FileTable ft = m_img.MetadataRoot.Streams.TablesHeap [typeof(FileTable)] as FileTable;
            if (ft != null && ft.Rows.Count > 0) {
                foreach (FileRow fr in ft.Rows) {
                    if ((fr.Flags & Mono.Cecil.FileAttributes.ContainsNoMetaData) > 0) {
                        LinkedResource lr = new LinkedResource (
                            m_img.MetadataRoot.Streams.StringsHeap [fr.Name],
                            ManifestResourceAttributes.Public,
                            new FileInfo (m_img.MetadataRoot.Streams.StringsHeap [fr.Name]).FullName);

                        lr.Hash = m_img.MetadataRoot.Streams.BlobHeap.Read (fr.HashValue);

                        resources [lr.Name] = lr;
                    }
                }
            }
        }

        public void Visit (IEmbeddedResource res)
        {
        }

        public void Visit (ILinkedResource res)
        {
        }

        public void Visit (IModuleDefinition module)
        {
        }

        public void Visit (IModuleDefinitionCollection modules)
        {
            ModuleTable mt = m_img.MetadataRoot.Streams.TablesHeap [typeof(ModuleTable)] as ModuleTable;
            if (mt == null || mt.Rows.Count != 1)
                throw new ReflectionException ("Can not read main module");

            ModuleRow mr = mt.Rows [0] as ModuleRow;
            string name = m_img.MetadataRoot.Streams.StringsHeap [mr.Name];
            ModuleDefinition main = new ModuleDefinition (name, m_asmDef, m_ir, true);
            main.Mvid = m_img.MetadataRoot.Streams.GuidHeap [mr.Mvid];
            modules [name] = m_module = main;

            FileTable ftable = m_img.MetadataRoot.Streams.TablesHeap [typeof(FileTable)] as FileTable;
            if (ftable != null && ftable.Rows.Count > 0) {
                foreach (FileRow frow in ftable.Rows) {
                    if ((frow.Flags & Mono.Cecil.FileAttributes.ContainsMetaData) != 0) {
                        name = m_img.MetadataRoot.Streams.StringsHeap [frow.Name];
                        FileInfo location = new FileInfo (name);
                        if (!File.Exists (location.FullName))
                            throw new FileNotFoundException ("Module not found : " + name);

                        try {
                            ImageReader module = new ImageReader (new FileInfo (name).FullName);
                            mt = module.Image.MetadataRoot.Streams.TablesHeap [typeof(ModuleTable)] as ModuleTable;
                            if (mt == null || mt.Rows.Count != 1)
                                throw new ReflectionException ("Can not read module : " + name);

                            mr = mt.Rows [0] as ModuleRow;
                            ModuleDefinition modext = new ModuleDefinition (name, m_asmDef, module, false);
                            modext.Mvid = module.Image.MetadataRoot.Streams.GuidHeap [mr.Mvid];

                            modules [name] = modext;
                        } catch (ReflectionException) {
                            throw;
                        } catch (Exception e) {
                            throw new ReflectionException ("Can not read module : " + name, e);
                        }
                    }
                }
            }
        }

        public void Visit (IModuleReference module)
        {
        }

        public void Visit (IModuleReferenceCollection modules)
        {
            ModuleDefinition mod = modules.Container as ModuleDefinition;
            ModuleRefTable mrt = mod.Reader.Image.MetadataRoot.Streams.TablesHeap [typeof(ModuleRefTable)] as ModuleRefTable;
            if (mrt != null && mrt.Rows.Count > 0) {
                foreach (ModuleRefRow mrr in mrt.Rows) {
                    string name = m_img.MetadataRoot.Streams.StringsHeap [mrr.Name];
                    modules [name] = new ModuleReference (name);
                }
            }
        }
    }
}

