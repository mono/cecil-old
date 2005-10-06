//
// ReflectionHelper.cs
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

namespace Mono.Cecil {

	using System;
	using System.Collections;
	using SR = System.Reflection;
	using System.Text;

	internal sealed class ReflectionHelper {

		ModuleDefinition m_module;

		IDictionary m_asmCache;
		IDictionary m_memberRefCache;

		public ReflectionHelper (ModuleDefinition module)
		{
			m_module = module;
			m_asmCache = new Hashtable ();
			m_memberRefCache = new Hashtable ();
		}

		public AssemblyNameReference ImportAssembly (SR.Assembly asm)
		{
			AssemblyNameReference asmRef = (AssemblyNameReference) m_asmCache [asm.FullName];
			if (asmRef != null)
				return asmRef;

			SR.AssemblyName asmName = asm.GetName ();
			asmRef = new AssemblyNameReference (
				asmName.Name, asmName.CultureInfo.Name, asmName.Version);
			asmRef.PublicKeyToken = asmName.GetPublicKeyToken ();
			asmRef.HashAlgorithm = (Mono.Cecil.AssemblyHashAlgorithm) asmName.HashAlgorithm;
			asmRef.Culture = asmName.CultureInfo.ToString ();
			asmRef.Flags = (Mono.Cecil.AssemblyFlags) asmName.Flags;
			m_module.AssemblyReferences.Add (asmRef);
			m_asmCache [asm.FullName] = asmRef;
			return asmRef;
		}

		public static string GetTypeSignature (Type t)
		{
			if (t.DeclaringType != null)
				return string.Concat (t.DeclaringType.FullName, "/", t.Name);

			if (t.Namespace == null || t.Namespace.Length == 0)
				return t.Name;

			return string.Concat (t.Namespace, ".", t.Name);
		}

		public TypeReference ImportType (Type t)
		{
			TypeReference type = m_module.TypeReferences [GetTypeSignature (t)];
			if (type != null)
				return type;

			AssemblyNameReference asm = ImportAssembly (t.Assembly);
			type = new TypeReference (t.Name, t.Namespace, asm, t.IsValueType);

			m_module.TypeReferences.Add (type);
			return type;
		}

		static string GetMethodBaseSignature (SR.MethodBase meth, Type retType)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetTypeSignature (retType));
			sb.Append (' ');
			sb.Append (GetTypeSignature (meth.DeclaringType));
			sb.Append ("::");
			sb.Append (meth.Name);
			sb.Append ("(");
			SR.ParameterInfo [] parameters = meth.GetParameters ();
			for (int i = 0; i < parameters.Length; i++) {
				if (i > 0)
					sb.Append (", ");
				sb.Append (GetTypeSignature (parameters [i].ParameterType));
			}
			sb.Append (")");
			return sb.ToString ();
		}

		MethodReference ImportMethodBase (SR.MethodBase mb, Type retType)
		{
			string sig = GetMethodBaseSignature (mb, retType);
			MethodReference meth = m_memberRefCache [sig] as MethodReference;
			if (meth != null)
				return meth;

			meth = new MethodReference (
				mb.Name,
				ImportType (mb.DeclaringType),
				ImportType (retType),
				(mb.CallingConvention & SR.CallingConventions.HasThis) > 0,
				(mb.CallingConvention & SR.CallingConventions.ExplicitThis) > 0,
				MethodCallingConvention.Default); // TODO: get the real callconv

			SR.ParameterInfo [] parameters = mb.GetParameters ();
			for (int i = 0; i < parameters.Length; i++)
				meth.Parameters.Add (new ParameterDefinition (ImportType (parameters [i].ParameterType)));

			m_module.MemberReferences.Add (meth);
			m_memberRefCache [sig] = meth;
			return meth;
		}

		public MethodReference ImportConstructor (SR.ConstructorInfo ci)
		{
			return ImportMethodBase (ci, typeof (void));
		}

		public MethodReference ImportMethod (SR.MethodInfo mi)
		{
			return ImportMethodBase (mi, mi.ReturnType);
		}

		static string GetFieldSignature (SR.FieldInfo field)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetTypeSignature (field.FieldType));
			sb.Append (' ');
			sb.Append (GetTypeSignature (field.DeclaringType));
			sb.Append ("::");
			sb.Append (field.Name);
			return sb.ToString ();
		}

		public FieldReference ImportField (SR.FieldInfo fi)
		{
			string sig = GetFieldSignature (fi);
			FieldReference f = (FieldReference) m_memberRefCache [sig];
			if (f != null)
				return f;

			f = new FieldReference (
				fi.Name, ImportType (fi.DeclaringType), ImportType (fi.FieldType));

			m_module.MemberReferences.Add (f);
			m_memberRefCache [sig] = f;
			return f;
		}

		public AssemblyNameReference ImportAssembly (AssemblyDefinition asm)
		{
			return null;
		}

		public TypeReference ImportTypeReference (TypeReference t)
		{
			TypeReference type = m_module.TypeReferences [t.FullName];
			if (type != null)
				return type;

			AssemblyNameReference asm = ImportAssembly (t.Module.Assembly);
			type = new TypeReference (t.Name, t.Namespace, asm, t.IsValueType);

			m_module.TypeReferences.Add (type);
			return type;
		}

		public MethodReference ImportMethodReference (MethodReference mr)
		{
			MethodReference meth = m_memberRefCache [mr.ToString ()] as MethodReference;
			if (meth != null)
				return meth;

			meth = new MethodReference (
				mr.Name,
				ImportTypeReference (mr.DeclaringType),
				ImportTypeReference (mr.ReturnType.ReturnType),
				mr.HasThis,
				mr.ExplicitThis,
				mr.CallingConvention);

			foreach (ParameterDefinition param in mr.Parameters)
				meth.Parameters.Add (new ParameterDefinition (ImportTypeReference (param.ParameterType)));

			m_module.MemberReferences.Add (meth);
			m_memberRefCache [mr.ToString ()] = meth;
			return meth;
		}

		public FieldReference ImportFieldReference (FieldReference field)
		{
			FieldReference f = (FieldReference) m_memberRefCache [field.ToString ()];
			if (f != null)
				return f;

			f = new FieldReference (
				field.Name,
				ImportTypeReference (field.DeclaringType),
				ImportTypeReference (field.FieldType));

			m_module.MemberReferences.Add (f);
			m_memberRefCache [field.ToString ()] = f;
			return f;
		}
	}
}
