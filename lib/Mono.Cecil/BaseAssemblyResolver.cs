//
// BaseAssemblyResolver.cs
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
	using SR = System.Reflection;
	using System.Text;

	public abstract class BaseAssemblyResolver : IAssemblyResolver {

		public virtual AssemblyDefinition Resolve (string fullName)
		{
			return Resolve (AssemblyNameReference.Parse (fullName));
		}

		public virtual AssemblyDefinition Resolve (AssemblyNameReference name)
		{
			if (name.Name == "mscorlib")
				return GetCorlib (name);
			else if (IsInGac (name))
				return AssemblyFactory.GetAssembly (GetFromGac (name));

			string [] exts = new string [] { ".dll", ".exe" };
			string [] dirs = new string [] { ".", "bin" };

			foreach (string dir in dirs) {
				foreach (string ext in exts) {
					string file = Path.Combine (dir, name.Name + ext);
					if (File.Exists (file))
						return AssemblyFactory.GetAssembly (file);
				}
			}

			throw new FileNotFoundException ("Could not resolve: " + name);
		}

		AssemblyDefinition GetCorlib (AssemblyNameReference reference)
		{
			SR.AssemblyName corlib = typeof (object).Assembly.GetName ();
			if (corlib.Version == reference.Version)
				return AssemblyFactory.GetAssembly (typeof (object).Module.FullyQualifiedName);

			string path = Directory.GetParent (
				Directory.GetParent (
					typeof (object).Module.FullyQualifiedName).FullName
				).FullName;

			if (OnMono ()) {
				if (reference.Version.Major == 1)
					path = Path.Combine (path, "1.0");
				else if (reference.Version.Major == 2)
					path = Path.Combine (path, "2.0");
				else
					throw new NotSupportedException ("Version not supported: " + reference.Version);
			} else {
				if (reference.Version.ToString () == "1.0.3300.0")
					path = Path.Combine (path, "v1.0.3705");
				else if (reference.Version.ToString () == "1.0.5000.0")
					path = Path.Combine (path, "v1.1.4322");
				else if (reference.Version.ToString () == "2.0.0.0")
					path = Path.Combine (path, "v2.0.50727");
				else
					throw new NotSupportedException ("Version not supported: " + reference.Version);
			}

			return AssemblyFactory.GetAssembly (Path.Combine (path, "mscorlib.dll"));
		}

		bool OnMono ()
		{
			return typeof (object).Assembly.GetType ("System.MonoType", false) != null;
		}

		static bool IsInGac (AssemblyNameReference reference)
		{
			if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
				return false;

			return File.Exists (GetFromGac (reference));
		}

		static string GetFromGac (AssemblyNameReference reference)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (reference.Version);
			sb.Append ("__");
			for (int i = 0; i < reference.PublicKeyToken.Length; i++)
				sb.Append (reference.PublicKeyToken [i].ToString ("x2"));

			return Path.Combine (
				Path.Combine (
					Path.Combine (GetGacPath (), reference.Name), sb.ToString ()),
					string.Concat (reference.Name, ".dll"));
		}

		static string GetGacPath ()
		{
			return Directory.GetParent (
				Directory.GetParent (
					Path.GetDirectoryName (
						typeof (Uri).Module.FullyQualifiedName)
					).FullName
				).FullName;
		}
	}
}
