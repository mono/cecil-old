//
// WorkingSet.cs
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
using System.IO;
using System.Xml.Serialization;

namespace Monoxide {

	[Serializable]
	public class WorkingSet {

		private ArrayList _assemblies;
		private ArrayList _plugins;

		public IList Assemblies {
			get {
				if (_assemblies == null)
					_assemblies = new ArrayList ();
				return _assemblies;
			}
		}

		public IList PlugIns {
			get {
				if (_plugins == null)
					_plugins = new ArrayList ();
				return _plugins;
			}
		}

		static public WorkingSet Load (string filename)
		{
			XmlSerializer xs =  new XmlSerializer (typeof (WorkingSet));
			using (FileStream fs = File.Open (filename, FileMode.OpenOrCreate)) {
				return (WorkingSet) xs.Deserialize (fs);
			}
		}

		public void Save (string filename)
		{
			XmlSerializer xs = new XmlSerializer (typeof (WorkingSet));
			using (StreamWriter sw = new StreamWriter (filename)) {
				xs.Serialize (sw, this);
			}
		}
	}
}
