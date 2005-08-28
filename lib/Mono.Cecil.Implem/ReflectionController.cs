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

	internal sealed class ReflectionController {

		private ReflectionReader m_reader;
		private ReflectionWriter m_writer;
		private ReflectionHelper m_helper;

		public ReflectionReader Reader {
			get { return m_reader; }
		}

		public ReflectionWriter Writer {
			get { return m_writer; }
		}

		public ReflectionHelper Helper {
			get { return m_helper; }
		}

		public ReflectionController (ModuleDefinition module)
		{
			m_reader = new AggressiveReflectionReader (module);
			m_writer = new ReflectionWriter (module);
			m_helper = new ReflectionHelper (module);
		}
	}
}
