//
// MetadataPlugIn.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
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

using System;
using System.Collections;
using Gtk;
using Mono.Cecil;
using Monoxide.Framework.PlugIns;

namespace Monoxide.Metadata {

	public class MetadataPlugIn : IPlugIn {

		static private ArrayList _assemblies;

		private IDisplay[] displays;

		public MetadataPlugIn ()
		{
			displays = new IDisplay [2];
			displays[0] = new DeclarativeSecurityView ();
			displays[1] = new AssemblyDependenciesView ();
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			if (_assemblies == null)
				_assemblies = new ArrayList ();
			_assemblies.Add (assembly);
		}

		public string Name {
			get { return "Metadata Views"; }
		}

		public IDisplay[] Displays {
			get { return displays; }
		}

		// internal stuff

		internal static IList Assemblies {
			get { return _assemblies; }
		}
	}
}
