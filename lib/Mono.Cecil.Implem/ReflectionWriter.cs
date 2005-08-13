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
	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class ReflectionWriter : BaseReflectionVisitor {

		private StructureWriter m_structureWriter;
		private ModuleDefinition m_mod;
		private SignatureWriter m_sigWriter;
		private CodeWriter m_codeWriter;
		private MetadataWriter m_mdWriter;
		private MetadataTableWriter m_tableWriter;
		private MetadataRowWriter m_rowWriter;

		private IDictionary m_types;
		private IDictionary m_typesRef;
		private IDictionary m_methods;
		private IDictionary m_fields;
		private IDictionary m_membersRef;
		private IDictionary m_primitives;

		private IList m_membersRefContainer;
		private IList m_methodStack;

		private uint m_methodIndex;
		private uint m_fieldIndex;
		private uint m_paramIndex;

		public StructureWriter StructureWriter {
			get { return m_structureWriter; }
			set {
				m_structureWriter = value;
				m_mdWriter = new MetadataWriter (
					m_mod.Image.MetadataRoot,
					m_structureWriter.AssemblyKind,
					m_mod.Assembly.Runtime,
					m_structureWriter.GetWriter ());
				m_tableWriter = m_mdWriter.GetTableVisitor ();
				m_rowWriter = m_tableWriter.GetRowVisitor () as MetadataRowWriter;
				m_sigWriter = new SignatureWriter (m_mdWriter, this);
				m_codeWriter = new CodeWriter (this, m_mdWriter.CilWriter);
			}
		}

		public CodeWriter CodeWriter {
			get { return m_codeWriter; }
		}

		public MetadataWriter MetadataWriter {
			get { return m_mdWriter; }
		}

		public MetadataTableWriter MetadataTableWriter {
			get { return m_tableWriter; }
		}

		public MetadataRowWriter MetadataRowWriter {
			get { return m_rowWriter; }
		}

		public ReflectionWriter (ModuleDefinition mod)
		{
			m_mod = mod;

			m_types = new Hashtable ();
			m_typesRef = new Hashtable ();
			m_methods = new Hashtable ();
			m_fields = new Hashtable ();
			m_membersRef = new Hashtable ();
			m_primitives = new Hashtable ();

			m_membersRefContainer = new ArrayList ();
			m_methodStack = new ArrayList ();

			m_methodIndex = 1;
			m_fieldIndex = 1;
			m_paramIndex = 1;

			FillPrimitives ();
		}

		private void FillPrimitives ()
		{
			object o = new object ();
			m_primitives [Constants.Void] = o;
			m_primitives [Constants.String] = o;
			m_primitives [Constants.Boolean] = o;
			m_primitives [Constants.Char] = o;
			m_primitives [Constants.Single] = o;
			m_primitives [Constants.Double] = o;
			m_primitives [Constants.SByte] = o;
			m_primitives [Constants.Byte] = o;
			m_primitives [Constants.Int16] = o;
			m_primitives [Constants.UInt16] = o;
			m_primitives [Constants.Int32] = o;
			m_primitives [Constants.UInt32] = o;
			m_primitives [Constants.Int64] = o;
			m_primitives [Constants.UInt64] = o;
			m_primitives [Constants.IntPtr] = o;
			m_primitives [Constants.UIntPtr] = o;
		}

		public void AddMemberRef (IMemberReference member)
		{
			m_membersRefContainer.Add (member);
		}

		private void ImportMemberRefFromReader ()
		{
			ReflectionReader reader = m_mod.Controller.Reader;
			if (reader.MemberReferences != null && reader.MemberReferences.Length > 0)
				foreach (MemberReference member in m_mod.Controller.Reader.MemberReferences)
					AddMemberRef (member);
		}

		public ITypeReference GetCoreType (string name)
		{
			return m_mod.Controller.Reader.SearchCoreType (name);
		}

		public uint GetRidFor (ITypeDefinition t)
		{
			if (m_types.Contains (t.FullName))
				return (uint) m_types [t.FullName];

			return 0;
		}

		public uint GetRidFor (ITypeReference t)
		{
			if (m_typesRef.Contains (t.FullName))
				return (uint) m_typesRef [t.FullName];

			return 0;
		}

		public uint GetRidFor (IMethodDefinition meth)
		{
			if (m_methods.Contains (meth.ToString ()))
				return (uint) m_methods [meth.ToString ()];

			return 0;
		}

		public uint GetRidFor (IFieldDefinition field)
		{
			if (m_fields.Contains (field.ToString ()))
				return (uint) m_fields [field.ToString ()];

			return 0;
		}

		public uint GetRidFor (IMemberReference member)
		{
			if (m_membersRef.Contains (member.ToString ()))
				return (uint) m_membersRef [member.ToString ()];

			return 0;
		}

		public uint GetRidFor (IAssemblyNameReference asmName)
		{
			return (uint) m_mod.AssemblyReferences.IndexOf (asmName) + 1;
		}

		public uint GetRidFor (IModuleDefinition mod)
		{
			return (uint) m_mod.Assembly.Modules.IndexOf (mod) + 1;
		}

		public uint GetRidFor (IModuleReference modRef)
		{
			return (uint) m_mod.ModuleReferences.IndexOf (modRef) + 1;
		}

		private MetadataToken GetTypeDefOrRefToken (ITypeReference type)
		{
			// TODO
			if (type is IArrayType) {

			} else if (type is IFunctionPointerType) {

			} else if (type is IModifierType) {

			} else if (type is IPinnedType) {

			} else if (type is IPointerType) {

			} else if (type is IReferenceType) {

			} else if (type is ITypeDefinition) {
				return new MetadataToken (
					TokenType.TypeDef, GetRidFor (type as ITypeDefinition));
			} else if (type != null) {
				return new MetadataToken (
					TokenType.TypeRef, GetRidFor (type));
			} else { // <Module> and interfaces
				return new MetadataToken (TokenType.TypeRef, 0);
			}

			return new MetadataToken ();
		}

		public override void Visit (ITypeDefinitionCollection types)
		{
			ArrayList orderedTypes = new ArrayList (types.Count);
			TypeDefTable tdTable = m_tableWriter.GetTypeDefTable ();

			if (types [Constants.ModuleType] == null) {
				TypeDefRow tdRow = m_rowWriter.CreateTypeDefRow (
					(TypeAttributes) 0,
					m_mdWriter.AddString ("<Module>"),
					m_mdWriter.AddString (""),
					new MetadataToken (TokenType.TypeRef, 0),
					m_fieldIndex,
					m_methodIndex
				);
				tdTable.Rows.Add (tdRow);
			}

			foreach (ITypeDefinition t in types)
				orderedTypes.Add (t);

			orderedTypes.Sort (TypeDefComparer.Instance);

			foreach (TypeDefinition t in orderedTypes)
				t.Accept (this);
		}

		public override void Visit (ITypeDefinition t)
		{
			TypeDefTable tdTable = m_tableWriter.GetTypeDefTable ();
			MetadataToken ext = GetTypeDefOrRefToken (t.BaseType);
			TypeDefRow tdRow = m_rowWriter.CreateTypeDefRow (
				t.Attributes,
				m_mdWriter.AddString (t.Name),
				m_mdWriter.AddString (t.Namespace),
				ext,
				m_fieldIndex,
				m_methodIndex
				);

			tdTable.Rows.Add (tdRow);
			m_types [t.FullName] = (uint) tdTable.Rows.Count;
		}

		private class TypeDefComparer : IComparer {

			public static readonly TypeDefComparer Instance = new TypeDefComparer ();

			private TypeDefComparer ()
			{
			}

			private bool Implements (TypeDefinition cur, TypeDefinition t)
			{
				if (t == null || cur == t || cur.Interfaces.Count == 0 || !t.IsInterface)
					return false;

				if (cur.Interfaces.Contains (t))
					return true;

				// Process hierarchy
				if(cur.BaseType != null && cur.BaseType is TypeDefinition)
					return Implements(cur.BaseType as TypeDefinition, t);

				return false;
			}

			private bool Extends (TypeDefinition cur, TypeDefinition t)
			{
				if (cur == t || cur.BaseType == null || !(cur.BaseType is TypeDefinition))
					return false;
				else if (cur.BaseType is TypeDefinition && cur.BaseType == t)
					return true;

				return Extends (cur.BaseType as TypeDefinition, t);
			}

			private bool DerivesFrom (TypeDefinition cur, TypeDefinition t)
			{
				if (t.IsInterface)
					return Implements (cur, t);
				else
					return Extends (cur, t);
			}

			public int Compare (object x, object y)
			{
				TypeDefinition a = x as TypeDefinition;
				TypeDefinition b = y as TypeDefinition;

				if (a == null || b == null)
					throw new ReflectionException ("TypeDefComparer can only compare TypeDefinition");

				if (a == b)
					return 0;

				if (a.Name == Constants.ModuleType)
					return -1;
				else if (b.Name == Constants.ModuleType)
					return 1;

				if (DerivesFrom (a, b))
					return 1;
				else if (DerivesFrom (b, a))
					return -1;

				return Comparer.Default.Compare (a.FullName, b.FullName);
			}
		}

		public override void Visit (ITypeReferenceCollection refs)
		{
			ImportMemberRefFromReader ();

			ArrayList orderedTypeRefs = new ArrayList (refs.Count);
			foreach (TypeReference tr in refs)
				if (!m_primitives.Contains (tr.FullName))
					orderedTypeRefs.Add (tr);

			orderedTypeRefs.Sort (TypeRefComparer.Instance);

			TypeRefTable trTable = m_tableWriter.GetTypeRefTable ();
			foreach (TypeReference t in orderedTypeRefs) {
				MetadataToken scope;
				if (t.DeclaringType != null)
					scope = new MetadataToken (TokenType.TypeRef, GetRidFor (t.DeclaringType));
				if (t.Scope is IAssemblyNameReference)
					scope = new MetadataToken (TokenType.AssemblyRef,
						GetRidFor (t.Scope as IAssemblyNameReference));
				else if (t.Scope is IModuleDefinition)
					scope = new MetadataToken (TokenType.Module,
						GetRidFor (t.Scope as IModuleDefinition));
				else if (t.Scope is IModuleReference)
					scope = new MetadataToken (TokenType.ModuleRef,
						GetRidFor (t.Scope as IModuleReference));
				else
					scope = new MetadataToken (TokenType.ExportedType, 0);

				TypeRefRow trRow = m_rowWriter.CreateTypeRefRow (
					scope,
					m_mdWriter.AddString (t.Name),
					m_mdWriter.AddString (t.Namespace));

				trTable.Rows.Add (trRow);
				m_typesRef [t.FullName] = (uint) trTable.Rows.Count;
			}

			Visit (m_mod.Types);
		}

		private class TypeRefComparer : IComparer {

			public static readonly TypeRefComparer Instance = new TypeRefComparer ();

			private TypeRefComparer ()
			{
			}

			public int Compare (object x, object y)
			{
				TypeReference a = x as TypeReference;
				TypeReference b = y as TypeReference;

				if (a == null || b == null)
					throw new ReflectionException ("TypeRefComparer can only compare TypeReference");

				if (b.DeclaringType == a)
					return -1;
				else if (a.DeclaringType == b)
					return 1;

				return Comparer.Default.Compare (a.FullName, b.FullName);
			}
		}

		public override void Visit (IInterfaceCollection interfaces)
		{
			// TODO
		}

		public override void Visit (IExternTypeCollection externs)
		{
			// TODO
		}

		public override void Visit (IOverrideCollection meth)
		{
			// TODO
		}

		public override void Visit (INestedTypeCollection nestedTypes)
		{
			// TODO
		}

		public override void Visit (IParameterDefinitionCollection parameters)
		{
			// TODO
		}

		public override void Visit (IParameterDefinition parameter)
		{
			// TODO
		}

		public override void Visit (IMethodDefinitionCollection methods)
		{
			// TODO
		}

		public override void Visit (IMethodDefinition method)
		{
			MethodTable mTable = m_tableWriter.GetMethodTable ();
			MethodRow mRow = m_rowWriter.CreateMethodRow (
				RVA.Zero,
				method.ImplAttributes,
				method.Attributes,
				m_mdWriter.AddString (method.Name),
				m_sigWriter.AddMethodDefSig (GetMethodDefSig (method)),
				m_paramIndex);

			mTable.Rows.Add (mRow);
			m_methodStack.Add (method);
			m_methods [method.ToString ()] = (uint) mTable.Rows.Count;
			m_methodIndex++;
		}

		public override void Visit (IPInvokeInfo pinvk)
		{
			// TODO
		}

		public override void Visit (IEventDefinitionCollection events)
		{
			// TODO
		}

		public override void Visit (IEventDefinition evt)
		{
			// TODO
		}

		public override void Visit (IFieldDefinitionCollection fields)
		{
			// TODO
		}

		public override void Visit (IFieldDefinition field)
		{
			FieldTable fTable = m_tableWriter.GetFieldTable ();
			FieldRow fRow = m_rowWriter.CreateFieldRow (
				field.Attributes,
				m_mdWriter.AddString (field.Name),
				m_sigWriter.AddFieldSig (GetFieldSig (field)));

			fTable.Rows.Add (fRow);
			m_fields [field.ToString ()] = (uint) fTable.Rows.Count;
			m_fieldIndex++;
		}

		public override void Visit (IPropertyDefinitionCollection properties)
		{
			// TODO
		}

		public override void Visit (IPropertyDefinition property)
		{
			// TODO
		}

		public override void Visit (ISecurityDeclarationCollection secDecls)
		{
			// TODO
		}

		public override void Visit (ISecurityDeclaration secDecl)
		{
			// TODO
		}

		public override void Visit (ICustomAttributeCollection customAttrs)
		{
			// TODO
		}

		public override void Visit (ICustomAttribute customAttr)
		{
			// TODO
		}

		public override void Visit (IMarshalSpec marshalSpec)
		{
			// TODO
		}

		public override void Terminate (ITypeDefinitionCollection colls)
		{
			MemberRefTable mrTable = m_tableWriter.GetMemberRefTable ();

			foreach (IMemberReference member in m_membersRefContainer) {
				uint sig = 0;
				if (member is IFieldReference)
					sig = m_sigWriter.AddFieldSig (GetFieldSig (member as IFieldReference));
				else if (member is IMethodReference)
					sig = m_sigWriter.AddMethodRefSig (GetMethodRefSig (member as IMethodReference));
				MemberRefRow mrRow = m_rowWriter.CreateMemberRefRow (
					GetTypeDefOrRefToken (member.DeclaringType),
					m_mdWriter.AddString (member.Name),
					sig);

				mrTable.Rows.Add (mrRow);
				m_membersRef [member.ToString ()] = (uint) mrTable.Rows.Count;
			}

			MethodTable mTable = m_tableWriter.GetMethodTable ();
			for (int i = 0; i < m_methodStack.Count; i++)
				mTable [i].RVA = m_codeWriter.WriteMethodBody (
					m_methodStack [i] as IMethodDefinition);

			if (m_mod.Assembly.EntryPoint != null)
				m_mdWriter.EntryPointToken =
					((uint) TokenType.Method) | GetRidFor (m_mod.Assembly.EntryPoint);

			(colls.Container as ModuleDefinition).Image.MetadataRoot.Accept (m_mdWriter);
		}

		public SigType GetSigType (ITypeReference type)
		{
			// TODO, complete ...
			string name = type.FullName;
			switch (name) {
			case Constants.Void :
				return new SigType (ElementType.Void);
			case Constants.Object :
				return new SigType (ElementType.Object);
			case Constants.Boolean :
				return new SigType (ElementType.Boolean);
			case Constants.String :
				return new SigType (ElementType.String);
			}

			CLASS c = new CLASS ();
			c.Type = GetTypeDefOrRefToken (type);
			return c;
		}

		public CustomMod [] GetCustomMods (ITypeReference type)
		{
			if (!(type is IModifierType))
				return new CustomMod [0];

			ArrayList cmods = new ArrayList ();
			ITypeReference cur = type;
			while (cur is IModifierType) {
				if (cur is IModifierOptional) {
					CustomMod cmod = new CustomMod ();
					cmod.CMOD = CustomMod.CMODType.OPT;
					cmod.TypeDefOrRef = GetTypeDefOrRefToken ((cur as IModifierOptional).ModifierType);
					cmods.Add (cmod);
				} else if (cur is IModifierRequired) {
					CustomMod cmod = new CustomMod ();
					cmod.CMOD = CustomMod.CMODType.REQD;
					cmod.TypeDefOrRef = GetTypeDefOrRefToken ((cur as IModifierRequired).ModifierType);
					cmods.Add (cmod);
				}

				cur = (cur as IModifierType).ElementType;
			}

			return cmods.ToArray (typeof (CustomMod)) as CustomMod [];
		}

		public Signature GetMemberRefSig (IMemberReference member)
		{
			if (member is IFieldReference)
				return GetFieldSig (member as IFieldReference);
			else
				return GetMemberRefSig (member as IMethodReference);
		}

		public FieldSig GetFieldSig (IFieldReference field)
		{
			FieldSig sig = new FieldSig ();
			sig.CallingConvention |= 0x6;
			sig.Field = true;
			sig.CustomMods = GetCustomMods (field.FieldType);
			sig.Type = GetSigType (field.FieldType);
			return sig;
		}

		private void CompleteMethodSig (IMethodReference meth, MethodSig sig)
		{
			sig.HasThis = meth.HasThis;
			sig.ExplicitThis = meth.ExplicitThis;
			if (sig.HasThis)
				sig.CallingConvention |= 0x20;
			if (sig.ExplicitThis)
				sig.CallingConvention |= 0x40;

			if ((meth.CallingConvention & MethodCallingConvention.VarArg) != 0)
				sig.CallingConvention |= 0x5;

			sig.ParamCount = meth.Parameters.Count;
			sig.Parameters = new Param [sig.ParamCount];
			for (int i = 0; i < sig.ParamCount; i++) {
				IParameterDefinition pDef = meth.Parameters [i];
				Param p = new Param ();
				p.CustomMods = GetCustomMods (pDef.ParameterType);
				if (pDef.ParameterType.FullName == Constants.TypedReference)
					p.TypedByRef = true;
				else if (pDef.ParameterType is IReferenceType) {
					p.ByRef = true;
					p.Type = GetSigType (
						(pDef.ParameterType as IReferenceType).ElementType);
				} else
					p.Type = GetSigType (pDef.ParameterType);
				sig.Parameters [i] = p;
			}

			RetType rtSig = new RetType ();
			rtSig.CustomMods = GetCustomMods (meth.ReturnType.ReturnType);

			if (meth.ReturnType.ReturnType.FullName == Constants.Void)
				rtSig.Void = true;
			else if (meth.ReturnType.ReturnType.FullName == Constants.TypedReference)
				rtSig.TypedByRef = true;
			else if (meth.ReturnType.ReturnType is IReferenceType) {
				rtSig.ByRef = true;
				rtSig.Type = GetSigType (
					(meth.ReturnType.ReturnType as IReferenceType).ElementType);
			} else
				rtSig.Type = GetSigType (meth.ReturnType.ReturnType);

			sig.RetType = rtSig;
		}


		public MethodRefSig GetMethodRefSig (IMethodReference meth)
		{
			MethodRefSig methSig = new MethodRefSig ();
			CompleteMethodSig (meth, methSig);

			if ((meth.CallingConvention & MethodCallingConvention.C) != 0)
				methSig.CallingConvention |= 0x1;
			else if ((meth.CallingConvention & MethodCallingConvention.StdCall) != 0)
				methSig.CallingConvention |= 0x2;
			else if ((meth.CallingConvention & MethodCallingConvention.ThisCall) != 0)
				methSig.CallingConvention |= 0x3;
			else if ((meth.CallingConvention & MethodCallingConvention.FastCall) != 0)
				methSig.CallingConvention |= 0x4;

			return methSig;
		}

		public MethodDefSig GetMethodDefSig (IMethodDefinition meth)
		{
			MethodDefSig sig = new MethodDefSig ();
			CompleteMethodSig (meth, sig);
			return sig;
		}
	}
}
