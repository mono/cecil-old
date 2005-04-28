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

    using System;

    public interface IMarshalSpec {

        NativeType NativeIntrinsic { get; }
        bool HasMoreData { get; }

        IExtendedMarshalSpec ExtendedData { get; }
    }

    public interface IExtendedMarshalSpec {
    }

    public interface IArrayDesc : IExtendedMarshalSpec {

        NativeType ElemType { get; set; }
        int ParamNum { get; set; }
        int ElemMult { get; set; }
        int NumElem { get; set; }
    }

    public interface ICustomMarshalerDesc : IExtendedMarshalSpec {

        Guid Guid { get; set; }
        string UnmanagedType { get; set; }
        ITypeDefinition ManagedType { get; set; }
        string Cookie { get; set; }
    }

    public interface IFixedArrayDesc : IExtendedMarshalSpec {

        int NumElem { get; set; }
        NativeType ElemType { get; set; }
    }

    public interface ISafeArrayDesc : IExtendedMarshalSpec {

        VariantType ElemType { get; set; }
    }
}

