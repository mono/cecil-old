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

    using System.Collections;
    using System.Collections.Specialized;

    internal sealed class CustomAttribute : ICustomAttribute {

        private IMethodReference m_ctor;
        private IList m_parameters;
        private IDictionary m_fields;
        private IDictionary m_properties;

        public IMethodReference Constructor {
            get { return m_ctor; }
            set { m_ctor = value; }
        }

        public IList ConstructorParameters {
            get {
                if (m_parameters == null)
                    m_parameters = new ArrayList ();
                return m_parameters;
            }
        }

        public IDictionary Fields {
            get {
                if (m_fields == null)
                    m_fields = new HybridDictionary ();
                return m_fields;
            }
        }

        public IDictionary Properties {
            get {
                if (m_properties == null)
                    m_properties = new HybridDictionary ();
                return m_properties;
            }
        }

        public CustomAttribute (IMethodReference ctor)
        {
            m_ctor = ctor;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
