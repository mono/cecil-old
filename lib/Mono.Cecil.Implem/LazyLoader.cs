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

namespace Mono.Cecil.Implem {

    using System;
    using System.Collections;

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Metadata;

    internal sealed class LazyLoader {

        private ReflectionReader m_reflectReader;

        public IReflectionVisitor ReflectionReader {
            get { return m_reflectReader; }
        }

        //TODO: here will goes a code reader

        public LazyLoader (ImageReader reader)
        {
            m_reflectReader = new ReflectionReader (reader);
        }
    }
}
