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

		private AssemblyNameReferenceCollection m_asmRefs;
		private ModuleReferenceCollection m_modRefs;
		private ResourceCollection m_res;
		private TypeDefinitionCollection m_types;
		private TypeReferenceCollection m_refs;
		private ExternTypeCollection m_externs;
		private CustomAttributeCollection m_customAttrs;

		private AssemblyDefinition m_asm;
		private Image m_image;

		private ImageReader m_imgReader;
		private ReflectionController m_controller;

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

		public IExternTypeCollection ExternTypes {
			get {
				if (m_externs == null)
					m_externs = new ExternTypeCollection (this, m_controller);
				return m_externs;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this, m_controller);
				return m_customAttrs;
			}
		}

		public AssemblyDefinition Assembly {
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

		public ModuleDefinition (string name, AssemblyDefinition asm) : this (name, asm, null, false)
		{
		}

		public ModuleDefinition (string name, AssemblyDefinition asm, bool main) : this (name, asm, null, main)
		{
		}

		public ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader) : this (name, asm, reader, false)
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

			if (reader != null) {
				m_image = reader.Image;
				m_imgReader = reader;
			} else
				m_image = Image.CreateImage ();

			m_controller = new ReflectionController (this, asm.LoadingType);
			m_modRefs = new ModuleReferenceCollection (this);
			m_asmRefs = new AssemblyNameReferenceCollection (this);
			m_res = new ResourceCollection (this);
			m_types = new TypeDefinitionCollection (this);
			m_refs = new TypeReferenceCollection (this);
		}

		public IAssemblyNameReference DefineAssemblyReference (string name)
		{
			AssemblyNameReference asmRef = new AssemblyNameReference ();
			asmRef.Name = name;
			m_asmRefs.Add (asmRef);
			return asmRef;
		}

		public IModuleReference DefineModuleReference (string module)
		{
			ModuleReference mod = new ModuleReference (module);
			m_modRefs.Add (mod);
			return mod;
		}

		public IEmbeddedResource DefineEmbeddedResource (string name, ManifestResourceAttributes attributes, byte [] data)
		{
			EmbeddedResource res = new EmbeddedResource (name, attributes, this, data);
			m_res [name] = res;
			return res;
		}

		public ILinkedResource DefineLinkedResource (string name, ManifestResourceAttributes attributes, string file)
		{
			LinkedResource res = new LinkedResource (name, attributes, this, file);
			m_res [name] = res;
			return res;
		}

		public IAssemblyLinkedResource DefineAssemblyLinkedResource (string name, ManifestResourceAttributes attributes,
																	 IAssemblyNameReference asm)
		{
			AssemblyLinkedResource res = new AssemblyLinkedResource (name, attributes, this, asm as AssemblyNameReference);
			m_res [name] = res;
			return res;
		}

		public ITypeDefinition DefineType (string name, string ns, TypeAttributes attributes)
		{
			return DefineType (name, ns, attributes, typeof (object));
		}

		public ITypeDefinition DefineType (string name, string ns, TypeAttributes attributes, ITypeReference baseType)
		{
			TypeDefinition type = new TypeDefinition (name, ns, attributes, this);
			type.BaseType = baseType;
			m_types [type.FullName] = type;
			return type;
		}

		public ITypeDefinition DefineType (string name, string ns, TypeAttributes attributes, Type baseType)
		{
			return DefineType (name, ns, attributes, m_controller.Helper.RegisterType (baseType));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor)
		{
			CustomAttribute ca = new CustomAttribute(ctor);
			m_customAttrs.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor)
		{
			return DefineCustomAttribute (m_controller.Helper.RegisterConstructor(ctor));
		}

		public ICustomAttribute DefineCustomAttribute (IMethodReference ctor, byte [] data)
		{
			CustomAttribute ca = m_controller.Reader.GetCustomAttribute (ctor, data);
			m_customAttrs.Add (ca);
			return ca;
		}

		public ICustomAttribute DefineCustomAttribute (System.Reflection.ConstructorInfo ctor, byte [] data)
		{
			return DefineCustomAttribute (
				m_controller.Helper.RegisterConstructor(ctor), data);
		}

		public ITypeReference DefineTypeReference (string name, string ns, IAssemblyNameReference asm)
		{
			TypeReference typeRef = new TypeReference (name, ns, this, asm);
			m_refs [typeRef.FullName] = typeRef;
			return typeRef;
		}

		public IFieldReference DefineFieldReference (string name, ITypeReference declaringType,
			ITypeReference fieldType)
		{
			FieldReference fieldRef = new FieldReference (name,
				declaringType, fieldType);
			m_controller.Writer.AddMemberRef (fieldRef);
			return fieldRef;
		}

		public IMethodReference DefineMethodReference (string name, ITypeReference declaringType,
			ITypeReference returnType, ITypeReference [] parametersTypes,
			bool hasThis, bool explicitThis, MethodCallingConvention conv)
		{
			MethodReference meth = new MethodReference (name, declaringType, hasThis, explicitThis, conv);
			meth.ReturnType.ReturnType = returnType;
			int seq = 1;
			foreach (ITypeReference t in parametersTypes)
				(meth.Parameters as ParameterDefinitionCollection).Add (new ParameterDefinition (
						string.Empty, seq++, (ParamAttributes) 0, t));
			m_controller.Writer.AddMemberRef (meth);
			return meth;
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
			visitor.Visit (this);

			this.AssemblyReferences.Accept (visitor);
			this.ModuleReferences.Accept (visitor);
			this.Resources.Accept (visitor);
		}
	}
}

