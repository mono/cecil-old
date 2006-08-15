//
// LinkContext.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.Linker {

	using System.Collections;

	using Mono.Cecil;

	public class LinkContext {

		Hashtable _asmCtx;
		string _outputDirectory;
		bool _preserveCoreLibraries;

		public string OutputDirectory {
			get { return _outputDirectory; }
			set { _outputDirectory = value; }
		}

		public bool PreserveCoreLibraries {
			get { return _preserveCoreLibraries; }
			set { _preserveCoreLibraries = value; }
		}

		public LinkContext ()
		{
			_asmCtx = new Hashtable ();
		}

		public AssemblyMarker Resolve (IMetadataScope scope)
		{
			AssemblyNameReference reference;
			if (scope is ModuleDefinition) {
				AssemblyDefinition asm = ((ModuleDefinition) scope).Assembly;
				foreach (AssemblyMarker am in _asmCtx.Values)
					if (am.Assembly == asm)
						return am;

				reference = asm.Name;
			} else
				reference = ((AssemblyNameReference)scope);

			if (_asmCtx.Contains (reference.FullName))
				return (AssemblyMarker) _asmCtx [reference.FullName];

			AssemblyMarker marker = new AssemblyMarker (
				_preserveCoreLibraries && IsCore (reference) ? AssemblyAction.Preserve : AssemblyAction.Link,
				AssemblyResolver.Resolve (reference));

			_asmCtx.Add (reference.FullName, marker);
			return marker;
		}

		bool IsCore (AssemblyNameReference name)
		{
			return name.Name == "mscorlib" || name.Name.StartsWith ("System");
		}

		public void AddMarker (AssemblyMarker marker)
		{
			_asmCtx [marker.Assembly.Name.FullName] = marker;
		}

		public AssemblyMarker [] GetAssemblies ()
		{
			AssemblyMarker [] markers = new AssemblyMarker [_asmCtx.Count];
			_asmCtx.Values.CopyTo (markers, 0);
			return markers;
		}
	}
}