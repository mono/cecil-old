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

    using System;

    [AttributeUsage (AttributeTargets.Struct)]
    internal class ColumnSizeAttribute : Attribute {

        private int m_size;

        public int Size {
            get { return m_size; }
        }

        public ColumnSizeAttribute (int size)
        {
            m_size = size;
        }
    }
}
