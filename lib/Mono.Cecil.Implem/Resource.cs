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

    using Mono.Cecil;
    using Mono.Cecil.Metadata;

    internal abstract class Resource :  IResource {

        private string m_name;
        private ManifestResourceAttributes m_attributes;

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public ManifestResourceAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        protected Resource(string name, ManifestResourceAttributes attributes)
        {
            m_name = name;
            m_attributes = attributes;
        }

        public abstract void Accept(IReflectionStructureVisitor visitor);
    }
}

