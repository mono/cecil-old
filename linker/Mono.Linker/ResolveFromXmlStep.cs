//
// ResolveFromXmlStep.cs
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

	using SR = System.Reflection;
	using System.Text;
	using System.Xml.XPath;

	using Mono.Cecil;

	public class ResolveFromXmlStep : ResolveStep {

		const string _signature = "signature";
		const string _fullname = "fullname";
		const string _ns = "";

		XPathDocument _document;

		public ResolveFromXmlStep (XPathDocument document)
		{
			_document = document;
		}

		public override void Process (LinkContext context)
		{
			XPathNavigator nav = _document.CreateNavigator ();
			nav.MoveToFirstChild ();
			ProcessAssemblies (context, nav.SelectChildren ("assembly", _ns));
		}

		void ProcessAssemblies (LinkContext context, XPathNodeIterator iterator)
		{
			while (iterator.MoveNext ()) {
				AssemblyMarker am = GetAssemblyMarker (context, GetFullName (iterator.Current));
				ProcessTypes (am, iterator.Current.SelectChildren ("type", _ns));
			}
		}

		void ProcessTypes (AssemblyMarker am, XPathNodeIterator iterator)
		{
			while (iterator.MoveNext ()) {
				TypeDefinition type = am.Assembly.MainModule.Types [GetFullName (iterator.Current)];
				if (type != null) {
					TypeMarker tm = am.Mark (type);

					XPathNodeIterator fieldsIterator = iterator.Current.SelectChildren ("field", _ns);
					XPathNodeIterator methodsIterator = iterator.Current.SelectChildren ("method", _ns);

					if (fieldsIterator.Count != 0 || methodsIterator.Count != 0) {
						ProcessFields (tm, fieldsIterator);
						ProcessMethods (tm, methodsIterator);
					} else
						MarkEntireType (tm);
				}
			}
		}

		void MarkEntireType (TypeMarker tm)
		{
			foreach (FieldDefinition field in tm.Type.Fields)
				tm.Mark (field);
			foreach (MethodDefinition meth in tm.Type.Methods)
				tm.Mark (meth);
			foreach (MethodDefinition ctor in tm.Type.Constructors)
				tm.Mark(ctor);
		}

		void ProcessFields (TypeMarker tm, XPathNodeIterator iterator)
		{
			while (iterator.MoveNext ()) {
				string signature = GetSignature (iterator.Current);
				FieldDefinition field = GetField (tm.Type, signature);
				if (field != null)
					tm.Mark (field);
				else
					AddUnresolveMarker (string.Format ("T: {0}; F: {1}", tm.Type, signature));
			}
		}

		FieldDefinition GetField (TypeDefinition type, string signature)
		{
			foreach (FieldDefinition field in type.Fields)
				if (signature == GetFieldSignature (field))
					return field;

			return null;
		}

		string GetFieldSignature (FieldDefinition field)
		{
			return field.FieldType.FullName + " " + field.Name;
		}

		void ProcessMethods (TypeMarker tm, XPathNodeIterator iterator)
		{
			while (iterator.MoveNext()) {
				string signature = GetSignature (iterator.Current);
				MethodDefinition meth = GetMethod (tm.Type, signature);
				if (meth != null)
					tm.Mark (meth);
				else
					AddUnresolveMarker (string.Format ("T: {0}; M: {1}", tm.Type, signature));
			}
		}

		MethodDefinition GetMethod (TypeDefinition type, string signature)
		{
			foreach (MethodDefinition meth in type.Methods)
				if (signature == GetMethodSignature (meth))
					return meth;

			foreach (MethodDefinition ctor in type.Constructors)
				if (signature == GetMethodSignature (ctor))
					return ctor;

			return null;
		}

		string GetMethodSignature (MethodDefinition meth)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (meth.ReturnType.ReturnType.FullName);
			sb.Append (" ");
			sb.Append (meth.Name);
			sb.Append ("(");
			for (int i = 0; i < meth.Parameters.Count; i++) {
				if (i > 0)
					sb.Append (",");

				sb.Append (meth.Parameters [i].ParameterType.FullName);
			}
			sb.Append (")");
			return sb.ToString ();
		}

		AssemblyMarker GetAssemblyMarker(LinkContext context, string assemblyName)
		{
			SR.AssemblyName name = SR.AssemblyName.GetAssemblyName (assemblyName);
			AssemblyNameReference nameRef = new AssemblyNameReference ();
			nameRef.Name = name.Name;
			nameRef.Version = name.Version;
			nameRef.PublicKeyToken = name.GetPublicKeyToken ();
			return context.Resolve (nameRef);
		}

		static string GetSignature(XPathNavigator nav)
		{
			return nav.GetAttribute (_signature, _ns);
		}

		static string GetFullName (XPathNavigator nav)
		{
			return nav.GetAttribute (_fullname, _ns);
		}
	}
}
