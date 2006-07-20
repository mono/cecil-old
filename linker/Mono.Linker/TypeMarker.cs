//
// TypeMarker.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.Linker {

	using System.Collections;

	using Mono.Cecil;

	class TypeMarker : Marker {

		TypeDefinition _type;

		IDictionary _fields;
		IDictionary _methods;

		public TypeDefinition Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public TypeMarker (TypeDefinition type)
		{
			_type = type;

			_fields = new Hashtable ();
			_methods = new Hashtable ();
		}

		string Sig (MemberReference member)
		{
			return member.ToString ();
		}

		public MethodMarker Mark (MethodDefinition method)
		{
			return Mark (method, MethodAction.ParseIfLinked);
		}

		public MethodMarker Mark (MethodDefinition method, MethodAction action)
		{
			MethodMarker mm = (MethodMarker) _methods [Sig (method)];
			if (mm == null) {
				mm = new MethodMarker (method, action);
				_methods.Add (Sig (method), mm);
			}

			return mm;
		}

		public FieldMarker Mark (FieldDefinition field)
		{
			FieldMarker fm = (FieldMarker) _fields [Sig (field)];
			if (fm == null) {
				fm = new FieldMarker (field);
				_fields.Add (Sig (field), fm);
			}

			return fm;
		}

		public FieldMarker [] GetFields ()
		{
			FieldMarker [] markers = new FieldMarker[_fields.Count];
			_fields.Values.CopyTo (markers, 0);
			return markers;
		}

		public MethodMarker [] GetMethods ()
		{
			MethodMarker [] markers = new MethodMarker[_methods.Count];
			_methods.Values.CopyTo (markers, 0);
			return markers;
		}
	}
}
