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

namespace Mono.Cecil.Binary {

    using System;
    using System.IO;

    using Mono.Cecil.Metadata;

    internal sealed class Image : IBinaryVisitable {

        private DOSHeader m_dosHeader;
        private PEFileHeader m_peFileHeader;
        private PEOptionalHeader m_peOptionalHeader;

        private SectionCollection m_sections;

        private CLIHeader m_cliHeader;

        private MetadataRoot m_mdRoot;

        private FileInfo m_img;

        public DOSHeader DOSHeader {
            get { return m_dosHeader; }
            set { m_dosHeader = value; }
        }

        public PEFileHeader PEFileHeader {
            get { return m_peFileHeader; }
            set { m_peFileHeader = value; }
        }

        public PEOptionalHeader PEOptionalHeader {
            get { return m_peOptionalHeader; }
            set { m_peOptionalHeader = value; }
        }

        public SectionCollection Sections {
            get { return m_sections; }
            set { m_sections = value; }
        }

        public CLIHeader CLIHeader {
            get { return m_cliHeader; }
            set { m_cliHeader = value; }
        }

        public MetadataRoot MetadataRoot {
            get { return m_mdRoot; }
            set { m_mdRoot = value; }
        }

        public FileInfo FileInformation {
            get { return m_img; }
        }

        private Image(FileInfo img) {
            m_img = img;
            m_dosHeader = new DOSHeader();
            m_peFileHeader = new PEFileHeader();
            m_peOptionalHeader = new PEOptionalHeader();
            m_sections = new SectionCollection();
            m_cliHeader = new CLIHeader();
            m_mdRoot = new MetadataRoot(this);
        }

        public long ResolveVirtualAddress(RVA rva) {
            foreach (Section sect in this.Sections) {
                if (rva >= sect.VirtualAddress &&
                    rva < sect.VirtualAddress + sect.SizeOfRawData) {

                    return rva + sect.PointerToRawData - sect.VirtualAddress;
                }
            }
            return 0;
        }

        public void Accept(IBinaryVisitor visitor) {
            visitor.Visit(this);

            m_dosHeader.Accept(visitor);
            m_peFileHeader.Accept(visitor);
            m_peOptionalHeader.Accept(visitor);

            m_sections.Accept(visitor);

            m_cliHeader.Accept(visitor);

            visitor.Terminate(this);
        }

        public static Image GetImage(string file) {
            if (file == null || file.Length == 0) {
                throw new ArgumentException("file");
            }
            FileInfo img = new FileInfo(file);
            if (!File.Exists(img.FullName)) {
                throw new FileNotFoundException(img.FullName);
            }
            Image ret = new Image(img);
            return ret;
        }
    }
}
