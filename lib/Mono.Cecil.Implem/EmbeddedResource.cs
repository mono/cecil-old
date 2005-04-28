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

    internal sealed class EmbeddedResource : Resource, IEmbeddedResource {

        private byte [] m_data;

        public byte [] Data {
            get { return m_data; }
            set { m_data = value; }
        }

        public EmbeddedResource (string name, ManifestResourceAttributes attributes, ModuleDefinition owner) : base (name, attributes, owner)
        {
        }

        public EmbeddedResource (string name, ManifestResourceAttributes attributes, ModuleDefinition owner, byte [] data) :
            base (name, attributes, owner)
        {
            m_data = data;
        }

        public override void Accept (IReflectionStructureVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
