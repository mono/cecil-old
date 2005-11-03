//
// Gendarme.Framework.AssemblyManager class
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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
using System.IO;
using System.Reflection;
using System.Text;

using Mono.Cecil;

namespace Gendarme.Framework {

	public class AssemblyManager {

		static public IMethodDefinition GetMethod (object method)
		{
			IMethodDefinition md = (method as IMethodDefinition);
			if (md != null)
				return md;

			IMethodReference mr = (method as IMethodReference);
			if (mr == null)
				return null;

			// convert the IMethodReference into a IMethodDefinition
			//AssemblyNameReference anr = (mr.DeclaringType.Scope as AssemblyNameReference);
			return null;

			// TODO - Gendarme needs a list of "related" assemblies to load so we can
			// return Definitions instead of References
		}

		static string defaultAssemblyLocation;

		static public string DefaultAssemblyLocation {
			get {
				if (defaultAssemblyLocation == null)
					defaultAssemblyLocation = Path.GetDirectoryName (typeof (int).Assembly.Location);
				return defaultAssemblyLocation;
			}
		}
	}
}
