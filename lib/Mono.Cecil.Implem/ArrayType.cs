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

    using System;

    using Mono.Cecil;
    using Mono.Cecil.Signatures;

    internal sealed class ArrayType : TypeReference, IArrayType {

        private ITypeReference m_elementsType;
        private ArrayDimensionCollection m_dimensions;

        public ITypeReference ElementsType {
            get { return m_elementsType; }
            set { m_elementsType = value; }
        }

        public IArrayDimensionCollection Dimensions {
            get { return m_dimensions; }
        }

        public override string Name {
            get { return m_elementsType.Name; }
            set { m_elementsType.Name = value; }
        }

        public override string Namespace {
            get { return m_elementsType.Namespace; }
            set { m_elementsType.Namespace = value; }
        }

        public override string FullName {
            get {
                string fname = string.Concat (base.FullName, "[");
                foreach (ArrayDimension dim in m_dimensions) {
                    string rank = dim.ToString ();
                    if (rank.Length == 0)
                        fname = string.Concat (fname, ",");
                    else
                        fname = string.Concat (fname, ", ", rank);
                }
                return string.Concat (fname, "]");
            }
        }

        public ArrayType (ITypeReference elementsType, ArrayShape shape) : this (elementsType)
        {
            for (int i = 0; i < shape.Rank; i++) {
                if (i < shape.NumSizes)
                    m_dimensions.Add (new ArrayDimension (shape.LoBounds [i], shape.LoBounds [i] + shape.Sizes [i] - 1));
                else
                    m_dimensions.Add (new ArrayDimension (0, 0));
            }
        }

        public ArrayType (ITypeReference elementsType) : base (elementsType.Name, elementsType.Namespace)
        {
            m_elementsType = elementsType;
            m_dimensions = new ArrayDimensionCollection (this);
        }
    }
}
