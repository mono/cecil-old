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

    [AttributeUsage (AttributeTargets.Class)]
    internal class RIdAttribute : Attribute {

        private int m_id;

        public int Id {
            get { return m_id; }
        }

        public RIdAttribute (int id) {
            m_id = id;
        }
    }
}
