using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Mono.Cecil;

namespace Mono.Util.CorCompare.Cecil
{
    /// <summary>
    /// Summary description for Resolver.
    /// </summary>
    /// 
    public class AssemblyResolver : IAssemblyResolver
    {
        private readonly string[] _libpath;
        private readonly Hashtable _asmdefs = new Hashtable();
        private readonly char c_dSep = Path.DirectorySeparatorChar;

        public AssemblyResolver(string libpath)
        {
            _libpath = libpath.Split(';');
        }

		public virtual AssemblyDefinition Resolve (string fullName)
		{
			return Resolve (AssemblyNameReference.Parse (fullName));
		}

        public AssemblyDefinition Resolve(AssemblyNameReference asmName)
        {
            string name = asmName.Name.Trim();
			if (name == "CommonLanguageRuntimeLibrary") {
				name = "mscorlib";
			}
            if (_asmdefs.ContainsKey(name))
                return (AssemblyDefinition) _asmdefs[name];

            foreach (string dir in _libpath)
            {
                if (dir.Trim() == "")
                    continue;
                //try dlls first
                string finalname;
                if (File.Exists(dir + c_dSep + name + ".dll"))
                    finalname = dir + c_dSep + name + ".dll";
                else if (File.Exists(dir + c_dSep + name + ".exe"))
                    finalname = dir + c_dSep + name + ".exe";
                else if (File.Exists(dir + c_dSep + name))
                    finalname = dir + c_dSep + name;
                else
                    continue;

                AssemblyDefinition asm = AssemblyFactory.GetAssembly(finalname);
                _asmdefs.Add(name, asm);
				asm.Resolver = this;
                return asm;
            }

            throw new FileNotFoundException(name + ".dll");
        }
    }
}
