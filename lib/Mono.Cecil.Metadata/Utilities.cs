/*
* Copyright (c) 2004 DotNetGuru and the individuals listed
* on the ChangeLog entries.
*
* Authors :
*   Jb Evain   (jb.evain@dotnetguru.org)
*
* This is a free software distributed under a MIT/X11 license
* See LICENSE.MIT file for more details
*
*****************************************************************************/

namespace Mono.Cecil.Metadata {

    using System;

    internal sealed class Utilities {

        private Utilities ()
        {
        }

        public static int ReadCompressedInteger (byte [] data, int pos, out int start)
        {
            int integer = 0;
            start = pos;
            if ((data [pos] & 0x80) == 0) {
                integer = data [pos];
                start++;
            } else if ((data [pos] & 0x40) == 0) {
                integer = (data [start] & ~0x80) << 8;
                integer |= data [pos + 1];
                start += 2;
            } else {
                integer = (data [start] & ~0xc0) << 24;
                integer |= data [pos + 1] << 16;
                integer |= data [pos + 2] << 8;
                integer |= data [pos + 3];
                start += 4;
            }
            return integer;
        }
    }
}
