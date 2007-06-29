//
// MainWindow.cs
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using Gtk;
using Glade;
using Pango;

using Mono.Addins;
using Mono.Addins.Gui;
using Mono.Cecil;
using Monoxide.Framework;
using Monoxide.Framework.Addins;

namespace Monoxide {
		
	public partial class MainWindow : Gtk.Window {

		private TreeStore tree_store;
		private TreeIter tree_cursor;

		private string current_directory;
		private FileFilter dotnet_filter;
		private FileFilter list_filter;
		private FileFilter all_filter;
		
		private List<AssemblyDefinition> assemblies;
		private List<string> locations;
		
		private Menu popup_assemblies;
		private Menu popup_types;
		private Menu popup_methods;
		
		private Dictionary<MenuItem,IVisualizer> visualizers;

		public MainWindow (string[] args) : base (Gtk.WindowType.Toplevel)
		{
			this.DeleteEvent += delegate {
				OnQuitActivated (this, EventArgs.Empty);
			};
			
			this.Build ();
			
			notebook.Remove (notebook.GetNthPage (0));

			visualizers = new Dictionary<MenuItem, IVisualizer> ();
			popup_assemblies = new Menu ();
			popup_types = new Menu ();
			popup_methods = new Menu ();

			AddinManager.AddinLoadError += OnLoadError;

			AddinManager.Initialize (".");
			AddinManager.Registry.Update (null);
			AddinManager.AddExtensionNodeHandler ("/Monoxide/Assembly", OnAssemblyExtensionChanged);
			AddinManager.AddExtensionNodeHandler ("/Monoxide/Type", OnTypeExtensionChanged);
			AddinManager.AddExtensionNodeHandler ("/Monoxide/Method", OnMethodExtensionChanged);
			
			popup_assemblies.ShowAll ();
			popup_types.ShowAll ();
			popup_methods.ShowAll ();

			tree_store = new TreeStore (typeof (string), typeof (object));

			treeview.Model = tree_store;
			treeview.EnableSearch = false;
			treeview.AppendColumn ("Object", new CellRendererText (), "text", 0);
			treeview.Selection.Changed += OnSelectionChanged;
			OnSelectionChanged (null, EventArgs.Empty);
			
			if (args.Length > 0)
				Load (args);
		}
		
		private void BuildFilters ()
		{
			if (dotnet_filter != null)
				return;
				
			dotnet_filter = new FileFilter ();
			dotnet_filter.Name = ".NET assemblies (*.exe, *.dll)";
			dotnet_filter.AddPattern ("*.exe");
			dotnet_filter.AddPattern ("*.dll");

			list_filter = new FileFilter ();
			list_filter.Name = "Assembly List (*.lst)";
			list_filter.AddPattern ("*.lst");

			all_filter = new FileFilter ();
			all_filter.Name = "All (*.*)";
			all_filter.AddPattern ("*.*");
		}
		
		private object GetSelectedObject ()
		{
			TreeIter iter;
			TreeModel model;
			if (treeview.Selection.GetSelected (out model, out iter))
				return model.GetValue (iter, 1);
			else
				return null;
		}

		private void OnAssemblySelection (AssemblyDefinition ad)
		{
			objectLabel.Text = ad.Name.FullName;
			// very useful for debugging
			textview.Buffer.Text = String.Empty;
		}

		private void OnTypeSelection (TypeDefinition td)
		{
			objectLabel.Text = td.ToString ();
			// very useful for debugging
			textview.Buffer.Text = String.Empty;
		}

		private void OnMethodSelection (MethodDefinition md)
		{
			objectLabel.Text = md.ToString ();
			// very useful for debugging
			textview.Buffer.Text = String.Empty;
		}
		
		private void OnSelectionChanged (object o, EventArgs args)
		{
			TreeIter iter;
			TreeModel model;
			if (treeview.Selection.GetSelected (out model, out iter)) {
				object obj = model.GetValue (iter, 1);
				
				AssemblyDefinition ad = (obj as AssemblyDefinition);
				if (ad != null) {
					OnAssemblySelection (ad);
				} else {
					TypeDefinition td = (obj as TypeDefinition);
					if (td != null) {
						OnTypeSelection (td);
					} else {
						MethodDefinition md = (obj as MethodDefinition);
						if (md != null)
							OnMethodSelection (md);
					}
				}
			}
		}

		protected virtual void OnAddinManagerActivated (object sender, System.EventArgs e)
		{
			AddinManagerWindow.Run (this);
		}

		private void LoadAssembly (string filename)
		{
			if (assemblies == null)
				assemblies = new List<AssemblyDefinition> ();
			if (locations == null)
				locations = new List<string> ();

			try {
				AssemblyDefinition ad = AssemblyFactory.GetAssembly (filename);
				assemblies.Add (ad);

				foreach (IVisualizer v in visualizers.Values) {
					v.AddAssembly (ad);
				}

				// add assembly to the treeview
				PopulateStore (ad);
				
				locations.Add (filename);
			}
			catch (Exception e) {
				Console.WriteLine (e);
			}
		}
		
		private void LoadAssemblyList (string listname)
		{
			using (StreamReader sr = new StreamReader (listname)) {
				while (!sr.EndOfStream) {
					string filename = sr.ReadLine ();
					if (filename.Length > 0)
						LoadAssembly (filename);
				}
			}
		}

		private void Load (string[] filenames)
		{
			foreach (string filename in filenames) {
				switch (System.IO.Path.GetExtension (filename).ToLower ()) {
				case ".lst":
					LoadAssemblyList (filename);
					break;
				default:
					LoadAssembly (filename);
					break;
				}
			}
		}

		private void SaveAssemblyList (string listname)
		{
			using (StreamWriter sw = new StreamWriter (listname)) {
				if (locations != null) {
					foreach (string filename in locations)
						sw.WriteLine (filename);
				}
			}
		}
		
		private void PopulateStore (AssemblyDefinition assembly) 
		{
			tree_cursor = tree_store.AppendValues (assembly.Name.FullName, assembly);
			foreach (ModuleDefinition module in assembly.Modules) {

				foreach (TypeDefinition type in module.Types) {
					TreeIter titer = tree_cursor;

					tree_cursor = tree_store.AppendValues (tree_cursor, type.ToString (), type);

					TreeIter miter = tree_cursor;
					foreach (MethodDefinition method in type.Constructors) {
						tree_store.AppendValues (miter, GetMethodName (method), method);
					}
					foreach (MethodDefinition method in type.Methods) {
						tree_store.AppendValues (miter, GetMethodName (method), method);
					}
					tree_cursor = titer;
				}
			}
			tree_store.SetSortColumnId (0, SortType.Ascending);
		}

		private string GetMethodName (MethodDefinition method)
		{
			StringBuilder name = new StringBuilder (method.Name);
			name.Append ("(");

			int count = method.Parameters.Count;
			if (count > 0) {
				name.Append (method.Parameters [0].ParameterType.ToString ());
				for (int i=1; i < count; i++) {
					name.Append (",");
					name.Append (method.Parameters [i].ParameterType.ToString ());
				}
			}
			
			name.Append (")");
			return name.ToString ();
		}

		[GLib.ConnectBefore]
		protected virtual void OnTreeviewButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			// check for a right-click
			if (args.Event.Button == 3) {
				// note: at this stage GetSelectedObject is "late" and return the previous item
				object obj = null; 
				TreePath path;
				if (treeview.GetPathAtPos ((int)args.Event.X, (int)args.Event.Y, out path)) {
					TreeIter iter;
					if (treeview.Model.GetIter (out iter, path)) {
						obj = treeview.Model.GetValue (iter, 1);
						Console.WriteLine (obj);
					}
				}
			
				if ((obj is AssemblyDefinition) && (popup_assemblies.Children.Length > 0))
					popup_assemblies.Popup ();
				else if ((obj is TypeDefinition) && (popup_types.Children.Length > 0))
					popup_types.Popup ();
				else if ((obj is MethodDefinition) && (popup_methods.Children.Length > 0))
					popup_methods.Popup ();
			}
		}
		
		// File Menu
		
		protected virtual void OnOpenActivated (object sender, System.EventArgs e)
		{
			BuildFilters ();
			using (FileChooserDialog fcd = new FileChooserDialog ("Open Assembly...", this, FileChooserAction.Open, 
				"Cancel", ResponseType.Cancel, "Open", ResponseType.Accept)) {
				
				if (current_directory != null)
					fcd.SetCurrentFolder (current_directory);
					
				fcd.SelectMultiple = true;
				fcd.AddFilter (dotnet_filter);
				fcd.AddFilter (list_filter);
				fcd.AddFilter (all_filter);
				
				bool ok = (fcd.Run () == (int) ResponseType.Accept);
				fcd.Hide ();
				if (ok) {
					Load (fcd.Filenames);
					
					current_directory = fcd.CurrentFolder;
				}
			}
		}

		protected virtual void OnSaveActivated (object sender, System.EventArgs e)
		{
			using (FileChooserDialog fcd = new FileChooserDialog ("Save Assembly List...", this, FileChooserAction.Save, 
				"Cancel", ResponseType.Cancel, "Save", ResponseType.Accept)) {
				
				if (current_directory != null)
					fcd.SetCurrentFolder (current_directory);
					
				bool ok = (fcd.Run () == (int) ResponseType.Accept);
				fcd.Hide ();
				if (ok) {
					SaveAssemblyList (fcd.Filename);
					
					current_directory = fcd.CurrentFolder;
				}
			}
		}

		protected virtual void OnQuitActivated (object sender, System.EventArgs e)
		{
			Application.Quit ();
		}
		
		// View Menu

		protected virtual void OnRefreshActivated (object sender, System.EventArgs e)
		{
			AddinScrolledWindow asw = (notebook.CurrentPageWidget as AddinScrolledWindow);
			if (asw != null)
				asw.Refresh ();
		}

		// Help Menu
		
		protected virtual void OnAboutActivated (object sender, System.EventArgs e)
		{
			using (AboutDialog about = new AboutDialog ()) {
				// get information from the main assembly
				Assembly a = Assembly.GetExecutingAssembly ();

				object[] attr = a.GetCustomAttributes (typeof (AssemblyCopyrightAttribute), false);
				if (attr.Length > 0)
					about.Copyright = ((AssemblyCopyrightAttribute) attr [0]).Copyright;

				attr = a.GetCustomAttributes (typeof (AssemblyProductAttribute), false);
				if (attr.Length > 0)
					about.Name = ((AssemblyProductAttribute) attr [0]).Product;
					
				about.Version = a.GetName ().Version.ToString ();
				
				attr = a.GetCustomAttributes (typeof (AssemblyDescriptionAttribute), false);
				if (attr.Length > 0)
					about.Comments = ((AssemblyDescriptionAttribute) attr [0]).Description;
					
				// some static stuff
				about.Website = "http://www.mono-project.com/monoxide";
				about.Authors = new string [1] { 
					"Sebastien Pouliot  <sebastien@ximian.com>"
				};
				
				// read license from resources
				using (Stream s = a.GetManifestResourceStream ("MIT.X11")) {
					using (StreamReader sr = new StreamReader (s)) {
						about.License = sr.ReadToEnd ();
					}
				}
				
				// add logo (from resources) to about box
/*				using (Gdk.Pixbuf logo = new Gdk.Pixbuf (null, "logo.png")) {
					about.Logo = logo;
					about.Run ();
				}*/
				about.Run ();
			}
		}

		// Mono.Addins

		void OnLoadError (object s, AddinErrorEventArgs args)
		{
			Console.WriteLine ("Add-in error: " + args.Message);
			Console.WriteLine (args.AddinId);
			Console.WriteLine (args.Exception);
		}
		
		private void AddAssemblyExtension (IAssemblyVisualizer visualizer)
		{
			string item_name = visualizer.Name + "...";
			
			MenuItem mi = new MenuItem (item_name);
			visualizers.Add (mi, visualizer);

			mi.Activated += delegate (object obj, EventArgs args) {
				AssemblyDefinition ad = (GetSelectedObject () as AssemblyDefinition);
				if (ad == null)
					return;

				IAssemblyVisualizer av = visualizers [(obj as MenuItem)] as IAssemblyVisualizer;
				if (av == null)
					return;

				string title = String.Format ("{0} for {1}", av.Name, ad.Name);
				
				Tab tab = new Tab (title);
				tab.Content =  av.GetWidget (ad);
				tab.CloseButtonClicked += delegate (object o, EventArgs ea) {
					notebook.Remove ((o as Tab).Content);
				};
				
				int new_page = notebook.AppendPage (tab.Content, tab);
				notebook.Page = new_page;
			};
			popup_assemblies.Append (mi);
			popup_assemblies.ShowAll ();
		}
		
		private void AddTypeExtension (ITypeVisualizer visualizer)
		{
			string item_name = visualizer.Name + "...";
			
			MenuItem mi = new MenuItem (item_name);
			visualizers.Add (mi, visualizer);

			mi.Activated += delegate (object obj, EventArgs args) {
				TypeDefinition td = (GetSelectedObject () as TypeDefinition);
				if (td == null)
					return;

				ITypeVisualizer tv = visualizers [(obj as MenuItem)] as ITypeVisualizer;
				if (tv == null)
					return;

				string title = String.Format ("{0} for {1}", tv.Name, td.Name);
				
				Tab tab = new Tab (title);
				tab.Content =  tv.GetWidget (td);
				tab.CloseButtonClicked += delegate (object o, EventArgs ea) {
					notebook.Remove ((o as Tab).Content);
				};
				
				int new_page = notebook.AppendPage (tab.Content, tab);
				notebook.Page = new_page;
			};
			popup_types.Append (mi);
			popup_types.ShowAll ();
		}

		private void AddMethodExtension (IMethodVisualizer visualizer)
		{
			string item_name = visualizer.Name + "...";
			
			MenuItem mi = new MenuItem (item_name);
			visualizers.Add (mi, visualizer);

			mi.Activated += delegate (object obj, EventArgs args) {
				MethodDefinition md = (GetSelectedObject () as MethodDefinition);
				if (md == null)
					return;

				IMethodVisualizer mv = visualizers [(obj as MenuItem)] as IMethodVisualizer;
				if (mv == null)
					return;

				string title = String.Format ("{0} for {1}", mv.Name, md.Name);
				
				Tab tab = new Tab (title);
				tab.Content =  mv.GetWidget (md);
				tab.CloseButtonClicked += delegate (object o, EventArgs ea) {
					notebook.Remove ((o as Tab).Content);
				};
				
				int new_page = notebook.AppendPage (tab.Content, tab);
				notebook.Page = new_page;
			};
			popup_methods.Append (mi);
			popup_methods.ShowAll ();
		}

		private void RemoveExtension (IVisualizer visualizer)
		{
			MenuItem mi = null;
			foreach (KeyValuePair<MenuItem,IVisualizer> kvp in visualizers) {
				if (kvp.Value == visualizer) {
					mi = kvp.Key;
					break;
				}
			}
			if (mi != null) {
				visualizers.Remove (mi);
				
				if (visualizer is IAssemblyVisualizer)
					popup_assemblies.Remove (mi);
				if (visualizer is ITypeVisualizer)
					popup_types.Remove (mi);
				if (visualizer is IMethodVisualizer)
					popup_methods.Remove (mi);
			}
		}

		void OnAssemblyExtensionChanged (object s, ExtensionNodeEventArgs args)
		{
			switch (args.Change) {
			case ExtensionChange.Add:
				AddAssemblyExtension (args.ExtensionObject as IAssemblyVisualizer);
				break;
			case ExtensionChange.Remove:
				RemoveExtension (args.ExtensionObject as IVisualizer);
				break;
			}
		}

		void OnTypeExtensionChanged (object s, ExtensionNodeEventArgs args)
		{
			switch (args.Change) {
			case ExtensionChange.Add:
				AddTypeExtension (args.ExtensionObject as ITypeVisualizer);
				break;
			case ExtensionChange.Remove:
				RemoveExtension (args.ExtensionObject as IVisualizer);
				break;
			}
		}

		void OnMethodExtensionChanged (object s, ExtensionNodeEventArgs args)
		{
			switch (args.Change) {
			case ExtensionChange.Add:
				AddMethodExtension (args.ExtensionObject as IMethodVisualizer);
				break;
			case ExtensionChange.Remove:
				RemoveExtension (args.ExtensionObject as IVisualizer);
				break;
			}
		}
	}
}
