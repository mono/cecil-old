/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Binary {

    public interface IBinaryVisitor {
        void Visit (Image img);
        void Visit (DOSHeader header);
        void Visit (PEFileHeader header);
        void Visit (PEOptionalHeader header);
        void Visit (PEOptionalHeader.StandardFieldsHeader header);
        void Visit (PEOptionalHeader.NTSpecificFieldsHeader header);
        void Visit (PEOptionalHeader.DataDirectoriesHeader header);
        void Visit (SectionCollection coll);
        void Visit (Section section);
        void Visit (CLIHeader header);

        void Terminate (Image img);
    }
}
