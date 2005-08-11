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

	internal sealed class Constants {

		private Constants ()
		{
		}

		public const string Corlib = "mscorlib.dll";

		public const string ModuleType = "<Module>";

		public const string Void = "System.Void";
		public const string Object = "System.Object";
		public const string String = "System.String";
		public const string Boolean = "System.Boolean";
		public const string Char = "System.Char";
		public const string Single = "System.Single";
		public const string Double = "System.Double";
		public const string SByte = "System.SByte";
		public const string Byte = "System.Byte";
		public const string Int16 = "System.Int16";
		public const string UInt16 = "System.UInt16";
		public const string Int32 = "System.Int32";
		public const string UInt32 = "System.UInt32";
		public const string Int64 = "System.Int64";
		public const string UInt64 = "System.UInt64";
		public const string IntPtr = "System.IntPtr";
		public const string UIntPtr = "System.UIntPtr";
		public const string TypedReference = "System.TypedReference";
		public const string Type = "System.Type";
	}
}
