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

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;

	using Mono.Cecil;
	using Mono.Cecil.Metadata;

	internal sealed class TableComparers {

		public sealed class TypeDef : IComparer {

			public static readonly TypeDef Instance = new TypeDef ();

			public void GetExtendLevel (TypeDefinition type, ref int level)
			{
				if (type.BaseType != null) {
					level++;
					if (type.BaseType is TypeDefinition)
						GetExtendLevel (type.BaseType as TypeDefinition, ref level);
				}
			}

			public int Compare (object x, object y)
			{
				TypeDefinition a = x as TypeDefinition;
				TypeDefinition b = y as TypeDefinition;

				if (a == null || b == null)
					throw new ReflectionException ("TypeDefComparer can only compare TypeDefinition");

				if (a == b)
					return 0;

				if (a.Name == Constants.ModuleType)
					return -1;
				else if (b.Name == Constants.ModuleType)
					return 1;

				int alev = 0;
				if (a.DeclaringType != null) {
					GetExtendLevel (a.DeclaringType as TypeDefinition, ref alev);
					alev++;
				} else
					GetExtendLevel (a, ref alev);

				int blev = 0;
				if (b.DeclaringType != null) {
					GetExtendLevel (b.DeclaringType as TypeDefinition, ref blev);
					blev--;
				} else
					GetExtendLevel (b, ref blev);

				if (alev < blev)
					return -1;
				else if (blev > alev)
					return 1;

				return Comparer.Default.Compare (a.FullName, b.FullName);
			}
		}

		public class TypeRef : IComparer {

			public static readonly TypeRef Instance = new TypeRef ();

			public int Compare (object x, object y)
			{
				TypeReference a = x as TypeReference;
				TypeReference b = y as TypeReference;

				if (a == null || b == null)
					throw new ReflectionException ("TypeRefComparer can only compare TypeReference");

				if (b.DeclaringType == a)
					return -1;
				else if (a.DeclaringType == b)
					return 1;

				return Comparer.Default.Compare (a.FullName, b.FullName);
			}
		}

		public class NestedClass : IComparer {

			public static readonly NestedClass Instance = new NestedClass ();

			public int Compare (object x, object y)
			{
				NestedClassRow a = x as NestedClassRow;
				NestedClassRow b = y as NestedClassRow;

				return Comparer.Default.Compare (a.NestedClass, b.NestedClass);
			}
		}

		public class InterfaceImpl : IComparer {

			public static readonly InterfaceImpl Instance = new InterfaceImpl ();

			public int Compare (object x, object y)
			{
				InterfaceImplRow a = x as InterfaceImplRow;
				InterfaceImplRow b = y as InterfaceImplRow;

				int klass = Comparer.Default.Compare (a.Class, b.Class);
				if (klass == 0) {
					return Comparer.Default.Compare (a.Interface.RID, b.Interface.RID);
				}

				return klass;
			}
		}
	}
}
