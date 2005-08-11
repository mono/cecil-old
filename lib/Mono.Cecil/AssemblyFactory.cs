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
	using System.IO;
	using System.Reflection;

	using Mono.Cecil.Binary;
	using Mono.Cecil.Implem;
	using Mono.Cecil.Metadata;

	public sealed class AssemblyFactory {

		private AssemblyFactory ()
		{
		}

		public static IAssemblyDefinition GetAssembly (string file, LoadingType loadType)
		{
			try {
				ImageReader brv = new ImageReader (file);
				StructureReader srv = new StructureReader (brv);
				AssemblyDefinition asm = new AssemblyDefinition (
					new AssemblyNameDefinition (), srv, loadType);

				asm.Accept (srv);
				return asm;
			} catch (ReflectionException) {
				throw;
			} catch (MetadataFormatException) {
				throw;
			} catch (ImageFormatException) {
				throw;
			} catch (Exception e) {
				throw new ReflectionException ("Can not disassemble assembly", e);
			}
		}

		public static IAssemblyDefinition GetAssembly (string file)
		{
			return GetAssembly (file, LoadingType.Lazy);
		}

		public static IAssemblyDefinition DefineAssembly (string assemblyName, string moduleName, TargetRuntime rt)
		{
			AssemblyNameDefinition asmName = new AssemblyNameDefinition ();
			asmName.Name = assemblyName;
			AssemblyDefinition asm = new AssemblyDefinition (asmName);
			asm.Runtime = rt;
			ModuleDefinition main = new ModuleDefinition (moduleName, asm, true);
			(asm.Modules as ModuleDefinitionCollection).Add (main);
			return asm;
		}

		public static void SaveAssembly (IAssemblyDefinition asm, string file, AssemblyKind kind)
		{
			using (FileStream fs = new FileStream (
					file, FileMode.Create, FileAccess.Write, FileShare.None)) {
				using (BinaryWriter bw = new BinaryWriter (fs)) {

					WriteAssembly (asm, kind, bw);
				}
			}
		}

		public static Assembly CreateReflectionAssembly (IAssemblyDefinition asm, AssemblyKind kind, AppDomain domain)
		{
			using (MemoryStream ms = new MemoryStream ()) {
				using (BinaryWriter bw = new BinaryWriter (ms)) {

					WriteAssembly (asm, kind, bw);

					return domain.Load (ms.ToArray ());
				}
			}
		}

		private static void WriteAssembly (IAssemblyDefinition asm, AssemblyKind kind, BinaryWriter bw)
		{
			asm.Accept (new StructureWriter (
					(asm as AssemblyDefinition), kind, bw));
		}

		public static Assembly CreateReflectionAssembly (IAssemblyDefinition asm, AssemblyKind kind)
		{
			return CreateReflectionAssembly (asm, kind, AppDomain.CurrentDomain);
		}

		public static Image GetUnderlyingImage (IModuleDefinition module)
		{
			return (module as ModuleDefinition).Image;
		}
	}
}
