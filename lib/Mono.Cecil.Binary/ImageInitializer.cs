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

namespace Mono.Cecil.Binary {

	using System;
	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Metadata;

	internal sealed class ImageInitializer : BaseImageVisitor {

		private Image m_image;
		private MetadataInitializer m_mdinit;

		public Image Image {
			get { return m_image; }
		}

		public MetadataInitializer Metadata {
			get { return m_mdinit; }
		}

		public ImageInitializer (Image image)
		{
			m_image = image;
			m_mdinit = new MetadataInitializer (this);
		}

		public override void VisitDOSHeader (DOSHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void VisitPEOptionalHeader (PEOptionalHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void VisitPEFileHeader (PEFileHeader header)
		{
			header.SetDefaultValues ();
			header.TimeDateStamp = (uint) DateTime.UtcNow.Subtract (
				new DateTime (1970, 1, 1)).TotalSeconds;
		}

		public override void VisitNTSpecificFieldsHeader (PEOptionalHeader.NTSpecificFieldsHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void VisitStandardFieldsHeader (PEOptionalHeader.StandardFieldsHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void VisitDataDirectoriesHeader (PEOptionalHeader.DataDirectoriesHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void VisitSectionCollection (SectionCollection coll)
		{
			Section text = new Section ();
			text.Name = Section.Text;
			text.Characteristics = SectionCharacteristics.ContainsCode |
				SectionCharacteristics.MemoryRead | SectionCharacteristics.MemExecute;
			m_image.TextSection = text;

			Section reloc = new Section ();
			reloc.Name =  Section.Relocs;
			reloc.Characteristics = SectionCharacteristics.ContainsInitializedData |
				SectionCharacteristics.MemDiscardable | SectionCharacteristics.MemoryRead;

			coll.Add (text);
			coll.Add (reloc);
		}

		public override void VisitSection (Section sect)
		{
			sect.SetDefaultValues ();
		}

		public override void VisitCLIHeader (CLIHeader header)
		{
			header.SetDefaultValues ();
			m_image.MetadataRoot.Accept (m_mdinit);
		}

		public override void VisitImportTable (ImportTable it)
		{
			it.ImportAddressTable = new RVA (0x2000);
		}

		public override void VisitHintNameTable (HintNameTable hnt)
		{
			hnt.Hint = 0;
			hnt.RuntimeLibrary = HintNameTable.RuntimeCorEE;
			hnt.EntryPoint = 0x25ff;
			hnt.RVA = new RVA (0x402000);
		}
	}
}
