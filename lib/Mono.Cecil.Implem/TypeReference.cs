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

namespace Mono.Cecil.Implem {

    using System;

    using Mono.Cecil;

    internal sealed class TypeReference : ITypeReference {

        private string m_name;
        private string m_namespace;

        public String Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public String Namespace {
            get { return m_namespace; }
            set { m_namespace = value; }
        }

        public String FullName {
            get { throw new NotImplementedException (); }
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
