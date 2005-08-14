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

	using Mono.Cecil;

	internal class MarshalSig {

		public NativeType NativeInstrinsic;
		public IMarshalSigSpec Spec;

		public MarshalSig (NativeType nt)
		{
			this.NativeInstrinsic = nt;
		}

		internal interface IMarshalSigSpec {
		}

		internal class Array : IMarshalSigSpec {

			public NativeType ArrayElemType;
			public int ParamNum;
			public int ElemMult;
			public int NumElem;

			public Array ()
			{
				this.ParamNum = -1;
				this.ElemMult = -1;
				this.NumElem = -1;
			}
		}

		internal class CustomMarshaler : IMarshalSigSpec {

			public string Guid;
			public string UnmanagedType;
			public string ManagedType;
			public string Cookie;
		}

		internal class FixedArray : IMarshalSigSpec {

			public int NumElem;
			public NativeType ArrayElemType;

			public FixedArray ()
			{
				this.NumElem = -1;
				this.ArrayElemType = NativeType.NONE;
			}
		}

		internal class SafeArray : IMarshalSigSpec {

			public VariantType ArrayElemType;
		}

		internal class FixedSysString : IMarshalSigSpec {

			public int Size;
		}
	}
}
