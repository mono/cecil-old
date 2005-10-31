//
// StructureDisassembler.cs
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

namespace Mono.Disassembler {

	using System;

	using Mono.Cecil;

	class StructureDisassembler : BaseStructureVisitor {

		ReflectionDisassembler m_rd;
		CilWriter m_writer;

		public CilWriter Writer {
			get { return m_writer; }
		}

		public StructureDisassembler ()
		{
			m_rd = new ReflectionDisassembler (this);
		}

		public void DisassembleManifest (AssemblyDefinition asm, CilWriter writer)
		{
			m_writer = writer;

			VisitModuleReferenceCollection (asm.MainModule.ModuleReferences);
			VisitAssemblyNameReferenceCollection (asm.MainModule.AssemblyReferences);
			VisitAssemblyDefinition (asm);
			VisitModuleDefinition (asm.MainModule);
		}

		public void DisassembleAssembly (AssemblyDefinition asm, CilWriter writer)
		{
			writer.WriteLine ("// Mono CIL Disassembler");
			writer.WriteLine ("// Prototype using Mono.Cecil");
			writer.WriteLine ();

			DisassembleManifest (asm, writer);

			m_rd.DisassembleModule (asm.MainModule, writer);
		}

		public override void VisitModuleReferenceCollection (ModuleReferenceCollection modules)
		{
			foreach (ModuleReference module in modules) {
				m_writer.Write (".module extern ");
				m_writer.WriteLine (Formater.Escape (module.Name));
			}

			if (modules.Count > 0)
				m_writer.WriteLine ();
		}

		public override void VisitAssemblyNameReferenceCollection (AssemblyNameReferenceCollection references)
		{
			foreach (AssemblyNameReference reference in references) {
				VisitAssemblyNameReference (reference);
				m_writer.WriteLine ();
			}
		}

		void AssemblyDetails (AssemblyNameReference name)
		{
			m_writer.Write (".ver ");
			if (name.Version == null)
				m_writer.BaseWriter.WriteLine ("0:0:0:0");
			else {
				Version ver = name.Version;
				m_writer.BaseWriter.WriteLine ("{0}:{1}:{2}:{3}", ver.Major, ver.Minor, ver.Revision, ver.Build);
			}

			if (name.PublicKeyToken != null && name.PublicKeyToken.Length > 0) {
				m_writer.Write (".publickeytoken = ");
				m_writer.Write (name.PublicKeyToken);
			}

			if (name.PublicKey != null && name.PublicKey.Length > 0) {
				m_writer.Write (".publickey = ");
				m_writer.Write (name.PublicKey);
			}

			if (name.HashAlgorithm != AssemblyHashAlgorithm.None) {
				m_writer.Write (".hash algorithm 0x");
				m_writer.BaseWriter.WriteLine (name.HashAlgorithm.ToString ("x"));
			}
		}

		public override void VisitAssemblyNameReference (AssemblyNameReference reference)
		{
			m_writer.Write (".assembly extern ");
			m_writer.WriteLine (Formater.Escape (reference.Name));
			m_writer.OpenBlock ();
			AssemblyDetails (reference);
			m_writer.CloseBlock ();
		}

		public override void VisitAssemblyDefinition (AssemblyDefinition asm)
		{
			m_writer.Write (".assembly ");
			m_writer.WriteLine (Formater.Escape (asm.Name.Name));
			m_writer.OpenBlock ();
			AssemblyDetails (asm.Name);

			m_rd.VisitCustomAttributeCollection (asm.CustomAttributes);
			m_rd.VisitSecurityDeclarationCollection (asm.SecurityDeclarations);

			m_writer.CloseBlock ();
			m_writer.WriteLine ();
		}

		public override void VisitModuleDefinition (ModuleDefinition module)
		{
			m_writer.Write (".module ");
			m_writer.Write (Formater.Escape (module.Name));
			m_writer.Write (" // GUID: ");
			m_writer.WriteLine (module.Mvid);

			m_writer.WriteLine ();
		}

		public override void VisitResourceCollection (ResourceCollection resources)
		{
			for (int i = 0; i < resources.Count; i++) {
				if (i > 0)
					m_writer.WriteLine ();

				Resource r = resources [i];
				WriteResourceHeader (r);
				m_writer.OpenBlock ();
				r.Accept (this);
				m_writer.CloseBlock ();
			}
		}

		void WriteResourceHeader (Resource r)
		{
			m_writer.Write ("mresource ");
			m_writer.BaseWriter.Write ((r.Flags & ManifestResourceAttributes.Public) > 0 ? "public" : "private");
			m_writer.BaseWriter.Write (" ");
			m_writer.BaseWriter.WriteLine (Formater.Escape (r.Name));
		}

		public override void VisitAssemblyLinkedResource (AssemblyLinkedResource alr)
		{
			m_writer.Write (".assembly extern ");
			m_writer.BaseWriter.WriteLine (Formater.Escape (alr.Assembly.Name));
		}

		public override void VisitLinkedResource (LinkedResource lr)
		{
			m_writer.Write (".file ");
			m_writer.BaseWriter.Write (Formater.Escape (lr.File));
			m_writer.BaseWriter.WriteLine (" at 0x0");
		}

		public override void VisitEmbeddedResource (EmbeddedResource er)
		{
			// TODO
		}
	}
}
