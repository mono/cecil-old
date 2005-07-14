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

	public sealed class ImportAddressTable : IBinaryVisitable {

		public RVA HintNameTableRVA;

		internal ImportAddressTable ()
		{
		}

		public void Accept (IBinaryVisitor visitor)
		{
			visitor.Visit (this);
		}
	}

	public sealed class ImportTable : IBinaryVisitable {

		public RVA ImportLookupTable;
		public uint DateTimeStamp;
		public uint ForwardChain;
		public RVA Name;
		public RVA ImportAddressTable;

		internal ImportTable ()
		{
		}

		public void Accept (IBinaryVisitor visitor)
		{
			visitor.Visit (this);
		}
	}

	public sealed class ImportLookupTable : IBinaryVisitable {

		public RVA HintNameRVA;

		internal ImportLookupTable ()
		{
		}

		public void Accept (IBinaryVisitor visitor)
		{
			visitor.Visit (this);
		}
	}

	public sealed class HintNameTable : IBinaryVisitable {

		public ushort Hint;
		public string RuntimeMain;
		public string RuntimeLibrary;
		public ushort EntryPoint;

		internal HintNameTable ()
		{
		}

		public void Accept (IBinaryVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}
