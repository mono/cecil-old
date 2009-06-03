//
// ConfigMerger.cs
//
// Author:
//	 Andrés G. Aragoneses (aaragoneses@novell.com)
//
// (C) 2009 Novell, Inc.
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
using System.IO;
using System.Xml;

namespace Mono.Merge {
	
	internal class ConfigMerger {
		
		internal static void Process (List<string> assemblies, string outputPath)
		{
			List<string> valid_config_files = new List<string> ();
			foreach (string assembly in assemblies) {
				string assemblyConfig = assembly + ".config";
				if (File.Exists (assemblyConfig)) {
					XmlDocument doc = new XmlDocument ();
					try {
						doc.Load (assemblyConfig);
					} catch (XmlException) {
						doc = null;
					}
					if (doc != null)
						valid_config_files.Add (assemblyConfig);
				}
			}
			
			if (valid_config_files.Count == 0)
				return;

			string first_file = valid_config_files [0];
			System.Data.DataSet dataset = new System.Data.DataSet ();
			dataset.ReadXml (first_file);
			valid_config_files.Remove (first_file);
			
			foreach (string config_file in valid_config_files) {
				System.Data.DataSet next_dataset = new System.Data.DataSet ();
				next_dataset.ReadXml (config_file);
				dataset.Merge (next_dataset);
			}
			dataset.WriteXml (outputPath + ".config");
		}
	}
}
