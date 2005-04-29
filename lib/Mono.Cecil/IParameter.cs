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

    public interface IParameterReference : IReflectionVisitable {

        string Name { get; set; }
        int Sequence { get; }
        ParamAttributes Attributes { get; set; }
        ITypeReference ParameterType { get; set; }
        object Constant { get; set; }
    }

    public interface IParameterDefinition : IParameterReference, IHasMarshalSpec, ICustomAttributeProvider {
    }
}
