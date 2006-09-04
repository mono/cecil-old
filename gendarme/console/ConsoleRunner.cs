//
// Gendarme Console Runner
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
using System.Xml;

using Mono.Cecil;
using Gendarme.Framework;

class ConsoleRunner : Runner {

	private const string defaultConfiguration = "rules.xml";
	private const string defaultRuleSet = "default";

	private string config;
	private string set;
	private ArrayList assemblies;

	private static Assembly assembly;

	static Assembly Assembly {
		get {
			if (assembly == null)
				assembly = Assembly.GetExecutingAssembly ();
			return assembly;
		}
	}

	static string GetFullPath (string filename)
	{
		if (Path.GetDirectoryName (filename) != String.Empty)
			return filename;
		return Path.Combine (Path.GetDirectoryName (Assembly.Location), filename);
	}

	static string GetNext (string[] args, int index, string defaultValue)
	{
		if ((args == null) || (index < 0) || (index >= args.Length))
			return defaultValue;
		return args [index];
	}

	bool ParseOptions (string[] args)
	{
		// defaults
		config = GetFullPath (defaultConfiguration);
		set = defaultRuleSet;
		assemblies = new ArrayList ();

		// TODO - we probably want (i.e. later) the possibility to 
		// include/exclude certain rules from executing
		for (int i=0; i < args.Length; i++) {
			switch (args [i]) {
			case "--config":
				config = GetNext (args, ++i, defaultConfiguration);
				break;
			case "--set":
				set = GetNext (args, ++i, defaultRuleSet);
				break;
			case "--debug":
				debug = true;
				break;
			case "--help":
				return false;
			default:
				string filename = args[i];
				if (filename.IndexOfAny (new char[] { '*', '?' }) >= 0) {
					string[] files = Directory.GetFiles (Path.GetDirectoryName (filename),
						Path.GetFileName (filename));
					foreach (string file in files) {
						assemblies.Add (file);
					}
				} else {
					assemblies.Add (Path.GetFullPath (filename));
				}
				break;
			}
		}
		return (assemblies.Count > 0);
	}

	static string GetAttribute (XmlElement xel, string name, string defaultValue)
	{
		XmlAttribute xa = xel.Attributes [name];
		if (xa == null)
			return defaultValue;
		return xa.Value;
	}

	bool LoadConfiguration ()
	{
		XmlDocument doc = new XmlDocument ();
		doc.Load (config);
		if (doc.DocumentElement.Name != "gendarme")
			return false;

		bool result = false;
		foreach (XmlElement ruleset in doc.DocumentElement.SelectNodes("ruleset")) {
			if (ruleset.Attributes["name"].Value != set)
				continue;
			foreach (XmlElement assembly in ruleset.SelectNodes("rules")) {
				string include = GetAttribute (assembly, "include", "*");
				string exclude = GetAttribute (assembly, "exclude", String.Empty);
				string from = GetFullPath (GetAttribute (assembly, "from", String.Empty));
				try {
					int n = LoadRulesFromAssembly (from, include, exclude);
					result = (result || (n > 0));
				}
				catch (Exception e) {
					Console.WriteLine ("Error reading rules{1}Details: {0}", e, Environment.NewLine);
					return false;
				}
			}
		}
		return result;
	}

	static void Header ()
	{
		Assembly a = Assembly.GetExecutingAssembly();
		Version v = a.GetName ().Version;
		if (v.ToString () != "0.0.0.0") {
			Console.WriteLine ("Gendarme v{0}", v);
			object[] attr = a.GetCustomAttributes (typeof (AssemblyCopyrightAttribute), false);
			if (attr.Length > 0)
				Console.WriteLine (((AssemblyCopyrightAttribute) attr [0]).Copyright);
		} else {
			Console.WriteLine ("Gendarme - Development Snapshot");
		}
		Console.WriteLine ();
	}

	static void Help ()
	{
		Console.WriteLine ("Usage: gendarme [--config configfile] [--set ruleset] assembly");
		Console.WriteLine ("Where");
		Console.WriteLine ("  --config configfile\tSpecify the configuration file. Default is 'rules.xml'.");
		Console.WriteLine ("  --set ruleset\t\tSpecify the set of rules to verify. Default is '*'.");
		Console.WriteLine ("  --debug\t\tEnable debugging output.");
		Console.WriteLine ("  assembly\t\tSpecify the assembly to verify.");
		Console.WriteLine ();
	}

	static int Main (string[] args)
	{
		Header ();
		ConsoleRunner runner = new ConsoleRunner ();

		try {
			if (!runner.ParseOptions (args)) {
				Help ();
				return 1;
			}
			if (!runner.LoadConfiguration ()) {
				return 1;
			}
		}
		catch (Exception e) {
			Console.WriteLine (e);
			return 1;
		}

		foreach (string assembly in runner.assemblies) {
			AssemblyDefinition ad = null;
			try {
				ad = AssemblyFactory.GetAssembly (assembly);
			}
			catch (Exception e) {
				Console.WriteLine ("Error processing assembly '{0}'{1}Details: {2}",
					assembly, Environment.NewLine, e);
			}
			try {
				runner.Process (ad);
			}
			catch (Exception e) {
				Console.WriteLine ("Error executing rules on assembly '{0}'{1}Details: {2}",
					assembly, Environment.NewLine, e);
			}
		}

		int i = 0;
		foreach (Violation v in runner.Violations.List) {
			RuleInformation ri = RuleInformationManager.GetRuleInformation (v.Rule);
			Console.WriteLine ("{0}. {1}", ++i, ri.Name);
			Console.WriteLine ();
			Console.WriteLine ("Problem: {0}", String.Format (ri.Problem, v.Violator));
			Console.WriteLine ();
			if(v.Messages != null && v.Messages.Count > 0) {
				Console.WriteLine ("Details:");
				foreach(object message in v.Messages) {
					Console.WriteLine("  {0}", message);
				}
				Console.WriteLine ();
			}
			Console.WriteLine ("Solution: {0}", String.Format (ri.Solution, v.Violator));
			Console.WriteLine ();
			string url = ri.Uri;
			if (url.Length > 0) {
				Console.WriteLine ("More info available at: {0}", url);
				Console.WriteLine ();
			}
			Console.WriteLine ();
		}

		if (i == 0) {
			Console.WriteLine ("No rule's violation were found.");
			return 0;
		}
		return 1;
	}
}
