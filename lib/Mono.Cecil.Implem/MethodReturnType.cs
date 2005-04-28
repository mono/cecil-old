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

    using System.Reflection;

    using Mono.Cecil;

    internal sealed class MethodReturnType : IMethodReturnType {

        private MethodReference m_method;
        private ParameterDefinition m_customAttrOwner;

        private ITypeReference m_returnType;

        public MethodReference Method {
            get { return m_method; }
            set { m_method = value; }
        }

        public ITypeReference ReturnType {
            get { return m_returnType; }
            set { m_returnType = value; }
        }

        public ParameterDefinition CustomAttributeOwner {
            get { return m_customAttrOwner; }
            set { m_customAttrOwner = value; }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrOwner == null)
                    m_customAttrOwner = new ParameterDefinition (string.Empty, 0, (ParamAttributes) 0, null);
                return m_customAttrOwner.CustomAttributes;
            }
        }

        public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
        {
            CustomAttribute ca = new CustomAttribute(ctor);
            (m_customAttrOwner.CustomAttributes as CustomAttributeCollection).Add (ca);
            return ca;
        }

        public ICustomAttribute DefineCustomAttribute (ConstructorInfo ctor)
        {
            //TODO: implement this
            return null;
        }

        public MethodReturnType (ITypeReference retType)
        {
            m_returnType = retType;
        }
    }
}
