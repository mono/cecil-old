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

    internal sealed class PInvokeInfo : IPInvokeInfo {

        private PInvokeAttributes m_attributes;
        private string m_entryPoint;
        private IModuleReference m_module;

        public PInvokeAttributes Attributes {
            get { return m_attributes; }
            set { m_attributes = value; }
        }

        public string EntryPoint {
            get { return m_entryPoint; }
            set { m_entryPoint = value; }
        }

        public IModuleReference Module {
            get { return m_module; }
            set { m_module = value; }
        }

        public PInvokeInfo (PInvokeAttributes attrs, string entryPoint, IModuleReference mod)
        {
            m_attributes = attrs;
            m_entryPoint = entryPoint;
            m_module = mod;
        }
    }
}
