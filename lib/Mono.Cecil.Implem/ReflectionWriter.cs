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

	internal sealed class ReflectionWriter : BaseReflectionVisitor {

		private StructureWriter m_structureWriter;
		private ModuleDefinition m_mod;
		private CodeWriter m_codeWriter;
		private MetadataWriter m_mdWriter;
		private MetadataTableWriter m_tableWriter;
		private MetadataRowWriter m_rowWriter;

		public StructureWriter StructureWriter {
			get { return m_structureWriter; }
			set {
				m_structureWriter = value;
				m_mdWriter = new MetadataWriter (
					m_mod.Image.MetadataRoot, m_structureWriter.GetWriter ());
				m_tableWriter = m_mdWriter.GetTableVisitor ();
				m_rowWriter = m_tableWriter.GetRowVisitor () as MetadataRowWriter;
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
		}

		public override void Visit (ITypeDefinitionCollection types)
		{
//			TypeDefTable tdTable = m_tableWriter.GetTypeDefTable ();
//			foreach (TypeDefinition type in types) {
//				TypeDefRow tdRow = m_rowWriter.CreateTypeDefRow ();
//			}
		}

		public override void Visit (ITypeDefinition type)
		{
			// TODO
		}

		public override void Visit (ITypeReferenceCollection refs)
		{
			// TODO
		}

		public override void Visit (ITypeReference type)
		{
			// TODO
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
			// TODO
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
	}
}
