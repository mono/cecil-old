//
// DeclarativeSecurityVisualizer.cs
//
// Authors:
//	Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2005-2007 Novell, Inc (http://www.novell.com)
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
using System.Text;

using Mono.Addins;
using Mono.Cecil;
using Monoxide.Framework.Addins;

using Gtk;
using Pango;

namespace Monoxide.Metadata {

	internal struct Action {
	
		public SecurityAction action;
		public Expander Expander;
		public TextView TextView;

		public Action (SecurityAction action)
		{
			this.action = action;
			this.TextView = new TextView ();
			this.TextView.Editable = false;
			this.Expander = new Expander (action.ToString ());
			this.Expander.Add (this.TextView);
		}

		public void SetText (string xml)
		{
			if ((xml != null) && (xml.Length > 0)) {
				this.TextView.Buffer.Text = xml;
				this.Expander.ShowAll ();
			} else {
				this.Expander.HideAll ();
			}
		}
	}

	[Extension ("/Monoxide/Assembly")]
	[Extension ("/Monoxide/Type")]
	[Extension ("/Monoxide/Method")]
	internal class DeclarativeSecurityView : IAssemblyVisualizer, ITypeVisualizer, IMethodVisualizer {

		static SecurityAction[] list = new SecurityAction[] { 
			SecurityAction.Request,
			SecurityAction.Demand,
			SecurityAction.Assert, 
			SecurityAction.Deny,
			SecurityAction.PermitOnly,
			SecurityAction.LinkDemand,
			SecurityAction.InheritDemand, 
			SecurityAction.RequestMinimum, 
			SecurityAction.RequestOptional, 
			SecurityAction.RequestRefuse,
			SecurityAction.PreJitGrant,
			SecurityAction.PreJitDeny,
			SecurityAction.NonCasDemand, 
			SecurityAction.NonCasLinkDemand,
			SecurityAction.NonCasInheritance };

		private Action[] actions;

		public string Name {
			get { return "Declarative Security"; }
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			// nothing to do when a new assembly is loaded
		}

		public Widget GetWidget (AssemblyDefinition assembly)
		{
			SecurityDeclarationCollection sdc = (assembly == null) ? null : assembly.SecurityDeclarations;
			return GetWidget (sdc);
		}

		public Widget GetWidget (TypeDefinition type)
		{
			SecurityDeclarationCollection sdc = (type == null) ? null : type.SecurityDeclarations;
			return GetWidget (sdc);
		}

		public Widget GetWidget (MethodDefinition method)
		{
			SecurityDeclarationCollection sdc = (method == null) ? null : method.SecurityDeclarations;
			return GetWidget (sdc);
		}
		
		private Widget GetWidget (SecurityDeclarationCollection sdc)
		{
			FontDescription fd = FontDescription.FromString ("Courier 10 Pitch 10");

			VBox vbox = new VBox (false, 0);

			actions = new Action[list.Length];
			for (int i=0; i < list.Length; i++) {
				actions [i] = new Action (list [i]);
				actions [i].TextView.ModifyFont (fd);
				vbox.Add (actions [i].Expander);
			}

			ScrolledWindow sw = new ScrolledWindow ();
			sw.AddWithViewport (vbox);
			
			if ((sdc != null) && (sdc.Count >= 0)) {
				foreach (SecurityDeclaration declsec in sdc) {
					actions [(int)declsec.Action - 1].SetText (declsec.PermissionSet.ToString ());
				}
			}
			
			vbox.Show ();
			sw.Show ();
			return sw;
 		}
	}
}
