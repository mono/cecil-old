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

    using Mono.Cecil.Metadata;

    public sealed class Image : IBinaryVisitable {

        private DOSHeader m_dosHeader;
        private PEFileHeader m_peFileHeader;
        private PEOptionalHeader m_peOptionalHeader;

        private SectionCollection m_sections;

        private CLIHeader m_cliHeader;

        private MetadataRoot m_mdRoot;

        private FileInfo m_img;

        public DOSHeader DOSHeader {
            get { return m_dosHeader; }
        }

        public PEFileHeader PEFileHeader {
            get { return m_peFileHeader; }
        }

        public PEOptionalHeader PEOptionalHeader {
            get { return m_peOptionalHeader; }
        }

        public SectionCollection Sections {
            get { return m_sections; }
        }

        public CLIHeader CLIHeader {
            get { return m_cliHeader; }
        }

        public MetadataRoot MetadataRoot {
            get { return m_mdRoot; }
        }

        public FileInfo FileInformation {
            get { return m_img; }
        }

        private Image ()
        {
            m_dosHeader = new DOSHeader ();
            m_peFileHeader = new PEFileHeader ();
            m_peOptionalHeader = new PEOptionalHeader ();
            m_sections = new SectionCollection ();
            m_cliHeader = new CLIHeader ();
            m_mdRoot = new MetadataRoot (this);
        }

        private Image (FileInfo img) : this ()
        {
            m_img = img;
        }

        public long ResolveVirtualAddress (RVA rva)
        {
            foreach (Section sect in this.Sections) {
                if (rva >= sect.VirtualAddress &&
                    rva < sect.VirtualAddress + sect.SizeOfRawData)

                    return rva + sect.PointerToRawData - sect.VirtualAddress;
            }
            return 0;
        }

        public void Accept (IBinaryVisitor visitor)
        {
            visitor.Visit (this);

            m_dosHeader.Accept (visitor);
            m_peFileHeader.Accept (visitor);
            m_peOptionalHeader.Accept (visitor);

            m_sections.Accept (visitor);

            m_cliHeader.Accept (visitor);

            visitor.Terminate (this);
        }

        public static Image CreateImage ()
        {
            Image img = new Image ();

            ImageInitializer init = new ImageInitializer (img);
            img.Accept (init);

            return img;
        }

        public static Image GetImage (string file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException ("file");

            FileInfo img = new FileInfo (file);
            if (!File.Exists (img.FullName))
                throw new FileNotFoundException (img.FullName);

            Image ret = new Image (img);
            return ret;
        }
    }
}
