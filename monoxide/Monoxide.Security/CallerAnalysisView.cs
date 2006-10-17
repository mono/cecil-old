//
// CallerAnalysisView.cs
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
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

using Gtk;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Monoxide.Framework.PlugIns;

namespace Monoxide.Security {

	public class CallerAnalysisView : IGraphicDisplay, IMethodView {

		static private Hashtable clusters = new Hashtable ();
		static private ArrayList calls = new ArrayList ();

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
			get { return "Callers Analysis"; }
		}

		public void Render (MethodDefinition method)
		{
			if (method != null) {
				if (_display) {
					_image.FromFile = BuildDotImage (method);
					_image.Visible = true;
				}
			}
		}

		// don't duplicate entries (as they will complicate the resulting graph)
		private void AddToList (MethodDefinition method, IList list)
		{
			if (!list.Contains (method))
				list.Add (method);
		}

		// return true for public/protected so the analysis can be stopped
		private bool Add (MethodDefinition method)
		{
			Cluster c = (Cluster)clusters[method.DeclaringType];
			if (c == null) {
				c = new Cluster (method.DeclaringType);
				clusters.Add (method.DeclaringType, c);
			}

			if ((method.ImplAttributes & MethodImplAttributes.InternalCall) != 0)
				AddToList (method, c.InternalCalls);

			if (method.Name == ".cctor") {
				// static constructors are specials because we don't know when they are
				// called (so we don't know what's on the stack at that time!)
				AddToList (method, c.Specials);
			}

			if ((method.Attributes & MethodAttributes.Private) != 0) {
				AddToList (method, c.Others);
			} else if ((method.Attributes & MethodAttributes.Public) == MethodAttributes.Family) {
				// protected
				AddToList (method, c.Protected);
				return true;
			} else if ((method.Attributes & MethodAttributes.Public) == MethodAttributes.Public) {
				AddToList (method, c.Publics);
				return true;
			} else {
				// private, internal
				AddToList (method, c.Others);
			}
			return false;
		}

		private bool BackwardAnalysis (string header, MethodDefinition callee, MethodDefinition method)
		{
			string m = Helper.GetMethodString (method);

			if (callee != null) {
				string color = "black";
/*				switch (header) {
				case "call":
					break;
				case "callvirt":
					color = "blue";
					break;
				case "newobj":
					color = "green";
					break;
				default:
					break;
				}*/
				string s = String.Format ("\t\"{0}\" -> \"{1}\" [color={2}{3}]",
					m, Helper.GetMethodString (callee), color, Helper.GetSecurity (method, callee));
				if (!calls.Contains (s))
					calls.Add (s);
			}

			if (Add (method) && (callee != null))
				return true; // public/protected - stop analysis

			ArrayList list = (CallerPlugIn.calledFrom [method] as ArrayList);
			if (list == null)
				return false;

			foreach (Calls c in list) {
				BackwardAnalysis (c.How, method, c.Callee);
			}

			return true;
		}

		private string BuildDotFile (MethodDefinition method)
		{
			StringBuilder dot = new StringBuilder ();
			dot.AppendFormat ("digraph Test {{{0}", Environment.NewLine);
//		dot.AppendFormat ("\tconcentrate=true;{0}", Environment.NewLine);
			//		dot.AppendFormat ("\tlabel=\"{0}\" [fontname=tahoma,fontsize=10]{1}", method.ToString (), Environment.NewLine);
			dot.AppendFormat ("\tlabel=\"{0}\";{1}", method.ToString (), Environment.NewLine);
			dot.AppendFormat ("\tfontname=tahoma;{0}", Environment.NewLine);
			dot.AppendFormat ("\tfontsize=10;{0}", Environment.NewLine);
			dot.AppendFormat ("\tnode [fontname=tahoma,fontsize=8];{0}", Environment.NewLine);
			dot.AppendFormat ("\tedge [fontname=tahoma,fontsize=8,labelfontname=tahoma,labelfontsize=8];{0}", Environment.NewLine);

			bool hasInternalCall = false;
			bool hasPublic = false;
			bool hasProtected = false;
			bool hasSpecial = false;
			foreach (Cluster c in clusters.Values) {
				hasInternalCall |= c.HasInternalCall;
				hasPublic |= c.HasPublic;
				hasProtected |= c.HasProtected;
				hasSpecial |= c.HasSpecial;
				dot.Append (c.ToString ());
			}
			if (hasInternalCall)
				dot.AppendFormat ("\t\"runtime\";{0}", Environment.NewLine);
//			if (hasPublic)
//				dot.AppendFormat ("\t\"any public code\";{0}", Environment.NewLine);
//			if (hasProtected)
//				dot.AppendFormat ("\t\"any inherited code\";{0}", Environment.NewLine);
			if (hasSpecial)
				dot.AppendFormat ("\t\"unknown caller\";{0}", Environment.NewLine);

			// calls
			if (calls.Count > 0) {
				foreach (string call in calls) {
					dot.AppendFormat ("{0}{1}", call, Environment.NewLine);
				}
			}
			dot.Append ("}");
			return dot.ToString ();
		}

		private string BuildDotImage (MethodDefinition method)
		{
			// clean (optional ?)
			calls.Clear ();
			clusters.Clear ();

			BackwardAnalysis (">", null, method);

			string dotfile = Path.GetTempFileName ();
			string pngfile = Path.ChangeExtension (dotfile, "png");

			using (StreamWriter sw = new StreamWriter (dotfile)) {
				sw.WriteLine (BuildDotFile (method));
				sw.Close ();
			}

			string dotexe = null;
			if ((int)Environment.OSVersion.Platform == 128) {
				dotexe = "dot";
			} else {
				dotexe = @"C:\Program Files\ATT\Graphviz\bin\dot.exe";
				// -Tcmap 
			}
			string options = String.Format ("-Tpng \"{0}\" -o \"{1}\"", dotfile, pngfile);

			// process output dot file to get a PNG image
			ProcessStartInfo psi = new ProcessStartInfo (dotexe, options);
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			Process p = System.Diagnostics.Process.Start (psi);
			Console.WriteLine ("{0} {1}", psi.FileName, psi.Arguments);
			p.WaitForExit ();
			// return image filename
			return pngfile;
		}
	}

	struct Calls {
		public string How;
		public MethodDefinition Callee;

		public Calls (MethodDefinition callee, string how)
		{
			Callee = callee;
			How = how;
		}
	}

	class Cluster {
		private TypeReference _type;
		private ArrayList _internalcalls;
		private ArrayList _publics;
		private ArrayList _protected;
		private ArrayList _specials;
		private ArrayList _others;

		public Cluster (TypeReference type)
		{
			_type = type;
		}

		public IList InternalCalls {
			get {
				if (_internalcalls == null)
					_internalcalls = new ArrayList ();
				return _internalcalls;
			}
		}

		public IList Publics {
			get {
				if (_publics == null)
					_publics = new ArrayList ();
				return _publics;
			}
		}

		public IList Protected {
			get {
				if (_protected == null)
					_protected = new ArrayList ();
				return _protected;
			}
		}

		public IList Specials {
			get {
				if (_specials == null)
					_specials = new ArrayList ();
				return _specials;
			}
		}

		public IList Others {
			get {
				if (_others == null)
					_others = new ArrayList ();
				return _others;
			}
		}

		public bool HasInternalCall {
			get { return ((_internalcalls != null) && (_internalcalls.Count > 0)); }
		}

		public bool HasPublic {
			get { return ((_publics != null) && (_publics.Count > 0)); }
		}

		public bool HasProtected {
			get { return ((_protected != null) && (_protected.Count > 0)); }
		}

		public bool HasSpecial {
			get { return ((_specials != null) && (_specials.Count > 0)); }
		}

		private string GetClassLabel ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (_type.FullName);
			return sb.ToString ();
		}
		
		private string GetScopeName (IMetadataScope scope)
		{
			if (scope == null)
				return String.Empty;
			
			ModuleDefinition md = (scope as ModuleDefinition);
			if (md != null)
				return md.Name.Replace (", ", ",\\n");
				
			AssemblyNameReference anr = (scope as AssemblyNameReference);
			if (anr != null)
				return anr.FullName.Replace (", ", ",\\n");
			
			Console.WriteLine ("GetScopeName {0}", scope.GetType ());
			return String.Empty;
		}

		// FIXME : Determine a maximum to stop generation
		public override string ToString ()
		{
			// some things don't go inside the cluster
			StringBuilder noncluster = new StringBuilder ();

			StringBuilder dot = new StringBuilder ();
			dot.AppendFormat ("\tsubgraph cluster{0} {{{1}", Math.Abs (GetHashCode ()), Environment.NewLine);

			bool publicType = false;
			TypeDefinition type = (_type as TypeDefinition);
			if (type != null) {
				// much more interesting info in a definition
				if ((type.Attributes & TypeAttributes.Public) != 0) {
					publicType = true;
					dot.AppendFormat ("\t\tcolor=blue");
				}
			}

			dot.AppendFormat ("\t\tlabel = \"{0}\";{1}", GetClassLabel (), Environment.NewLine);
	//		dot.AppendFormat ("\t\tstyle=filled;{0}\t\tcolor=lightgrey;{0}", Environment.NewLine);
			dot.AppendFormat ("\t\tfontname=tahoma;{0}\t\tfontsize=8;{0}", Environment.NewLine);
			dot.AppendFormat ("\t\tlabelloc=b;{0}\t\tlabeljust=l;{0}", Environment.NewLine); 
	//		dot.AppendFormat ("\t\tnode [style=filled,color=white];{0}", Environment.NewLine);
	//		dot.AppendFormat ("fontname=tahoma;fontsize=8;,labelfontname=tahoma,labelfontsize=8];{0}", Environment.NewLine);

			// icalls
			if (HasInternalCall) {
				foreach (MethodDefinition method in _internalcalls) {
					string icall = Helper.GetMethodString (method);
					dot.AppendFormat ("\t\t\"{0}\" [shape=box,peripheries=2];{1}", icall, Environment.NewLine);
					noncluster.AppendFormat ("\t\"{0}\" -> \"runtime\" [label=\"icall\"{1}];{2}", icall, Helper.GetSecurity (method, null), Environment.NewLine);
				}
			}
			// publics
			if (HasPublic) {
				foreach (MethodDefinition method in _publics) {
					string pub = Helper.GetMethodString (method);
					string url = Helper.Url (method);
					string style = String.Empty;
					if ((method.Attributes & MethodAttributes.Static) == MethodAttributes.Static)
						style = ",style=bold";
					if ((method.Attributes & (MethodAttributes.Virtual | MethodAttributes.Final)) == MethodAttributes.Virtual)
						style = ",style=dashed";
					dot.AppendFormat ("\t\t\"{0}\" [shape=box,color=blue{1}{2}];{3}", pub, url, style, Environment.NewLine);

					string caller = "any code from\\n";
					if (publicType) {
						caller += "anywhere";
					} else {
						caller += GetScopeName (_type.Scope);
					}
					noncluster.AppendFormat ("\t\"{0}\" -> \"{1}\" [style=dotted{2}];{3}", caller, pub, Helper.GetSecurity (null, method), Environment.NewLine);
				}
			}
			// protected
			if (HasProtected) {
				foreach (MethodDefinition method in _protected) {
					string pub = Helper.GetMethodString (method);
					string url = Helper.Url (method);

					string style = String.Empty;
					if ((method.Attributes & (MethodAttributes.Virtual | MethodAttributes.Final)) == MethodAttributes.Virtual)
						style = ",style=dashed";

					dot.AppendFormat ("\t\t\"{0}\" [shape=box,color=blueviolet{1}{2}];{3}", pub, style, url, Environment.NewLine);

					string caller = "any inherited class";
					if (!publicType) {
						caller += String.Concat (" from\\n", GetScopeName (_type.Scope));
					}
					noncluster.AppendFormat ("\t\"{0}\" -> \"{1}\" [style=dotted{2}];{3}", caller, pub, Helper.GetSecurity (null, method), Environment.NewLine);
				}
			}
			// specials (e.g. .cctor)
			if (HasSpecial)	{
				// no URL available for internal stuff
				foreach (MethodDefinition method in _specials) {
					string pub = Helper.GetMethodString (method);
					dot.AppendFormat ("\t\t\"{0}\" [shape=box,color=green];{1}", pub, Environment.NewLine);
					noncluster.AppendFormat ("\t\"unknown caller\" -> \"{0}\" [style=dotted{1}];{2}", pub, Helper.GetSecurity (null, method), Environment.NewLine);
				}
			}
			// others
			if ((_others != null) && (_others.Count > 0)) {
				// no URL available for internal stuff
				foreach (MethodDefinition method in _others) {
					string pub = Helper.GetMethodString (method);
					dot.AppendFormat ("\t\t\"{0}\" [shape=box{1}];{2}", pub, Helper.GetSecurity (null, method), Environment.NewLine);
				}
			}
			dot.AppendFormat ("\t}}{0}{0}", Environment.NewLine).Append (noncluster);
			return dot.ToString ();
		}
	}

	public class Helper {

		public static string GetMethodString (MethodDefinition method)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (method.ReturnType.ReturnType.Name);
			sb.Append ("\\l");
			sb.Append (method.DeclaringType.Name);
			sb.Append ("\\l");
			sb.Append (method.Name);
			sb.Append ("\\l");
			sb.Append ("(");
			for (int i = 0; i < method.Parameters.Count; i++) {
				if (i > 0)
					sb.Append (",");
				sb.Append (method.Parameters[i].ParameterType.Name);
			}
			sb.Append (")\\l");
			return sb.ToString ();
		}

		public static string Url (MethodDefinition m)
		{
			if ((m.Attributes & MethodAttributes.Public) == 0)
				return String.Empty;

			string fullname = m.ToString ();
			string monodoc = "http://www.go-mono.com/docs/monodoc.ashx?link={0}";
			// remove return type
			string method = fullname.Substring (fullname.IndexOf (' ') + 1);
			// replace :: by .
			int index = method.IndexOf ("::");
			string classname = method.Substring (0, index);
			string methodname = method.Substring (index + 2);
			if (methodname.StartsWith ("get_") || methodname.StartsWith ("set_"))
			{
				// property - remove get_|set_ and ()
//				string property = methodname.Substring (4, methodname.Length - 6);
				string url = String.Format ("P:{0}.{1}", classname, methodname.Substring (4, methodname.Length - 6));
				monodoc = String.Format (monodoc, url);
			} else {
				// method
				string url = String.Format ("M:{0}.{1}", classname, methodname);
				monodoc = String.Format (monodoc, url);
			}
			return String.Concat (",URL=\"", monodoc, "\"");
		}

		public static string GetSecurity (MethodDefinition method, MethodDefinition callee)
		{
			string before = String.Empty;
			if ((method != null) && (method.SecurityDeclarations.Count > 0)) {
				// show stack modifiers that applies to callee
				before = Security (method.SecurityDeclarations, true);
			}
			string after = String.Empty;
			if ((callee != null) && (callee.SecurityDeclarations.Count > 0)) {
				// show actions that occurs before the callee is called
				after = Security (callee.SecurityDeclarations, false);
			}
			return String.Concat (before, after);
		}

		private static string GetEnvironmentPermission (EnvironmentPermission ep)
		{
			if (ep.IsUnrestricted ())
				return "  EnvironmentPermission - Unrestricted\\l";

			StringBuilder sb = new StringBuilder ("  EnvironmentPermission\\l");
			string s = ep.GetPathList (EnvironmentPermissionAccess.Read);
			if ((s != null) && (s.Length > 0))
				sb.AppendFormat ("    Read: {0}\\l", s);
			s = ep.GetPathList (EnvironmentPermissionAccess.Write);
			if ((s != null) && (s.Length > 0))
				sb.AppendFormat ("    Write: {0}\\l", s);
			return sb.ToString ();
		}

		private static string GetFileIOPermission (FileIOPermission ep)
		{
			if (ep.IsUnrestricted ())
				return "  FileIOPermission - Unrestricted\\l";

			StringBuilder sb = new StringBuilder ("  FileIOPermission\\l");
/*		string[] list = ep.GetPathList (FileIOPermissionAccess.PathDiscovery);
		if ((list != null) && (list.Length > 0)) {
						sb.AppendFormat ("    PathDiscovery: {0}\\l", s);
					}
					s = ep.GetPathList (FileIOPermissionAccess.Read);
					if ((s != null) && (s.Length > 0))
						sb.AppendFormat ("    Read: {0}\\l", s);
					s = ep.GetPathList (FileIOPermissionAccess.Write);
					if ((s != null) && (s.Length > 0))
						sb.AppendFormat ("    Write: {0}\\l", s);
					s = ep.GetPathList (FileIOPermissionAccess.Append);
					if ((s != null) && (s.Length > 0))
						sb.AppendFormat ("    Append: {0}\\l", s);*/
			return sb.ToString ();
		}

		private static string GetReflectionPermission (ReflectionPermission rp)
		{
			return String.Format ("  ReflectionPermission\\l    {0}\\l",
				(rp as ReflectionPermission).Flags);
		}

		private static string GetIsolatedStorageFilePermission (IsolatedStorageFilePermission isfp)
		{
			return String.Format ("  IsolatedStorageFilePermission\\l    {0}\\l",
				(isfp as IsolatedStorageFilePermission).UsageAllowed);
		}

		private static string Security (SecurityDeclarationCollection declarations, bool stackmods)
		{
			StringBuilder sb = null;
			foreach (SecurityDeclaration declsec in declarations) {
				switch (declsec.Action) {
					case Mono.Cecil.SecurityAction.Assert:
					case Mono.Cecil.SecurityAction.PermitOnly:
					case Mono.Cecil.SecurityAction.Deny:
						if (!stackmods)
							continue;
						break;
					default:
						if (stackmods)
							continue;
						break;
				}

				if (sb == null) {
					sb = new StringBuilder ();
					sb.AppendFormat (",{0}label=\"", !stackmods ? String.Empty /*"head"*/ : "tail");
				}
				sb.AppendFormat ("{0}\\l", declsec.Action);
				PermissionSet pset = declsec.PermissionSet;
				if (pset.Count > 0) {
					foreach (IPermission p in pset) {
						if (p is SecurityPermission) {
							sb.AppendFormat ("  SecurityPermission\\l    {0}\\l", (p as SecurityPermission).Flags);
						} else if (p is ReflectionPermission) {
							sb.Append (GetReflectionPermission (p as ReflectionPermission));
						} else if (p is FileIOPermission) {
							sb.Append (GetFileIOPermission (p as FileIOPermission));
						} else if (p is EnvironmentPermission) {
							sb.Append (GetEnvironmentPermission (p as EnvironmentPermission));
						} else if (p is IsolatedStorageFilePermission) {
							sb.Append (GetIsolatedStorageFilePermission (p as IsolatedStorageFilePermission));
						} else {
							string ps = p.ToString ().Replace ('"', '\'');
							ps = ps.Replace (Environment.NewLine, "\\l");
							sb.AppendFormat ("  {0}\\l", ps);
						}
					}
				} else if (pset.IsUnrestricted ()) {
					sb.Append ("  PermissionSet Unrestricted\\l");
				}
			}
			if (sb == null)
				return String.Empty;
			return sb.Append ("\"").ToString ();
		}
	}
}
