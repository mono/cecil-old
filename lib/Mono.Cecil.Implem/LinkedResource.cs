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

    internal sealed class LinkedResource : Resource, ILinkedResource {

        private byte [] m_hash;
        private string m_file;

        public byte [] Hash {
            get { return m_hash; }
            set { m_hash = value; }
        }

        public string File {
            get { return m_file; }
            set { m_file = value; }
        }

        public LinkedResource (string name, ManifestResourceAttributes attributes, ModuleDefinition owner, string file) :
            base (name, attributes, owner)
        {
            m_file = file;
        }

        public override void Accept (IReflectionStructureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}

