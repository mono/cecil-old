//
// AssemblyFactory.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Mono.Cecil {

	using System;
	using System.IO;
	using System.Reflection;

	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;

	public sealed class AssemblyFactory {

		AssemblyFactory ()
		{
		}

		public static AssemblyDefinition GetAssembly (string file)
		{
			try {
				ImageReader brv = new ImageReader (file);
				StructureReader srv = new StructureReader (brv);
				AssemblyDefinition asm = new AssemblyDefinition (
					new AssemblyNameDefinition (), srv);

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

		public static AssemblyDefinition DefineAssembly (string assemblyName, string moduleName, TargetRuntime rt)
		{
			AssemblyNameDefinition asmName = new AssemblyNameDefinition ();
			asmName.Name = assemblyName;
			AssemblyDefinition asm = new AssemblyDefinition (asmName);
			asm.Runtime = rt;
			ModuleDefinition main = new ModuleDefinition (moduleName, asm, true);
			asm.Modules.Add (main);
			return asm;
		}

		static void WriteAssembly (AssemblyDefinition asm, AssemblyKind kind, MemoryBinaryWriter bw)
		{
			asm.Accept (new StructureWriter (asm, kind, bw));
		}

		public static void SaveAssembly (AssemblyDefinition asm, string file, AssemblyKind kind)
		{
			using (FileStream fs = new FileStream (
					file, FileMode.Create, FileAccess.Write, FileShare.None)) {
				using (BinaryWriter bw = new BinaryWriter (fs)) {

					MemoryBinaryWriter writer = new MemoryBinaryWriter ();
					WriteAssembly (asm, kind, writer);
					bw.Write (writer.ToArray ());
				}
			}
		}

		public static Assembly CreateReflectionAssembly (AssemblyDefinition asm, AssemblyKind kind, AppDomain domain)
		{
			using (MemoryBinaryWriter writer = new MemoryBinaryWriter ()) {

				WriteAssembly (asm, kind, writer);
				return domain.Load (writer.ToArray ());
			}
		}

		public static Assembly CreateReflectionAssembly (AssemblyDefinition asm, AssemblyKind kind)
		{
			return CreateReflectionAssembly (asm, kind, AppDomain.CurrentDomain);
		}
	}
}
