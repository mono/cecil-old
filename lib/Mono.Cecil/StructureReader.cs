//
// StructureReader.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil {

	using System;
	using System.IO;

	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;

	internal sealed class StructureReader : BaseStructureVisitor {

		ImageReader m_ir;
		Image m_img;
		AssemblyDefinition m_asmDef;
		ModuleDefinition m_module;
		TablesHeap m_tHeap;

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

		public override void VisitAssemblyDefinition (AssemblyDefinition asm)
		{
			if (!m_tHeap.HasTable (typeof (AssemblyTable)))
				throw new ReflectionException ("No assembly manifest");

			m_asmDef = asm;

			if (m_img.CLIHeader.MajorRuntimeVersion == 1)
				if (m_img.CLIHeader.MinorRuntimeVersion == 1)
					asm.Runtime = TargetRuntime.NET_1_1;
				else
					asm.Runtime = TargetRuntime.NET_1_0;
			else if (m_img.CLIHeader.MajorRuntimeVersion == 2)
				asm.Runtime = TargetRuntime.NET_2_0;
		}

		public override void VisitAssemblyNameDefinition (AssemblyNameDefinition name)
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

		public override void VisitAssemblyNameReferenceCollection (AssemblyNameReferenceCollection names)
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
				names.Add (aname);
			}
		}

		public override void VisitResourceCollection (ResourceCollection resources)
		{
			if (!m_tHeap.HasTable (typeof (ManifestResourceTable)))
				return;

			ManifestResourceTable mrTable = m_tHeap [typeof(ManifestResourceTable)] as ManifestResourceTable;
			FileTable fTable = m_tHeap [typeof(FileTable)] as FileTable;

			BinaryReader br;

			for (int i = 0; i < mrTable.Rows.Count; i++) {
				ManifestResourceRow mrRow = mrTable [i];
				if (mrRow.Implementation.RID == 0) {
					EmbeddedResource eres = new EmbeddedResource (
						m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name], mrRow.Flags);

					br = m_ir.MetadataReader.GetDataReader (
						m_img.CLIHeader.Resources.VirtualAddress);
					br.BaseStream.Position += mrRow.Offset;

					eres.Data = br.ReadBytes (br.ReadInt32 ());

					resources.Add (eres);
					continue;
				}

				switch (mrRow.Implementation.TokenType) {
				case TokenType.File :
					FileRow fRow = fTable [(int) mrRow.Implementation.RID - 1];
					LinkedResource lres = new LinkedResource (
						m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name], mrRow.Flags,
						m_img.MetadataRoot.Streams.StringsHeap [fRow.Name]);
					lres.Hash = m_img.MetadataRoot.Streams.BlobHeap.Read (fRow.HashValue);
					resources.Add (lres);
					break;
				case TokenType.AssemblyRef :
					AssemblyNameReference asm =
						m_module.AssemblyReferences [(int) mrRow.Implementation.RID - 1];
					AssemblyLinkedResource alr = new AssemblyLinkedResource (
						m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name],
						mrRow.Flags, asm);
					resources.Add (alr);
					break;
				}
			}
		}

		public override void VisitModuleDefinitionCollection (ModuleDefinitionCollection modules)
		{
			ModuleTable mt = m_tHeap [typeof(ModuleTable)] as ModuleTable;
			if (mt == null || mt.Rows.Count != 1)
				throw new ReflectionException ("Can not read main module");

			ModuleRow mr = mt.Rows [0] as ModuleRow;
			string name = m_img.MetadataRoot.Streams.StringsHeap [mr.Name];
			ModuleDefinition main = new ModuleDefinition (name, m_asmDef, m_ir, true);
			main.Mvid = m_img.MetadataRoot.Streams.GuidHeap [mr.Mvid];
			modules.Add (main);
			m_module = main;
			m_module.Accept (this);

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

							modules.Add (modext);
						} catch (ReflectionException) {
							throw;
						} catch (Exception e) {
							throw new ReflectionException ("Can not read module : " + name, e);
						}
					}
				}
			}
		}

		public override void VisitModuleReferenceCollection (ModuleReferenceCollection modules)
		{
			if (!m_tHeap.HasTable (typeof (ModuleRefTable)))
				return;

			ModuleRefTable mrTable = m_tHeap [typeof(ModuleRefTable)] as ModuleRefTable;
			for (int i = 0; i < mrTable.Rows.Count; i++) {
				ModuleRefRow mrRow = mrTable [i];
				modules.Add (
					new ModuleReference (m_img.MetadataRoot.Streams.StringsHeap [mrRow.Name]));
			}
		}

		public override void TerminateAssemblyDefinition (AssemblyDefinition asm)
		{
			foreach (ModuleDefinition mod in asm.Modules)
				mod.Controller.Reader.VisitModuleDefinition (mod);
		}
	}
}
