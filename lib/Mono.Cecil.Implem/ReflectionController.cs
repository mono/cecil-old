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

namespace Mono.Cecil.Implem {

    using System;

    using Mono.Cecil;

    internal sealed class ReflectionController {

        private ReflectionReader m_reader;
        private ReflectionWriter m_writer;
        private ReflectionHelper m_helper;

        public ReflectionReader Reader {
            get { return m_reader; }
        }

        public ReflectionWriter Writer {
            get { return m_writer; }
        }

        public ReflectionHelper Helper {
            get { return m_helper; }
        }

        public ReflectionController (ModuleDefinition module, LoadingType lt)
        {
            if (lt == LoadingType.Aggressive)
                m_reader = new AggressiveReflectionReader (module);
            else if (lt == LoadingType.Lazy)
                m_reader = new LazyReflectionReader (module);
            else
                throw new ReflectionException ("Unknow loading type");

            m_writer = new ReflectionWriter ();
            m_helper = new ReflectionHelper (module);
        }
    }
}
