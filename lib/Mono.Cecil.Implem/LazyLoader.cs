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
    using Mono.Cecil.Metadata;

    internal sealed class LazyLoader {

        public static readonly LazyLoader Instance = new LazyLoader ();

        private ReflectionBasisReader m_basisReader;

        public IReflectionVisitor BasisReader {
            get { return m_basisReader; }
        }

        public IReflectionVisitor CompleteReader {
            get { throw new NotImplementedException (); }
        }

        private LazyLoader ()
        {
            m_basisReader = new ReflectionBasisReader ();
        }

        public void LazyLoadByName (IReflectionVisitable target, string name)
        {
            //TODO: implement lazy loading of a visitable by name
            throw new NotImplementedException ();
        }

        public int GetCount (ILazyLoadableCollection coll)
        {
            if (coll.Loaded)
                return coll.Count;
            //TODO: implement get count from ILazyLoadableCollection
            throw new NotImplementedException ();
        }
    }
}
