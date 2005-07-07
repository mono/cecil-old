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

	using Mono.Cecil;

	internal sealed class ArrayDimension : IArrayDimension {

		private int m_lowerBound;
		private int m_upperBound;

		public int LowerBound {
			get { return m_lowerBound; }
			set { m_lowerBound = value; }
		}

		public int UpperBound {
			get { return m_upperBound; }
			set { m_upperBound = value; }
		}

		public ArrayDimension (int lb, int ub)
		{
			m_lowerBound = lb;
			m_upperBound = ub;
		}

		public override string ToString ()
		{
			if (m_lowerBound == 0 && m_upperBound == 0)
				return string.Empty;
			return string.Concat (m_lowerBound, "...", m_upperBound);
		}
	}
}
