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

			public int Compare (object x, object y)
			{
				TypeDefinition a = x as TypeDefinition;
				TypeDefinition b = y as TypeDefinition;

				if (a == null || b == null)
					throw new ReflectionException ("TypeDefComparer can only compare TypeDefinition");

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

		public class Constant : IComparer {

			public static readonly Constant Instance = new Constant ();

			public int Compare (object x, object y)
			{
				ConstantRow a = x as ConstantRow;
				ConstantRow b = y as ConstantRow;

				return Comparer.Default.Compare (
					Utilities.CompressMetadataToken (CodedIndex.HasConstant, a.Parent),
					Utilities.CompressMetadataToken (CodedIndex.HasConstant, b.Parent));
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
					return Comparer.Default.Compare (
						Utilities.CompressMetadataToken (CodedIndex.TypeDefOrRef, a.Interface),
						Utilities.CompressMetadataToken (CodedIndex.TypeDefOrRef, b.Interface));
				}

				return klass;
			}
		}
	}
}
