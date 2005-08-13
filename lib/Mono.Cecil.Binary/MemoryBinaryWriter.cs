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

namespace Mono.Cecil.Binary {

	using System.IO;
	using System.Text;

	internal sealed class MemoryBinaryWriter : BinaryWriter {

		public MemoryStream MemoryStream {
			get { return this.BaseStream as MemoryStream; }
		}

		public MemoryBinaryWriter () : base (new MemoryStream ())
		{
		}

		public MemoryBinaryWriter (Encoding enc) : base (new MemoryStream (), enc)
		{
		}

		public void Empty ()
		{
			this.BaseStream.Position = 0;
			this.BaseStream.SetLength (0);
		}

		public void Write (MemoryBinaryWriter writer)
		{
			Write (writer.ToArray ());
		}

		public byte [] ToArray ()
		{
			return this.MemoryStream.ToArray ();
		}

		public void QuadAlign ()
		{
			this.BaseStream.Position += 3;
			this.BaseStream.Position &= ~3;
		}
	}
}
