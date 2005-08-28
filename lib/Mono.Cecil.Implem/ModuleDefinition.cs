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

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;

	internal sealed class ModuleDefinition : IModuleDefinition {

		private string m_name;
		private Guid m_mvid;
		private bool m_main;
		private bool m_new;

		private AssemblyNameReferenceCollection m_asmRefs;
		private ModuleReferenceCollection m_modRefs;
		private ResourceCollection m_res;
		private TypeDefinitionCollection m_types;
		private TypeReferenceCollection m_refs;
		private ExternTypeCollection m_externs;
		private MemberReferenceCollection m_members;
		private CustomAttributeCollection m_customAttrs;

		private AssemblyDefinition m_asm;
		private Image m_image;

		private ImageReader m_imgReader;
		private ReflectionController m_controller;
		private ReflectionFactories m_factories;

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public Guid Mvid {
			get { return m_mvid; }
			set { m_mvid = value; }
		}

		public bool Main {
			get { return m_main; }
			set { m_main = value; }
		}

		public bool IsNew {
			get { return m_new; }
			set { m_new = value; }
		}

		public IAssemblyNameReferenceCollection AssemblyReferences {
			get { return m_asmRefs; }
		}

		public IModuleReferenceCollection ModuleReferences {
			get { return m_modRefs; }
		}

		public IResourceCollection Resources {
			get { return m_res; }
		}

		public ITypeDefinitionCollection Types {
			get { return m_types; }
		}

		public ITypeReferenceCollection TypeReferences {
			get { return m_refs; }
		}

		public IMemberReferenceCollection MemberReferences {
			get { return m_members; }
		}

		public IExternTypeCollection ExternTypes {
			get {
				if (m_externs == null)
					m_externs = new ExternTypeCollection (this);

				return m_externs;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public IAssemblyDefinition Assembly {
			get { return m_asm; }
		}

		public AssemblyDefinition Asm {
			get { return m_asm; }
		}

		public ReflectionController Controller {
			get { return m_controller; }
		}

		public ImageReader ImageReader {
			get { return m_imgReader; }
		}

		public Image Image {
			get { return m_image; }
			set { m_image = value; }
		}

		public IReflectionFactories Factories {
			get { return m_factories; }
		}

		public ModuleDefinition (string name, AssemblyDefinition asm) :
			this (name, asm, null, false)
		{
		}

		public ModuleDefinition (string name, AssemblyDefinition asm, bool main) :
			this (name, asm, null, main)
		{
		}

		public ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader) :
			this (name, asm, reader, false)
		{
		}

		public ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader, bool main)
		{
			if (asm == null)
				throw new ArgumentNullException ("asm");
			if (name == null || name.Length == 0)
				throw new ArgumentNullException ("name");

			m_asm = asm;
			m_name = name;
			m_main = main;
			m_mvid = Guid.NewGuid ();

			m_new = reader == null;

			if (!m_new) {
				m_image = reader.Image;
				m_imgReader = reader;
			} else
				m_image = Image.CreateImage ();

			m_factories = new ReflectionFactories (this);

			m_controller = new ReflectionController (this);
			m_modRefs = new ModuleReferenceCollection (this);
			m_asmRefs = new AssemblyNameReferenceCollection (this);
			m_res = new ResourceCollection (this);
			m_types = new TypeDefinitionCollection (this);
			m_types.OnTypeDefinitionAdded += new TypeDefinitionEventHandler (OnTypeDefinitionAdded);
			m_types.OnTypeDefinitionRemoved += new TypeDefinitionEventHandler (OnTypeDefinitionRemoved);
			m_refs = new TypeReferenceCollection (this);
			m_refs.OnTypeReferenceAdded += new TypeReferenceEventHandler (OnTypeReferenceAdded);
			m_refs.OnTypeReferenceRemoved += new TypeReferenceEventHandler (OnTypeReferenceRemoved);
			m_members = new MemberReferenceCollection (this);
		}

		private void OnTypeDefinitionAdded (Object sender, TypeDefinitionEventArgs ea)
		{
			if (ea.TypeDefinition.Module != null)
				throw new ReflectionException ("Type is already attached, clone it instead");

			(ea.TypeDefinition as TypeDefinition).Mod = this;
		}

		private void OnTypeDefinitionRemoved (Object sender, TypeDefinitionEventArgs ea)
		{
			(ea.TypeDefinition as TypeDefinition).Mod = null;
		}

		private void OnTypeReferenceAdded (Object sender, TypeReferenceEventArgs ea)
		{
			(ea.TypeReference as TypeReference).Mod = this;
		}

		private void OnTypeReferenceRemoved (Object sender, TypeReferenceEventArgs ea)
		{
			(ea.TypeReference as TypeReference).Mod = null;
		}

		public IMetadataTokenProvider LookupByToken (MetadataToken token)
		{
			return m_controller.Reader.LookupByToken (token);
		}

		public IMetadataTokenProvider LookupByToken (TokenType table, int rid)
		{
			return LookupByToken (new MetadataToken (table, (uint) rid));
		}

		public void Accept (IReflectionStructureVisitor visitor)
		{
			visitor.VisitModuleDefinition (this);

			this.AssemblyReferences.Accept (visitor);
			this.ModuleReferences.Accept (visitor);
			this.Resources.Accept (visitor);
		}

		public void Accept (IReflectionVisitor visitor)
		{
			visitor.VisitModuleDefinition (this);

			this.Types.Accept (visitor);
			this.TypeReferences.Accept (visitor);
		}
	}
}
