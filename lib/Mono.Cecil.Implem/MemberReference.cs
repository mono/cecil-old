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

	using Mono.Cecil;
	using Mono.Cecil.Metadata;

	internal abstract class MemberReference : IMemberReference {

		private string m_name;
		private ITypeReference m_decType;
		private MetadataToken m_token;

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public ITypeReference DeclaringType {
			get { return m_decType; }
		}

		public MetadataToken MetadataToken {
			get { return m_token; }
			set { m_token = value; }
		}

		public MemberReference (string name, ITypeReference declaringType)
		{
			m_name = name;
			m_decType = declaringType;
		}

		public override string ToString ()
		{
			return string.Concat (m_decType.FullName, "::", m_name);
		}
	}
}
