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

    using Mono.Cecil;

    internal sealed class ModuleReference : IModuleReference {

        private string m_name;

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public ModuleReference(string name) {
            m_name = name;
        }

        public void Accept(IReflectionStructureVisitor visitor) {
            visitor.Visit(this);
        }
    }
}

