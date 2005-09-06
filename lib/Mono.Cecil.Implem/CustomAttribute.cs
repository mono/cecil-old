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

	using System.Collections;

	internal sealed class CustomAttribute : ICustomAttribute {

		private IMethodReference m_ctor;
		private IList m_parameters;
		private IDictionary m_fields;
		private IDictionary m_properties;
		private IDictionary m_fieldTypes;
		private IDictionary m_propTypes;

		private bool m_readable;
		private byte [] m_blob;

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
				if (m_fields == null) {
					m_fields = new Hashtable ();
					m_fieldTypes = new Hashtable ();
				}
				return m_fields;
			}
		}

		public IDictionary Properties {
			get {
				if (m_properties == null) {
					m_properties = new Hashtable ();
					m_propTypes = new Hashtable ();
				}
				return m_properties;
			}
		}

		public bool IsReadable {
			get { return m_readable; }
			set { m_readable = value; }
		}

		public byte [] Blob {
			get { return m_blob; }
			set { m_blob = value; }
		}

		public CustomAttribute (IMethodReference ctor)
		{
			m_ctor = ctor;
			m_readable = true;
		}

		public ITypeReference GetFieldType (string fieldName)
		{
			return m_fieldTypes [fieldName] as ITypeReference;
		}

		public ITypeReference GetPropertyType (string propertyName)
		{
			return m_propTypes [propertyName] as ITypeReference;
		}

		public void SetFieldType (string fieldName, ITypeReference type)
		{
			m_fieldTypes [fieldName] = type;
		}

		public void SetPropertyType (string propertyName, ITypeReference type)
		{
			m_propTypes [propertyName] = type;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitCustomAttribute (this);
		}
	}
}
