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
	using Mono.Cecil.Metadata;

	internal sealed class AssemblyLinkedResource : Resource, IAssemblyLinkedResource {

		private IAssemblyNameReference m_asmRef;

		public IAssemblyNameReference Assembly {
			get { return m_asmRef; }
			set { m_asmRef = value; }
		}

		public AssemblyLinkedResource (string name, ManifestResourceAttributes flags,
			AssemblyNameReference asmRef) : base (name, flags)
		{
			m_asmRef = asmRef;
		}

		public override void Accept (IReflectionStructureVisitor visitor)
		{
			visitor.VisitAssemblyLinkedResource (this);
		}
	}
}
