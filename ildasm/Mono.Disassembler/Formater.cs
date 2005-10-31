//
// Formater.cs
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

namespace Mono.Disassembler {

	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text;

	using Mono.Cecil;

	class Formater {

		static readonly StringDictionary m_aliases;

		static Formater ()
		{
			m_aliases = new StringDictionary ();

			m_aliases.Add ("System.Void", "void");
			m_aliases.Add ("System.Object", "object");
			m_aliases.Add ("System.String", "string");
			m_aliases.Add ("System.Boolean", "bool");
			m_aliases.Add ("System.Char", "char");
			m_aliases.Add ("System.Single", "float32");
			m_aliases.Add ("System.Double", "float64");
			m_aliases.Add ("System.SByte", "int8");
			m_aliases.Add ("System.Byte", "unsigned int8");
			m_aliases.Add ("System.Int16", "int16");
			m_aliases.Add ("System.UInt16", "unsigned int16");
			m_aliases.Add ("System.Int32", "int32");
			m_aliases.Add ("System.UInt32", "unsigned int32");
			m_aliases.Add ("System.Int64", "long");
			m_aliases.Add ("System.UInt64", "unsigned long");
			m_aliases.Add ("System.IntPtr", "native int");
			m_aliases.Add ("Systen.UIntPtr", "unsigned native int");
			m_aliases.Add ("System.TypedReference", "typedreference");
		}

		public static string Escape (string name) // TODO
		{
			return name;
		}

		static string GetScope (TypeReference type)
		{
			if (type is TypeDefinition)
				return string.Empty;

			return string.Concat ("[", Escape (type.Scope.Name), "]");
		}

		static bool IsPrimitive (TypeReference type)
		{
			return m_aliases.ContainsKey (GetTypeName (type));
		}

		static string GetAlias (TypeReference type)
		{
			return m_aliases [GetTypeName (type)];
		}

		static string GetTypeName (TypeReference type)
		{
			if (type.Namespace.Length == 0)
				return Escape (type.Name);

			return string.Concat (Escape (type.Namespace), ".", Escape (type.Name));
		}

		public static string Format (TypeReference type)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetScope (type));

			if (type.DeclaringType != null) {
				sb.Append (GetTypeName (type.DeclaringType));
				sb.Append ("/");
				sb.Append (Escape (type.Name));
				return sb.ToString ();
			}

			sb.Append (GetTypeName (type));

			return sb.ToString ();
		}

		public static string Signature (TypeReference type)
		{
			StringBuilder sb = new StringBuilder ();

			if (type is PinnedType)
				sb.Append ("pinned ");

			if (IsPrimitive (type))
				sb.Append (GetAlias (type));
			else {
				sb.Append (type.IsValueType ? "valuetype" : "class");
				sb.Append (" ");
				sb.Append (Format (type));
			}

			if (type is ReferenceType)
				sb.Append ("&");
			else if (type is PointerType)
				sb.Append ("*");
			else if (type is ArrayType) {
				ArrayType ary = (ArrayType) type;
				if (ary.IsSizedArray)
					sb.Append ("[]");
				// TODO else
			}

			return sb.ToString ();
		}
	}
}
