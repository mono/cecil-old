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

namespace Mono.Cecil {

    public interface IMethod : IMemberReference, IReflectionVisitable {

        MethodAttributes Attributes { get; set; }
        MethodImplAttributes ImplAttributes { get; set; }
        MethodSemanticsAttributes SemanticsAttributes { get; set; }
    }

    public interface IMethodReference : IMethod, IReflectionVisitable {

    }

    public interface IMethodDefinition : IMemberDefinition, IMethodReference, IReflectionVisitable {

        IOverrideCollection Overrides { get; }
    }
}
