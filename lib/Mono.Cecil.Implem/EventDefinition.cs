/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
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
				if (!this.DecTypeDef.IsLazyLoadable && !m_semanticLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadSemantic (this);

				return m_addMeth;
			}
			set { m_addMeth = value; }
		}

		public IMethodDefinition InvokeMethod {
			get {
				if (!this.DecTypeDef.IsLazyLoadable && !m_semanticLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadSemantic (this);

				return m_invMeth;
			}
			set { m_invMeth = value; }
		}

		public IMethodDefinition RemoveMethod {
			get {
				if (!this.DecTypeDef.IsLazyLoadable && !m_semanticLoaded)
					this.DecTypeDef.Mod.Controller.Reader.ReadSemantic (this);

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
					if (this.DecTypeDef.IsLazyLoadable) {
						m_customAttrs = new CustomAttributeCollection (this, this.DecTypeDef.Mod.Controller);
						m_customAttrs.Load ();
					} else
						m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public bool IsRuntimeSpecialName {
			get { return (m_attributes & EventAttributes.RTSpecialName) != 0; }
			set { m_attributes |= value ? EventAttributes.RTSpecialName : 0; }
		}

		public bool IsSpecialName {
			get { return (m_attributes & EventAttributes.SpecialName) != 0; }
			set { m_attributes |= value ? EventAttributes.SpecialName : 0; }
		}

		public EventDefinition (string name, ITypeReference eventType,
			EventAttributes attrs) : base (name)
		{
			m_eventType = eventType;
			m_attributes = attrs;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitEventDefinition (this);

			this.CustomAttributes.Accept (visitor);
		}
	}
}
