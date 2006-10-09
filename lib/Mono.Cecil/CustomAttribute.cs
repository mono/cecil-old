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

	public sealed class CustomAttribute : IRequireResolving, IReflectionVisitable {

		MethodReference m_ctor;
		IList m_parameters;
		IDictionary m_fields;
		IDictionary m_properties;
		IDictionary m_fieldTypes;
		IDictionary m_propTypes;

		bool m_resolved;
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
				if (m_fields == null)
					m_fields = new Hashtable ();

				return m_fields;
			}
		}

		public IDictionary Properties {
			get {
				if (m_properties == null)
					m_properties = new Hashtable ();

				return m_properties;
			}
		}

		internal IDictionary FieldTypes {
			get {
				if (m_fieldTypes == null)
					m_fieldTypes = new Hashtable ();

				return m_fieldTypes;
			}
		}

		internal IDictionary PropertyTypes {
			get {
				if (m_propTypes == null)
					m_propTypes = new Hashtable ();

				return m_propTypes;
			}
		}

		public bool Resolved {
			get { return m_resolved; }
			set { m_resolved = value; }
		}

		public byte [] Blob {
			get { return m_blob; }
			set { m_blob = value; }
		}

		public CustomAttribute (MethodReference ctor)
		{
			m_ctor = ctor;
			m_resolved = true;
		}

		public TypeReference GetFieldType (string fieldName)
		{
			return (TypeReference) this.FieldTypes [fieldName];
		}

		public TypeReference GetPropertyType (string propertyName)
		{
			return (TypeReference) this.PropertyTypes [propertyName];
		}

		public void SetFieldType (string fieldName, TypeReference type)
		{
			this.FieldTypes [fieldName] = type;
		}

		public void SetPropertyType (string propertyName, TypeReference type)
		{
			this.PropertyTypes [propertyName] = type;
		}

		public CustomAttribute Clone ()
		{
			return Clone (this, new ImportContext ());
		}

		static void Clone (IDictionary original, IDictionary target)
		{
			foreach (DictionaryEntry entry in original)
				target.Add (entry.Key, entry.Value);
		}

		internal static CustomAttribute Clone (CustomAttribute custattr, ImportContext context)
		{
			CustomAttribute ca = new CustomAttribute (context.Import (custattr.Constructor));
			if (!custattr.Resolved) {
				ca.Resolved = false;
				ca.Blob = custattr.Blob;
				return ca;
			}

			foreach (object o in custattr.ConstructorParameters)
				ca.ConstructorParameters.Add (o);
			Clone (custattr.Fields, ca.Fields);
			Clone (custattr.FieldTypes, ca.FieldTypes);
			Clone (custattr.Properties, ca.Properties);
			Clone (custattr.PropertyTypes, ca.PropertyTypes);
			return ca;
		}

		public bool Resolve ()
		{
			throw new NotImplementedException ();
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitCustomAttribute (this);
		}
	}
}
