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

namespace Mono.Cecil {

	using System;

	public sealed class MetadataFormatException : ImageFormatException {

		internal MetadataFormatException () : base ()
		{
		}

		internal MetadataFormatException (string message) : base (message)
		{
		}

		internal MetadataFormatException (string message, params string [] parameters) : base (string.Format (message, parameters))
		{
		}

		internal MetadataFormatException (string message, Exception inner) : base (message, inner)
		{
		}
	}
}
