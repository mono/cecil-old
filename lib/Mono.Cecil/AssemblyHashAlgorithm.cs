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

	public enum AssemblyHashAlgorithm : uint {
		None		= 0x0000,
		Reserved	= 0x8003,	// MD5
		SHA1		= 0x8004
	}
}

