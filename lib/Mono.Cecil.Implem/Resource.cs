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

	using Mono.Cecil;
	using Mono.Cecil.Metadata;

	internal abstract class Resource :  IResource {

		private ModuleDefinition m_module;
		private string m_name;
		private ManifestResourceAttributes m_flags;

		public ModuleDefinition Module {
			get { return m_module; }
		}

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public ManifestResourceAttributes Flags {
			get { return m_flags; }
			set { m_flags = value; }
		}

		protected Resource(string name, ManifestResourceAttributes flags, ModuleDefinition owner) : this (owner)
		{
			m_name = name;
			m_flags = flags;
		}

		private Resource (ModuleDefinition owner)
		{
			m_module = owner;
		}

		public abstract void Accept(IReflectionStructureVisitor visitor);
	}
}

