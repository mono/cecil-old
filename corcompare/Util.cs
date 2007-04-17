using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace Mono.Util.CorCompare.Cecil {
	static class TypeHelper {

		internal static bool IsPublic (TypeReference typeref) {
			if (typeref == null) {
				throw new ArgumentException ("typeref must not be null");
			}
			TypeDefinition td = GetTypeDefinition (typeref);
			return (td.Attributes & TypeAttributes.Public) != 0;
		}

		internal static TypeDefinition GetTypeDefinition (TypeReference typeref) {
			if (typeref is TypeDefinition)
				return (TypeDefinition)typeref;
			if (typeref.Module == null)
				return null;
			return typeref.Module.Assembly.Resolver.Resolve (typeref.Scope.Name).MainModule.Types [typeref.FullName];
		}

		internal static bool IsDelegate (TypeReference typeref) {
			return IsDerivedFrom (typeref, "System.MulticastDelegate");
		}

		internal static bool IsDerivedFrom (TypeReference type, string derivedFrom) {

			for (TypeDefinition t = GetTypeDefinition (type); t != null; t = GetBaseType(t)) {
				if (t.FullName == derivedFrom)
					return true;
			}
			return false;
		}

		internal static TypeDefinition GetBaseType (TypeDefinition child) {
			if (child.BaseType == null)
				return null;
			return GetTypeDefinition (child.BaseType);
		}

		internal static bool IsPublic (CustomAttribute att) {
			TypeDefinition td = GetTypeDefinition (att);
			return (td.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public;
		}

		internal static string GetFullName (CustomAttribute att) {
			return att.Constructor.DeclaringType.FullName;
		}

		internal static TypeDefinition GetTypeDefinition (CustomAttribute att) {

			return att.Constructor.DeclaringType.Module.Assembly.Resolver.Resolve (att.Constructor.DeclaringType.Scope.Name).MainModule.Types [GetFullName (att)];
		}

	}
}
