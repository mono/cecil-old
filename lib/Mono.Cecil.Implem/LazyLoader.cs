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

    using Mono.Cecil.Cil;

    internal sealed class LazyLoader {

        private ReflectionReader m_reflectReader;
        private CodeReader m_codeReader;

        public IReflectionVisitor ReflectionReader {
            get { return m_reflectReader; }
        }

        public IDetailReader DetailReader {
            get { return m_reflectReader; }
        }

        public ICodeVisitor CodeReader {
            get { return m_codeReader; }
        }

        public LazyLoader (ModuleDefinition module, LoadingType lt)
        {
            if (lt == LoadingType.Aggressive)
                m_reflectReader = new AggressiveReflectionReader (module);
            else if (lt == LoadingType.Lazy)
                m_reflectReader = new LazyReflectionReader (module);
            else
                throw new ReflectionException ("Unknow loading type");
            m_codeReader = new CodeReader (m_reflectReader);
        }
    }
}
