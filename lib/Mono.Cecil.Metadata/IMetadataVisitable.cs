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
