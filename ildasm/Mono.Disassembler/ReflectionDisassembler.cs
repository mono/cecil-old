//
// ReflectionDisassembler.cs
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

	using Mono.Cecil;

	class ReflectionDisassembler : BaseReflectionVisitor {

		StructureDisassembler m_structDis;
		CodeDisassembler m_codeDis;
		CilWriter m_writer;

		public ReflectionDisassembler (StructureDisassembler sd)
		{
			m_structDis = sd;
			m_codeDis = new CodeDisassembler (this);
		}

		public void DisassembleModule (ModuleDefinition module, CilWriter writer)
		{
			m_writer = writer;

			VisitTypeDefinitionCollection (module.Types);
		}

		public override void VisitTypeDefinitionCollection (TypeDefinitionCollection types)
		{
			foreach (TypeDefinition type in types) {
				VisitTypeDefinition (type);
			}
		}

		static readonly int [] m_typeVisibilityVals = new int [] {
			(int) TypeAttributes.NotPublic, (int) TypeAttributes.Public, (int) TypeAttributes.NestedPublic,
			(int) TypeAttributes.NestedPrivate, (int) TypeAttributes.NestedFamily, (int) TypeAttributes.NestedFamANDAssem,
			(int) TypeAttributes.NestedFamORAssem,
		};

		static readonly string [] m_typeVisibilityMap = new string [] {
			"private", "public", "nested public", "nested private",
			"nested family", "nested assembly", "nested famandassem",
			"nested famorassem"
		};

		static readonly int [] m_typeLayoutVals = new int [] {
			(int) TypeAttributes.AutoLayout, (int) TypeAttributes.SequentialLayout, (int) TypeAttributes.ExplicitLayout
		};

		static readonly string [] m_typeLayoutMap = new string [] {
			"auto", "sequential", "explicit"
		};

		static readonly int [] m_typeFormatVals = new int [] {
			(int) TypeAttributes.AnsiClass, (int) TypeAttributes.UnicodeClass, (int) TypeAttributes.AutoClass
		};

		static readonly string [] m_typeFormatMap = new string [] {
			"ansi", "unicode", "auto"
		};

		static readonly int [] m_typeVals = new int [] {
			(int) TypeAttributes.Abstract, (int) TypeAttributes.Sealed, (int) TypeAttributes.SpecialName,
			(int) TypeAttributes.Import, (int) TypeAttributes.Serializable, (int) TypeAttributes.BeforeFieldInit
		};

		static readonly string [] m_typeMap = new string [] {
			"abstract", "sealed", "special-name", "import", "serializable", "beforefieldinit"
		};

		void WriteTypeHeader (TypeDefinition type)
		{
			m_writer.Write (".class ");

			if ((type.Attributes & TypeAttributes.ClassSemanticMask) == TypeAttributes.Interface)
				m_writer.BaseWriter.Write ("interface ");

			int attributes = (int) type.Attributes;
			m_writer.WriteFlags (attributes, (int) TypeAttributes.VisibilityMask,
				m_typeVisibilityVals, m_typeVisibilityMap);
			m_writer.WriteFlags (attributes, (int) TypeAttributes.LayoutMask,
				m_typeLayoutVals, m_typeLayoutMap);
			m_writer.WriteFlags (attributes, (int) TypeAttributes.StringFormatMask,
				m_typeFormatVals, m_typeFormatMap);
			m_writer.WriteFlags (attributes, m_typeVals, m_typeMap);

			m_writer.BaseWriter.WriteLine (Formater.Escape (type.Name));

			if (type.BaseType != null) {
				m_writer.Indent ();
				m_writer.Write ("extends ");
				m_writer.BaseWriter.WriteLine (Formater.Format (type.BaseType));
				m_writer.Unindent ();
			}
		}

		void WriteTypeBody (TypeDefinition type)
		{
			VisitCustomAttributeCollection (type.CustomAttributes);
			VisitSecurityDeclarationCollection (type.SecurityDeclarations);

			if (type.HasLayoutInfo) {
				m_writer.Write (".pack ");
				m_writer.BaseWriter.WriteLine (type.LayoutInfo.PackingSize);
				m_writer.Write (".size ");
				m_writer.BaseWriter.WriteLine (type.LayoutInfo.ClassSize);
			}

			VisitFieldDefinitionCollection (type.Fields);
			VisitConstructorCollection (type.Constructors);
			VisitMethodDefinitionCollection (type.Methods);
			VisitPropertyDefinitionCollection (type.Properties);
			VisitEventDefinitionCollection (type.Events);
		}

		public override void VisitTypeDefinition (TypeDefinition type)
		{
			if (type.DeclaringType != null)
				return;

			if (type.Namespace.Length > 0) {
				m_writer.Write (".namespace ");
				m_writer.BaseWriter.WriteLine (type.Namespace);
				m_writer.OpenBlock ();
			}

			WriteTypeHeader (type);

			m_writer.OpenBlock ();

			WriteTypeBody (type);
			VisitNestedTypeCollection (type.NestedTypes);

			m_writer.CloseBlock ();

			if (type.Namespace.Length > 0)
				m_writer.CloseBlock ();

			m_writer.WriteLine ();
		}

		public override void VisitFieldDefinitionCollection (FieldDefinitionCollection fields)
		{
			foreach (FieldDefinition field in fields)
				VisitFieldDefinition (field);
		}

		public override void VisitFieldDefinition (FieldDefinition field)
		{
		}

		public override void VisitMethodDefinitionCollection (MethodDefinitionCollection methods)
		{
			foreach (MethodDefinition meth in methods)
				VisitMethodDefinition (meth);
		}

		public override void VisitConstructorCollection (ConstructorCollection ctors)
		{
			foreach (MethodDefinition ctor in ctors)
				VisitMethodDefinition (ctor);
		}

		public override void VisitMethodDefinition (MethodDefinition meth)
		{
			//m_codeDis.DisassembleMethod (meth, m_writer);
		}

		public override void VisitNestedTypeCollection (NestedTypeCollection nesteds)
		{
			for (int i = 0; i < nesteds.Count; i++) {
				if (i > 0)
					m_writer.WriteLine ();
				VisitNestedType (nesteds [i]);
			}
		}

		public override void VisitNestedType (TypeDefinition type)
		{
			WriteTypeHeader (type);

			m_writer.OpenBlock ();
			WriteTypeBody (type);
			m_writer.CloseBlock ();
		}
	}
}
