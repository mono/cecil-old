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

		public override void Visit (DOSHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (PEOptionalHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (PEFileHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (PEOptionalHeader.NTSpecificFieldsHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (PEOptionalHeader.StandardFieldsHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (PEOptionalHeader.DataDirectoriesHeader header)
		{
			header.SetDefaultValues ();
		}

		public override void Visit (SectionCollection coll)
		{
			string [] names = new string [] {".text", ".reloc"};
			foreach (string name in names) {
				Section s = new Section ();
				s.Name = name;
				coll.Add (s);
			}
		}

		public override void Visit (Section sect)
		{
			sect.SetDefaultValues ();
		}

		public override void Visit (CLIHeader header)
		{
			header.SetDefaultValues ();
			m_image.MetadataRoot.Accept (m_mdinit);
		}
	}
}
