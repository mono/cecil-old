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
            return string.Concat (meth.Name, ParametersSignature (meth.Parameters));
        }

        public static string FullMethodSignature (IMethodReference meth)
        {
            return string.Concat (meth.ReturnType.ReturnType.FullName, " ", MemberSignature(meth), ParametersSignature(meth.Parameters));
        }

        public static string ParametersSignature (IParameterDefinitionCollection parameters)
        {
            string sig = "(";
            for (int i = 0; i < parameters.Count; i++) {
                sig += TypeFullName(parameters [i].ParameterType);
                if (i < parameters.Count - 1)
                    sig += ",";
            }
            return string.Concat (sig, ")");
        }
    }
}
