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
	using System.Reflection;
	using System.Text;

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

		public IAssemblyNameReference RegisterAssembly (Assembly asm)
		{
			IAssemblyNameReference asmRef = m_asmCache [asm.FullName] as IAssemblyNameReference;
			if (asmRef != null)
				return asmRef;

			foreach (IAssemblyNameReference ext in m_module.AssemblyReferences)
				if (ext.Name == asm.GetName ().Name)
					return ext;

			AssemblyName asmName = asm.GetName ();
			asmRef = new AssemblyNameReference (asmName.Name, asmName.CultureInfo.Name, asmName.Version);
			asmRef.PublicKey = asmName.GetPublicKey ();
			asmRef.PublicKeyToken = asmName.GetPublicKeyToken ();
			asmRef.HashAlgorithm = (Mono.Cecil.AssemblyHashAlgorithm) asmName.HashAlgorithm;
			asmRef.Culture = asmName.CultureInfo.ToString ();
			asmRef.Flags = (Mono.Cecil.AssemblyFlags) asmName.Flags;
			(m_module.AssemblyReferences as AssemblyNameReferenceCollection).Add (asmRef);
			m_asmCache [asm.FullName] = asmRef;
			return asmRef;
		}

		private string GetTypeSignature (Type t)
		{
			if (t.DeclaringType != null)
				return string.Concat (t.DeclaringType.FullName, "/", t.Name);

			if (t.Namespace == null || t.Namespace.Length == 0)
				return t.Name;

			return string.Concat (t.Namespace, ".", t.Name);
		}

		public ITypeReference RegisterType (Type t)
		{
			TypeReference typeRef = m_module.TypeReferences [GetTypeSignature (t)] as TypeReference;
			if (typeRef != null)
				return typeRef;

			IAssemblyNameReference asm = RegisterAssembly (t.Assembly);
			return m_module.DefineTypeReference (t.Name, t.Namespace, asm);
		}

		private string GetMethodBaseSignature (MethodBase meth)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetTypeSignature (meth.DeclaringType));
			sb.Append ("::");
			sb.Append (meth.Name);
			sb.Append ("(");
			ParameterInfo [] parameters = meth.GetParameters ();
			for (int i = 0; i < parameters.Length; i++) {
				sb.Append (GetTypeSignature (parameters [i].ParameterType));
				if (i > 0)
					sb.Append (", ");
			}
			sb.Append (")");
			return sb.ToString ();
		}

		private IMethodReference RegisterMethodBase (MethodBase meth, Type retType)
		{
			IMethodReference methRef = m_memberRefCache [
				GetMethodBaseSignature (meth)] as MethodReference;
			if (methRef != null)
				return methRef;

			ParameterInfo [] methParams = meth.GetParameters ();
			ITypeReference [] parameters = new ITypeReference [methParams.Length];
			for (int i = 0; i < methParams.Length; i++)
				parameters [i] = RegisterType (methParams [i].ParameterType);

			return m_module.DefineMethodReference (meth.Name,
				RegisterType (meth.DeclaringType), RegisterType (retType), parameters,
				(meth.CallingConvention & CallingConventions.HasThis) > 0,
				(meth.CallingConvention & CallingConventions.ExplicitThis) > 0,
				MethodCallingConvention.Default); // TODO, get it from meth
		}

		public IMethodReference RegisterConstructor (ConstructorInfo ctor)
		{
			return RegisterMethodBase (ctor, typeof (void));
		}

		public IMethodReference RegisterMethod (MethodInfo meth)
		{
			return RegisterMethodBase (meth, meth.ReturnType);
		}

		private string GetFieldSignature (FieldInfo field)
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (GetTypeSignature (field.FieldType));
			sb.Append (' ');
			sb.Append (GetTypeSignature (field.DeclaringType));
			sb.Append ("::");
			sb.Append (field.Name);
			return sb.ToString ();
		}

		public IFieldReference RegisterField (FieldInfo field)
		{
			FieldReference fieldRef = m_memberRefCache [
				GetFieldSignature (field)] as FieldReference;
			if (fieldRef != null)
				return fieldRef;

			return m_module.DefineFieldReference (field.Name,
				RegisterType (field.DeclaringType), RegisterType (field.FieldType));
		}
	}
}
