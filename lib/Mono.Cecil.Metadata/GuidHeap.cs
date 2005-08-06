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

	public class GuidHeap : MetadataHeap {

		private readonly IDictionary m_guids;

		public IDictionary Guids {
			get { return m_guids; }
		}

		public GuidHeap (MetadataStream stream) : base (stream, "#GUID")
		{
			m_guids = new Hashtable ();
		}

		public Guid this [uint index] {
			get {
				if (index == 0)
					return new Guid (new byte [16]);

				int idx = (int) index - 1;

				if (m_guids.Contains (idx))
					return (Guid) m_guids [idx];

				if (idx + 16 > this.Data.Length)
					throw new IndexOutOfRangeException ();

				byte[] buffer = new byte [16];
				Buffer.BlockCopy (this.Data, idx, buffer, 0, 16);
				Guid res = new Guid (buffer);
				m_guids [idx] = res;
				return res;
			}
			set { m_guids [index] = value; }
		}

		public override void Accept (IMetadataVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
