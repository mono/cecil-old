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

    internal sealed class MethodReturnType : IMethodReturnType {

        private ITypeReference m_returnType;

        public ITypeReference ReturnType {
            get { return m_returnType; }
            set { m_returnType = value; }
        }

        public MethodReturnType (ITypeReference retType)
        {
            m_returnType = retType;
        }
    }
}
