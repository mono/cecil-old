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

namespace Mono.Cecil.Metadata {

    public interface IMetadataVisitable {
        void Accept (IMetadataVisitor visitor);
    }

    public interface IMetadataTableVisitable {
        void Accept (IMetadataTableVisitor visitor);
    }

    public interface IMetadataRowVisitable {
        void Accept (IMetadataRowVisitor visitor);
    }
}
