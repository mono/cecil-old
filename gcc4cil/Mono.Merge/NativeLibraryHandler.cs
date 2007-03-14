//
// NativeLibraryHandler.cs
//
// Author:
//	 Massimiliano Mantione (massi@ximian.com)
//
// (C) 2006 Novell
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Mono.Cecil;

namespace Mono.Merge {
	public class NativeLibraryHandler {
		
		static bool CompareMethodSignatures (MethodReference mr, MethodDefinition md) {
			if (mr.ReturnType.ReturnType.FullName != md.ReturnType.ReturnType.FullName) {
				return false;
			} else if (mr.Parameters.Count != md.Parameters.Count) {
				return false;
			} else {
				for (int i = 0; i < mr.Parameters.Count; i++) {
					if (mr.Parameters [i].ParameterType.FullName != md.Parameters [i].ParameterType.FullName) {
						return false;
					}
				}
				return true;
			}
		}
		
		Dictionary<string,List<MethodDefinition>> externalMethods = new Dictionary<string,List<MethodDefinition>> ();
		Dictionary<string,ModuleReference> referredLibraries = new Dictionary<string,ModuleReference> ();
		
		MethodDefinition CreateExternalMethod (MethodReference mr, string library, ModuleReference referredLibrary) {
			MethodDefinition md = new MethodDefinition(mr.Name,
					MethodAttributes.PInvokeImpl |  MethodAttributes.HideBySig | MethodAttributes.Private |
					MethodAttributes.Static, mr.ReturnType.ReturnType);
			foreach (ParameterDefinition pd in mr.Parameters) {
				md.Parameters.Add (pd);
			}
			md.PInvokeInfo = new PInvokeInfo(md,
           			PInvokeAttributes.CharSetAnsi | PInvokeAttributes.CallConvCdecl, md.Name, referredLibrary);
           	return md;
			
		}
		public MethodDefinition GetExternalMethod (MethodReference mr) {
			string library = GetSymbolLibrary (mr.Name);
			
			if (library != null) {
				List<MethodDefinition> methods = null;
				if (externalMethods.ContainsKey (mr.Name)) {
					methods = externalMethods [mr.Name];
					foreach (MethodDefinition method in methods) {
						if (CompareMethodSignatures (mr, method)) {
							return method;
						}
					}
				} else {
					methods = new List<MethodDefinition> ();
					externalMethods [mr.Name] = methods;
				}
				if (! referredLibraries.ContainsKey (library)) {
					referredLibraries [library] = new ModuleReference (library);
				}
				MethodDefinition md = CreateExternalMethod (mr, library, referredLibraries [library]);
				methods.Add (md);
				return md;
			} else {
				return null;
			}
		}
		
		List<string> libraries = new List<string> ();
		List<string> librariesSearchPaths = new List<string> ();
		
		public List<string> Libraries {
			get { return libraries; }
		}
		public List<string> LibrariesSearchPaths {
			get { return librariesSearchPaths; }
		}
		
		Hashtable SymbolTable = new Hashtable ();
		
		void AddSymbol (string name, string library) {
			SymbolTable [name] = library;
		}
		
		public string GetSymbolLibrary (string name) {
			if (SymbolTable.ContainsKey (name)) {
				return (string) SymbolTable [name];
			} else {
				return null;
			}
		}
		
		public void Initialize () {
			foreach (string libraryName in Libraries) {
				foreach (string directory in LibrariesSearchPaths) {
					try {
						string[] fileEntries = Directory.GetFiles(directory, "lib" + libraryName + ".so.*");
						if (fileEntries.Length > 0) {
							int fileNameLength = 0;
							string libraryFile = null;
							string libraryFullName = null;
							foreach (string fileName in fileEntries) {
								//Console.WriteLine ("Found file {0} in directory {1}", fileName, directory);
								if (fileName.Length > fileNameLength) {
									fileNameLength = fileName.Length;
									libraryFile = fileName;
									libraryFullName = fileName.Substring (directory.Length + 1);
								}
							}
							//Console.WriteLine ("Choose library {0} in directory {1}", libraryFullName, directory);
							
							Process p = new Process ();
							p.StartInfo.UseShellExecute = false;
							p.StartInfo.RedirectStandardOutput = true;
							
							p.StartInfo.FileName = "/usr/bin/nm";
							p.StartInfo.Arguments = "-D " + libraryFile;
							
							p.Start();
							// Do not wait for the child process to exit before
							// reading to the end of its redirected stream.
							// p.WaitForExit ();
							// Read the output stream first and then wait.
							string output = p.StandardOutput.ReadToEnd ();
							p.WaitForExit ();
							string[] outputLines = output.Split ('\n');
							
							Regex r = new Regex ("[a-fA-F0-9]+\\s[a-zA-Z]\\s(.*)$");
							foreach (string outputLine in outputLines) {
								Match m = r.Match (outputLine);
								if (m.Success) {
									//String symbol = m.Groups [0].Captures [0].Value;
									String symbol = m.Groups [1].Value;
									AddSymbol (symbol, libraryFullName);
									//Console.WriteLine ("+++ Added symbol {0} in library {1}", symbol, libraryFullName);
								} //else {
									//Console.Error.WriteLine ("--- Unrecognized nm output: {0}", outputLine);
								//}
							}
						}
					} catch (Exception e) {
						Console.Error.WriteLine ("Exception: {0}", e.Message);
					}
					
				}
			}
		}
	}
}
