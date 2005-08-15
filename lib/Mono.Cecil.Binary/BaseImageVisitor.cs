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

		public virtual void VisitImage (Image img)
		{
		}

		public virtual void VisitDOSHeader (DOSHeader header)
		{
		}

		public virtual void VisitPEFileHeader (PEFileHeader header)
		{
		}

		public virtual void VisitPEOptionalHeader (PEOptionalHeader header)
		{
		}

		public virtual void VisitStandardFieldsHeader (PEOptionalHeader.StandardFieldsHeader header)
		{
		}

		public virtual void VisitNTSpecificFieldsHeader (PEOptionalHeader.NTSpecificFieldsHeader header)
		{
		}

		public virtual void VisitDataDirectoriesHeader (PEOptionalHeader.DataDirectoriesHeader header)
		{
		}

		public virtual void VisitSectionCollection (SectionCollection coll)
		{
		}

		public virtual void VisitSection (Section section)
		{
		}

		public virtual void VisitImportAddressTable (ImportAddressTable iat)
		{
		}

		public virtual void VisitCLIHeader (CLIHeader header)
		{
		}

		public virtual void VisitImportTable (ImportTable it)
		{
		}

		public virtual void VisitImportLookupTable (ImportLookupTable ilt)
		{
		}

		public virtual void VisitHintNameTable (HintNameTable hnt)
		{
		}

		public virtual void TerminateImage (Image img)
		{
		}
	}
}
