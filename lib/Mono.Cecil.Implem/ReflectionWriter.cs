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

	using System.IO;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;

	internal sealed class ReflectionWriter : IReflectionVisitor {

		private StructureWriter m_structureWriter;
		private ModuleDefinition m_mod;
		private CodeWriter m_codeWriter;
		private MetadataWriter m_mdWriter;
		private MetadataTableWriter m_tableWriter;
		private MetadataRowWriter m_rowWriter;

		public StructureWriter StructureWriter {
			get { return m_structureWriter; }
			set { m_structureWriter = value; }
		}

		public CodeWriter CodeWriter {
			get { return m_codeWriter; }
		}

		public ReflectionWriter (ModuleDefinition mod)
		{
			m_mod = mod;
			m_codeWriter = new CodeWriter ();
			m_mdWriter = new MetadataWriter (this, m_mod.Image.MetadataRoot);
			m_tableWriter = m_mdWriter.GetTableVisitor ();
			m_rowWriter = m_tableWriter.GetRowVisitor () as MetadataRowWriter;
		}

		public BinaryWriter GetWriter ()
		{
			if (m_structureWriter != null)
				return m_structureWriter.GetWriter ();

			return null;
		}

		public void Visit (ITypeDefinitionCollection types)
		{
			TypeDefTable tdTable = m_tableWriter.GetTypeDefTable ();
			foreach (TypeDefinition type in types) {
				//TypeDefRow tdRow = m_rowWriter.CreateTypeDefRow ()
			}
		}

		public void Visit (ITypeDefinition type)
		{

		}

		public void Visit (ITypeReferenceCollection refs)
		{
			// TODO
		}

		public void Visit (ITypeReference type)
		{
			// TODO
		}

		public void Visit (IInterfaceCollection interfaces)
		{
			// TODO
		}

		public void Visit (IExternTypeCollection externs)
		{
			// TODO
		}

		public void Visit (IOverrideCollection meth)
		{
			// TODO
		}

		public void Visit (INestedTypeCollection nestedTypes)
		{
			// TODO
		}

		public void Visit (IParameterDefinitionCollection parameters)
		{
			// TODO
		}

		public void Visit (IParameterDefinition parameter)
		{
			// TODO
		}

		public void Visit (IMethodDefinitionCollection methods)
		{
			// TODO
		}

		public void Visit (IMethodDefinition method)
		{
			// TODO
		}

		public void Visit (IPInvokeInfo pinvk)
		{
			// TODO
		}

		public void Visit (IEventDefinitionCollection events)
		{
			// TODO
		}

		public void Visit (IEventDefinition evt)
		{
			// TODO
		}

		public void Visit (IFieldDefinitionCollection fields)
		{
			// TODO
		}

		public void Visit (IFieldDefinition field)
		{
			// TODO
		}

		public void Visit (IPropertyDefinitionCollection properties)
		{
			// TODO
		}

		public void Visit (IPropertyDefinition property)
		{
			// TODO
		}

		public void Visit (ISecurityDeclarationCollection secDecls)
		{
			// TODO
		}

		public void Visit (ISecurityDeclaration secDecl)
		{
			// TODO
		}

		public void Visit (ICustomAttributeCollection customAttrs)
		{
			// TODO
		}

		public void Visit (ICustomAttribute customAttr)
		{
			// TODO
		}

		public void Visit (IMarshalSpec marshalSpec)
		{
			// TODO
		}
	}
}
