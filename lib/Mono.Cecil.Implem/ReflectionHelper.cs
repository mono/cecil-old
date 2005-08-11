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
		private IDictionary m_primitives;
		private IDictionary m_memberRefCache;

		public ReflectionHelper (ModuleDefinition module)
		{
			m_module = module;
			m_asmCache = new Hashtable ();
			m_primitives = new Hashtable ();
			m_memberRefCache = new Hashtable ();

			FillPrimitives ();
		}

		private void AddPrimitive (Type t)
		{
			m_primitives [t.FullName] = new TypeReference (t.Name, t.Namespace);
		}

		private bool IsPrimitive (Type t)
		{
			return m_primitives.Contains (t.FullName);
		}

		private ITypeReference GetPrimitive (Type t)
		{
			return m_primitives [t.FullName] as ITypeReference;
		}

		private void FillPrimitives ()
		{
			AddPrimitive (typeof (void));
			AddPrimitive (typeof (string));
			AddPrimitive (typeof (bool));
			AddPrimitive (typeof (char));
			AddPrimitive (typeof (float));
			AddPrimitive (typeof (double));
			AddPrimitive (typeof (sbyte));
			AddPrimitive (typeof (byte));
			AddPrimitive (typeof (short));
			AddPrimitive (typeof (ushort));
			AddPrimitive (typeof (int));
			AddPrimitive (typeof (uint));
			AddPrimitive (typeof (long));
			AddPrimitive (typeof (ulong));
			AddPrimitive (typeof (IntPtr));
			AddPrimitive (typeof (UIntPtr));
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
			if (IsPrimitive (t))
				return GetPrimitive (t);

			TypeReference typeRef = m_module.TypeReferences [GetTypeSignature (t)] as TypeReference;
			if (typeRef != null)
				return typeRef;

			IAssemblyNameReference asm = RegisterAssembly (t.Assembly);
			typeRef = new TypeReference (t.Name, t.Namespace, m_module, asm);
			(m_module.TypeReferences as TypeReferenceCollection) [typeRef.FullName] = typeRef;
			return typeRef;
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
			MethodReference methRef = m_memberRefCache [
				GetMethodBaseSignature (meth)] as MethodReference;
			if (methRef != null)
				return methRef;

			methRef = new MethodReference (meth.Name, RegisterType (meth.DeclaringType));
			int seq = 1;
			foreach (ParameterInfo p in meth.GetParameters ()) {
				ParameterDefinition paramDef = new ParameterDefinition (
					p.Name, seq++, (ParamAttributes) 0, RegisterType (p.ParameterType));
				(methRef.Parameters as ParameterDefinitionCollection).Add (paramDef);
			}
			methRef.ReturnType.ReturnType = RegisterType (retType);
			// TODO: add it somewhere in the reflection writer for the MemberRef table
			return methRef;
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

			fieldRef = new FieldReference (field.Name,
				RegisterType (field.DeclaringType), RegisterType (field.FieldType));
			// TODO: add it somewhere in the reflection writer for the MemberRef table
			return fieldRef;
		}
	}
}
