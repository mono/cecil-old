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

    internal sealed class CodeReader : ICodeVisitor {

        public void Visit (IMethodBody body)
        {
        }

        public void Visit (IInstructionCollection instructions)
        {
        }

        public void Visit (IInstruction instr)
        {
        }

        public void Visit (IExceptionHandlerCollection seh)
        {
        }

        public void Visit (IExceptionHandler eh)
        {
        }

        public void Visit (IVariableDefinitionCollection variables)
        {
        }

        public void Visit (IVariableDefinition var)
        {
        }
    }
}
