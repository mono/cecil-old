//
// CustomAttributesVisualizer.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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
using System.Text;

using Mono.Addins;
using Mono.Cecil;
using Monoxide.Framework.Addins;

using Gtk;
using Pango;

namespace Monoxide.Metadata {

	[Extension ("/Monoxide/Assembly")]
	[Extension ("/Monoxide/Type")]
	[Extension ("/Monoxide/Method")]
	internal class CustomAttributesVisualizer : IAssemblyVisualizer, ITypeVisualizer, IMethodVisualizer {

		public string Name {
			get { return "Custom Attributes"; }
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			// nothing to do when a new assembly is loaded
		}

		public Widget GetWidget (AssemblyDefinition assembly)
		{
			CustomAttributeCollection cac = (assembly == null) ? null : assembly.CustomAttributes;
			return GetWidget (cac);
		}

		public Widget GetWidget (TypeDefinition type)
		{
			CustomAttributeCollection cac = (type == null) ? null : type.CustomAttributes;
			return GetWidget (cac);
		}

		public Widget GetWidget (MethodDefinition method)
		{
			CustomAttributeCollection cac = (method == null) ? null : method.CustomAttributes;
			return GetWidget (cac);
		}
		
		private Widget GetWidget (CustomAttributeCollection cac)
		{
			FontDescription fd = FontDescription.FromString ("Courier 10 Pitch 10");

			VBox vbox = new VBox (false, 0);

			ScrolledWindow sw = new ScrolledWindow ();
			sw.AddWithViewport (vbox);
			
			if ((cac != null) && (cac.Count > 0)) {
				foreach (CustomAttribute ca in cac) {
 					TextView textview = new TextView ();
					textview.Editable = false;
					textview.Buffer.Text = Format (ca);
					textview.ModifyFont (fd);
					
					Expander expander = new Expander (ca.Constructor.DeclaringType.FullName);
					expander.Add (textview);
					
					vbox.Add (expander);
				}
			}
			
			sw.ShowAll ();
			return sw;
 		}

 		private string Format (CustomAttribute ca)
 		{
 			if ((ca.Fields.Count == 0) && (ca.Properties.Count == 0))
 				return FormatBlob (ca);
 				
 			StringBuilder sb = new StringBuilder ();
 			foreach (DictionaryEntry de in ca.Fields) {
 				sb.AppendFormat ("Field {0} = {1}{2}", de.Key, de.Value, Environment.NewLine);
 			}
 			foreach (DictionaryEntry de in ca.Properties) {
 				sb.AppendFormat ("Property {0} = {1}{2}", de.Key, de.Value, Environment.NewLine);
 			}
 			sb.Append (FormatBlob (ca));
 			return sb.ToString ();
 		}
 		
 		private string FormatBlob (CustomAttribute ca)
 		{
 			if (ca.Blob == null)
 				return String.Empty;

 			switch (ca.Constructor.DeclaringType.FullName) {
			// TODO - better display for well known attributes
 			default:
				return "Blob: " + BitConverter.ToString (ca.Blob);
			}
		}
	}
}
