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
    using Mono.Cecil.Signatures;

    internal sealed class ReferenceType : TypeReference, IReferenceType {

        private ITypeReference m_type;

        public override string Name {
            get { return m_type.Name; }
            set { m_type.Name = value; }
        }

        public override string Namespace {
            get { return m_type.Namespace; }
            set { m_type.Namespace = value; }
        }

        public override IMetadataScope Scope {
            get { return m_type.Scope; }
        }

        public ITypeReference ElementType {
            get { return m_type; }
            set { m_type = value; }
        }

        public override string FullName {
            get { return string.Concat (m_type.FullName, "&"); }
        }

        public ReferenceType (ITypeReference type) : base (string.Empty, string.Empty)
        {
            m_type = type;
        }
    }
}
