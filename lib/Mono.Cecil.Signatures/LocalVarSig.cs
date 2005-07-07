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

	internal sealed class LocalVarSig : Signature {

		public bool Local;
		public int Count;
		public LocalVariable [] LocalVariables;

		public LocalVarSig () : base ()
		{
		}

		public LocalVarSig (uint blobIndex) : base (blobIndex)
		{
		}

		public override void Accept (ISignatureVisitor visitor)
		{
			visitor.Visit (this);
		}

		public struct LocalVariable {

			public Constraint Constraint;
			public bool ByRef;
			public SigType Type;
		}
	}
}
