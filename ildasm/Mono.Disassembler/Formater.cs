//
// Formater.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
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

namespace Mono.Disassembler {

	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Text;

	using Mono.Cecil;

	class Formater {

		static readonly StringDictionary m_aliases;
		static readonly Hashtable m_keywords;

		static Formater ()
		{
			m_aliases = new StringDictionary ();

			m_aliases.Add ("System.Void", "void");
			m_aliases.Add ("System.Object", "object");
			m_aliases.Add ("System.String", "string");
			m_aliases.Add ("System.Boolean", "bool");
			m_aliases.Add ("System.Char", "char");
			m_aliases.Add ("System.Single", "float32");
			m_aliases.Add ("System.Double", "float64");
			m_aliases.Add ("System.SByte", "int8");
			m_aliases.Add ("System.Byte", "unsigned int8");
			m_aliases.Add ("System.Int16", "int16");
			m_aliases.Add ("System.UInt16", "unsigned int16");
			m_aliases.Add ("System.Int32", "int32");
			m_aliases.Add ("System.UInt32", "unsigned int32");
			m_aliases.Add ("System.Int64", "int64");
			m_aliases.Add ("System.UInt64", "unsigned int64");
			m_aliases.Add ("System.IntPtr", "native int");
			m_aliases.Add ("Systen.UIntPtr", "unsigned native int");
			m_aliases.Add ("System.TypedReference", "typedreference");

			/* Taken from mono/dis/get.c:key_table */
			m_keywords = new Hashtable ();

			m_keywords.Add ("9", "9");
			m_keywords.Add ("abstract", "abstract");
			m_keywords.Add ("add", "add");
			m_keywords.Add ("add.ovf", "add.ovf");
			m_keywords.Add ("add.ovf.un", "add.ovf.un");
			m_keywords.Add ("algorithm", "algorithm");
			m_keywords.Add ("alignment", "alignment");
			m_keywords.Add ("and", "and");
			m_keywords.Add ("ansi", "ansi");
			m_keywords.Add ("any", "any");
			m_keywords.Add ("arglist", "arglist");
			m_keywords.Add ("array", "array");
			m_keywords.Add ("as", "as");
			m_keywords.Add ("assembly", "assembly");
			m_keywords.Add ("assert", "assert");
			m_keywords.Add ("at", "at");
			m_keywords.Add ("autochar", "autochar");
			m_keywords.Add ("auto", "auto");
			m_keywords.Add ("beforefieldinit", "beforefieldinit");
			m_keywords.Add ("bestfit", "bestfit");
			m_keywords.Add ("beq", "beq");
			m_keywords.Add ("beq.s", "beq.s");
			m_keywords.Add ("bge", "bge");
			m_keywords.Add ("bge.s", "bge.s");
			m_keywords.Add ("bge.un", "bge.un");
			m_keywords.Add ("bge.un.s", "bge.un.s");
			m_keywords.Add ("bgt", "bgt");
			m_keywords.Add ("bgt.s", "bgt.s");
			m_keywords.Add ("bgt.un", "bgt.un");
			m_keywords.Add ("bgt.un.s", "bgt.un.s");
			m_keywords.Add ("ble", "ble");
			m_keywords.Add ("ble.s", "ble.s");
			m_keywords.Add ("ble.un", "ble.un");
			m_keywords.Add ("ble.un.s", "ble.un.s");
			m_keywords.Add ("blob", "blob");
			m_keywords.Add ("blob_object", "blob_object");
			m_keywords.Add ("blt", "blt");
			m_keywords.Add ("blt.s", "blt.s");
			m_keywords.Add ("blt.un", "blt.un");
			m_keywords.Add ("blt.un.s", "blt.un.s");
			m_keywords.Add ("bne.un", "bne.un");
			m_keywords.Add ("bne.un.s", "bne.un.s");
			m_keywords.Add ("bool", "bool");
			m_keywords.Add ("box", "box");
			m_keywords.Add ("break", "break");
			m_keywords.Add ("brfalse", "brfalse");
			m_keywords.Add ("brfalse.s", "brfalse.s");
			m_keywords.Add ("br", "br");
			m_keywords.Add ("brinst", "brinst");
			m_keywords.Add ("brinst.s", "brinst.s");
			m_keywords.Add ("brnull", "brnull");
			m_keywords.Add ("brnull.s", "brnull.s");
			m_keywords.Add ("br.s", "br.s");
			m_keywords.Add ("brtrue", "brtrue");
			m_keywords.Add ("brtrue.s", "brtrue.s");
			m_keywords.Add ("brzero", "brzero");
			m_keywords.Add ("brzero.s", "brzero.s");
			m_keywords.Add ("bstr", "bstr");
			m_keywords.Add ("bytearray", "bytearray");
			m_keywords.Add ("byvalstr", "byvalstr");
			m_keywords.Add ("call", "call");
			m_keywords.Add ("callconv", "callconv");
			m_keywords.Add ("calli", "calli");
			m_keywords.Add ("callmostderived", "callmostderived");
			m_keywords.Add ("callvirt", "callvirt");
			m_keywords.Add ("carray", "carray");
			m_keywords.Add ("castclass", "castclass");
			m_keywords.Add ("catch", "catch");
			m_keywords.Add ("cdecl", "cdecl");
			m_keywords.Add ("ceq", "ceq");
			m_keywords.Add ("cf", "cf");
			m_keywords.Add ("cgt", "cgt");
			m_keywords.Add ("cgt.un", "cgt.un");
			m_keywords.Add ("char", "char");
			m_keywords.Add ("charmaperror", "charmaperror");
			m_keywords.Add ("cil", "cil");
			m_keywords.Add ("ckfinite", "ckfinite");
			m_keywords.Add ("class", "class");
			m_keywords.Add ("clsid", "clsid");
			m_keywords.Add ("clt", "clt");
			m_keywords.Add ("clt.un", "clt.un");
			m_keywords.Add ("Compilercontrolled", "Compilercontrolled");
			m_keywords.Add ("const", "const");
			m_keywords.Add ("conv.i1", "conv.i1");
			m_keywords.Add ("conv.i2", "conv.i2");
			m_keywords.Add ("conv.i4", "conv.i4");
			m_keywords.Add ("conv.i8", "conv.i8");
			m_keywords.Add ("conv.i", "conv.i");
			m_keywords.Add ("conv.ovf.i1", "conv.ovf.i1");
			m_keywords.Add ("conv.ovf.i1.un", "conv.ovf.i1.un");
			m_keywords.Add ("conv.ovf.i2", "conv.ovf.i2");
			m_keywords.Add ("conv.ovf.i2.un", "conv.ovf.i2.un");
			m_keywords.Add ("conv.ovf.i4", "conv.ovf.i4");
			m_keywords.Add ("conv.ovf.i4.un", "conv.ovf.i4.un");
			m_keywords.Add ("conv.ovf.i8", "conv.ovf.i8");
			m_keywords.Add ("conv.ovf.i8.un", "conv.ovf.i8.un");
			m_keywords.Add ("conv.ovf.i", "conv.ovf.i");
			m_keywords.Add ("conv.ovf.i.un", "conv.ovf.i.un");
			m_keywords.Add ("conv.ovf.u1", "conv.ovf.u1");
			m_keywords.Add ("conv.ovf.u1.un", "conv.ovf.u1.un");
			m_keywords.Add ("conv.ovf.u2", "conv.ovf.u2");
			m_keywords.Add ("conv.ovf.u2.un", "conv.ovf.u2.un");
			m_keywords.Add ("conv.ovf.u4", "conv.ovf.u4");
			m_keywords.Add ("conv.ovf.u4.un", "conv.ovf.u4.un");
			m_keywords.Add ("conv.ovf.u8", "conv.ovf.u8");
			m_keywords.Add ("conv.ovf.u8.un", "conv.ovf.u8.un");
			m_keywords.Add ("conv.ovf.u", "conv.ovf.u");
			m_keywords.Add ("conv.ovf.u.un", "conv.ovf.u.un");
			m_keywords.Add ("conv.r4", "conv.r4");
			m_keywords.Add ("conv.r8", "conv.r8");
			m_keywords.Add ("conv.r.un", "conv.r.un");
			m_keywords.Add ("conv.u1", "conv.u1");
			m_keywords.Add ("conv.u2", "conv.u2");
			m_keywords.Add ("conv.u4", "conv.u4");
			m_keywords.Add ("conv.u8", "conv.u8");
			m_keywords.Add ("conv.u", "conv.u");
			m_keywords.Add ("cpblk", "cpblk");
			m_keywords.Add ("cpobj", "cpobj");
			m_keywords.Add ("currency", "currency");
			m_keywords.Add ("custom", "custom");
			m_keywords.Add ("date", "date");
			m_keywords.Add ("decimal", "decimal");
			m_keywords.Add ("default", "default");
			m_keywords.Add ("demand", "demand");
			m_keywords.Add ("deny", "deny");
			m_keywords.Add ("div", "div");
			m_keywords.Add ("div.un", "div.un");
			m_keywords.Add ("dup", "dup");
			m_keywords.Add ("endfault", "endfault");
			m_keywords.Add ("endfilter", "endfilter");
			m_keywords.Add ("endfinally", "endfinally");
			m_keywords.Add ("endmac", "endmac");
			m_keywords.Add ("enum", "enum");
			m_keywords.Add ("error", "error");
			m_keywords.Add ("explicit", "explicit");
			m_keywords.Add ("extends", "extends");
			m_keywords.Add ("extern", "extern");
			m_keywords.Add ("false", "false");
			m_keywords.Add ("famandassem", "famandassem");
			m_keywords.Add ("family", "family");
			m_keywords.Add ("famorassem", "famorassem");
			m_keywords.Add ("fastcall", "fastcall");
			m_keywords.Add ("fault", "fault");
			m_keywords.Add ("field", "field");
			m_keywords.Add ("filetime", "filetime");
			m_keywords.Add ("filter", "filter");
			m_keywords.Add ("final", "final");
			m_keywords.Add ("finally", "finally");
			m_keywords.Add ("fixed", "fixed");
			m_keywords.Add ("flags", "flags");
			m_keywords.Add ("float32", "float32");
			m_keywords.Add ("float64", "float64");
			m_keywords.Add ("float", "float");
			m_keywords.Add ("forwardref", "forwardref");
			m_keywords.Add ("fromunmanaged", "fromunmanaged");
			m_keywords.Add ("handler", "handler");
			m_keywords.Add ("hidebysig", "hidebysig");
			m_keywords.Add ("hresult", "hresult");
			m_keywords.Add ("idispatch", "idispatch");
			m_keywords.Add ("il", "il");
			m_keywords.Add ("illegal", "illegal");
			m_keywords.Add ("implements", "implements");
			m_keywords.Add ("implicitcom", "implicitcom");
			m_keywords.Add ("implicitres", "implicitres");
			m_keywords.Add ("import", "import");
			m_keywords.Add ("in", "in");
			m_keywords.Add ("inheritcheck", "inheritcheck");
			m_keywords.Add ("initblk", "initblk");
			m_keywords.Add ("init", "init");
			m_keywords.Add ("initobj", "initobj");
			m_keywords.Add ("initonly", "initonly");
			m_keywords.Add ("instance", "instance");
			m_keywords.Add ("int16", "int16");
			m_keywords.Add ("int32", "int32");
			m_keywords.Add ("int64", "int64");
			m_keywords.Add ("int8", "int8");
			m_keywords.Add ("interface", "interface");
			m_keywords.Add ("internalcall", "internalcall");
			m_keywords.Add ("int", "int");
			m_keywords.Add ("isinst", "isinst");
			m_keywords.Add ("iunknown", "iunknown");
			m_keywords.Add ("jmp", "jmp");
			m_keywords.Add ("lasterr", "lasterr");
			m_keywords.Add ("lcid", "lcid");
			m_keywords.Add ("ldarg.0", "ldarg.0");
			m_keywords.Add ("ldarg.1", "ldarg.1");
			m_keywords.Add ("ldarg.2", "ldarg.2");
			m_keywords.Add ("ldarg.3", "ldarg.3");
			m_keywords.Add ("ldarga", "ldarga");
			m_keywords.Add ("ldarga.s", "ldarga.s");
			m_keywords.Add ("ldarg", "ldarg");
			m_keywords.Add ("ldarg.s", "ldarg.s");
			m_keywords.Add ("ldc.i4.0", "ldc.i4.0");
			m_keywords.Add ("ldc.i4.1", "ldc.i4.1");
			m_keywords.Add ("ldc.i4.2", "ldc.i4.2");
			m_keywords.Add ("ldc.i4.3", "ldc.i4.3");
			m_keywords.Add ("ldc.i4.4", "ldc.i4.4");
			m_keywords.Add ("ldc.i4.5", "ldc.i4.5");
			m_keywords.Add ("ldc.i4.6", "ldc.i4.6");
			m_keywords.Add ("ldc.i4.7", "ldc.i4.7");
			m_keywords.Add ("ldc.i4.8", "ldc.i4.8");
			m_keywords.Add ("ldc.i4", "ldc.i4");
			m_keywords.Add ("ldc.i4.m1", "ldc.i4.m1");
			m_keywords.Add ("ldc.i4.M1", "ldc.i4.M1");
			m_keywords.Add ("ldc.i4.s", "ldc.i4.s");
			m_keywords.Add ("ldc.i8", "ldc.i8");
			m_keywords.Add ("ldc.r4", "ldc.r4");
			m_keywords.Add ("ldc.r8", "ldc.r8");
			m_keywords.Add ("ldelem", "ldelem");
			m_keywords.Add ("ldelema", "ldelema");
			m_keywords.Add ("ldelem.i1", "ldelem.i1");
			m_keywords.Add ("ldelem.i2", "ldelem.i2");
			m_keywords.Add ("ldelem.i4", "ldelem.i4");
			m_keywords.Add ("ldelem.i8", "ldelem.i8");
			m_keywords.Add ("ldelem.i", "ldelem.i");
			m_keywords.Add ("ldelem.r4", "ldelem.r4");
			m_keywords.Add ("ldelem.r8", "ldelem.r8");
			m_keywords.Add ("ldelem.ref", "ldelem.ref");
			m_keywords.Add ("ldelem.u1", "ldelem.u1");
			m_keywords.Add ("ldelem.u2", "ldelem.u2");
			m_keywords.Add ("ldelem.u4", "ldelem.u4");
			m_keywords.Add ("ldelem.u8", "ldelem.u8");
			m_keywords.Add ("ldflda", "ldflda");
			m_keywords.Add ("ldfld", "ldfld");
			m_keywords.Add ("ldftn", "ldftn");
			m_keywords.Add ("ldind.i1", "ldind.i1");
			m_keywords.Add ("ldind.i2", "ldind.i2");
			m_keywords.Add ("ldind.i4", "ldind.i4");
			m_keywords.Add ("ldind.i8", "ldind.i8");
			m_keywords.Add ("ldind.i", "ldind.i");
			m_keywords.Add ("ldind.r4", "ldind.r4");
			m_keywords.Add ("ldind.r8", "ldind.r8");
			m_keywords.Add ("ldind.ref", "ldind.ref");
			m_keywords.Add ("ldind.u1", "ldind.u1");
			m_keywords.Add ("ldind.u2", "ldind.u2");
			m_keywords.Add ("ldind.u4", "ldind.u4");
			m_keywords.Add ("ldind.u8", "ldind.u8");
			m_keywords.Add ("ldlen", "ldlen");
			m_keywords.Add ("ldloc.0", "ldloc.0");
			m_keywords.Add ("ldloc.1", "ldloc.1");
			m_keywords.Add ("ldloc.2", "ldloc.2");
			m_keywords.Add ("ldloc.3", "ldloc.3");
			m_keywords.Add ("ldloca", "ldloca");
			m_keywords.Add ("ldloca.s", "ldloca.s");
			m_keywords.Add ("ldloc", "ldloc");
			m_keywords.Add ("ldloc.s", "ldloc.s");
			m_keywords.Add ("ldnull", "ldnull");
			m_keywords.Add ("ldobj", "ldobj");
			m_keywords.Add ("ldsflda", "ldsflda");
			m_keywords.Add ("ldsfld", "ldsfld");
			m_keywords.Add ("ldstr", "ldstr");
			m_keywords.Add ("ldtoken", "ldtoken");
			m_keywords.Add ("ldvirtftn", "ldvirtftn");
			m_keywords.Add ("leave", "leave");
			m_keywords.Add ("leave.s", "leave.s");
			m_keywords.Add ("linkcheck", "linkcheck");
			m_keywords.Add ("literal", "literal");
			m_keywords.Add ("localloc", "localloc");
			m_keywords.Add ("lpstr", "lpstr");
			m_keywords.Add ("lpstruct", "lpstruct");
			m_keywords.Add ("lptstr", "lptstr");
			m_keywords.Add ("lpvoid", "lpvoid");
			m_keywords.Add ("lpwstr", "lpwstr");
			m_keywords.Add ("managed", "managed");
			m_keywords.Add ("marshal", "marshal");
			m_keywords.Add ("method", "method");
			m_keywords.Add ("mkrefany", "mkrefany");
			m_keywords.Add ("modopt", "modopt");
			m_keywords.Add ("modreq", "modreq");
			m_keywords.Add ("mul", "mul");
			m_keywords.Add ("mul.ovf", "mul.ovf");
			m_keywords.Add ("mul.ovf.un", "mul.ovf.un");
			m_keywords.Add ("native", "native");
			m_keywords.Add ("neg", "neg");
			m_keywords.Add ("nested", "nested");
			m_keywords.Add ("newarr", "newarr");
			m_keywords.Add ("newobj", "newobj");
			m_keywords.Add ("newslot", "newslot");
			m_keywords.Add ("noappdomain", "noappdomain");
			m_keywords.Add ("noinlining", "noinlining");
			m_keywords.Add ("nomachine", "nomachine");
			m_keywords.Add ("nomangle", "nomangle");
			m_keywords.Add ("nometadata", "nometadata");
			m_keywords.Add ("noncasdemand", "noncasdemand");
			m_keywords.Add ("noncasinheritance", "noncasinheritance");
			m_keywords.Add ("noncaslinkdemand", "noncaslinkdemand");
			m_keywords.Add ("nop", "nop");
			m_keywords.Add ("noprocess", "noprocess");
			m_keywords.Add ("not", "not");
			m_keywords.Add ("not_in_gc_heap", "not_in_gc_heap");
			m_keywords.Add ("notremotable", "notremotable");
			m_keywords.Add ("notserialized", "notserialized");
			m_keywords.Add ("null", "null");
			m_keywords.Add ("nullref", "nullref");
			m_keywords.Add ("object", "object");
			m_keywords.Add ("objectref", "objectref");
			m_keywords.Add ("off", "off");
			m_keywords.Add ("on", "on");
			m_keywords.Add ("opt", "opt");
			m_keywords.Add ("optil", "optil");
			m_keywords.Add ("or", "or");
			m_keywords.Add ("out", "out");
			m_keywords.Add ("permitonly", "permitonly");
			m_keywords.Add ("pinned", "pinned");
			m_keywords.Add ("pinvokeimpl", "pinvokeimpl");
			m_keywords.Add ("pop", "pop");
			m_keywords.Add ("prefix1", "prefix1");
			m_keywords.Add ("prefix2", "prefix2");
			m_keywords.Add ("prefix3", "prefix3");
			m_keywords.Add ("prefix4", "prefix4");
			m_keywords.Add ("prefix5", "prefix5");
			m_keywords.Add ("prefix6", "prefix6");
			m_keywords.Add ("prefix7", "prefix7");
			m_keywords.Add ("prefixref", "prefixref");
			m_keywords.Add ("prejitdeny", "prejitdeny");
			m_keywords.Add ("prejitgrant", "prejitgrant");
			m_keywords.Add ("preservesig", "preservesig");
			m_keywords.Add ("private", "private");
			m_keywords.Add ("privatescope", "privatescope");
			m_keywords.Add ("property", "property");
			m_keywords.Add ("protected", "protected");
			m_keywords.Add ("public", "public");
			m_keywords.Add ("readonly", "readonly");
			m_keywords.Add ("record", "record");
			m_keywords.Add ("refany", "refany");
			m_keywords.Add ("refanytype", "refanytype");
			m_keywords.Add ("refanyval", "refanyval");
			m_keywords.Add ("rem", "rem");
			m_keywords.Add ("rem.un", "rem.un");
			m_keywords.Add ("reqmin", "reqmin");
			m_keywords.Add ("reqopt", "reqopt");
			m_keywords.Add ("reqrefuse", "reqrefuse");
			m_keywords.Add ("reqsecobj", "reqsecobj");
			m_keywords.Add ("request", "request");
			m_keywords.Add ("ret", "ret");
			m_keywords.Add ("rethrow", "rethrow");
			m_keywords.Add ("retval", "retval");
			m_keywords.Add ("rtspecialname", "rtspecialname");
			m_keywords.Add ("runtime", "runtime");
			m_keywords.Add ("safearray", "safearray");
			m_keywords.Add ("sealed", "sealed");
			m_keywords.Add ("sequential", "sequential");
			m_keywords.Add ("serializable", "serializable");
			m_keywords.Add ("shl", "shl");
			m_keywords.Add ("shr", "shr");
			m_keywords.Add ("shr.un", "shr.un");
			m_keywords.Add ("sizeof", "sizeof");
			m_keywords.Add ("special", "special");
			m_keywords.Add ("specialname", "specialname");
			m_keywords.Add ("starg", "starg");
			m_keywords.Add ("starg.s", "starg.s");
			m_keywords.Add ("static", "static");
			m_keywords.Add ("stdcall", "stdcall");
			m_keywords.Add ("stelem", "stelem");
			m_keywords.Add ("stelem.i1", "stelem.i1");
			m_keywords.Add ("stelem.i2", "stelem.i2");
			m_keywords.Add ("stelem.i4", "stelem.i4");
			m_keywords.Add ("stelem.i8", "stelem.i8");
			m_keywords.Add ("stelem.i", "stelem.i");
			m_keywords.Add ("stelem.r4", "stelem.r4");
			m_keywords.Add ("stelem.r8", "stelem.r8");
			m_keywords.Add ("stelem.ref", "stelem.ref");
			m_keywords.Add ("stfld", "stfld");
			m_keywords.Add ("stind.i1", "stind.i1");
			m_keywords.Add ("stind.i2", "stind.i2");
			m_keywords.Add ("stind.i4", "stind.i4");
			m_keywords.Add ("stind.i8", "stind.i8");
			m_keywords.Add ("stind.i", "stind.i");
			m_keywords.Add ("stind.r4", "stind.r4");
			m_keywords.Add ("stind.r8", "stind.r8");
			m_keywords.Add ("stloc", "stloc");
			m_keywords.Add ("stobj", "stobj");
			m_keywords.Add ("storage", "storage");
			m_keywords.Add ("stored_object", "stored_object");
			m_keywords.Add ("streamed_object", "streamed_object");
			m_keywords.Add ("stream", "stream");
			m_keywords.Add ("strict", "strict");
			m_keywords.Add ("string", "string");
			m_keywords.Add ("struct", "struct");
			m_keywords.Add ("stsfld", "stsfld");
			m_keywords.Add ("sub", "sub");
			m_keywords.Add ("sub.ovf", "sub.ovf");
			m_keywords.Add ("sub.ovf.un", "sub.ovf.un");
			m_keywords.Add ("switch", "switch");
			m_keywords.Add ("synchronized", "synchronized");
			m_keywords.Add ("syschar", "syschar");
			m_keywords.Add ("sysstring", "sysstring");
			m_keywords.Add ("tbstr", "tbstr");
			m_keywords.Add ("thiscall", "thiscall");
			m_keywords.Add ("tls", "tls");
			m_keywords.Add ("to", "to");
			m_keywords.Add ("true", "true");
			m_keywords.Add ("type", "type");
			m_keywords.Add ("typedref", "typedref");
			m_keywords.Add ("uint", "uint");
			m_keywords.Add ("uint8", "uint8");
			m_keywords.Add ("uint16", "uint16");
			m_keywords.Add ("uint32", "uint32");
			m_keywords.Add ("uint64", "uint64");
			m_keywords.Add ("unbox", "unbox");
			m_keywords.Add ("unicode", "unicode");
			m_keywords.Add ("unmanagedexp", "unmanagedexp");
			m_keywords.Add ("unmanaged", "unmanaged");
			m_keywords.Add ("unsigned", "unsigned");
			m_keywords.Add ("userdefined", "userdefined");
			m_keywords.Add ("value", "value");
			m_keywords.Add ("valuetype", "valuetype");
			m_keywords.Add ("vararg", "vararg");
			m_keywords.Add ("variant", "variant");
			m_keywords.Add ("vector", "vector");
			m_keywords.Add ("virtual", "virtual");
			m_keywords.Add ("void", "void");
			m_keywords.Add ("wchar", "wchar");
			m_keywords.Add ("winapi", "winapi");
			m_keywords.Add ("with", "with");
			m_keywords.Add ("xor", "xor");

		}

		//FIXME
		public static string Escape (string name)
		{
			if (m_keywords.ContainsKey (name))
				return String.Concat ("'" + name + "'");

			foreach (char c in name.ToCharArray ()) {
				if (Char.IsLetterOrDigit (c) || c == '_' || c == '$' || c == '@' ||
		                    c == '?' || c == '.' || c == 0 || c == '!' || c == '`')
                		        continue;

				string ret = name.Replace ("'", "\\'");
				return String.Concat ("'", ret.Replace ("\\", "\\\\"), "'");
			}

			return name;
		}

		static string GetScope (TypeReference type)
		{
			if (type is TypeDefinition)
				return string.Empty;

			if (type != null && type.Scope == null)
				Console.WriteLine ("GetScope, type = {0}", type);
			return string.Concat ("[", Escape (type.Scope.Name), "]");
		}

		static bool IsPrimitive (TypeReference type)
		{
			return m_aliases.ContainsKey (GetTypeName (type));
		}

		static string GetAlias (TypeReference type)
		{
			return m_aliases [GetTypeName (type)];
		}

		static string GetTypeName (TypeReference type)
		{
			if (type.Namespace.Length == 0)
				return Escape (type.Name);

			if (type is TypeSpecification)
				return GetTypeName (((TypeSpecification) type).ElementType);
			else
				return string.Concat (Escape (type.Namespace), ".", Escape (type.Name));
		}

		public static string Format (TypeReference type)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetScope (type));

			if (type.DeclaringType != null) {
				sb.Append (GetTypeName (type.DeclaringType));
				sb.Append ("/");
				sb.Append (Escape (type.Name));
				return sb.ToString ();
			}

			sb.Append (GetTypeName (type));

			return sb.ToString ();
		}

		public static string Signature (TypeReference type)
		{
			return Signature (type, false);
		}


		public static string Signature (TypeReference type, bool only_name)
		{
			return Signature (type, only_name, false);
		}

		public static string Signature (TypeReference type, bool only_name, bool convert_primitive)
		{
			//FIXME: Move this elsewhere?
			StringBuilder sb = new StringBuilder ();

			if (type is PinnedType)
				sb.Append ("pinned ");

			if (type is ArrayType) {
				sb.Append (Signature (((ArrayType)type).ElementType));
			} else if (IsPrimitive (type)) {
				if (convert_primitive)
					sb.Append (GetAlias (type));
				else
					sb.Append (Format (type));
			} else {
				if (!only_name) {
					sb.Append (type.IsValueType ? "valuetype" : "class");
					sb.Append (" ");
				}
				sb.Append (Format (type));
			}

			if (type is ReferenceType)
				sb.Append ("&");
			else if (type is PointerType)
				sb.Append ("*");
			else if (type is ArrayType) {
				ArrayType ary = (ArrayType) type;
				if (ary.IsSizedArray)
					sb.Append ("[]");
				// TODO else
			}

			return sb.ToString ();
		}
	}
}
