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

namespace Mono.Cecil.Signatures {

    using Mono.Cecil;

    internal class CustomAttrib {

        public const ushort StdProlog = 0x0001;

        public IMethodReference Constructor;

        public ushort Prolog;
        public FixedArg [] FixedArgs;
        public ushort NumNamed;
        public NamedArg [] NamedArgs;

        public CustomAttrib (IMethodReference ctor)
        {
            Constructor = ctor;
        }

        internal struct FixedArg {

            public bool SzArray;
            public uint NumElem;
            public Elem [] Elems;
        }

        internal struct Elem {

            public bool Simple;
            public bool String;
            public bool Type;
            public bool BoxedValueType;

            public ElementType FieldOrPropType;
            public object Value;

            public ITypeReference ElemType;
        }

        internal struct NamedArg {

            public bool Field;
            public bool Property;

            public ElementType FieldOrPropType;
            public string FieldOrPropName;
            public FixedArg FixedArg;
        }
    }
}
