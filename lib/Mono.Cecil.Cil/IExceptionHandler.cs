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

namespace Mono.Cecil.Cil {

    using Mono.Cecil;

    public interface IExceptionHandler : ICodeVisitable {

        int TryOffset { get; set; }
        int TryLength { get; set; }

        int HandlerOffset { get; set; }
        int HandlerLength { get; set; }

        int FilterOffset { get; set; }

        ITypeReference CatchType { get; set; }
        ExceptionHandlerType Type { get; set; }
    }
}
