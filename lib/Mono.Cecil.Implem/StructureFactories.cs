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
	using SR = System.Reflection;

	using Mono.Cecil;

	internal sealed class StructureFactories :  IReflectionStructureFactories, IAssemblyReferenceFactory,
		IResourceFactory, IModuleFactory {

		private ModuleDefinition m_mod;

		public IAssemblyReferenceFactory AssemblyReferenceFactory {
			get { return this; }
		}

		public IResourceFactory ResourceFactory {
			get { return this; }
		}

		public IModuleFactory ModuleFactory {
			get { return this; }
		}

		public StructureFactories (AssemblyDefinition asm)
		{
			m_mod = asm.MainModuleDefinition;
		}

		public IAssemblyNameReference CreateAssemblyNameReference (string name)
		{
			return CreateAssemblyNameReference (name, string.Empty);
		}

		public IAssemblyNameReference CreateAssemblyNameReference (string name, string culture)
		{
			return CreateAssemblyNameReference (name, string.Empty, new Version (0, 0, 0, 0));
		}

		public IAssemblyNameReference CreateAssemblyNameReference (string name, string culture, Version ver)
		{
			return CreateAssemblyNameReference (name, string.Empty, new Version (0, 0, 0, 0), new byte [0]);
		}

		public IAssemblyNameReference CreateAssemblyNameReference (string name, string culture,
			Version ver, byte [] publicKeyToken)
		{
			AssemblyNameReference asmRef = new AssemblyNameReference (name, culture, ver);
			asmRef.PublicKey = new byte [0];
			asmRef.Hash = new byte [0];
			asmRef.PublicKeyToken = publicKeyToken;
			return asmRef;
		}

		public IAssemblyNameReference CreateFromAssembly (SR.Assembly asm)
		{
			return m_mod.Controller.Helper.CheckAssemblyReference (asm);
		}

		public IAssemblyNameReference CreateFromFullyQualifiedName (string fqName)
		{
			throw new NotImplementedException (); // TODO
		}

		public IEmbeddedResource CreateEmbeddedResource (string name,
			ManifestResourceAttributes attributes, byte [] data)
		{
			return new EmbeddedResource (name, attributes, data);
		}

		public ILinkedResource CreateLinkedResource (string name,
			ManifestResourceAttributes attributes, string file)
		{
			return new LinkedResource (name, attributes, file);
		}

		public IAssemblyLinkedResource CreateAssemblyLinkedResource (string name,
			ManifestResourceAttributes attributes, IAssemblyNameReference asm)
		{
			return new AssemblyLinkedResource (name, attributes, asm as AssemblyNameReference);
		}

		public IAssemblyLinkedResource CreateAssemblyLinkedResource (string name,
			ManifestResourceAttributes attributes, SR.Assembly asm)
		{
			return new AssemblyLinkedResource (name, attributes,
				m_mod.Controller.Helper.CheckAssemblyReference (asm));
		}

		public IModuleDefinition CreateModule (string name)
		{
			throw new NotImplementedException ("Multiple module assemblies are not implemented");
		}

		public IModuleReference CreateModuleReference (string name)
		{
			return new ModuleReference (name);
		}
	}
}

