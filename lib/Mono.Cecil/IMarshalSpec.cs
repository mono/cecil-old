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

    using System;

    public interface IMarshalSpec : IReflectionVisitable {

        NativeType NativeIntrinsic { get; }
        IHasMarshalSpec Container { get; }
    }

    public interface IArrayDesc : IMarshalSpec {

        NativeType ElemType { get; set; }
        int ParamNum { get; set; }
        int ElemMult { get; set; }
        int NumElem { get; set; }
    }

    public interface ICustomMarshalerDesc : IMarshalSpec {

        Guid Guid { get; set; }
        string UnmanagedType { get; set; }
        ITypeDefinition ManagedType { get; set; }
        string Cookie { get; set; }
    }

    public interface IFixedArrayDesc : IMarshalSpec {

        int NumElem { get; set; }
        NativeType ElemType { get; set; }
    }

    public interface ISafeArrayDesc : IMarshalSpec {

        VariantType ElemType { get; set; }
    }

    public interface IFixedSysStringDesc : IMarshalSpec {

        int Size { get; set; }
    }
}
