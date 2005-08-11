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

namespace Mono.Cecil.Signatures {

	using System.Collections;

	using Mono.Cecil.Implem;
	using Mono.Cecil.Metadata;

	internal sealed class SignatureWriter : ISignatureVisitor {

		private MetadataWriter m_mdWriter;
		private ReflectionWriter m_reflectWriter;

		private IDictionary m_sigCache;
		private IDictionary m_methCache;

		public SignatureWriter (MetadataWriter mdWriter, ReflectionWriter reflectWriter)
		{
			m_mdWriter = mdWriter;
			m_reflectWriter = reflectWriter;

			m_sigCache = new Hashtable ();
			m_methCache = new Hashtable ();
		}

		public void Visit (MethodDefSig methodDef)
		{
		}

		public void Visit (MethodRefSig methodRef)
		{
		}

		public void Visit (FieldSig field)
		{
		}

		public void Visit (PropertySig property)
		{
		}

		public void Visit (LocalVarSig localvar)
		{
		}
	}
}
