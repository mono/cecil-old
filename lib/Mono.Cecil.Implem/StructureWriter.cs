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

	internal sealed class StructureWriter : BaseStructureVisitor {

		private MetadataWriter m_mdWriter;
		private MetadataTableWriter m_tableWriter;
		private MetadataRowWriter m_rowWriter;
		private AssemblyKind m_kind;

		private AssemblyDefinition m_asm;
		private BinaryWriter m_binaryWriter;

		public AssemblyKind AssemblyKind {
			get { return m_kind; }
		}

		public StructureWriter (AssemblyDefinition asm, AssemblyKind kind, BinaryWriter writer)
		{
			m_asm = asm;
			m_kind = kind;
			m_binaryWriter = writer;

			ReflectionWriter rw = (asm.MainModule as ModuleDefinition).Controller.Writer;
			rw.StructureWriter = this;

			m_mdWriter = rw.MetadataWriter;
			m_tableWriter = rw.MetadataTableWriter;
			m_rowWriter = rw.MetadataRowWriter;

			// reset images
			foreach (ModuleDefinition module in asm.Modules)
				if (module.Image.CLIHeader.Metadata.VirtualAddress != RVA.Zero)
					module.Image = Image.CreateImage ();
		}

		public BinaryWriter GetWriter ()
		{
			return m_binaryWriter;
		}

		public override void Visit (IAssemblyNameDefinition name)
		{
			AssemblyTable asmTable = m_tableWriter.GetAssemblyTable ();
			AssemblyRow asmRow = m_rowWriter.CreateAssemblyRow (
				name.HashAlgorithm,
				(ushort) name.Version.Major,
				(ushort) name.Version.Minor,
				(ushort) name.Version.Build,
				(ushort) name.Version.Revision,
				name.Flags,
				m_mdWriter.AddBlob (name.PublicKey, true),
				m_mdWriter.AddString (name.Name),
				m_mdWriter.AddString (name.Culture));

			asmTable.Rows.Add (asmRow);
		}

		public override void Visit (IAssemblyNameReference name)
		{
			byte [] pkortoken;
			if (name.PublicKey.Length > 0)
				pkortoken = name.PublicKey;
			else if (name.PublicKeyToken.Length > 0)
				pkortoken = name.PublicKeyToken;
			else
				pkortoken = new byte [0];

			AssemblyRefTable arTable = m_tableWriter.GetAssemblyRefTable ();
			AssemblyRefRow arRow = m_rowWriter.CreateAssemblyRefRow (
				(ushort) name.Version.Major,
				(ushort) name.Version.Minor,
				(ushort) name.Version.Build,
				(ushort) name.Version.Revision,
				name.Flags,
				m_mdWriter.AddBlob (pkortoken, true),
				m_mdWriter.AddString (name.Name),
				m_mdWriter.AddString (name.Culture),
				m_mdWriter.AddBlob (name.Hash, true));

			arTable.Rows.Add (arRow);
		}

		public override void Visit (IEmbeddedResource res)
		{
			AddManifestResource (
				m_mdWriter.AddResource (res.Data),
				res.Name, res.Flags,
				new MetadataToken (TokenType.ManifestResource, 0));
		}

		public override void Visit (ILinkedResource res)
		{
			FileTable fTable = m_tableWriter.GetFileTable ();
			FileRow fRow = m_rowWriter.CreateFileRow (
				Mono.Cecil.FileAttributes.ContainsNoMetaData,
				m_mdWriter.AddString (res.File),
				m_mdWriter.AddBlob (res.Hash, true));

			fTable.Rows.Add (fRow);

			AddManifestResource (
				0, res.Name, res.Flags,
				new MetadataToken (TokenType.File, (uint) fTable.Rows.IndexOf (fRow) + 1));
		}

		public override void Visit (IAssemblyLinkedResource res)
		{
			MetadataToken impl = new MetadataToken (TokenType.AssemblyRef,
				(uint) m_asm.MainModule.AssemblyReferences.IndexOf (res.Assembly) + 1);

			AddManifestResource (0, res.Name, res.Flags, impl);
		}

		private void AddManifestResource (uint offest, string name, ManifestResourceAttributes flags, MetadataToken impl)
		{
			ManifestResourceTable mrTable = m_tableWriter.GetManifestResourceTable ();
			ManifestResourceRow mrRow = m_rowWriter.CreateManifestResourceRow (
				(uint) 0,
				flags,
				m_mdWriter.AddString (name),
				impl);

			mrTable.Rows.Add (mrRow);
		}

		public override void Visit (IModuleDefinition module)
		{
			if (module.Main) {
				ModuleTable modTable = m_tableWriter.GetModuleTable ();
				ModuleRow modRow = m_rowWriter.CreateModuleRow (
					(ushort) 0,
					m_mdWriter.AddString (module.Name),
					m_mdWriter.AddGuid (module.Mvid),
					(uint) 0,
					(uint) 0);

				modTable.Rows.Add (modRow);
			} else {
				// multiple module assemblies
				throw new NotImplementedException ();
			}
		}

		public override void Visit (IModuleReference module)
		{
			ModuleRefTable mrTable = m_tableWriter.GetModuleRefTable ();
			ModuleRefRow mrRow = m_rowWriter.CreateModuleRefRow (
				m_mdWriter.AddString (module.Name));

			mrTable.Rows.Add (mrRow);
		}

		public override void Terminate (IAssemblyDefinition asm)
		{
			foreach (ModuleDefinition mod in asm.Modules)
				mod.Types.Accept (mod.Controller.Writer);
		}
	}
}
