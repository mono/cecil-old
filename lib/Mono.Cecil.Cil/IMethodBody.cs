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

    public interface IMethodBody : ICodeVisitable {
        int MaxStack { get; set; }
        IMethodDefinition Method { get; }
        IInstructionCollection Instructions { get; }
        IExceptionHandlerCollection ExceptionHandlers { get; }
        IVariableDefinitionCollection Variables { get; }
    }
}
