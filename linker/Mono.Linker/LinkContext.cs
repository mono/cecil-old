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

using System.Collections;

using Mono.Cecil;

namespace Mono.Linker {

	public class LinkContext {

		Pipeline _pipeline;
		Hashtable _asmCtx;
		AssemblyAction _coreAction;
		string _outputDirectory;

		IAssemblyResolver _resolver;

		public Pipeline Pipeline {
			get { return _pipeline; }
		}

		public string OutputDirectory {
			get { return _outputDirectory; }
			set { _outputDirectory = value; }
		}

		public AssemblyAction CoreAction {
			get { return _coreAction; }
			set { _coreAction = value; }
		}

		public bool OnMono {
			get { return BaseAssemblyResolver.OnMono (); }
		}

		public LinkContext (Pipeline pipeline)
		{
			_pipeline = pipeline;
			_asmCtx = new Hashtable ();
			_resolver = new DefaultAssemblyResolver ();
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
				reference = (AssemblyNameReference) scope;

			if (_asmCtx.Contains (reference.FullName))
				return (AssemblyMarker) _asmCtx [reference.FullName];

			AssemblyAction action = AssemblyAction.Link;
			if (IsCore (reference))
				action = _coreAction;

			AssemblyDefinition assembly = _resolver.Resolve (reference);

			AssemblyMarker marker = new AssemblyMarker (
				action,
				assembly);

			_asmCtx.Add (assembly.Name.FullName, marker);
			return marker;
		}

		static bool IsCore (AssemblyNameReference name)
		{
			return name.Name == "mscorlib"
				|| name.Name == "Accessibility"
				|| name.Name.StartsWith ("System")
				|| name.Name.StartsWith ("Microsoft");
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