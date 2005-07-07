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

	[Flags]
	public enum ParamAttributes : ushort {
		In					= 0x0001,	// Param is [In]
		Out					= 0x0002,	// Param is [Out]
		Optional			= 0x0004,	// Param is optional
		HasDefault			= 0x1000,	// Param has default value
		HasFieldMarshal		= 0x2000,	// Param has field marshal
		Unused				= 0xcfe0	 // Reserved: shall be zero in a conforming implementation
	}
}
