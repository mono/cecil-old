// TreeView.cs - Fun TreeView demo
//
// Author: Kristian Rietveld <kris@gtk.org>
//
// (c) 2002 Kristian Rietveld

// A rewrite of the TreeView demo using Cecil
// Author: Jb Evain <jb@evain.net>

namespace Cecil.Feed {
    
	using System;
	using Gtk;

    using Mono.Cecil;

	public class TreeViewDemo : IReflectionStructureVisitor {
        
        private string m_file;
        
		private TreeStore m_store = null;
		private int m_count = 0;

        private TreeIter m_cursor;
        private bool m_newBranch = false;
        private bool m_root = true;
		
		public TreeViewDemo(string file) {
            
            m_file = file;
            
			DateTime start = DateTime.Now;

			Application.Init ();
			
			PopulateStore ();

			Window win = new Window("Cecil ~ Feed");
			win.DeleteEvent += new DeleteEventHandler(this.DeleteCB);
			win.SetDefaultSize(640,480);

			ScrolledWindow sw = new ScrolledWindow ();
			win.Add (sw);

			TreeView tv = new TreeView(m_store);
			tv.HeadersVisible = true;
			tv.EnableSearch = false;

			tv.AppendColumn ("Name", new CellRendererText (), "text", 0);
			tv.AppendColumn ("Type", new CellRendererText (), "text", 1);

			sw.Add (tv);

			win.ShowAll ();
			
			Console.WriteLine (m_count + " nodes added.");
			Console.WriteLine ("Startup time: " + DateTime.Now.Subtract (start));
			Application.Run ();
		}
        
        public void Visit(IAssemblyDefinition asm) {
            AddToTree(m_file, "AssemblyDefinition");
        }
        
        public void Visit(IAssemblyName name) {
            AddToTree(name.FullName, "AssemblyName");
        }
        
        public void Visit(IAssemblyNameReferenceCollection names) {
            m_newBranch = false;
        }
            
        public void Visit(IAssemblyNameReference name) {
            AddToTree(name.FullName, "AssemblyNameReference");
        }
        
        public void Visit(IResourceCollection resources) {
            m_newBranch = false;
        }
        
        public void Visit(IEmbeddedResource res) {
            AddToTree(res.Name, "EmbeddedResource");
        }
        
        public void Visit(ILinkedResource res) {
            AddToTree(res.Name, "LinkedResource");
        }
        
        public void Visit(IModuleDefinition module) {
            AddToTree(module.Name, "ModuleDefinition");
        }
        
        public void Visit(IModuleDefinitionCollection modules) {
            m_newBranch = true;
        }
        
        public void Visit(IModuleReference module) {
            AddToTree(module.Name, "ModuleReference");
        }
        
        public void Visit(IModuleReferenceCollection modules) {
            m_newBranch = false;
        }

		private void PopulateStore () {
			m_store = new TreeStore (typeof (string), typeof (string));

			IAssemblyDefinition asm = AssemblyFactory.GetAssembly(m_file);
            asm.Accept(this);
		}

        private void AddToTree(string name, string type) {
            if (m_newBranch) {
                m_cursor = m_store.AppendValues(m_cursor, name, type);
                m_newBranch = false;
            } else {
                m_store.AppendValues(m_cursor, name, type);
            }
            if (m_root) {
                m_cursor = m_store.AppendValues(name, type);
                m_root = false;
            }
            m_count++;
        }
        
        private void DeleteCB(object o, DeleteEventArgs args) {
			Application.Quit ();
		}

		public static void Main (string[] args) {
			if (args.Length == 0) {
                Console.WriteLine("usage: cecil-feed.exe assembly");
                return;
            }
            try {
                TreeViewDemo tvd = new TreeViewDemo(args[0]);
            } catch (Exception e) {
                Console.WriteLine(e);
            }
		}
	}
}
