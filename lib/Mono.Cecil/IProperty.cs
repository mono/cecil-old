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

namespace Mono.Cecil {

    public interface IPropertyReference : IMemberReference, IReflectionVisitable {

        ITypeReference PropertyType { get; set; }
        IParameterDefinitionCollection Parameters { get; }
    }

    public interface IPropertyDefinition : IMemberDefinition, IPropertyReference, ICustomAttributeProvider, IReflectionVisitable {

        PropertyAttributes Attributes { get; set; }

        IMethodDefinition GetMethod { get; set; }
        IMethodDefinition SetMethod { get; set; }

        object Constant { get; set; }
    }
}
