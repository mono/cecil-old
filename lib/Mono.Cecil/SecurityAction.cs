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

namespace Mono.Cecil {

    public enum SecurityAction : short {
        Request = 1,
        Demand = 2,
        Assert = 3,
        Deny = 4,
        PermitOnly = 5,
        LinkDemand = 6,
        InheritDemand = 7,
        RequestMinimum = 8,
        RequestOptional = 9,
        RequestRefuse = 10,
        PreJitGrant = 11,
        PreJitDeny = 12,
        NonCasDemand = 13,
        NonCasLinkDemand = 14,
        NonCasInheritance = 15,
        LinkDemandChoice = 16,
        InheritDemandChoice = 17,
        DemandChoice = 18
    }
}
