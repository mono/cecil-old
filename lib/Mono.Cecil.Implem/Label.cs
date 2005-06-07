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

    using Mono.Cecil.Cil;

    internal class Label : ILabel {

        private int m_offset = 0;
        private int m_position = 0;
        private bool m_marked = false;
        private bool m_used = false;
        private Instruction m_owner;

        public int Offset {
            get { return m_offset; }
            set { m_offset = value; }
        }

        public int Position {
            get { return m_position; }
            set { m_position = value; }
        }

        public bool Marked {
            get { return m_marked; }
            set { m_marked = value; }
        }

        public bool Used {
            get { return m_used; }
            set { m_used = value; }
        }

        public Instruction Owner {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public Label ()
        {
        }

        public Label (int offset)
        {
            m_offset = offset;
        }
    }
}
