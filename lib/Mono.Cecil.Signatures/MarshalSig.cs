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
		public ValueType Spec;

		public MarshalSig (NativeType nt)
		{
			this.NativeInstrinsic = nt;
		}

		internal struct Array {

			public NativeType ArrayElemType;
			public int ParamNum;
			public int ElemMult;
			public int NumElem;
		}

		internal struct CustomMarshaler {

			public string Guid;
			public string UnmanagedType;
			public string ManagedType;
			public string Cookie;
		}

		internal struct FixedArray {

			public int NumElem;
			public NativeType ArrayElemType;
		}

		internal struct SafeArray {

			public VariantType ArrayElemType;
		}

		internal struct FixedSysString {

			public int Size;
		}
	}
}
