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
    using System.Reflection;

    internal sealed class ReflectionHelper {

        private ModuleDefinition m_module;

        public ReflectionHelper (ModuleDefinition module)
        {
            m_module = module;
        }

        public IAssemblyNameReference RegisterAssembly (Assembly asm)
        {
            foreach (IAssemblyNameReference ext in m_module.AssemblyReferences)
                if (ext.Name == asm.GetName ().Name)
                    return ext;

            AssemblyName asmName = asm.GetName ();
            AssemblyNameReference asmRef = new AssemblyNameReference (asmName.Name, asmName.CultureInfo.Name, asmName.Version);
            (m_module.AssemblyReferences as AssemblyNameReferenceCollection).Add (asmRef);
            return asmRef;
        }

        public ITypeReference RegisterType (Type t)
        {
            RegisterAssembly (t.Assembly);
            throw new NotImplementedException ();
        }

        private IMethodReference RegisterMethodBase (MethodBase meth)
        {
            throw new NotImplementedException ();
        }

        public IMethodReference RegisterConstructor (ConstructorInfo ctor)
        {
            return RegisterMethodBase (ctor);
        }

        public IMethodReference RegisterMethod (MethodInfo meth)
        {
            return RegisterMethodBase (meth);
        }

        public IFieldReference RegisterField (FieldInfo field)
        {
            throw new NotImplementedException ();
        }
    }
}
