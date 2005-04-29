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

        private CustomAttributeCollection m_customAttrs;

        private bool m_semanticLoaded;
        private IMethodDefinition m_addMeth;
        private IMethodDefinition m_invMeth;
        private IMethodDefinition m_remMeth;

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
                ((TypeDefinition)this.DeclaringType).Module.Loader.DetailReader.ReadSemantic (this);
                return m_addMeth;
            }
            set { m_addMeth = value; }
        }

        public IMethodDefinition InvokeMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.DetailReader.ReadSemantic (this);
                return m_invMeth;
            }
            set { m_invMeth = value; }
        }

        public IMethodDefinition RemoveMethod {
            get {
                ((TypeDefinition)this.DeclaringType).Module.Loader.DetailReader.ReadSemantic (this);
                return m_remMeth;
            }
            set { m_remMeth = value; }
        }

        public bool SemanticLoaded {
            get { return m_semanticLoaded; }
            set { m_semanticLoaded = value; }
        }

        public ICustomAttributeCollection CustomAttributes {
            get {
                if (m_customAttrs == null)
                    m_customAttrs = new CustomAttributeCollection (this, (this.DeclaringType as TypeDefinition).Module.Loader);
                return m_customAttrs;
            }
        }

        public EventDefinition (string name, TypeDefinition decType, ITypeReference eventType, EventAttributes attrs) : base (name, decType)
        {
            m_eventType = eventType;
            m_attributes = attrs;
        }

        public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
        {
            CustomAttribute ca = new CustomAttribute(ctor);
            m_customAttrs.Add (ca);
            return ca;
        }

        public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
        {
            //TODO: implement this
            return null;
        }

        public void Accept (IReflectionVisitor visitor)
        {
            visitor.Visit (this);
        }
    }
}
