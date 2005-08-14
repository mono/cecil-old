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

namespace Mono.Cecil.Signatures {

	using System;

	internal abstract class Signature : ISignatureVisitable {

		public byte CallingConvention;
		public uint BlobIndex;

		public Signature (uint blobIndex)
		{
			BlobIndex = blobIndex;
		}

		public Signature ()
		{
			BlobIndex = 0;
		}

		public abstract void Accept (ISignatureVisitor visitor);
	}
}
