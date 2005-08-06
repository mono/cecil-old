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

namespace Mono.Cecil.Metadata {

	public abstract class BaseMetadataVisitor : IMetadataVisitor {

		public virtual void Visit (MetadataRoot root)
		{
		}

		public virtual void Visit (MetadataRoot.MetadataRootHeader header)
		{
		}

		public virtual void Visit (MetadataStreamCollection streams)
		{
		}

		public virtual void Visit (MetadataStream stream)
		{
		}

		public virtual void Visit (MetadataStream.MetadataStreamHeader header)
		{
		}

		public virtual void Visit (GuidHeap heap)
		{
		}

		public virtual void Visit (StringsHeap heap)
		{
		}

		public virtual void Visit (TablesHeap heap)
		{
		}

		public virtual void Visit (BlobHeap heap)
		{
		}

		public virtual void Visit (UserStringsHeap heap)
		{
		}

		public virtual void Terminate (MetadataStreamCollection streams)
		{
		}

		public virtual void Terminate (MetadataRoot root)
		{
		}
	}
}
