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

namespace Mono.Cecil.Signatures {

    internal sealed class ArrayShape {

        private int m_rank;
        private int m_numSizes;
        private int [] m_sizes;
        private int m_numLoBounds;
        private int [] m_loBounds;

        public int Rank {
            get { return m_rank; }
            set { m_rank = value; }
        }

        public int NumSizes {
            get { return m_numSizes; }
            set { m_numSizes = value; }
        }

        public int [] Sizes {
            get { return m_sizes; }
            set { m_sizes = value; }
        }

        public int NumLoBounds {
            get { return m_numLoBounds; }
            set { m_numLoBounds = value; }
        }

        public int [] LoBounds {
            get { return m_loBounds; }
            set { m_loBounds = value; }
        }

        public ArrayShape ()
        {
        }
    }
}
