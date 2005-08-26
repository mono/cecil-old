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

	internal sealed class EmbeddedResource : Resource, IEmbeddedResource {

		private byte [] m_data;

		public byte [] Data {
			get { return m_data; }
			set { m_data = value; }
		}

		public EmbeddedResource (string name, ManifestResourceAttributes flags) :
			base (name, flags)
		{
		}

		public EmbeddedResource (string name, ManifestResourceAttributes flags, byte [] data) :
			base (name, flags)
		{
			m_data = data;
		}

		public override void Accept (IReflectionStructureVisitor visitor)
		{
			visitor.VisitEmbeddedResource (this);
		}
	}
}
