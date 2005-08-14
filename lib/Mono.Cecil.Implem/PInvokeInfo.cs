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

	using Mono.Cecil;

	internal sealed class PInvokeInfo : IPInvokeInfo {

		private MethodDefinition m_meth;

		private PInvokeAttributes m_attributes;
		private string m_entryPoint;
		private IModuleReference m_module;

		public IMethodDefinition Method {
			get { return m_meth; }
		}

		public PInvokeAttributes Attributes {
			get { return m_attributes; }
			set { m_attributes = value; }
		}

		public string EntryPoint {
			get { return m_entryPoint; }
			set { m_entryPoint = value; }
		}

		public IModuleReference Module {
			get { return m_module; }
			set { m_module = value; }
		}

		public PInvokeInfo (MethodDefinition meth)
		{
			m_meth = meth;
		}

		public PInvokeInfo (MethodDefinition meth, PInvokeAttributes attrs, string entryPoint, IModuleReference mod) : this (meth)
		{
			m_attributes = attrs;
			m_entryPoint = entryPoint;
			m_module = mod;
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitPInvokeInfo (this);
		}
	}
}
