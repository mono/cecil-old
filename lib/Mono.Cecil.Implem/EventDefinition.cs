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

        private IMethodDefinition m_addMeth;
        private IMethodDefinition m_invMeth;
        private IMethodDefinition m_remMeth;

        private bool m_readed;

        public ITypeReference EventType {
            get { return m_eventType; }
            set { m_eventType = value; }
        }

        public EventAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public IMethodDefinition AddMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.SemanticReader.ReadMethods (this);
                return m_addMeth;
            }
            set { m_addMeth = value; }
        }

        public IMethodDefinition InvokeMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.SemanticReader.ReadMethods (this);
                return m_invMeth;
            }
            set { m_invMeth = value; }
        }

        public IMethodDefinition RemoveMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.SemanticReader.ReadMethods (this);
                return m_remMeth;
            }
            set { m_remMeth = value; }
        }

        public bool Readed {
            get { return m_readed; }
            set { m_readed = value; }
        }

        public EventDefinition (string name, TypeDefinition decType, ITypeReference eventType, EventAttributes attrs) : base (name, decType)
        {
            m_eventType = eventType;
            m_attributes = attrs;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
