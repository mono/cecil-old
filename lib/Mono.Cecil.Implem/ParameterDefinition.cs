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

    internal sealed class ParameterDefinition : IParameterDefinition {

        private string m_name;
        private int m_sequence;
        private ParamAttributes m_attributes;
        private ITypeReference m_paramType;
        private object m_def;

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public int Sequence {
            get { return m_sequence; }
            set { m_sequence = value; }
        }

        public ParamAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public ITypeReference ParameterType {
            get { return m_paramType; }
            set { m_paramType = value; }
        }

        public object DefaultValue {
            get { return m_def; }
            set { m_def = value; }
        }

        public ParameterDefinition (string name, int seq, ParamAttributes attrs, ITypeReference paramType)
        {
            m_name = name;
            m_sequence = seq;
            m_attributes = attrs;
            m_paramType = paramType;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}

