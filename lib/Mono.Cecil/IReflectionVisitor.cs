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

    public interface IReflectionVisitor {

        void Visit (ITypeDefinitionCollection types);
        void Visit (ITypeDefinition type);
        void Visit (ITypeReferenceCollection refs);
        void Visit (ITypeReference type);
        void Visit (IInterfaceCollection interfaces);
        void Visit (IOverrideCollection meth);
        void Visit (IParameterDefinitionCollection parameters);
        void Visit (IParameterDefinition parameter);
        void Visit (IMethodDefinitionCollection methods);
        void Visit (IMethodDefinition method);
        void Visit (IEventDefinitionCollection events);
        void Visit (IEventDefinition evt);
        void Visit (IFieldDefinitionCollection fields);
        void Visit (IFieldDefinition field);
        void Visit (IPropertyDefinitionCollection properties);
        void Visit (IPropertyDefinition property);
    }
}
