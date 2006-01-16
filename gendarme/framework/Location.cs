/*
 * Location.cs: an encapsulation of several pieces of information used
 * to identify a location in an assembly.
 *
 * Authors:
 *   Aaron Tomb <atomb@soe.ucsc.edu>
 *
 * Copyright (c) 2005 Aaron Tomb and the contributors listed
 * in the ChangeLog.
 *
 * This is free software, distributed under the MIT/X11 license.
 * See the included LICENSE.MIT file for details.
 **********************************************************************/

namespace Gendarme.Framework {

public class Location {
    private string type;
    private string method;
    private int offset; /* Offset of instruction into method */

    public Location(string type, string method, int offset)
    {
        this.type = type;
        this.method = method;
        this.offset = offset;
    }

    public override string ToString()
    {
        string result = "";
        if(type != null)
            result += type;
        if(method != null) {
            if(type != null)
                result += "::";
            result += method;
        }
        if(offset >= 0) {
            if(result.Length != 0)
                result += ":";
            result += offset.ToString("x4");
        }
        return result;
    }
}

}
