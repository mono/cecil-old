//
// CustomAttribute.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil {

	using System;
	using System.Collections;

	public sealed class CustomAttribute : ICustomAttribute, ICloneable {

		MethodReference m_ctor;
		IList m_parameters;
		IDictionary m_fields;
		IDictionary m_properties;
		IDictionary m_fieldTypes;
		IDictionary m_propTypes;

		bool m_readable;
		byte [] m_blob;

		public MethodReference Constructor {
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

		public CustomAttribute (MethodReference ctor)
		{
			m_ctor = ctor;
			m_readable = true;
		}

		public TypeReference GetFieldType (string fieldName)
		{
			return (TypeReference) m_fieldTypes [fieldName];
		}

		public TypeReference GetPropertyType (string propertyName)
		{
			return (TypeReference) m_propTypes [propertyName];
		}

		public void SetFieldType (string fieldName, ITypeReference type)
		{
			m_fieldTypes [fieldName] = type;
		}

		public void SetPropertyType (string propertyName, ITypeReference type)
		{
			m_propTypes [propertyName] = type;
		}

		object ICloneable.Clone ()
		{
			return this.Clone ();
		}

		public CustomAttribute Clone ()
		{
			return Clone (this, null);
		}

		internal static CustomAttribute Clone (CustomAttribute custattr, ReflectionHelper helper)
		{
			CustomAttribute ca = new CustomAttribute (custattr.Constructor);
			if (!custattr.IsReadable) {
				ca.IsReadable = false;
				ca.Blob = custattr.Blob;
				return ca;
			}

			foreach (object o in custattr.ConstructorParameters)
				ca.ConstructorParameters.Add (o);
			foreach (DictionaryEntry de in custattr.Fields)
				ca.Fields.Add (de.Key, de.Value);
			foreach (DictionaryEntry de in custattr.Properties)
				ca.Properties.Add (de.Key, de.Value);
			return ca;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitCustomAttribute (this);
		}
	}
}
