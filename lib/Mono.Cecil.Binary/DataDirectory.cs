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

    using Mono.Cecil.Metadata;

    public struct DataDirectory {

        public static readonly DataDirectory Zero = new DataDirectory(RVA.Zero, 0);

        private RVA m_virtualAddress;
        private uint m_size;

        public RVA VirtualAddress {
            get { return m_virtualAddress; }
            set { m_virtualAddress = value; }
        }

        public uint Size {
            get { return m_size; }
            set { m_size = value; }
        }

        public DataDirectory (RVA virtualAddress, uint size)
        {
            m_virtualAddress = virtualAddress;
            m_size = size;
        }

        public override int GetHashCode ()
        {
            return (m_virtualAddress.GetHashCode () ^ (int) m_size << 1);
        }

        public override bool Equals (object other)
        {
            if (other is DataDirectory) {
                DataDirectory odd = (DataDirectory)other;
                return (this.m_virtualAddress == odd.m_virtualAddress &&
                this.m_size == odd.m_size);
            }
            return false;
        }

        public override string ToString ()
        {
            return string.Format ("{0} [{1}]",
                                  m_virtualAddress.ToString (), m_size.ToString ("X"));
        }

        public static bool operator == (DataDirectory one, DataDirectory other)
        {
            return one.Equals (other);
        }

        public static bool operator != (DataDirectory one, DataDirectory other)
        {
            return !one.Equals (other);
        }
    }
}
