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
	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;

	internal sealed class StructureReader : BaseStructureVisitor {

		private ImageReader m_ir;
		private Image m_img;
		private AssemblyDefinition m_asmDef;
		private ModuleDefinition m_module;
		private TablesHeap m_tHeap;

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
			m_tHeap = m_img.MetadataRoot.Streams.TablesHeap;
		}

		public override void Visit (IAssemblyDefinition asm)
		{
			m_asmDef = asm as AssemblyDefinition;
			if (!m_tHeap.HasTable (typeof (AssemblyTable)))
				throw new ReflectionException ("No assembly manifest");

			MetadataRoot root = m_img.MetadataRoot;
			if (root.Header.MajorVersion == 1)
				if (root.Header.MinorVersion == 1)
					asm.Runtime = TargetRuntime.NET_1_1;
				else
					asm.Runtime = TargetRuntime.NET_1_0;
			else if (root.Header.MajorVersion == 2)
				asm.Runtime = TargetRuntime.NET_2_0;
		}

		public override void Visit (IAssemblyNameDefinition name)
		{
			AssemblyTable atable = m_tHeap [typeof(AssemblyTable)] as AssemblyTable;
			AssemblyRow arow = atable [0];
			name.Name = m_img.MetadataRoot.Streams.StringsHeap [arow.Name];
			name.Flags = arow.Flags;
			if (arow.PublicKey != 0)
				name.PublicKey = m_img.MetadataRoot.Streams.BlobHeap.Read (arow.PublicKey);

			name.Culture = m_img.MetadataRoot.Streams.StringsHeap [arow.Culture];
			name.Version = new Version (
				arow.MajorVersion, arow.MinorVersion,
				arow.BuildNumber, arow.RevisionNumber);
			name.HashAlgorithm = arow.HashAlgId;
		}

		public override void Visit (IAssemblyNameReferenceCollection names)
		{
			if (!m_tHeap.HasTable (typeof (AssemblyRefTable)))
				return;

			AssemblyRefTable arTable = m_tHeap [typeof(AssemblyRefTable)] as AssemblyRefTable;
			for (int i = 0; i < arTable.Rows.Count; i++) {
				AssemblyRefRow arRow = arTable [i];
				AssemblyNameReference aname = new AssemblyNameReference (
					m_img.MetadataRoot.Streams.StringsHeap [arRow.Name],
					m_img.MetadataRoot.Streams.StringsHeap [arRow.Culture],
					new Version (arRow.MajorVersion, arRow.MinorVersion,
								 arRow.BuildNumber, arRow.RevisionNumber));
				aname.PublicKeyToken = m_img.MetadataRoot.Streams.BlobHeap.Read (arRow.PublicKeyOrToken);
				aname.Hash = m_img.MetadataRoot.Streams.BlobHeap.Read (arRow.HashValue);
				(names as AssemblyNameReferenceCollection).Add (aname);
			}
		}

		public override void Visit (IResourceCollection resources)
		{
			if (!m_tHeap.HasTable (typeof (ManifestResourceTable)))
				return;

			ModuleDefinition module = resources.Container as ModuleDefinition;

			ManifestResourceTable mrTable = m_tHeap [typeof(ManifestResourceTable)] as ManifestResourceTable;
			FileTable fTable = m_tHeap [typeof(FileTable)] as FileTable;

			BinaryReader br = m_ir.GetReader ();

			for (int i = 0; i < mrTable.Rows.Count; i++) {
				ManifestResourceRow mrRow = mrTable [i];
				if (mrRow.Implementation.RID == 0) {
					EmbeddedResource eres = new EmbeddedResource (
						m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name], mrRow.Flags, module);

					br.BaseStream.Position = m_img.ResolveTextVirtualAddress (
						m_img.CLIHeader.Resources.VirtualAddress);
					br.BaseStream.Position += mrRow.Offset;

					eres.Data = br.ReadBytes (br.ReadInt32 ());

					(resources as ResourceCollection) [eres.Name] = eres;
					continue;
				}

				switch (mrRow.Implementation.TokenType) {
				case TokenType.File :
					FileRow fRow = fTable [(int) mrRow.Implementation.RID - 1];
					LinkedResource lres = new LinkedResource (
						m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name], mrRow.Flags,
						module, m_img.MetadataRoot.Streams.StringsHeap [fRow.Name]);
					lres.Hash = m_img.MetadataRoot.Streams.BlobHeap.Read (fRow.HashValue);
					(resources as ResourceCollection) [lres.Name] = lres;
					break;
				case TokenType.AssemblyRef :
					AssemblyNameReference asm =
						m_module.AssemblyReferences [(int) mrRow.Implementation.RID - 1] as AssemblyNameReference;
					AssemblyLinkedResource alr = new AssemblyLinkedResource (
						m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name],
						mrRow.Flags, module, asm);
					(resources as ResourceCollection) [alr.Name] = alr;
					break;
				}
			}
		}

		public override void Visit (IModuleDefinitionCollection modules)
		{
			ModuleTable mt = m_tHeap [typeof(ModuleTable)] as ModuleTable;
			if (mt == null || mt.Rows.Count != 1)
				throw new ReflectionException ("Can not read main module");

			ModuleRow mr = mt.Rows [0] as ModuleRow;
			string name = m_img.MetadataRoot.Streams.StringsHeap [mr.Name];
			ModuleDefinition main = new ModuleDefinition (name, m_asmDef, m_ir, true);
			ModuleDefinitionCollection mods = modules as ModuleDefinitionCollection;
			main.Mvid = m_img.MetadataRoot.Streams.GuidHeap [mr.Mvid];
			mods.Add (main);
			m_module = main;

			FileTable ftable = m_tHeap [typeof(FileTable)] as FileTable;
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

		public override void Visit (IModuleReferenceCollection modules)
		{
			if (!m_tHeap.HasTable (typeof (ModuleRefTable)))
				return;

			ModuleRefTable mrTable = m_tHeap [typeof(ModuleRefTable)] as ModuleRefTable;
			for (int i = 0; i < mrTable.Rows.Count; i++) {
				ModuleRefRow mrRow = mrTable [i];
				(modules as ModuleReferenceCollection).Add (
					new ModuleReference (m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name]));
			}
		}

		public override void Terminate (IAssemblyDefinition asm)
		{
			foreach (ModuleDefinition mod in asm.Modules)
				mod.Controller.Reader.Visit (mod.Types);
		}
	}
}
