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
    using System.Text;
    using System.Reflection;

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Signatures;

    internal class MethodReference : MemberReference, IMethodReference {

        private ParameterDefinitionCollection m_parameters;
        private MethodReturnType m_returnType;

        private bool m_hasThis;
        private bool m_explicitThis;
        private MethodCallingConvention m_callConv;

        public bool HasThis {
            get { return m_hasThis; }
            set { m_hasThis = value; }
        }

        public bool ExplicitThis {
            get { return m_explicitThis; }
            set { m_explicitThis = value; }
        }

        public MethodCallingConvention CallingConvention {
            get { return m_callConv; }
            set { m_callConv = value; }
        }

        public IParameterDefinitionCollection Parameters {
            get { return m_parameters; }
        }

        public IMethodReturnType ReturnType {
            get { return m_returnType; }
            set { m_returnType = value as MethodReturnType; }
        }

        public MethodReference (string name, ITypeReference decType, bool hasThis,
                                 bool explicitThis, MethodCallingConvention callConv) : base (name, decType)
        {
            m_parameters = new ParameterDefinitionCollection (this);
            m_hasThis = hasThis;
            m_explicitThis = explicitThis;
            m_callConv = callConv;
        }

        public virtual void Accept (IReflectionVisitor visitor)
        {
        }

        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();
            sb.Append (m_returnType.ReturnType.FullName);
            sb.Append (" ");
            sb.Append (base.ToString ());
            sb.Append ("(");
            for (int i = 0; i < m_parameters.Count; i++) {
                sb.Append (m_parameters [i].ParameterType.FullName);
                if (i < m_parameters.Count - 1)
                    sb.Append (",");
            }
            sb.Append (")");
            return sb.ToString ();
        }
    }
}
