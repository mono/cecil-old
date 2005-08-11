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

		private uint m_methodIndex;
		private uint m_fieldIndex;

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
				m_codeWriter = new CodeWriter (m_mdWriter.CilWriter);
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

			m_methodIndex = 1;
			m_fieldIndex = 1;
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
			MetadataToken ext;
			if (t.BaseType is ITypeDefinition)
				ext = new MetadataToken (TokenType.TypeDef, GetRidFor (
						t.BaseType as ITypeDefinition));
			else if (t.BaseType is ITypeReference)
				ext = new MetadataToken (TokenType.TypeRef, GetRidFor (
						t.BaseType));
			else
				ext = new MetadataToken (TokenType.TypeRef, 0);
			// TODO, deal with type spec
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
				throw new NotImplementedException ();
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

				if (a.Name == Constants.ModuleType)
					return -1;
				else if (b.Name == Constants.ModuleType)
					return 1;

				if (DerivesFrom (a, b))
					return -1;
				else if (DerivesFrom (b, a))
					return 1;

				return Comparer.Default.Compare (a.FullName, b.FullName);
			}
		}

		public override void Visit (ITypeReferenceCollection refs)
		{
			ArrayList orderedTypeRefs = new ArrayList (refs.Count);
			foreach (TypeReference tr in refs)
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
			// TODO
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
//			FieldTable fTable = m_tableWriter.GetFieldTable ();
//			FieldRow fRow = m_rowWriter.CreateFieldRow (
//				field.Attributes,
//				m_mdWriter.AddString (field.Name),
//				m_sigWriter.AddFieldSig (field));
//
//			fTable.Rows.Add (fRow);
//			m_fieldIndex++;
//			m_fields [field.ToString ()] = field;
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
			// for each method codewrite!
			(colls.Container as ModuleDefinition).Image.MetadataRoot.Accept (m_mdWriter);
		}
	}
}
