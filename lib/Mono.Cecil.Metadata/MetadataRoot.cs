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

namespace Mono.Cecil.Metadata {

    using Mono.Cecil.Binary;

    internal sealed class MetadataRoot : IMetadataVisitable {

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

        public MetadataRoot (Image img)
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

            private uint m_signature;
            private ushort m_minorVersion;
            private ushort m_majorVersion;
            private uint m_reserved;
            private string m_version;
            private ushort m_flags;
            private ushort m_streams;

            private MetadataRoot m_owner;

            public uint Signature {
                get { return m_signature; }
                set { m_signature = value; }
            }

            public ushort MinorVersion {
                get { return m_minorVersion; }
                set { m_minorVersion = value; }
            }

            public ushort MajorVersion {
                get { return m_majorVersion; }
                set { m_majorVersion = value; }
            }

            public uint Reserved {
                get { return m_reserved; }
                set { m_reserved = value; }
            }

            public string Version {
                get { return m_version; }
                set { m_version = value; }
            }

            public ushort Flags {
                get { return m_flags; }
                set { m_flags = value; }
            }

            public ushort Streams {
                get { return m_streams; }
                set { m_streams = value; }
            }

            public MetadataRootHeader (MetadataRoot owner)
            {
                m_owner = owner;
            }

            public void SetDefaultValues ()
            {
                m_signature = 0x424A5342;
                m_majorVersion = 1;
                m_minorVersion = 0;
                m_reserved = 0;
                m_flags = 0;
            }

            public void Accept (IMetadataVisitor visitor)
            {
                visitor.Visit (this);
            }
        }
    }
}
