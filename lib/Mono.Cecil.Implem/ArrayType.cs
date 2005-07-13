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

namespace Mono.Cecil.Implem {

	using System;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Signatures;

	internal sealed class ArrayType : TypeReference, IArrayType {

		private ITypeReference m_elementsType;
		private ArrayDimensionCollection m_dimensions;

		public ITypeReference ElementType {
			get { return m_elementsType; }
			set { m_elementsType = value; }
		}

		public IArrayDimensionCollection Dimensions {
			get { return m_dimensions; }
		}

		public int Rank {
			get { return m_dimensions.Count; }
		}

		public override string Name {
			get { return m_elementsType.Name; }
			set { m_elementsType.Name = value; }
		}

		public override string Namespace {
			get { return m_elementsType.Namespace; }
			set { m_elementsType.Namespace = value; }
		}

		public override IMetadataScope Scope {
			get { return m_elementsType.Scope; }
		}

		public override string FullName {
			get {
				StringBuilder sb = new StringBuilder ();
				sb.Append (base.FullName);
				sb.Append ("[");
				for (int i = 0; i < m_dimensions.Count; i++) {
					IArrayDimension dim = m_dimensions [i];
					string rank = dim.ToString ();
					if (i < m_dimensions.Count - 1)
						sb.Append (",");
					if (rank.Length > 0) {
						sb.Append (" ");
						sb.Append (rank);
					}
				}
				sb.Append ("]");
				return sb.ToString ();
			}
		}

		public ArrayType (ITypeReference elementsType, ArrayShape shape) : this (elementsType)
		{
			for (int i = 0; i < shape.Rank; i++) {
				if (i < shape.NumSizes)
					DefineDimension (shape.LoBounds [i], shape.LoBounds [i] + shape.Sizes [i] - 1);
				else
					DefineDimension ();
			}
		}

		public ArrayType (ITypeReference elementsType) : base (elementsType.Name, elementsType.Namespace)
		{
			m_elementsType = elementsType;
			m_dimensions = new ArrayDimensionCollection (this);
		}

		public IArrayDimension DefineDimension ()
		{
			return DefineDimension (0, 0);
		}

		public IArrayDimension DefineDimension (int lowerBound, int upperBound)
		{
			ArrayDimension dim = new ArrayDimension (lowerBound, upperBound);
			m_dimensions.Add (dim);
			return dim;
		}
	}
}
