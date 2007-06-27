//
// DeclarativeSecurityView.cs
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
using System.Collections;
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

		private ScrolledWindow sw;
		private VBox vbox;
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
			Widget w = SetUp ();
			
			if (assembly == null)
				return w;

			if (assembly.SecurityDeclarations.Count > 0) {
				// hide old stuff
				for (int i = 0; i < list.Length; i++)
					actions[i].SetText (null);
				// add new stuff (if any)
				foreach (SecurityDeclaration declsec in assembly.SecurityDeclarations) {
					actions [(int)declsec.Action - 1].SetText (declsec.PermissionSet.ToString ());
				}
			}
			vbox.Show ();
			sw.Show ();
			return w;
		}

		public Widget GetWidget (TypeDefinition type)
		{
			Widget w = SetUp ();

			if (type == null)
				return w;

			if (type.SecurityDeclarations.Count > 0) {
				// hide old stuff
				for (int i = 0; i < list.Length; i++)
					actions[i].SetText (null);
				// add new stuff (if any)
				foreach (SecurityDeclaration declsec in type.SecurityDeclarations) {
					actions [(int)declsec.Action - 1].SetText (declsec.PermissionSet.ToString ());
				}
			}
			vbox.Show ();
			sw.Show ();
			return w;
		}

		public Widget GetWidget (MethodDefinition method)
		{
			Widget w = SetUp ();

			if (method == null)
				return w;

			if (method.SecurityDeclarations.Count > 0) {
				// hide old stuff
				for (int i = 0; i < list.Length; i++)
					actions[i].SetText (null);
				// add new stuff (if any)
				foreach (SecurityDeclaration declsec in method.SecurityDeclarations) {
					actions [(int)declsec.Action - 1].SetText (declsec.PermissionSet.ToString ());
				}
			}
			vbox.Show ();
			sw.Show ();
			return w;
		}
		
		private Widget SetUp ()
		{
			FontDescription fd = FontDescription.FromString ("Courier 10 Pitch 10");

			vbox = new VBox (false, 0);

			actions = new Action[list.Length];
			for (int i=0; i < list.Length; i++) {
				actions [i] = new Action (list [i]);
				actions [i].TextView.ModifyFont (fd);
				vbox.Add (actions [i].Expander);
			}

			sw = new ScrolledWindow ();
			sw.AddWithViewport (vbox);
			return sw;
 		}
	}
}
