/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;
	using SR = System.Reflection;
	using System.Text;

	using Mono.Cecil;

	internal sealed class ReflectionHelper {

		private ModuleDefinition m_module;

		private IDictionary m_asmCache;
		private IDictionary m_memberRefCache;

		public ReflectionHelper (ModuleDefinition module)
		{
			m_module = module;
			m_asmCache = new Hashtable ();
			m_memberRefCache = new Hashtable ();
		}

		private bool IsForeign (ITypeReference t)
		{
			return t.Module != null && t.Module != m_module;
		}

		public AssemblyNameReference CheckAssemblyReference (SR.Assembly asm)
		{
			AssemblyNameReference asmRef = m_asmCache [asm.FullName] as AssemblyNameReference;
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

		public IMetadataScope CheckScope (IMetadataScope scope) // TODO
		{
			if (scope is IAssemblyNameReference) {

			} else
				throw new NotImplementedException ();

			return scope;
		}

		public string GetTypeSignature (Type t)
		{
			if (t.DeclaringType != null)
				return string.Concat (t.DeclaringType.FullName, "/", t.Name);

			if (t.Namespace == null || t.Namespace.Length == 0)
				return t.Name;

			return string.Concat (t.Namespace, ".", t.Name);
		}

		public ITypeReference CheckType (Type t)
		{
			ITypeReference type = m_module.TypeReferences [GetTypeSignature (t)] as TypeReference;
			if (type != null)
				return type;

			IAssemblyNameReference asm = CheckAssemblyReference (t.Assembly);
			type = m_module.Factories.TypeFactory.CreateTypeReference (t.Name, t.Namespace, asm, t.IsValueType);

			m_module.TypeReferences.Add (type);
			return type;
		}

		private string GetMethodBaseSignature (SR.MethodBase meth, Type retType)
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

		private IMethodReference CheckMethodBase (SR.MethodBase mb, Type retType)
		{
			string sig = GetMethodBaseSignature (mb, retType);
			IMethodReference meth = m_memberRefCache [sig] as MethodReference;
			if (meth != null)
				return meth;

			SR.ParameterInfo [] parameters = mb.GetParameters ();
			ITypeReference [] parametersTypes = new ITypeReference [parameters.Length];
			for (int i = 0; i < parametersTypes.Length; i++)
				parametersTypes [i] = CheckType (parameters [i].ParameterType);

			meth = m_module.Factories.MethodFactory.CreateMethodReference (
				mb.Name, CheckType (mb.DeclaringType), CheckType (retType), parametersTypes,
				(mb.CallingConvention & SR.CallingConventions.HasThis) > 0,
				(mb.CallingConvention & SR.CallingConventions.ExplicitThis) > 0,
				MethodCallingConvention.Default); // TODO: get the real callconv

			m_module.MemberReferences.Add (meth);
			m_memberRefCache [sig] = meth;
			return meth;
		}

		public IMethodReference CheckConstructor (SR.ConstructorInfo ci)
		{
			return CheckMethodBase (ci, typeof (void));
		}

		public IMethodReference CheckMethod (SR.MethodInfo mi)
		{
			return CheckMethodBase (mi, mi.ReturnType);
		}

		private string GetFieldSignature (SR.FieldInfo field)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetTypeSignature (field.FieldType));
			sb.Append (' ');
			sb.Append (GetTypeSignature (field.DeclaringType));
			sb.Append ("::");
			sb.Append (field.Name);
			return sb.ToString ();
		}

		public IFieldReference CheckField (SR.FieldInfo fi)
		{
			string sig = GetFieldSignature (fi);
			IFieldReference f = m_memberRefCache [sig] as FieldReference;
			if (f != null)
				return f;

			f = m_module.Factories.FieldFactory.CreateFieldReference (
				fi.Name, CheckType (fi.DeclaringType), CheckType (fi.FieldType));

			m_module.MemberReferences.Add (f);
			m_memberRefCache [sig] = f;
			return f;
		}

		public ITypeReference CheckType (ITypeReference type)
		{
			if (type is ITypeDefinition)
				return CheckType (type as ITypeDefinition);

			if (IsForeign (type)) {
				ITypeReference t = m_module.TypeReferences [type.FullName] as TypeReference;
				if (t != null)
					return t;

				t = m_module.Factories.TypeFactory.CreateTypeReference (
					type.Name, type.Namespace, CheckScope (type.Scope), type.IsValueType);
				m_module.TypeReferences.Add (t);
				return t;
			}

			return type;
		}

		public ITypeDefinition CheckType (ITypeDefinition type)
		{
			if (IsForeign (type))
				throw new NotImplementedException ("TODO"); // TODO

			return type;
		}

		public IMethodReference CheckMethod (IMethodReference m)
		{
			string sig = m.ToString ();
			IMethodReference meth = m_memberRefCache [sig] as IMethodReference;
			if (meth != null)
				return meth;

			ITypeReference [] parametersTypes = new ITypeReference [meth.Parameters.Count];
			for (int i = 0; i < parametersTypes.Length; i++)
				parametersTypes [i] = CheckType (meth.Parameters [i].ParameterType);

			meth = m_module.Factories.MethodFactory.CreateMethodReference (
				m.Name, CheckType (m.DeclaringType), CheckType (m.ReturnType.ReturnType), parametersTypes,
				m.HasThis, m.ExplicitThis, m.CallingConvention);

			m_module.MemberReferences.Add (meth);
			m_memberRefCache [sig] = meth;
			return meth;
		}
	}
}
