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

    internal sealed class PointerType : TypeReference, IPointerType {

        private ITypeReference m_pointedType;

        public override string Name {
            get { return m_pointedType.Name; }
            set { m_pointedType.Name = value; }
        }

        public override string Namespace {
            get { return m_pointedType.Namespace; }
            set { m_pointedType.Namespace = value; }
        }

        public ITypeReference ElementType {
            get { return m_pointedType; }
            set { m_pointedType = value; }
        }

        public override string FullName {
            get { return string.Concat (m_pointedType.FullName, "*"); }
        }

        public PointerType (ITypeReference pType) : base (string.Empty, string.Empty)
        {
            m_pointedType = pType;
        }
    }
}
