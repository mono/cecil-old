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

	using System;
	using System.Collections;
	using System.Text;

	public class UserStringsHeap : MetadataHeap {

		private readonly IDictionary m_strings;

		public string this [uint offset] {
			get {
				string us = m_strings [offset] as string;
				if (us == null) {
					us = ReadStringAt ((int) offset);
					if (us != null && us.Length != 0)
						m_strings [offset] = us;
				}
				return us;
			}
			set { m_strings [offset] = value; }
		}

		internal UserStringsHeap (MetadataStream stream) : base(stream, MetadataStream.UserStrings)
		{
			m_strings = new Hashtable ();
		}

		private string ReadStringAt (int offset)
		{
			int length = Utilities.ReadCompressedInteger (this.Data, offset, out offset);
			if (length == 0)
				return string.Empty;

			return Encoding.Unicode.GetString (this.Data, offset, length);
		}

		public override void Accept (IMetadataVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
