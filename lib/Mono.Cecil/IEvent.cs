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

    public interface IEventReference : IMemberReference, IReflectionVisitable {

        ITypeReference EventType { get; set; }
    }

    public interface IEventDefinition : IMemberDefinition, IEventReference, IReflectionVisitable {

        EventAttributes Attributes { get; set; }

        IMethodDefinition AddMethod { get; set; }
        IMethodDefinition InvokeMethod { get; set; }
        IMethodDefinition RemoveMethod { get; set; }
    }
}
