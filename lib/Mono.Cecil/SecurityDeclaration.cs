//
// SecurityDeclaration.cs
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
	using System.Security;
	using System.Text;

	public sealed class SecurityDeclaration : ISecurityDeclaration, ICloneable {

		SecurityAction m_action;
		PermissionSet m_permSet;

		bool m_readable;
		byte [] m_blob;

		public SecurityAction Action {
			get { return m_action; }
			set { m_action = value; }
		}

		public PermissionSet PermissionSet {
			get { return m_permSet; }
			set { m_permSet = value; }
		}

		public bool IsReadable {
			get { return m_readable; }
			set { m_readable = value; }
		}

		public byte [] Blob {
			get { return m_blob; }
			set { m_blob = value; }
		}

		public SecurityDeclaration (SecurityAction action)
		{
			m_action = action;
			m_readable = true;
		}

		object ICloneable.Clone ()
		{
			return this.Clone ();
		}

		public SecurityDeclaration Clone ()
		{
			return Clone (this);
		}

		internal static SecurityDeclaration Clone (SecurityDeclaration sec)
		{
			SecurityDeclaration sd = new SecurityDeclaration (sec.Action);
			if (!sec.IsReadable) {
				sd.IsReadable = false;
				sd.Blob = sec.Blob;
				return sd;
			}

			sd.PermissionSet = sec.PermissionSet.Copy ();
			return sd;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitSecurityDeclaration (this);
		}
	}
}

