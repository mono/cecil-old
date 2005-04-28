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
                    aname.Hash = m_img.MetadataRoot.Streams.BlobHeap.Read (arefrow.HashValue);
                    (names as AssemblyNameReferenceCollection).Add (aname);
                }
            }
        }

        public void Visit (IAssemblyNameReference name)
        {
        }

        public void Visit (IResourceCollection resources)
        {
            if (!m_img.MetadataRoot.Streams.TablesHeap.HasTable (typeof (ManifestResourceTable)))
                return;

            ModuleDefinition module = resources.Container as ModuleDefinition;

            ManifestResourceTable mrTable = m_img.MetadataRoot.Streams.TablesHeap [typeof(ManifestResourceTable)] as ManifestResourceTable;
            FileTable fTable = m_img.MetadataRoot.Streams.TablesHeap [typeof(FileTable)] as FileTable;

            BinaryReader br = m_ir.GetReader ();

            for (int i = 0; i < mrTable.Rows.Count; i++) {
                ManifestResourceRow mrRow = mrTable [i];
                if (mrRow.Implementation.RID == 0) {
                    EmbeddedResource eres = new EmbeddedResource (
                        m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name], mrRow.Flags, module);

                    br.BaseStream.Position = m_img.ResolveVirtualAddress (
                        m_img.CLIHeader.Resources.VirtualAddress);
                    br.BaseStream.Position += mrRow.Offset;

                    eres.Data = br.ReadBytes (br.ReadInt32 ());

                    (resources as ResourceCollection) [eres.Name] = eres;
                    continue;
                }

                switch (mrRow.Implementation.TokenType) {
                case TokenType.File :
                    FileRow fRow = fTable [(int) mrRow.Implementation.RID - 1];
                    LinkedResource lres = new LinkedResource (m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name], mrRow.Flags,
                                              module, m_img.MetadataRoot.Streams.StringsHeap [fRow.Name]);
                    lres.Hash = m_img.MetadataRoot.Streams.BlobHeap.Read (fRow.HashValue);
                    (resources as ResourceCollection) [lres.Name] = lres;
                    break;
                case TokenType.AssemblyRef :
                    // not sure how to implement this
                    break;
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
            ModuleDefinitionCollection mods = modules as ModuleDefinitionCollection;
            main.Mvid = m_img.MetadataRoot.Streams.GuidHeap [mr.Mvid];
            mods.Add (main);
            m_module = main;

            FileTable ftable = m_img.MetadataRoot.Streams.TablesHeap [typeof(FileTable)] as FileTable;
            if (ftable != null && ftable.Rows.Count > 0) {
                foreach (FileRow frow in ftable.Rows) {
                    if (frow.Flags == Mono.Cecil.FileAttributes.ContainsMetaData) {
                        name = m_img.MetadataRoot.Streams.StringsHeap [frow.Name];
                        FileInfo location = new FileInfo (Path.Combine(m_img.FileInformation.DirectoryName, name));
                        if (!File.Exists (location.FullName))
                            throw new FileNotFoundException ("Module not found : " + name);

                        try {
                            ImageReader module = new ImageReader (location.FullName);
                            mt = module.Image.MetadataRoot.Streams.TablesHeap [typeof(ModuleTable)] as ModuleTable;
                            if (mt == null || mt.Rows.Count != 1)
                                throw new ReflectionException ("Can not read module : " + name);

                            mr = mt.Rows [0] as ModuleRow;
                            ModuleDefinition modext = new ModuleDefinition (name, m_asmDef, module, false);
                            modext.Mvid = module.Image.MetadataRoot.Streams.GuidHeap [mr.Mvid];

                            mods.Add (modext);
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
            if (mrt != null && mrt.Rows.Count > 0)
                foreach (ModuleRefRow mrr in mrt.Rows)
                    (modules as ModuleReferenceCollection).Add (new ModuleReference (m_img.MetadataRoot.Streams.StringsHeap [mrr.Name]));
        }
    }
}

