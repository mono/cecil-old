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

        private MethodReference m_method;
        private CustomAttributeCollection m_customAttrs;

        private ITypeReference m_returnType;

        public MethodReference Method {
            get { return m_method; }
            set { m_method = value; }
        }

        public ITypeReference ReturnType {
            get { return m_returnType; }
            set { m_returnType = value; }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null && m_method != null)
                    m_customAttrs = new CustomAttributeCollection (this, (m_method.DeclaringType as TypeDefinition).Module.Loader);
                else if (m_customAttrs == null)
                    m_customAttrs = new CustomAttributeCollection (this);
                return m_customAttrs;
            }
        }

        public MethodReturnType (ITypeReference retType)
        {
            m_returnType = retType;
        }
    }
}
