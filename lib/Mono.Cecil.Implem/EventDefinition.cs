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

    internal sealed class EventDefinition : MemberDefinition, IEventDefinition {

        private ITypeReference m_eventType;
        private EventAttributes m_attributes;

        private IMethodReference m_addMeth;
        private IMethodReference m_invMeth;
        private IMethodReference m_remMeth;

        public ITypeReference EventType {
            get { return m_eventType; }
            set { m_eventType = value; }
        }

        public EventAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public IMethodReference AddMethod {
            get { return m_addMeth; }
            set { m_addMeth = value; }
        }

        public IMethodReference InvokeMethod {
            get { return m_invMeth; }
            set { m_invMeth = value; }
        }

        public IMethodReference RemoveMethod {
            get { return m_remMeth; }
            set { m_remMeth = value; }
        }

        public EventDefinition (string name, TypeDefinition decType, ITypeReference eventType, EventAttributes attrs)
        {
            this.Name = name;
            m_eventType = eventType;
            m_attributes = attrs;
            SetDeclaringType (decType);
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
