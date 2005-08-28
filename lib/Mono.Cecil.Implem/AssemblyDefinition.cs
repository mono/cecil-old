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
	using System.Reflection;

	using Mono.Cecil;

	internal class AssemblyDefinition : IAssemblyDefinition {

		private AssemblyNameDefinition m_asmName;
		private ModuleDefinitionCollection m_modules;
		private SecurityDeclarationCollection m_secDecls;
		private CustomAttributeCollection m_customAttrs;
		private MethodDefinition m_ep;
		private TargetRuntime m_runtime;

		private ModuleDefinition m_mainModule;
		private StructureReader m_reader;
		private StructureFactories m_factories;

		public IAssemblyNameDefinition Name {
			get { return m_asmName; }
		}

		public IModuleDefinitionCollection Modules {
			get { return m_modules; }
		}

		public ISecurityDeclarationCollection SecurityDeclarations {
			get {
				if (m_secDecls == null)
					m_secDecls = new SecurityDeclarationCollection (this);

				return m_secDecls;
			}
		}

		public ICustomAttributeCollection CustomAttributes {
			get {
				if (m_customAttrs == null)
					m_customAttrs = new CustomAttributeCollection (this);

				return m_customAttrs;
			}
		}

		public IMethodDefinition EntryPoint {
			get { return m_ep; }
			set { m_ep = value as MethodDefinition; }
		}

		public TargetRuntime Runtime {
			get { return m_runtime; }
			set { m_runtime = value; }
		}

		public IModuleDefinition MainModule {
			get { return this.MainModuleDefinition; }
		}

		public ModuleDefinition MainModuleDefinition {
			get {
				if (m_mainModule == null)
					foreach (ModuleDefinition module in m_modules)
						if (module.Main)
							m_mainModule = module;

				return m_mainModule;}
		}

		public StructureReader Reader {
			get { return m_reader; }
		}

		public IReflectionStructureFactories Factories {
			get { return m_factories; }
		}

		public AssemblyDefinition (AssemblyNameDefinition name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			m_asmName = name;
			m_modules = new ModuleDefinitionCollection (this);
			m_factories = new StructureFactories (this);
		}

		public AssemblyDefinition (AssemblyNameDefinition name, StructureReader reader) : this (name)
		{
			m_reader = reader;
		}

		public void Accept (IReflectionStructureVisitor visitor)
		{
			visitor.VisitAssemblyDefinition (this);

			m_asmName.Accept (visitor);
			m_modules.Accept (visitor);

			visitor.TerminateAssemblyDefinition (this);
		}
	}
}

