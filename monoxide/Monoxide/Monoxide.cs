//
// Monoxide.cs
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
using System.Reflection;
using System.Text;

using Gtk;
using Glade;
using Pango;

using Mono.Cecil;
using Monoxide.Framework.PlugIns;

namespace Monoxide {

	class Program {

		[Widget] private TreeView treeview;
		[Widget] private Gtk.Image image;
		[Widget] private Label objectLabel;
		[Widget] private Notebook pluginNotebook;
		[Widget] private MenuItem viewMenuItem;

		private TreeStore m_store = null;
		private TreeIter m_cursor;

		private ArrayList _plugins;
		private ArrayList _views;
		private IGraphicDisplay currentViewer;
		private Hashtable plugInMenuMapper = new Hashtable ();
		private ArrayList _assemblies;
		private Config _config;

		public Program (string file)
		{
			Application.Init ();
			Glade.XML xml = Glade.XML.FromAssembly ("monoxide.glade", "mainWindow", String.Empty);
			xml.Autoconnect (this);

			m_store = new TreeStore (typeof (string), typeof (object));

			treeview.Model = m_store;
			treeview.EnableSearch = false;
			treeview.AppendColumn ("Object", new CellRendererText (), "text", 0);
			treeview.Selection.Changed += OnSelectionChanged;
			OnSelectionChanged (null, EventArgs.Empty);

			if (file != null) {
				// open all future assemblies from the same directory (by default)
				string dir = Path.GetDirectoryName (file);
				if ((dir != null) && (dir.Length > 0)) {
					Directory.SetCurrentDirectory (dir);
				}

				switch (Path.GetExtension (file)) {
				case ".workingset":
					Configuration.Load (file);
					foreach (string plugin in Configuration.WorkingSet.PlugIns) {
						LoadPlugIn (plugin, true);
					}
					foreach (string assembly in Configuration.WorkingSet.Assemblies) {
						LoadAssembly (assembly, true);
					}
					break;
				case ".exe":
				case ".dll":
					if (_plugins == null) {
						LoadPlugIn ("Monoxide.Ilasm.IlasmPlugIn, Monoxide.Ilasm", false);
						LoadPlugIn ("Monoxide.Metadata.MetadataPlugIn, Monoxide.Metadata", false);
						LoadPlugIn ("Monoxide.Security.CallerPlugIn, Monoxide.Security", false);
					}

					string fname = Path.GetFileName (file);
					string[] files = Directory.GetFiles (dir, fname);
					foreach (string f in files)
						LoadAssembly (f, true);
					break;
				}
			}

			// 'dot' debugging helper plugin
			if (Monoxide.Framework.Dot.DotDebugPlugIn.DebugDotOutput) {
				IPlugIn plugin = new Monoxide.Framework.Dot.DotDebugPlugIn ();
				AddPlugIn (plugin);
			}
		}

		public Program (string file, string fullname)
			: this (file)
		{
			int sep = fullname.IndexOf ("::");
			string type = fullname.Substring (0, sep);
			string method = fullname.Substring (sep + 2);
			foreach (AssemblyDefinition assembly in _assemblies) {
				TypeDefinition t = assembly.MainModule.Types[type];
				if (t != null) {
					foreach (MethodDefinition meth in t.Methods) {
						if (meth.Name == method) {
							SetMethod (meth);
							break;
						}
					}
				}
			}
		}

		public void LoadPlugIn (string classname, bool startup)
		{
			Type t = Type.GetType (classname);
			if (t != null) {
				IPlugIn plugin = (IPlugIn)Activator.CreateInstance (t, null);
				if (plugin != null) {
					AddPlugIn (plugin);
					if (!startup)
						Configuration.WorkingSet.PlugIns.Add (classname);
				}
			}
		}

		public Config Configuration {
			get {
				if (_config == null)
					_config = new Config ();
				return _config;
			}
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

		private void PopulateStore (AssemblyDefinition assembly) 
		{
			m_cursor = m_store.AppendValues (assembly.Name.FullName, assembly);
			foreach (ModuleDefinition module in assembly.Modules) {

				foreach (TypeDefinition type in module.Types) {
					TreeIter titer = m_cursor;

					m_cursor = m_store.AppendValues (m_cursor, type.ToString (), type);

					TreeIter miter = m_cursor;
					foreach (MethodDefinition method in type.Constructors) {
						m_store.AppendValues (miter, GetMethodName (method), method);
					}
					foreach (MethodDefinition method in type.Methods) {
						m_store.AppendValues (miter, GetMethodName (method), method);
					}
					m_cursor = titer;
				}
			}
			m_store.SetSortColumnId (0, SortType.Ascending);
		}

		private void OnMainWindowDelete (object o, DeleteEventArgs args)
		{
			Configuration.Save ("default.workingset");
			Application.Quit ();
		}

		private void OnSelectionChanged (object o, EventArgs args)
		{
			OnViewRefresh (o, args);
		}

		public void SetAssembly (AssemblyDefinition assembly)
		{
			if (assembly != null) {
				objectLabel.Text = assembly.Name.FullName;
				image.Visible = false;
			}

			foreach (IView view in _views) {
				IAssemblyView av = (view as IAssemblyView);
				if (av != null)
					av.Render (assembly);
			}
		}

		public void SetType (TypeDefinition type)
		{
			if (type != null) {
				objectLabel.Text = type.ToString ();
				image.Visible = false;
			}

			foreach (IView view in _views) {
				ITypeView tv = (view as ITypeView);
				if (tv != null)
					tv.Render (type);
			}
		}

		public void SetMethod (MethodDefinition method)
		{
			if (method != null) {
				objectLabel.Text = method.ToString ();
				image.Visible = false;
			}

			foreach (IView view in _views) {
				IMethodView mv = (view as IMethodView);
				if (mv != null)
					mv.Render (method);
			}
		}

		public void AddPlugIn (IPlugIn plugin)
		{
			if (plugin == null)
				return;
			if (_plugins == null)
				_plugins = new ArrayList ();
			if (_views == null)
				_views = new ArrayList ();

			_plugins.Add (plugin);

			foreach (IDisplay d in plugin.Displays) {
				_views.Add (d);

				IGraphicDisplay gd = (d as IGraphicDisplay);
				if (gd != null) {
					Menu view = (Menu)viewMenuItem.Submenu;
					if (view == null) {
						view = new Menu ();
						viewMenuItem.Submenu = view;
					}
					MenuItem plugInMenu = new MenuItem (gd.Name);
					plugInMenu.Activated += new EventHandler (plugInMenu_ActivateItem);
					plugInMenu.Visible = true;
					plugInMenuMapper.Add (plugInMenu, gd);
					view.Prepend (plugInMenu);

					gd.SetUp (image);
				}

				ICustomDisplay cd = (d as ICustomDisplay);
				if (cd != null) {
					cd.SetUp (pluginNotebook);
				}
			}
		}

		void plugInMenu_ActivateItem (object sender, EventArgs e)
		{
			if (currentViewer != null)
				currentViewer.Display = false;
				
			IGraphicDisplay viewer = (plugInMenuMapper[sender] as IGraphicDisplay);
			if (viewer != null) {
				currentViewer = viewer;
				currentViewer.Display = true;
				OnViewRefresh (sender, e);
			}
		}

		private void LoadAssembly (string filename, bool startup)
		{
			if (_assemblies == null)
				_assemblies = new ArrayList ();

			try {
				AssemblyDefinition ad = AssemblyFactory.GetAssembly (filename);
				_assemblies.Add (ad);

				if (_plugins.Count > 0) {
					// signal all plug ins of the new loaded assembly
					foreach (IPlugIn plugin in _plugins) {
						plugin.AddAssembly (ad);
					}
				}

				// add assembly to the treeview
				PopulateStore (ad);

				if (!startup)
					Configuration.WorkingSet.Assemblies.Add (filename);
			}
			catch (Exception e) {
				Console.WriteLine (e);
			}
		}

		// TODO: add multiple assemblies
		public void OnAddAssembly (object sender, EventArgs args)
		{
			using (FileSelection fs = new FileSelection ("Add assembly")) {
				bool ok = (fs.Run () == (int)Gtk.ResponseType.Ok);
				fs.Hide ();
				if (ok) {
					LoadAssembly (fs.Filename, false);
				}
			}
		}

		public static void Main (string[] args) 
		{
			try {
				switch (args.Length) {
				case 0:
					new Program (null);
					break;
				case 1:
					new Program (args [0]);
					break;
				default:
					new Program (args [0], args [1]);
					break;
				}

				Application.Run ();
			}
			catch (Exception e) {
				Console.WriteLine(e);
			}
		}
		
		// File Menu

		public void OnFileQuit (object sender, EventArgs args)
		{
			Application.Quit ();
		}
		
		// View Menu
		
		public void OnViewRefresh (object sender, EventArgs args)
		{
			if (sender == null)
				return;

			TreeIter iter;
			TreeModel model;
			if (treeview.Selection.GetSelected (out model, out iter)) {
				object obj = model.GetValue (iter, 1);
				
				SetAssembly (obj as AssemblyDefinition);
				SetType (obj as TypeDefinition);
				SetMethod (obj as MethodDefinition);
			}
		}

		// Help Menu
		
		public void OnHelpAbout (object sender, EventArgs args)
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
			}
		}
	}
}
