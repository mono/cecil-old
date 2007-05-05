//
// mono-api-info.cs - Dumps public assembly information to an xml file.
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// Copyright (C) 2003-2005 Novell, Inc (http://www.novell.com)
//

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Util.CorCompare.Cecil;

namespace Mono.AssemblyInfo
{
	public class Driver
	{
		public static int Main (string [] args)
		{
			if (args.Length == 0)
				return 1;

			AssemblyCollection acoll = new AssemblyCollection ();
			
			foreach (string fullName in args) {
				acoll.Add (fullName);
			}

			XmlDocument doc = new XmlDocument ();
			acoll.Document = doc;
			acoll.DoOutput ();

			XmlTextWriter writer = new XmlTextWriter (Console.Out);
			writer.Formatting = Formatting.Indented;
			XmlNode decl = doc.CreateXmlDeclaration ("1.0", null, null);
			doc.InsertBefore (decl, doc.DocumentElement);
			doc.WriteTo (writer);
			return 0;
		}
	}

	class AssemblyCollection
	{
		XmlDocument document;
		List<Mono.Cecil.AssemblyDefinition> assemblies;

		public AssemblyCollection ()
		{
			assemblies = new List<Mono.Cecil.AssemblyDefinition> ();
		}

		public bool Add (string name)
		{
			AssemblyDefinition ass = LoadAssembly (name);
			if (ass == null)
				return false;

			assemblies.Add (ass);
			return true;
		}

		public void DoOutput ()
		{
			if (document == null)
				throw new InvalidOperationException ("Document not set");

			XmlNode nassemblies = document.CreateElement ("assemblies", null);
			document.AppendChild (nassemblies);
			foreach (AssemblyDefinition a in assemblies) {
				AssemblyData data = new AssemblyData (document, nassemblies, a);
				data.DoOutput ();
			}
		}

		public XmlDocument Document {
			set { document = value; }
		}

		static AssemblyDefinition LoadAssembly (string assPath)
		{
			string pathOnly = assPath.Substring( 0, assPath.LastIndexOf (System.IO.Path.DirectorySeparatorChar) );
			string name = assPath.Substring (assPath.LastIndexOf (System.IO.Path.DirectorySeparatorChar) + 1);
			return (new AssemblyResolver (pathOnly)).Resolve (name);
		}
	}

	abstract class BaseData
	{
		protected XmlDocument document;
		protected XmlNode parent;

		protected BaseData (XmlDocument doc, XmlNode parent)
		{
			this.document = doc;
			this.parent = parent;
		}

		public abstract void DoOutput ();

		protected void AddAttribute (XmlNode node, string name, string value)
		{
			XmlAttribute attr = document.CreateAttribute (name);
			attr.Value = value;
			node.Attributes.Append (attr);
		}
	}

	class AssemblyData : BaseData
	{
		AssemblyDefinition ass;

		public AssemblyData (XmlDocument document, XmlNode parent, AssemblyDefinition ass)
			: base (document, parent)
		{
			this.ass = ass;
		}

		public override void DoOutput ()
		{
			if (document == null)
				throw new InvalidOperationException ("Document not set");

			XmlNode nassembly = document.CreateElement ("assembly", null);
			AssemblyNameDefinition aname = ass.Name;
			AddAttribute (nassembly, "name", aname.Name);
			AddAttribute (nassembly, "version", aname.Version.ToString ());
			parent.AppendChild (nassembly);
			AttributeData.OutputAttributes (document, nassembly, ass.CustomAttributes);
			TypeDefinitionCollection typesCollection = ass.MainModule.Types;
			if (typesCollection == null || typesCollection.Count == 0)
				return;
			object [] typesArray = new object [typesCollection.Count];
			for (int i = 0; i < typesCollection.Count; i++) {
				typesArray [i] = typesCollection [i];
			}
			Array.Sort (typesArray, TypeReferenceComparer.Default);
			
			XmlNode nss = document.CreateElement ("namespaces", null);
			nassembly.AppendChild (nss);

			string currentNS = "$%&$&";
			XmlNode ns = null;
			XmlNode classes = null;
			foreach (TypeDefinition t in typesArray) {
				if (t.Namespace == null || t.Namespace == "")
					continue;

				if ((t.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public) {
					continue;
				}

				if (t.DeclaringType != null)
					continue; // enforce !nested
				
				if (t.Namespace != currentNS) {
					currentNS = t.Namespace;
					ns = document.CreateElement ("namespace", null);
					AddAttribute (ns, "name", currentNS);
					nss.AppendChild (ns);
					classes = document.CreateElement ("classes", null);
					ns.AppendChild (classes);
				}
				
				TypeData bd = new TypeData (document, classes, t);
				bd.DoOutput ();
			}
		}
	}

	abstract class MemberData : BaseData
	{
		MemberReference [] members;

		public MemberData (XmlDocument document, XmlNode parent, MemberReference [] members)
			: base (document, parent)
		{
			this.members = members;
		}

		public override void DoOutput ()
		{
			XmlNode mclass = document.CreateElement (ParentTag, null);
			parent.AppendChild (mclass);

			foreach (MemberReference member in members) {
				XmlNode mnode = document.CreateElement (Tag, null);
				mclass.AppendChild (mnode);
				AddAttribute (mnode, "name", GetName (member));
				if (!NoMemberAttributes)
					AddAttribute (mnode, "attrib", GetMemberAttributes (member));

				AttributeData.OutputAttributes (document, mnode, GetCustomAttributes (member));

				AddExtraData (mnode, member);
			}
		}


		protected abstract CustomAttributeCollection GetCustomAttributes (MemberReference member);

		protected virtual void AddExtraData (XmlNode p, MemberReference memberDefenition)
		{
		}

		protected virtual string GetName (MemberReference memberDefenition)
		{
			return "NoNAME";
		}

		protected virtual string GetMemberAttributes (MemberReference memberDefenition)
		{
			return null;
		}

		public virtual bool NoMemberAttributes {
			get { return false; }
			set {}
		}

		public virtual string ParentTag {
			get { return "NoPARENTTAG"; }
		}
		
		public virtual string Tag {
			get { return "NoTAG"; }
		}
	}

	class TypeData : MemberData
	{
		TypeDefinition type;

		public TypeData (XmlDocument document, XmlNode parent, TypeDefinition type)
			: base (document, parent, null)
		{
			this.type = type;
		}

		protected override CustomAttributeCollection GetCustomAttributes (MemberReference member) {
			return ((TypeDefinition) member).CustomAttributes;
		}

		public override void DoOutput ()
		{
			if (document == null)
				throw new InvalidOperationException ("Document not set");

			XmlNode nclass = document.CreateElement ("class", null);
			AddAttribute (nclass, "name", type.Name);
			string classType = GetClassType (type);
			AddAttribute (nclass, "type", classType);

			if (type.BaseType != null)
				AddAttribute (nclass, "base", type.BaseType.ToString ());

			if (type.IsSealed)
				AddAttribute (nclass, "sealed", "true");

			if (type.IsAbstract)
				AddAttribute (nclass, "abstract", "true");

			if ( (type.Attributes & TypeAttributes.Serializable) != 0 || type.IsEnum)
				AddAttribute (nclass, "serializable", "true");

			string charSet = GetCharSet (type);
			AddAttribute (nclass, "charset", charSet);

			string layout = GetLayout (type);
			if (layout != null)
				AddAttribute (nclass, "layout", layout);

			parent.AppendChild (nclass);
			
			AttributeData.OutputAttributes (document, nclass, GetCustomAttributes(type));

			InterfaceCollection interfaces = type.Interfaces;
			if (interfaces != null && interfaces.Count > 0) {
				XmlNode ifaces = document.CreateElement ("interfaces", null);
				nclass.AppendChild (ifaces);
				foreach (TypeReference t in interfaces) {
					try {
						if (!Mono.Util.CorCompare.Cecil.TypeHelper.IsPublic (t)) {
							// we're only interested in public interfaces
							continue;
						}
					}
					catch { }//TODO bug: t.Module is sometimes null.
					XmlNode iface = document.CreateElement ("interface", null);
					AddAttribute (iface, "name", t.ToString ());
					ifaces.AppendChild (iface);
				}
			}

#if NET_2_0
			// Generic constraints
			Type [] gargs = type.GetGenericArguments ();
			XmlElement ngeneric = (gargs.Length == 0) ? null :
				document.CreateElement ("generic-type-constraints");
			foreach (Type garg in gargs) {
				Type [] csts = garg.GetGenericParameterConstraints ();
				if (csts.Length == 0 || csts [0] == typeof (object))
					continue;
				XmlElement el = document.CreateElement ("generic-type-constraint");
				el.SetAttribute ("name", garg.ToString ());
				el.SetAttribute ("generic-attribute",
					garg.GenericParameterAttributes.ToString ());
				ngeneric.AppendChild (el);
				foreach (Type ct in csts) {
					XmlElement cel = document.CreateElement ("type");
					cel.AppendChild (document.CreateTextNode (ct.FullName));
					el.AppendChild (cel);
				}
			}
			if (ngeneric != null && ngeneric.FirstChild != null)
				nclass.AppendChild (ngeneric);
#endif

			ArrayList members = new ArrayList ();

			FieldDefinition [] fields = GetFields (type);
			if (fields.Length > 0) {
				Array.Sort (fields, MemberReferenceComparer.Default);
				FieldData fd = new FieldData (document, nclass, fields);
				// Special case for enum fields
				// TODO:Special case for enum fields
				//if (classType == "enum") {
				//    string etype = fields [0].GetType ().ToString ();
				//    AddAttribute (nclass, "enumtype", etype);
				//}
				members.Add (fd);
			}

			MethodDefinition [] ctors = GetConstructors (type);
			if (ctors.Length > 0) {
				Array.Sort (ctors, MemberReferenceComparer.Default);
				members.Add (new ConstructorData (document, nclass, ctors));
			}

			PropertyDefinition[] properties = GetProperties (type);
			if (properties.Length > 0) {
				Array.Sort (properties, MemberReferenceComparer.Default);
				members.Add (new PropertyData (document, nclass, properties));
			}

			EventDefinition [] events = GetEvents (type);
			if (events.Length > 0) {
				Array.Sort (events, MemberReferenceComparer.Default);
				members.Add (new EventData (document, nclass, events));
			}

			MethodDefinition [] methods = GetMethods (type);
			if (methods.Length > 0) {
				Array.Sort (methods, MemberReferenceComparer.Default);
				members.Add (new MethodData (document, nclass, methods));
			}

			foreach (MemberData md in members)
				md.DoOutput ();

			NestedTypeCollection nested = type.NestedTypes;
			//remove non public(familiy) and nested in second degree
			for (int i = nested.Count - 1; i >= 0; i--) {
				TypeDefinition t = nested [i];
				if ((t.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic ||
					(t.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily ||
					(t.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem) {
					// public
					if (t.DeclaringType == type)
						continue; // not nested of nested
				}

				nested.RemoveAt (i);
			}


			if (nested.Count > 0) {
				XmlNode classes = document.CreateElement ("classes", null);
				nclass.AppendChild (classes);
				foreach (TypeDefinition t in nested) {
					TypeData td = new TypeData (document, classes, t);
					td.DoOutput ();
				}
			}
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			if (memberDefenition != type)
				throw new InvalidOperationException ("odd");
				
			return ((int) type.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		public static bool MustDocumentMethod (MethodDefinition method) {
			// All other methods
			MethodAttributes maskedAccess = method.Attributes & MethodAttributes.MemberAccessMask;
			return maskedAccess == MethodAttributes.Public
				|| maskedAccess == MethodAttributes.Family
				|| maskedAccess == MethodAttributes.FamORAssem;
		}

		static string GetClassType (TypeDefinition t)
		{
			if (t.IsEnum)
				return "enum";

			if (t.IsValueType)
				return "struct";

			if (t.IsInterface)
				return "interface";
			
			if (TypeHelper.IsDelegate(t))
				return "delegate";

			return "class";
		}

		private static string GetCharSet (TypeDefinition type)
		{
			TypeAttributes maskedStringFormat = type.Attributes & TypeAttributes.StringFormatMask;
			if (maskedStringFormat == TypeAttributes.AnsiClass)
				return CharSet.Ansi.ToString ();

			if (maskedStringFormat == TypeAttributes.AutoClass)
				return CharSet.Auto.ToString ();

			if (maskedStringFormat == TypeAttributes.UnicodeClass)
				return CharSet.Unicode.ToString ();

			return CharSet.None.ToString ();
		}

		private static string GetLayout (TypeDefinition type)
		{
			TypeAttributes maskedLayout = type.Attributes & TypeAttributes.LayoutMask;
			if (maskedLayout == TypeAttributes.AutoLayout)
				return LayoutKind.Auto.ToString ();

			if (maskedLayout == TypeAttributes.ExplicitLayout)
				return LayoutKind.Explicit.ToString ();

			if (maskedLayout == TypeAttributes.SequentialLayout)
				return LayoutKind.Sequential.ToString ();

			return null;
		}

		private FieldDefinition [] GetFields (TypeDefinition type) {
			ArrayList list = new ArrayList ();

			FieldDefinitionCollection fields = type.Fields;//type.GetFields (flags);
			foreach (FieldDefinition field in fields) {
				if (field.IsSpecialName)
					continue;

				// we're only interested in public or protected members
				FieldAttributes maskedVisibility = (field.Attributes & FieldAttributes.FieldAccessMask);
				if (maskedVisibility == FieldAttributes.Public
					|| maskedVisibility == FieldAttributes.Family
					|| maskedVisibility == FieldAttributes.FamORAssem){
					list.Add (field);
				}
			}

			return (FieldDefinition []) list.ToArray (typeof (FieldDefinition));
		}


		internal static PropertyDefinition [] GetProperties (TypeDefinition type) {
			ArrayList list = new ArrayList ();

			PropertyDefinitionCollection properties = type.Properties;//type.GetProperties (flags);
			foreach (PropertyDefinition property in properties) {
				MethodDefinition getMethod = property.GetMethod;
				MethodDefinition setMethod = property.SetMethod;

				bool hasGetter = (getMethod != null) && MustDocumentMethod (getMethod);
				bool hasSetter = (setMethod != null) && MustDocumentMethod (setMethod);

				// if neither the getter or setter should be documented, then
				// skip the property
				if (hasGetter || hasSetter) {
					list.Add (property);
				}
			}

			return (PropertyDefinition []) list.ToArray (typeof (PropertyDefinition));
		}

		private MethodDefinition[] GetMethods (TypeDefinition type)
		{
			ArrayList list = new ArrayList ();

			MethodDefinitionCollection methods = type.Methods;//type.GetMethods (flags);
			foreach (MethodDefinition method in methods) {
				if (method.IsSpecialName)
					continue;

				// we're only interested in public or protected members
				if (!MustDocumentMethod(method))
					continue;

				list.Add (method);
			}

			return (MethodDefinition []) list.ToArray (typeof (MethodDefinition));
		}

		private MethodDefinition [] GetConstructors (TypeDefinition type)
		{
			ArrayList list = new ArrayList ();

			ConstructorCollection ctors = type.Constructors;//type.GetConstructors (flags);
			foreach (MethodDefinition constructor in ctors) {
				// we're only interested in public or protected members
				if (!MustDocumentMethod(constructor))
					continue;

				list.Add (constructor);
			}

			return (MethodDefinition []) list.ToArray (typeof (MethodDefinition));
		}

		private EventDefinition[] GetEvents (TypeDefinition type)
		{
			ArrayList list = new ArrayList ();

			EventDefinitionCollection events = type.Events;//type.GetEvents (flags);
			foreach (EventDefinition eventDef in events) {
				MethodDefinition addMethod = eventDef.AddMethod;//eventInfo.GetAddMethod (true);

				if (addMethod == null || !MustDocumentMethod (addMethod))
					continue;

				list.Add (eventDef);
			}

			return (EventDefinition []) list.ToArray (typeof (EventDefinition));
		}
	}

	class FieldData : MemberData
	{
		public FieldData (XmlDocument document, XmlNode parent, FieldDefinition [] members)
			: base (document, parent, members)
		{
		}

		protected override CustomAttributeCollection GetCustomAttributes (MemberReference member) {
			return ((FieldDefinition) member).CustomAttributes;
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			FieldDefinition field = (FieldDefinition) memberDefenition;
			return field.Name;
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			FieldDefinition field = (FieldDefinition) memberDefenition;
			return ((int) field.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		protected override void AddExtraData (XmlNode p, MemberReference memberDefenition)
		{
			base.AddExtraData (p, memberDefenition);
			FieldDefinition field = (FieldDefinition) memberDefenition;
			AddAttribute (p, "fieldtype", field.FieldType.ToString ());

			if (field.IsLiteral) {
				object value = field.Constant;//object value = field.GetValue (null);
				string stringValue = null;
				//if (value is Enum) {
				//    // FIXME: when Mono bug #60090 has been
				//    // fixed, we should just be able to use
				//    // Convert.ToString
				//    stringValue = ((Enum) value).ToString ("D", CultureInfo.InvariantCulture);
				//}
				//else {
					stringValue = Convert.ToString (value, CultureInfo.InvariantCulture);
				//}

				if (stringValue != null)
					AddAttribute (p, "value", stringValue);
			}
		}

		public override string ParentTag {
			get { return "fields"; }
		}

		public override string Tag {
			get { return "field"; }
		}
	}

	class PropertyData : MemberData
	{
		public PropertyData (XmlDocument document, XmlNode parent, PropertyDefinition [] members)
			: base (document, parent, members)
		{
		}

		protected override CustomAttributeCollection GetCustomAttributes (MemberReference member) {
			return ((PropertyDefinition) member).CustomAttributes;
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			PropertyDefinition prop = (PropertyDefinition) memberDefenition;
			return prop.Name;
		}

		protected override void AddExtraData (XmlNode p, MemberReference memberDefenition)
		{
			base.AddExtraData (p, memberDefenition);
			PropertyDefinition prop = (PropertyDefinition) memberDefenition;
			TypeReference t = prop.PropertyType;
			AddAttribute (p, "ptype", prop.PropertyType.ToString ());
			MethodDefinition _get = prop.GetMethod;
			MethodDefinition _set = prop.SetMethod;
			bool haveGet = (_get != null && TypeData.MustDocumentMethod(_get));
			bool haveSet = (_set != null && TypeData.MustDocumentMethod(_set));
			MethodDefinition [] methods;

			if (haveGet && haveSet) {
				methods = new MethodDefinition [] { _get, _set };
			} else if (haveGet) {
				methods = new MethodDefinition [] { _get };
			} else if (haveSet) {
				methods = new MethodDefinition [] { _set };
			} else {
				//odd
				return;
			}

			string parms = Parameters.GetSignature (methods [0].Parameters);
			AddAttribute (p, "params", parms);

			MethodData data = new MethodData (document, p, methods);
			//data.NoMemberAttributes = true;
			data.DoOutput ();
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			PropertyDefinition prop = (PropertyDefinition) memberDefenition;
			return ((int) prop.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		public override string ParentTag {
			get { return "properties"; }
		}

		public override string Tag {
			get { return "property"; }
		}
	}

	class EventData : MemberData
	{
		public EventData (XmlDocument document, XmlNode parent, EventDefinition [] members)
			: base (document, parent, members)
		{
		}

		protected override CustomAttributeCollection GetCustomAttributes (MemberReference member) {
			return ((EventDefinition) member).CustomAttributes;
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			EventDefinition evt = (EventDefinition) memberDefenition;
			return evt.Name;
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			EventDefinition evt = (EventDefinition) memberDefenition;
			return ((int) evt.Attributes).ToString (CultureInfo.InvariantCulture);
		}

		protected override void AddExtraData (XmlNode p, MemberReference memberDefenition)
		{
			base.AddExtraData (p, memberDefenition);
			EventDefinition evt = (EventDefinition) memberDefenition;
			AddAttribute (p, "eventtype", evt.EventType.FullName);
		}

		public override string ParentTag {
			get { return "events"; }
		}

		public override string Tag {
			get { return "event"; }
		}
	}

	class MethodData : MemberData
	{
		bool noAtts;

		public MethodData (XmlDocument document, XmlNode parent, MethodDefinition [] members)
			: base (document, parent, members)
		{
		}

		protected override CustomAttributeCollection GetCustomAttributes (MemberReference member) {
			return ((MethodDefinition) member).CustomAttributes;
		}

		protected override string GetName (MemberReference memberDefenition)
		{
			MethodDefinition method = (MethodDefinition) memberDefenition;
			string name = method.Name;
			string parms = Parameters.GetSignature (method.Parameters);
#if NET_2_0
			MethodInfo mi = method as MethodInfo;
			Type [] genArgs = mi == null ? Type.EmptyTypes :
				mi.GetGenericArguments ();
			if (genArgs.Length > 0) {
				string [] genArgNames = new string [genArgs.Length];
				for (int i = 0; i < genArgs.Length; i++) {
					genArgNames [i] = genArgs [i].Name;
					string genArgCsts = String.Empty;
					Type [] gcs = genArgs [i].GetGenericParameterConstraints ();
					if (gcs.Length > 0) {
						string [] gcNames = new string [gcs.Length];
						for (int g = 0; g < gcs.Length; g++)
							gcNames [g] = gcs [g].FullName;
						genArgCsts = String.Concat (
							"(",
							string.Join (", ", gcNames),
							") ",
							genArgNames [i]);
					}
					else
						genArgCsts = genArgNames [i];
					if ((genArgs [i].GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
						genArgCsts = "class " + genArgCsts;
					else if ((genArgs [i].GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
						genArgCsts = "struct " + genArgCsts;
					genArgNames [i] = genArgCsts;
				}
				return String.Format ("{0}<{2}>({1})",
					name,
					parms,
					string.Join (",", genArgNames));
			}
#endif
			return String.Format ("{0}({1})", name, parms);
		}

		protected override string GetMemberAttributes (MemberReference memberDefenition)
		{
			MethodDefinition method = (MethodDefinition) memberDefenition;
			return ((int)( method.Attributes)).ToString (CultureInfo.InvariantCulture);
		}

		protected override void AddExtraData (XmlNode p, MemberReference memberDefenition)
		{
			base.AddExtraData (p, memberDefenition);

			if (!(memberDefenition is MethodDefinition))
				return;

			MethodDefinition mbase = (MethodDefinition) memberDefenition;

			ParameterData parms = new ParameterData (document, p, mbase.Parameters);
			parms.DoOutput ();

			if (mbase.IsAbstract)
				AddAttribute (p, "abstract", "true");
			if (mbase.IsVirtual)
				AddAttribute (p, "virtual", "true");
			if (mbase.IsStatic)
				AddAttribute (p, "static", "true");

			//if (!(member is MethodInfo))
			//    return;

			//MethodInfo method = (MethodInfo) member;
			string rettype = mbase.ReturnType.ReturnType.FullName;
			if (rettype != "System.Void" || !mbase.IsConstructor)
				AddAttribute (p, "returntype", (rettype));
			
			AttributeData.OutputAttributes (document, p,mbase.ReturnType.CustomAttributes);
#if NET_2_0
			// Generic constraints
			Type [] gargs = method.GetGenericArguments ();
			XmlElement ngeneric = (gargs.Length == 0) ? null :
				document.CreateElement ("generic-method-constraints");
			foreach (Type garg in gargs) {
				Type [] csts = garg.GetGenericParameterConstraints ();
				if (csts.Length == 0 || csts [0] == typeof (object))
					continue;
				XmlElement el = document.CreateElement ("generic-method-constraint");
				el.SetAttribute ("name", garg.ToString ());
				el.SetAttribute ("generic-attribute",
					garg.GenericParameterAttributes.ToString ());
				ngeneric.AppendChild (el);
				foreach (Type ct in csts) {
					XmlElement cel = document.CreateElement ("type");
					cel.AppendChild (document.CreateTextNode (ct.FullName));
					el.AppendChild (cel);
				}
			}
			if (ngeneric != null && ngeneric.FirstChild != null)
				p.AppendChild (ngeneric);
#endif

		}

		public override bool NoMemberAttributes {
			get { return noAtts; }
			set { noAtts = value; }
		}
		
		public override string ParentTag {
			get { return "methods"; }
		}

		public override string Tag {
			get { return "method"; }
		}
	}

	class ConstructorData : MethodData
	{
		public ConstructorData (XmlDocument document, XmlNode parent, MethodDefinition [] members)
			: base (document, parent, members)
		{
		}

		public override string ParentTag {
			get { return "constructors"; }
		}

		public override string Tag {
			get { return "constructor"; }
		}
	}

	class ParameterData : BaseData
	{
		private ParameterDefinitionCollection parameters;

		public ParameterData (XmlDocument document, XmlNode parent, ParameterDefinitionCollection parameters)
			: base (document, parent)
		{
			this.parameters = parameters;
		}

		public override void DoOutput ()
		{
			XmlNode parametersNode = document.CreateElement ("parameters", null);
			parent.AppendChild (parametersNode);

			foreach (ParameterDefinition parameter in parameters) {
				XmlNode paramNode = document.CreateElement ("parameter", null);
				parametersNode.AppendChild (paramNode);
				AddAttribute (paramNode, "name", parameter.Name);
				AddAttribute (paramNode, "position", parameter.Method.Parameters.IndexOf(parameter).ToString(CultureInfo.InvariantCulture));
				AddAttribute (paramNode, "attrib", ((int) parameter.Attributes).ToString());

				string direction = "in";

				if (parameter.ParameterType.FullName.EndsWith ("&"))//is by ref
				{
					direction = (parameter.Attributes & ParameterAttributes.Out) != 0 ? "out" : "ref";
				}

				TypeReference t = parameter.ParameterType;
				AddAttribute (paramNode, "type", t.FullName);

				if ((parameter.Attributes & ParameterAttributes.Optional) != 0) {
					AddAttribute (paramNode, "optional", "true");
					if (parameter.Constant != System.DBNull.Value)
						AddAttribute (paramNode, "defaultValue", (parameter.Constant == null) ? "NULL" : parameter.Constant.ToString ());
				}

				if (direction != "in")
					AddAttribute (paramNode, "direction", direction);

				AttributeData.OutputAttributes (document, paramNode, parameter.CustomAttributes);
			}
		}
	}

	class AttributeData : BaseData
	{
		CustomAttributeCollection atts;

		AttributeData (XmlDocument doc, XmlNode parent, CustomAttributeCollection attributes)
			: base (doc, parent)
		{
			atts = attributes;
		}

		public override void DoOutput ()
		{
			if (document == null)
				throw new InvalidOperationException ("Document not set");

			if (atts == null || atts.Count == 0)
				return;

			XmlNode natts = parent.SelectSingleNode("attributes");
			if (natts == null) {
				natts = document.CreateElement ("attributes", null);
				parent.AppendChild (natts);
			}

			for (int i = 0; i < atts.Count; ++i) {
				CustomAttribute att = atts [i];
				try {
					att.Resolve ();
				}
				catch { }//TODO: fix this bug - exception is thrown when running on our System.Web.dll
				string attName = TypeHelper.GetFullName (att);
				bool attIsPublic = TypeHelper.IsPublic(att);
				if (!attIsPublic || attName.EndsWith ("TODOAttribute"))
					continue;

				// we ignore attributes that inherit from SecurityAttribute on purpose as they:
				// * aren't part of GetCustomAttributes in Fx 1.0/1.1;
				// * are encoded differently and in a different metadata table; and
				// * won't ever exactly match MS implementation (from a syntax pov)
				//TODO if (t.IsSubclassOf (typeof (SecurityAttribute)))
				//TODO 	  continue;

				XmlNode node = document.CreateElement ("attribute");
				AddAttribute (node, "name", attName);

				XmlNode properties = null;
				//foreach (string name in att.Properties.Keys) {
				foreach (string name in att.Properties.Keys) {
					if (name == "TypeId")
						continue;

					if (properties == null) {
						properties = node.AppendChild (document.CreateElement ("properties"));
					}

					object o = att.Properties[name];

					XmlNode n = properties.AppendChild (document.CreateElement ("property"));
					AddAttribute (n, "name", name);

					if (o == null) {
						AddAttribute (n, "value", "null");
						continue;
					}
					string value = o.ToString ();
					if (attName.EndsWith ("GuidAttribute"))
						value = value.ToUpper ();
					AddAttribute (n, "value", value);
				}

				natts.AppendChild (node);
			}
		}

		public static void OutputAttributes (XmlDocument doc, XmlNode parent, CustomAttributeCollection attributes)
		{
			AttributeData ad = new AttributeData (doc, parent, attributes);
			ad.DoOutput ();
		}

		private static bool MustDocumentAttribute (Type attributeType)
		{
			// only document MonoTODOAttribute and public attributes
			return attributeType.Name.EndsWith ("TODOAttribute") || attributeType.IsPublic;
		}
	}

	class Parameters
	{
		private Parameters () {}

		public static string GetSignature (ParameterDefinitionCollection infos)
		{
			if (infos == null || infos.Count == 0)
				return "";

			StringBuilder sb = new StringBuilder ();
			foreach (ParameterDefinition info in infos) {
				
				string modifier;
				if ((info.Attributes & ParameterAttributes.In) != 0)
					modifier = "in ";
				else if (((int)info.Attributes & 0x8) != 0)//TODO- check this: 8 == ParameterAttributes.Retval (see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfsystemreflectionparameterattributesclasstopic.asp)
					modifier = "ref ";
				else if ((info.Attributes & ParameterAttributes.Out) != 0)
					modifier = "out ";
				else
					modifier = "";

				string type_name = info.ParameterType.ToString ().Replace ('<', '[').Replace ('>', ']');
				sb.AppendFormat ("{0}{1}, ", modifier, type_name);
			}

			sb.Length -= 2; // remove ", "
			return sb.ToString ();
		}

	}

	class TypeReferenceComparer : IComparer
	{
		public static TypeReferenceComparer Default = new TypeReferenceComparer ();

		public int Compare (object a, object b)
		{
			TypeReference ta = (TypeReference) a;
			TypeReference tb = (TypeReference) b;
			int result = String.Compare (ta.Namespace, tb.Namespace);
			if (result != 0)
				return result;

			return String.Compare (ta.Name, tb.Name);
		}
	}

	class MemberReferenceComparer : IComparer
	{
		public static MemberReferenceComparer Default = new MemberReferenceComparer ();

		public int Compare (object a, object b)
		{
			MemberReference ma = (MemberReference) a;
			MemberReference mb = (MemberReference) b;
			return String.Compare (ma.Name, mb.Name);
		}
	}

	class MethodDefinitionComparer : IComparer
	{
		public static MethodDefinitionComparer Default = new MethodDefinitionComparer ();

		public int Compare (object a, object b)
		{
			MethodDefinition ma = (MethodDefinition) a;
			MethodDefinition mb = (MethodDefinition) b;
			int res = String.Compare (ma.Name, mb.Name);
			if (res != 0)
				return res;

			ParameterDefinitionCollection pia = ma.Parameters ;
			ParameterDefinitionCollection pib = mb.Parameters;
			res = pia.Count - pib.Count;
			if (res != 0)
				return res;

			string siga = Parameters.GetSignature (pia);
			string sigb = Parameters.GetSignature (pib);
			return String.Compare (siga, sigb);
		}
	}
}

