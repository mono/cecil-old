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

using System;
using SR = System.Reflection;
using System.Text;
using System.Xml.XPath;

using Mono.Cecil;

namespace Mono.Linker {

	public class ResolveFromXmlStep : ResolveStep {

		static readonly string _signature = "signature";
		static readonly string _fullname = "fullname";
		static readonly string _required = "required";
		static readonly string _preserve = "preserve";
		static readonly string _ns = "";

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
				XPathNavigator nav = iterator.Current;
				string fullname = GetFullName (nav);
				TypeDefinition type = am.Assembly.MainModule.Types [fullname];
				if (type == null)
					continue;

				TypePreserve preserve = GetTypePreserve (nav);

				if (!IsRequired (nav)) {
					AddPreserveInfo (am, fullname, preserve);
					continue;
				}

				TypeMarker tm = am.Mark (type);

				switch (preserve) {
				case TypePreserve.All:
					MarkEntireType (tm);
					break;
				case TypePreserve.Fields:
					MarkAllFields (tm);
					break;
				case TypePreserve.Methods:
					MarkAllMethods (tm);
					break;
				case TypePreserve.Nothing:
					if (nav.HasChildren) {
						MarkSelectedFields (nav, tm);
						MarkSelectedMethods (nav, tm);
					} else
						MarkEntireType (tm);
					break;
				}
			}
		}

		static void AddPreserveInfo (AssemblyMarker assembly, string fullname, TypePreserve preserve)
		{
			assembly.AddTypePreserveInfo (fullname, preserve);
		}

		void MarkSelectedFields (XPathNavigator nav, TypeMarker tm)
		{
			XPathNodeIterator fields = nav.SelectChildren ("field", _ns);
			if (fields.Count == 0)
				return;

			ProcessFields (tm, fields);
		}

		void MarkSelectedMethods (XPathNavigator nav, TypeMarker tm)
		{
			XPathNodeIterator methods = nav.SelectChildren ("method", _ns);
			if (methods.Count == 0)
				return;

			ProcessMethods (tm, methods);
		}

		static TypePreserve GetTypePreserve (XPathNavigator nav)
		{
			try {
				return (TypePreserve) Enum.Parse (typeof (TypePreserve), GetAttribute (nav, _preserve), true);
			} catch {
				return TypePreserve.Nothing;
			}
		}

		static void MarkEntireType (TypeMarker tm)
		{
			MarkAllFields (tm);
			MarkAllMethods (tm);
		}

		static void MarkAllMethods (TypeMarker tm)
		{
			foreach (MethodDefinition meth in tm.Type.Methods)
				tm.Mark (meth);
			foreach (MethodDefinition ctor in tm.Type.Constructors)
				tm.Mark(ctor);
		}

		static void MarkAllFields (TypeMarker tm)
		{
			foreach (FieldDefinition field in tm.Type.Fields)
				tm.Mark (field);
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

		static FieldDefinition GetField (TypeDefinition type, string signature)
		{
			foreach (FieldDefinition field in type.Fields)
				if (signature == GetFieldSignature (field))
					return field;

			return null;
		}

		static string GetFieldSignature (FieldDefinition field)
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

		static MethodDefinition GetMethod (TypeDefinition type, string signature)
		{
			foreach (MethodDefinition meth in type.Methods)
				if (signature == GetMethodSignature (meth))
					return meth;

			foreach (MethodDefinition ctor in type.Constructors)
				if (signature == GetMethodSignature (ctor))
					return ctor;

			return null;
		}

		static string GetMethodSignature (MethodDefinition meth)
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

		static AssemblyMarker GetAssemblyMarker (LinkContext context, string assemblyName)
		{
			AssemblyNameReference reference = AssemblyNameReference.Parse (assemblyName);
			if (IsSimpleName (assemblyName)) {
				foreach (AssemblyMarker marker in context.GetAssemblies ()) {
					if (marker.Assembly.Name.Name == assemblyName)
						return marker;
				}
			}
			AssemblyMarker res = context.Resolve (reference);
			ProcessReferences (res.Assembly, context);
			return res;
		}

		static void ProcessReferences (AssemblyDefinition assembly, LinkContext context)
		{
			foreach (AssemblyNameReference name in assembly.MainModule.AssemblyReferences) {
				context.Resolve (name);
			}
		}

		static bool IsSimpleName (string assemblyName)
		{
			return assemblyName.IndexOf (",") == -1;
		}

		static bool IsRequired (XPathNavigator nav)
		{
			string attribute = GetAttribute (nav, _required);
			if (attribute == null || attribute.Length == 0)
				return true;

			return TryParseBool (attribute);
		}

		static bool TryParseBool (string s)
		{
			try {
				return bool.Parse (s);
			} catch {
				return false;
			}
		}

		static string GetSignature (XPathNavigator nav)
		{
			return GetAttribute (nav, _signature);
		}

		static string GetFullName (XPathNavigator nav)
		{
			return GetAttribute (nav, _fullname);
		}

		static string GetAttribute (XPathNavigator nav, string attribute)
		{
			return nav.GetAttribute (attribute, _ns);
		}
	}
}
