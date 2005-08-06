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

namespace Mono.Cecil.Metadata {

	using System.Collections;

	public class StringsHeap : MetadataHeap {

		private IDictionary m_strings;

		public string this [uint index] {
			get { return m_strings.Contains (index) ? m_strings [index] as string : string.Empty; }
			set { m_strings [index] = value; }
		}

		internal StringsHeap (MetadataStream stream) : base (stream, MetadataStream.Strings)
		{
			m_strings = new Hashtable ();
		}

		public override void Accept (IMetadataVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
