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
    using System.Reflection;

    internal sealed class ReflectionHelper {

        private ModuleDefinition m_module;

        public ReflectionHelper (ModuleDefinition module)
        {
            m_module = module;
        }

        public IAssemblyNameReference RegisterAssembly (Assembly asm)
        {
            throw new NotImplementedException ();
        }

        public ITypeReference RegisterType (Type t)
        {
            //TODO: check first if present in the type ref list
            RegisterAssembly (t.Assembly);
            throw new NotImplementedException ();
        }

        private IMethodReference RegisterMethodBase (MethodBase meth)
        {
            MethodReference methref = new MethodReference (meth.Name, RegisterType (meth.DeclaringType),
                                                           (meth.CallingConvention & CallingConventions.HasThis)
                                                                == CallingConventions.HasThis,
                                                           (meth.CallingConvention & CallingConventions.ExplicitThis)
                                                                == CallingConventions.ExplicitThis,
                                                           (MethodCallingConvention) meth.CallingConvention);
            //TODO: add method ref in a list of members refs in the ReflectionWriter
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
