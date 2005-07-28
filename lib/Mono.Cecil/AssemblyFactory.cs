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
				AssemblyDefinition asm = new AssemblyDefinition (new AssemblyNameDefinition (), srv, loadType);

				asm.Accept (srv);
				(asm.MainModule.Types as TypeDefinitionCollection).Load ();
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

		public static IAssemblyDefinition DefineAssembly (string assemblyName, string moduleName)
		{
			AssemblyNameDefinition asmName = new AssemblyNameDefinition ();
			asmName.Name = assemblyName;
			AssemblyDefinition asm = new AssemblyDefinition (asmName);
			ModuleDefinition main = new ModuleDefinition (moduleName, asm, true);
			(asm.Modules as ModuleDefinitionCollection).Add (main);
			return asm;
		}

		public static void SaveAssembly (IAssemblyDefinition asm, string file)
		{
			using (FileStream fs = new FileStream (
					file, FileMode.Create, FileAccess.Write, FileShare.None)) {
				using (BinaryWriter bw = new BinaryWriter (fs)) {

					StructureWriter sw = new StructureWriter (asm as AssemblyDefinition, bw);
					asm.Accept (sw);
				}
			}
		}

		public static Assembly CreateReflectionAssembly (IAssemblyDefinition asm, AppDomain domain)
		{
			using (MemoryStream ms = new MemoryStream ()) {
				using (BinaryWriter bw = new BinaryWriter (ms)) {

					StructureWriter sw = new StructureWriter (asm as AssemblyDefinition, bw);
					asm.Accept (sw);

					return domain.Load (ms.ToArray ());
				}
			}
		}

		public static Assembly CreateReflectionAssembly (IAssemblyDefinition asm)
		{
			return CreateReflectionAssembly (asm, AppDomain.CurrentDomain);
		}

		public static Image GetUnderlyingImage (IModuleDefinition module)
		{
			return (module as ModuleDefinition).Image;
		}
	}
}
