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

namespace Mono.Cecil.Implem {

    using System;
    using System.Text;

    using Mono.Cecil;

    internal sealed class Utilities {

        public static string TypeFullName (ITypeReference t)
        {
            if (t.DeclaringType != null)
                return string.Concat (t.DeclaringType.FullName, "/", t.Name);

            if (t.Namespace == null || t.Namespace.Length == 0)
                return t.Name;

            return string.Concat (t.Namespace, ".", t.Name);
        }

        private static string MemberSignature (IMemberReference member)
        {
            return string.Concat (TypeFullName(member.DeclaringType), "::", member.Name);
        }

        public static string FieldSignature (IFieldReference f)
        {
            return Utilities.MemberSignature (f);
        }

        public static string MethodSignature (IMethodReference meth)
        {
            StringBuilder sb = new StringBuilder ();
            sb.Append (MemberSignature (meth));
            sb.Append ("(");
            for (int i = 0; i < meth.Parameters.Count; i++) {
                sb.Append (TypeFullName(meth.Parameters [i].ParameterType));
                if (i < meth.Parameters.Count - 1)
                    sb.Append (",");
            }
            sb.Append (")");
        }
    }
}
