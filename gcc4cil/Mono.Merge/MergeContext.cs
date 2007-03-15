//
// MergeContext.cs
//
// Authors:
//	 Alex Prudkiy (prudkiy@gmail.com)
//	 Massimiliano Mantione (massi@ximian.com)
//
// (C) 2006 Alex Prudkiy
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

using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace Mono.Merge {

	public class MergeContext {

		string outputPath = "a.exe";
		bool outputIsExecutable = true;
		AssemblyDefinition outputAssembly;
		
		string mainModuleName = "<Module>";
		string mainTypeName = "<Module>";
		string externalAssemblyName = "ExternalAssembly";
		
		List<string> assemblies = new List<string> ();
		List<AssemblyDefinition> merged_assemblies = new List<AssemblyDefinition> ();
		
		NativeLibraryHandler nativeLibraries = new NativeLibraryHandler ();
		InternalSymbolTable internalSymbolTable = new InternalSymbolTable ();
		
		ModuleDefinition mainModule;
		TypeDefinition mainType;
		
		bool linkFailed = false;
		
		public string OutputPath {
			get { return outputPath; }
			set { outputPath = value; }
		}
		public bool OutputIsExecutable {
			get { return outputIsExecutable; }
			set { outputIsExecutable = value; }
		}
		public AssemblyDefinition OutputAssembly {
			get { return outputAssembly; }
		}
		
		public string MainModuleName {
			get { return mainModuleName; }
			set { mainModuleName = value; }
		}
		public string MainTypeName {
			get { return mainTypeName; }
			set { mainTypeName = value; }
		}
		public string ExternalAssemblyName {
			get { return externalAssemblyName; }
			set { externalAssemblyName = value; }
		}

		public List<string> Assemblies {
			get { return assemblies; }
		}

		public List<AssemblyDefinition> MergedAssemblies {
			get { return merged_assemblies; }
		}
		
		public NativeLibraryHandler NativeLibraries {
			get { return nativeLibraries; }
		}
		public InternalSymbolTable InternalSymbols {
			get { return internalSymbolTable; }
		}
		
		public ModuleDefinition MainModule {
			get { return mainModule; }
		}
		public TypeDefinition MainType {
			get { return mainType; }
		}
		
		public bool LinkFailed {
			get { return linkFailed; }
			set { linkFailed = value; }
		}
		
		public void Link ()
		{
			outputAssembly = AssemblyFactory.GetAssembly (Assemblies [0]);
			mainModule = outputAssembly.MainModule;
			if (mainModule.Name != MainModuleName) {
				Console.Error.WriteLine ("Main module is not named \"" + MainModuleName + "\" in assembly " + outputAssembly.Name.FullName);
				Environment.Exit (1);
			}
			mainType = mainModule.Types [MainTypeName];
			if (mainType == null) {
				Console.Error.WriteLine ("Main module does not contain type \"" + MainTypeName + "\" in assembly " + outputAssembly.Name.FullName);
				Environment.Exit (1);
			}
			outputAssembly.Accept (new StructureMerger (this, outputAssembly, outputAssembly));
			
			for (int i = 1; i < Assemblies.Count; i++) {
				AssemblyDefinition asm = AssemblyFactory.GetAssembly (Assemblies [i]);
				asm.Accept (new StructureMerger (this, outputAssembly, asm));
			}

			FixReflectionAfterMerge fix = new FixReflectionAfterMerge (this, outputAssembly, outputAssembly);
			fix.Process ();
			
			nativeLibraries.AddExternalMethods (this);

			if (OutputIsExecutable) {
				outputAssembly.Kind = AssemblyKind.Console;
				outputAssembly.EntryPoint = InternalSymbols.EntryPoint;
			} else {
				outputAssembly.Kind = AssemblyKind.Dll;
			}
			AssemblyFactory.SaveAssembly (outputAssembly, OutputPath);
		}
	}
}
