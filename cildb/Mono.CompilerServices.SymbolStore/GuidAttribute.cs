//
// GuidAttribute.cs
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

namespace Mono.CompilerServices.SymbolStore {

	using System;
	using System.Reflection;

	[AttributeUsage (AttributeTargets.Field)]
	class GuidAttribute : Attribute {

		private Guid m_guid;

		public Guid Guid {
			get { return m_guid; }
		}

		public GuidAttribute (
			uint a,
			ushort b,
			ushort c,
			byte d,
			byte e,
			byte f,
			byte g,
			byte h,
			byte i,
			byte j,
			byte k)
		{
			m_guid = new Guid (a, b, c, d, e, f, g, h, i, j, k);
		}

		static bool CheckField (FieldInfo fi, Guid id)
		{
			GuidAttribute [] attributes = fi.GetCustomAttributes (typeof (GuidAttribute), false) as GuidAttribute [];
			if (attributes == null || attributes.Length == 0)
				return false;

			foreach (GuidAttribute attr in attributes)
				if (attr.Guid == id)
					return true;

			return false;
		}

		public static int GetValueFromGuid (Guid id, Type enumeration)
		{
			foreach (FieldInfo fi in enumeration.GetFields ())
				if (CheckField (fi, id))
					return (int) fi.GetValue (null);

			return -1;
		}
	}
}
