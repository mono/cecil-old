//
// AssemblyDependenciesView.cs
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
using System.Text;

using Mono.Cecil;
using Monoxide.Framework.PlugIns;
using Monoxide.Framework.Dot;

using Gtk;
using Pango;

namespace Monoxide.Metadata {

	public class AssemblyDependenciesView : IGraphicDisplay, IAssemblyView {

		private Image _image;
		private bool _display;

		public void SetUp (Image image)
		{
			_image = image;
		}

		public bool Display {
			get { return _display; }
			set { _display = value; }
		}

		public string Name {
			get { return "Assembly Dependencies"; }
		}


		public void Render (AssemblyDefinition assembly)
		{
			if ((assembly != null) && _display) {
				_image.FromFile = DotHelper.BuildDotImage (GetDotData (assembly));
				_image.Visible = true;
			}
		}

		AssemblyDefinition GetAssemblyFromReference (AssemblyNameReference assemblyName)
		{
			string fullname = assemblyName.FullName;
			foreach (AssemblyDefinition assembly in MetadataPlugIn.Assemblies) {
				if (assembly.Name.FullName == fullname)
					return assembly;
			}
			return null;
		}

		// move to DotHelper

		public static string GetTitleName (AssemblyNameReference assemblyName)
		{
			return assemblyName.FullName;
		}

		private string GetFriendlyName (AssemblyNameReference assemblyName)
		{
			return String.Concat ("\"", assemblyName.FullName.Replace (", ", ",\\n"), "\"");
		}

		private void RecurseAssembly (AssemblyDefinition assembly, Digraph dot)
		{
			// 2 assemblies can depend on each other (e.g. System.dll and System.Xml.dll)
			if (!AddToCluster (assembly.Name))
				return;

			string name = GetFriendlyName (assembly.Name);

			if (AllowPartiallyTrustedCallers (assembly)) {
				if (!_partialTrustCallers.Contains (name))
					_partialTrustCallers.Add (name);
			} else {
				if (!_trustedCallers.Contains (name))
					_trustedCallers.Add (name);
			}

			foreach (ModuleDefinition module in assembly.Modules) {
				foreach (AssemblyNameReference reference in module.AssemblyReferences) {
					AddToCluster (reference);
					string refname = GetFriendlyName (reference);

					dot.Edges.Add (new Edge (name, refname));

					AssemblyDefinition ad = GetAssemblyFromReference (reference);
					if (ad != null) {
						RecurseAssembly (ad, dot);
					}
				}
			}
		}

		private Digraph GetDotData (AssemblyDefinition assembly)
		{
			if (assembly == null)
				return null;

			_clusters.Clear ();
			_partialTrustCallers.Clear ();
			_trustedCallers.Clear ();

			Digraph dot = new Digraph ();
			dot.Name = "AssemblyDependencies";
			dot.AutoConcentrate = true;
			dot.Label = GetTitleName (assembly.Name);
			dot.LabelLoc = "t";
			dot.FontName = "tahoma";
			dot.FontSize = 10;

			dot.DefaultNode = new Node ();
			dot.DefaultNode.Attributes["fontname"] = "tahoma";
			dot.DefaultNode.Attributes["fontsize"] = 8;

			dot.DefaultEdge = new Edge ();
			dot.DefaultNode.Attributes["fontname"] = "tahoma";
			dot.DefaultNode.Attributes["fontsize"] = 8;
			dot.DefaultNode.Attributes["labelfontname"] = "tahoma";
			dot.DefaultNode.Attributes["labelfontsize"] = 8;

			RecurseAssembly (assembly, dot);

			// process clusters (by public key tokens)
			foreach (DictionaryEntry de in _clusters) {
				string clusterName = (string) de.Key;
				ArrayList members = (ArrayList) de.Value;

				Subgraph sg = new Subgraph ();
				sg.Label = clusterName;
				sg.LabelLoc = "b";
				dot.Subgraphs.Add (sg);

				foreach (AssemblyNameReference reference in members) {
					Node n = new Node ();
					n.Label = GetFriendlyName (reference);
					n.Attributes["shape"] = "box";

					AssemblyDefinition ad = GetAssemblyFromReference (reference);
					if (ad != null) {
						// reference assembly is loaded in memory so we can do more analysis
						if (HasUnverifiableCode (ad)) {
							n.Attributes["color"] = "red";
						}
					} else {
						// default if assembly isn't loaded
						n.Attributes["style"] = "dashed";
					}
					sg.Nodes.Add (n);
				}
			}

			// process edges (i.e. callers)
			foreach (string s in _partialTrustCallers) {
				Edge e = new Edge ("\"partially\\ntrusted\\ncallers\\n\"", s);
				e.Attributes["style"] = "dotted";
				dot.Edges.Add (e);
			}
			foreach (string s in _trustedCallers) {
				Edge e = new Edge ("\"trusted\\ncallers\\n\"", s);
				e.Attributes["style"] = "dotted";
				dot.Edges.Add (e);
			}

			return dot;
		}

		private Hashtable _clusters = new Hashtable ();
		private ArrayList _partialTrustCallers = new ArrayList ();
		private ArrayList _trustedCallers = new ArrayList ();

		private bool AddToCluster (AssemblyNameReference assembly)
		{
			string clusterName = "null";
			byte[] token = assembly.PublicKeyToken;
			if ((token != null) && (token.Length > 0)) {
				StringBuilder sb = new StringBuilder (16);
				foreach (byte b in token)
					sb.Append (b.ToString ("x2"));

				// well known public key tokens (e.g. ECMA, MS, Mono)
				switch (sb.ToString ()) {
				case "b77a5c561934e089":
					clusterName = "ECMA";
					break;
				case "b03f5f7f11d50a3a":
					clusterName = "Microsoft";
					break;
				case "35e10195dab3c99f":
					clusterName = "Mono Gtk#";
					break;
				case "0738eb9f132ed756":
					clusterName = "Mono";
					break;
				default:
					// group all others together
					clusterName = "Others";
					break;
				}
			}

			ArrayList cluster = (ArrayList) _clusters[clusterName];
			if (cluster == null) {
				cluster = new ArrayList ();
				_clusters[clusterName] = cluster;
			}

			if (cluster.Contains (assembly))
				return false;
			cluster.Add (assembly);
			return true;
		}

		private bool AllowPartiallyTrustedCallers (AssemblyDefinition assembly)
		{
			foreach (CustomAttribute custom in assembly.CustomAttributes) {
				if (custom.Constructor.DeclaringType.FullName == "System.Security.AllowPartiallyTrustedCallersAttribute")
					return true;
			}

			return false;
		}

		private bool HasUnverifiableCode (AssemblyDefinition assembly)
		{
			foreach (ModuleDefinition module in assembly.Modules) {
				foreach (CustomAttribute custom in module.CustomAttributes) {
					if (custom.Constructor.DeclaringType.FullName == "System.Security.UnverifiableCodeAttribute")
						return true;
				}
			}
			return false;
		}
	}
}
