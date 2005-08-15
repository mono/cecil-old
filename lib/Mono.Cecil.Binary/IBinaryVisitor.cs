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

	public interface IBinaryVisitor {
		void VisitImage (Image img);
		void VisitDOSHeader (DOSHeader header);
		void VisitPEFileHeader (PEFileHeader header);
		void VisitPEOptionalHeader (PEOptionalHeader header);
		void VisitStandardFieldsHeader (PEOptionalHeader.StandardFieldsHeader header);
		void VisitNTSpecificFieldsHeader (PEOptionalHeader.NTSpecificFieldsHeader header);
		void VisitDataDirectoriesHeader (PEOptionalHeader.DataDirectoriesHeader header);
		void VisitSectionCollection (SectionCollection coll);
		void VisitSection (Section section);
		void VisitImportAddressTable (ImportAddressTable iat);
		void VisitCLIHeader (CLIHeader header);
		void VisitImportTable (ImportTable it);
		void VisitImportLookupTable (ImportLookupTable ilt);
		void VisitHintNameTable (HintNameTable hnt);

		void TerminateImage (Image img);
	}
}
