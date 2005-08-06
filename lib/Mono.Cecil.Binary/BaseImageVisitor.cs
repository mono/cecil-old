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

	public abstract class BaseImageVisitor : IBinaryVisitor {

		public virtual void Visit (Image img)
		{
		}

		public virtual void Visit (DOSHeader header)
		{
		}

		public virtual void Visit (PEFileHeader header)
		{
		}

		public virtual void Visit (PEOptionalHeader header)
		{
		}

		public virtual void Visit (PEOptionalHeader.StandardFieldsHeader header)
		{
		}

		public virtual void Visit (PEOptionalHeader.NTSpecificFieldsHeader header)
		{
		}

		public virtual void Visit (PEOptionalHeader.DataDirectoriesHeader header)
		{
		}

		public virtual void Visit (SectionCollection coll)
		{
		}

		public virtual void Visit (Section section)
		{
		}

		public virtual void Visit (ImportAddressTable iat)
		{
		}

		public virtual void Visit (CLIHeader header)
		{
		}

		public virtual void Visit (ImportTable it)
		{
		}

		public virtual void Visit (ImportLookupTable ilt)
		{
		}

		public virtual void Visit (HintNameTable hnt)
		{
		}

		public virtual void Terminate (Image img)
		{
		}
	}
}
