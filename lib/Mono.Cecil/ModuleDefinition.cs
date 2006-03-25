//
// ModuleDefinition.cs
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
	using SR = System.Reflection;
	using SS = System.Security;
	using SSP = System.Security.Permissions;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;
	using Mono.Xml;

	public sealed class ModuleDefinition : ModuleReference, IModuleDefinition {

		Guid m_mvid;
		bool m_main;
		bool m_new;

		AssemblyNameReferenceCollection m_asmRefs;
		ModuleReferenceCollection m_modRefs;
		ResourceCollection m_res;
		TypeDefinitionCollection m_types;
		TypeReferenceCollection m_refs;
		ExternTypeCollection m_externs;
		MemberReferenceCollection m_members;
		CustomAttributeCollection m_customAttrs;

		AssemblyDefinition m_asm;
		Image m_image;

		ImageReader m_imgReader;
		ReflectionController m_controller;
		SecurityDeclarationReader m_secReader;

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

		public AssemblyNameReferenceCollection AssemblyReferences {
			get { return m_asmRefs; }
		}

		public ModuleReferenceCollection ModuleReferences {
			get { return m_modRefs; }
		}

		public ResourceCollection Resources {
			get { return m_res; }
		}

		public TypeDefinitionCollection Types {
			get { return m_types; }
		}

		public TypeReferenceCollection TypeReferences {
			get { return m_refs; }
		}

		public MemberReferenceCollection MemberReferences {
			get { return m_members; }
		}

		public ExternTypeCollection ExternTypes {
			get {
				if (m_externs == null)
					m_externs = new ExternTypeCollection (this);

				return m_externs;
			}
		}

		public CustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public AssemblyDefinition Assembly {
			get { return m_asm; }
		}

		internal ReflectionController Controller {
			get { return m_controller; }
		}

		internal ImageReader ImageReader {
			get { return m_imgReader; }
		}

		public Image Image {
			get { return m_image; }
			set {
				m_image = value;
				m_secReader = null;
			}
		}

		public ModuleDefinition (string name, AssemblyDefinition asm) :
			this (name, asm, null, false)
		{
		}

		public ModuleDefinition (string name, AssemblyDefinition asm, bool main) :
			this (name, asm, null, main)
		{
		}

		internal ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader) :
			this (name, asm, reader, false)
		{
		}

		internal ModuleDefinition (string name, AssemblyDefinition asm, ImageReader reader, bool main) : base (name)
		{
			if (asm == null)
				throw new ArgumentNullException ("asm");
			if (name == null || name.Length == 0)
				throw new ArgumentNullException ("name");

			m_asm = asm;
			m_main = main;
#if !CF_1_0
			m_mvid = Guid.NewGuid ();
#endif
			m_new = reader == null;

			if (!m_new) {
				m_image = reader.Image;
				m_imgReader = reader;
			} else
				m_image = Image.CreateImage ();

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

			m_controller = new ReflectionController (this);
		}

		void OnTypeDefinitionAdded (Object sender, TypeDefinitionEventArgs ea)
		{
			if (ea.TypeDefinition.Module != null)
				throw new ReflectionException ("Type is already attached, clone it instead");

			ea.TypeDefinition.Module = this;
			ea.TypeDefinition.AttachToScope (this);
		}

		void OnTypeDefinitionRemoved (Object sender, TypeDefinitionEventArgs ea)
		{
			ea.TypeDefinition.Module = null;
		}

		void OnTypeReferenceAdded (Object sender, TypeReferenceEventArgs ea)
		{
			ea.TypeReference.Module = this;
		}

		void OnTypeReferenceRemoved (Object sender, TypeReferenceEventArgs ea)
		{
			ea.TypeReference.Module = null;
		}

		public IMetadataTokenProvider LookupByToken (MetadataToken token)
		{
			return m_controller.Reader.LookupByToken (token);
		}

		public IMetadataTokenProvider LookupByToken (TokenType table, int rid)
		{
			return LookupByToken (new MetadataToken (table, (uint) rid));
		}

		public TypeReference Import (Type type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			return m_controller.Helper.ImportSystemType (type);
		}

		public MethodReference Import (SR.MethodBase meth)
		{
			if (meth == null)
				throw new ArgumentNullException ("meth");

			if (meth is SR.ConstructorInfo)
				return m_controller.Helper.ImportConstructorInfo (meth as SR.ConstructorInfo);
			else
				return m_controller.Helper.ImportMethodInfo (meth as SR.MethodInfo);
		}

		public FieldReference Import (SR.FieldInfo field)
		{
			if (field == null)
				throw new ArgumentNullException ("field");

			return m_controller.Helper.ImportFieldInfo (field);
		}

		public TypeReference Import (TypeReference type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			return m_controller.Helper.ImportTypeReference (type);
		}

		public MethodReference Import (MethodReference meth)
		{
			if (meth == null)
				throw new ArgumentNullException ("meth");

			return m_controller.Helper.ImportMethodReference (meth);
		}

		public FieldReference Import (FieldReference field)
		{
			if (field == null)
				throw new ArgumentNullException ("field");

			return m_controller.Helper.ImportFieldReference (field);
		}

		public TypeDefinition Inject (TypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			return m_controller.Helper.ImportTypeDefinition (type);
		}

		public MethodDefinition Inject (MethodDefinition meth)
		{
			if (meth == null)
				throw new ArgumentNullException ("meth");

			return m_controller.Helper.ImportMethodDefinition (meth);
		}

		public FieldDefinition Inject (FieldDefinition field)
		{
			if (field == null)
				throw new ArgumentNullException ("field");

			return m_controller.Helper.ImportFieldDefinition (field);
		}

		public void FullLoad ()
		{
			foreach (TypeDefinition type in this.Types) {
				foreach (MethodDefinition meth in type.Methods)
					meth.LoadBody ();
				foreach (MethodDefinition ctor in type.Constructors)
					ctor.LoadBody ();
			}
		}

		public byte [] GetAsByteArray (CustomAttribute ca)
		{
			CustomAttribute customAttr = ca as CustomAttribute;
			if (!ca.IsReadable)
				if (customAttr.Blob != null)
					return customAttr.Blob;
				else
					return new byte [0];

			return m_controller.Writer.SignatureWriter.CompressCustomAttribute (
				m_controller.Writer.GetCustomAttributeSig (ca), ca.Constructor);
		}

		public byte [] GetAsByteArray (SecurityDeclaration dec)
		{
			// TODO - add support for 2.0 format
			// note: the 1.x format is still supported in 2.0 so this isn't an immediate problem
			if (!dec.IsReadable)
				return dec.Blob;

			if (dec.PermissionSet != null)
				return Encoding.Unicode.GetBytes (dec.PermissionSet.ToXml ().ToString ());

			return new byte [0];
		}

		public CustomAttribute FromByteArray (MethodReference ctor, byte [] data)
		{
			return m_controller.Reader.GetCustomAttribute (ctor, data);
		}

		public SecurityDeclaration FromByteArray (SecurityAction action, byte [] declaration)
		{
			if (m_secReader == null)
				m_secReader = new SecurityDeclarationReader (Image.MetadataRoot, m_controller.Reader);
			return m_secReader.FromByteArray (action, declaration);
		}

		public override void Accept (IReflectionStructureVisitor visitor)
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

		public override string ToString ()
		{
			string s = (m_main ? "(main), Mvid=" : "Mvid=");
			return s + m_mvid.ToString ();
		}
	}
}
