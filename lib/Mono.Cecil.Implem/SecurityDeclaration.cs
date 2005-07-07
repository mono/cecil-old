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

	using System.Security;
	using System.Text;

	using Mono.Cecil;

	internal sealed class SecurityDeclaration : ISecurityDeclaration {

		private SecurityAction m_action;
		private PermissionSet m_permSet;

		public SecurityAction Action {
			get { return m_action; }
			set { m_action = value; }
		}

		public PermissionSet PermissionSet {
			get { return m_permSet; }
			set { m_permSet = value; }
		}

		public SecurityDeclaration (SecurityAction action)
		{
			m_action = action;
		}

		public byte [] GetAsByteArray ()
		{
			if (m_permSet != null)
				return Encoding.Unicode.GetBytes (m_permSet.ToXml ().ToString ());
			return new byte [0];
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

