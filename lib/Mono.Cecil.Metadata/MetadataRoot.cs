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

namespace Mono.Cecil.Metadata {

    using Mono.Cecil.Binary;

    public sealed class MetadataRoot : IMetadataVisitable {

        private MetadataRootHeader m_header;
        private Image m_image;

        private MetadataStreamCollection m_streams;

        public MetadataRootHeader Header {
            get { return m_header; }
            set { m_header = value; }
        }

        public MetadataStreamCollection Streams {
            get { return m_streams; }
            set { m_streams = value; }
        }

        internal MetadataRoot (Image img)
        {
            m_image = img;
        }

        public Image GetImage ()
        {
            return m_image;
        }

        public void Accept (IMetadataVisitor visitor)
        {
            visitor.Visit (this);

            m_header.Accept (visitor);
            m_streams.Accept (visitor);

            visitor.Terminate (this);
        }

        public sealed class MetadataRootHeader : IHeader, IMetadataVisitable {

            public const uint StandardSignature = 0x424a5342;

            public uint Signature;
            public ushort MinorVersion;
            public ushort MajorVersion;
            public uint Reserved;
            public string Version;
            public ushort Flags;
            public ushort Streams;

            internal MetadataRootHeader ()
            {
            }

            public void SetDefaultValues ()
            {
                Signature = StandardSignature;
                MajorVersion = 1; // TODO produce 1.0, 1.1, or 2.0 ...
                MinorVersion = 0;
                Reserved = 0;
                Flags = 0;
            }

            public void Accept (IMetadataVisitor visitor)
            {
                visitor.Visit (this);
            }
        }
    }
}
