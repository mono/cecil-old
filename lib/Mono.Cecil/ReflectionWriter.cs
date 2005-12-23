//
// ReflectionWriter.cs
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
	using System.Collections;
	using System.IO;
	using System.Text;

	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class ReflectionWriter : BaseReflectionVisitor {

		StructureWriter m_structureWriter;
		ModuleDefinition m_mod;
		SignatureWriter m_sigWriter;
		CodeWriter m_codeWriter;
		MetadataWriter m_mdWriter;
		MetadataTableWriter m_tableWriter;
		MetadataRowWriter m_rowWriter;

		IList m_methodStack;
		IList m_fieldStack;

		uint m_methodIndex;
		uint m_fieldIndex;
		uint m_paramIndex;
		uint m_eventIndex;
		uint m_propertyIndex;

		MemoryBinaryWriter m_constWriter;

		public StructureWriter StructureWriter {
			get { return m_structureWriter; }
			set {
				m_structureWriter = value;
				m_mdWriter = new MetadataWriter (
					m_mod.Assembly,
					m_mod.Image.MetadataRoot,
					m_structureWriter.Assembly.Kind,
					m_mod.Assembly.Runtime,
					m_structureWriter.GetWriter ());
				m_tableWriter = m_mdWriter.GetTableVisitor ();
				m_rowWriter = m_tableWriter.GetRowVisitor () as MetadataRowWriter;
				m_sigWriter = new SignatureWriter (m_mdWriter);
				m_codeWriter = new CodeWriter (this, m_mdWriter.CilWriter);
			}
		}

		public CodeWriter CodeWriter {
			get { return m_codeWriter; }
		}

		public SignatureWriter SignatureWriter {
			get { return m_sigWriter; }
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

			m_methodStack = new ArrayList ();
			m_fieldStack = new ArrayList ();

			m_methodIndex = 1;
			m_fieldIndex = 1;
			m_paramIndex = 1;
			m_eventIndex = 1;
			m_propertyIndex = 1;

			m_constWriter = new MemoryBinaryWriter ();
		}

		public ITypeReference GetCoreType (string name)
		{
			return m_mod.Controller.Reader.SearchCoreType (name);
		}

		public uint GetRidFor (IMetadataTokenProvider tp)
		{
			return tp.MetadataToken.RID;
		}

		public uint GetRidFor (AssemblyNameReference asmName)
		{
			return (uint) m_mod.AssemblyReferences.IndexOf (asmName) + 1;
		}

		public uint GetRidFor (ModuleDefinition mod)
		{
			return (uint) m_mod.Assembly.Modules.IndexOf (mod) + 1;
		}

		public uint GetRidFor (ModuleReference modRef)
		{
			return (uint) m_mod.ModuleReferences.IndexOf (modRef) + 1;
		}

		bool IsTypeSpec (ITypeReference type)
		{
			return type is IArrayType || type is IFunctionPointerType ||
				type is IPointerType || type is IGenericInstance;
		}

		public MetadataToken GetTypeDefOrRefToken (ITypeReference type)
		{
			if (IsTypeSpec (type)) {
				TypeSpecTable tsTable = m_tableWriter.GetTypeSpecTable ();
				TypeSpecRow tsRow = m_rowWriter.CreateTypeSpecRow (
					m_sigWriter.AddTypeSpec (GetTypeSpecSig (type)));
				tsTable.Rows.Add (tsRow);
				type.MetadataToken = new MetadataToken (TokenType.TypeSpec, (uint) tsTable.Rows.Count);
				return type.MetadataToken;
			} else if (type != null)
				return type.MetadataToken;
			else // <Module> and interfaces
				return new MetadataToken (TokenType.TypeRef, 0);
		}

		public MetadataToken GetMethodSpecToken (GenericInstanceMethod gim)
		{
			MethodSpecTable msTable = m_tableWriter.GetMethodSpecTable ();
			MethodSpecRow msRow = m_rowWriter.CreateMethodSpecRow (
				gim.ElementMethod.MetadataToken,
				m_sigWriter.AddMethodSpec (GetMethodSpecSig (gim)));
			msTable.Rows.Add (msRow);
			gim.MetadataToken = new MetadataToken (TokenType.MethodSpec, (uint) msTable.Rows.Count);
			return gim.MetadataToken;
		}

		public override void VisitModuleDefinition (ModuleDefinition mod)
		{
			// ensure that everything is loaded before writing
			foreach (TypeDefinition type in mod.Types) {
				foreach (MethodDefinition meth in type.Methods)
					meth.LoadBody ();
				foreach (MethodDefinition ctor in type.Constructors)
					ctor.LoadBody ();
			}
		}

		public override void VisitTypeDefinitionCollection (TypeDefinitionCollection types)
		{
			ArrayList orderedTypes = new ArrayList (types.Count);
			TypeDefTable tdTable = m_tableWriter.GetTypeDefTable ();

			if (types [Constants.ModuleType] == null)
				types.Add (new TypeDefinition (
						Constants.ModuleType, string.Empty, (TypeAttributes) 0));

			foreach (ITypeDefinition t in types)
				orderedTypes.Add (t);

			orderedTypes.Sort (TableComparers.TypeDef.Instance);

			for (int i = 0; i < orderedTypes.Count; i++) {
				TypeDefinition t = (TypeDefinition) orderedTypes [i];
				if (t.Module.Assembly != m_mod.Assembly)
					throw new ReflectionException ("A type as not been correctly imported");

				t.MetadataToken = new MetadataToken (TokenType.TypeDef, (uint) (i + 1));
			}

			foreach (TypeDefinition t in orderedTypes) {
				TypeDefRow tdRow = m_rowWriter.CreateTypeDefRow (
					t.Attributes,
					m_mdWriter.AddString (t.Name),
					m_mdWriter.AddString (t.Namespace),
					GetTypeDefOrRefToken (t.BaseType),
					0,
					0);

				tdTable.Rows.Add (tdRow);

				if (t.LayoutInfo.HasLayoutInfo)
					WriteLayout (t);
			}

			for (int i = 0; i < orderedTypes.Count; i++) {
				TypeDefRow tdRow = tdTable [i];
				TypeDefinition t = (TypeDefinition) orderedTypes [i];
				tdRow.FieldList = m_fieldIndex;
				tdRow.MethodList = m_methodIndex;
				foreach (FieldDefinition field in t.Fields)
					VisitFieldDefinition (field);
				foreach (MethodDefinition ctor in t.Constructors)
					VisitMethodDefinition (ctor);
				foreach (MethodDefinition meth in t.Methods)
					VisitMethodDefinition (meth);
			}

			foreach (FieldDefinition field in m_fieldStack)
				VisitCustomAttributeCollection (field.CustomAttributes);

			foreach (MethodDefinition meth in m_methodStack) {
				VisitOverrideCollection (meth.Overrides);
				VisitCustomAttributeCollection (meth.CustomAttributes);
				VisitSecurityDeclarationCollection (meth.SecurityDeclarations);
				if (meth.PInvokeInfo != null)
					VisitPInvokeInfo (meth.PInvokeInfo);
			}

			foreach (TypeDefinition t in orderedTypes)
				t.Accept (this);
		}

		public override void VisitTypeReferenceCollection (TypeReferenceCollection refs)
		{
			ArrayList orderedTypeRefs = new ArrayList (refs.Count);
			foreach (TypeReference tr in refs)
				orderedTypeRefs.Add (tr);

			orderedTypeRefs.Sort (TableComparers.TypeRef.Instance);

			TypeRefTable trTable = m_tableWriter.GetTypeRefTable ();
			foreach (TypeReference t in orderedTypeRefs) {
				MetadataToken scope;

				if (t.Module.Assembly != m_mod.Assembly)
					throw new ReflectionException ("A type as not been correctly imported");

				if (t.Scope == null)
					continue;

				if (t.DeclaringType != null)
					scope = new MetadataToken (TokenType.TypeRef, GetRidFor (t.DeclaringType));
				if (t.Scope is AssemblyNameReference)
					scope = new MetadataToken (TokenType.AssemblyRef,
						GetRidFor ((AssemblyNameReference) t.Scope));
				else if (t.Scope is ModuleDefinition)
					scope = new MetadataToken (TokenType.Module,
						GetRidFor ((ModuleDefinition) t.Scope));
				else if (t.Scope is ModuleReference)
					scope = new MetadataToken (TokenType.ModuleRef,
						GetRidFor ((ModuleReference) t.Scope));
				else
					scope = new MetadataToken (TokenType.ExportedType, 0);

				TypeRefRow trRow = m_rowWriter.CreateTypeRefRow (
					scope,
					m_mdWriter.AddString (t.Name),
					m_mdWriter.AddString (t.Namespace));

				trTable.Rows.Add (trRow);
				t.MetadataToken = new MetadataToken (TokenType.TypeRef, (uint) trTable.Rows.Count);
			}
		}

		public override void VisitMemberReferenceCollection (MemberReferenceCollection members)
		{
			if (members.Count == 0)
				return;

			MemberRefTable mrTable = m_tableWriter.GetMemberRefTable ();
			foreach (MemberReference member in members) {
				uint sig = 0;
				if (member is IFieldReference)
					sig = m_sigWriter.AddFieldSig (GetFieldSig (member as IFieldReference));
				else if (member is IMethodReference)
					sig = m_sigWriter.AddMethodRefSig (GetMethodRefSig (member as IMethodReference));
				MemberRefRow mrRow = m_rowWriter.CreateMemberRefRow (
					GetTypeDefOrRefToken (member.DeclaringType),
					m_mdWriter.AddString (member.Name),
					sig);

				mrTable.Rows.Add (mrRow);
				member.MetadataToken = new MetadataToken (
					TokenType.MemberRef, (uint) mrTable.Rows.Count);
				}
		}

		public override void VisitGenericParameterCollection (GenericParameterCollection parameters)
		{
			if (parameters.Count == 0)
				return;

			GenericParamTable gpTable = m_tableWriter.GetGenericParamTable ();
			GenericParamConstraintTable gpcTable = m_tableWriter.GetGenericParamConstraintTable ();

			for (int i = 0; i < parameters.Count; i++) {

				GenericParameter gp = parameters [i];
				GenericParamRow gpRow = m_rowWriter.CreateGenericParamRow (
					(ushort) i,
					gp.Attributes,
					gp.Owner.MetadataToken,
					m_mdWriter.AddString (gp.Name));

				gpTable.Rows.Add (gpRow);

				if (gp.Constraints.Count == 0)
					continue;

				foreach (TypeReference constraint in gp.Constraints) {
					GenericParamConstraintRow gpcRow = m_rowWriter.CreateGenericParamConstraintRow (
						(uint) gpTable.Rows.Count,
						GetTypeDefOrRefToken (constraint));

					gpcTable.Rows.Add (gpcRow);
				}
			}
		}

		public override void VisitInterfaceCollection (InterfaceCollection interfaces)
		{
			if (interfaces.Count == 0)
				return;

			InterfaceImplTable iiTable = m_tableWriter.GetInterfaceImplTable ();
			foreach (TypeReference interf in interfaces) {
				InterfaceImplRow iiRow = m_rowWriter.CreateInterfaceImplRow (
					GetRidFor (interfaces.Container),
					GetTypeDefOrRefToken (interf));

				iiTable.Rows.Add (iiRow);
			}
		}

		public override void VisitExternTypeCollection (ExternTypeCollection externs)
		{
			VisitCollection (externs);
		}

		public override void VisitExternType (TypeReference externType)
		{
			// TODO
		}

		public override void VisitOverrideCollection (OverrideCollection meths)
		{
			if (meths.Count == 0)
				return;

			MethodImplTable miTable = m_tableWriter.GetMethodImplTable ();
			foreach (MethodReference ov in meths) {
				MethodImplRow miRow = m_rowWriter.CreateMethodImplRow (
					GetRidFor (meths.Container.DeclaringType as ITypeDefinition),
					new MetadataToken (TokenType.Method, GetRidFor (meths.Container)),
					ov.MetadataToken);

				miTable.Rows.Add (miRow);
			}
		}

		public override void VisitNestedTypeCollection (NestedTypeCollection nestedTypes)
		{
			if (nestedTypes.Count == 0)
				return;

			NestedClassTable ncTable = m_tableWriter.GetNestedClassTable ();
			foreach (TypeDefinition nested in nestedTypes) {
				NestedClassRow ncRow = m_rowWriter.CreateNestedClassRow (
					nested.MetadataToken.RID,
					GetRidFor (nestedTypes.Container));

				ncTable.Rows.Add (ncRow);
			}
		}

		public override void VisitParameterDefinitionCollection (ParameterDefinitionCollection parameters)
		{
			if (parameters.Count == 0)
				return;

			ushort seq = 1;
			ParamTable pTable = m_tableWriter.GetParamTable ();
			foreach (ParameterDefinition param in parameters) {
				ParamRow pRow = m_rowWriter.CreateParamRow (
					param.Attributes,
					seq++,
					m_mdWriter.AddString (param.Name));

				pTable.Rows.Add (pRow);
				param.MetadataToken = new MetadataToken (TokenType.Param, (uint) pTable.Rows.Count);
				m_paramIndex++;
			}
		}

		public override void VisitMethodDefinition (MethodDefinition method)
		{
			MethodTable mTable = m_tableWriter.GetMethodTable ();
			MethodRow mRow = m_rowWriter.CreateMethodRow (
				RVA.Zero,
				method.ImplAttributes,
				method.Attributes,
				m_mdWriter.AddString (method.Name),
				m_sigWriter.AddMethodDefSig (GetMethodDefSig (method)),
				m_paramIndex);

			mTable.Rows.Add (mRow);
			m_methodStack.Add (method);
			method.MetadataToken = new MetadataToken (TokenType.Method, (uint) mTable.Rows.Count);
			m_methodIndex++;

			if (method.ReturnType.CustomAttributes.Count > 0 || method.ReturnType.MarshalSpec != null) {
				ParameterDefinition param = (method.ReturnType as MethodReturnType).Parameter;
				ParamTable pTable = m_tableWriter.GetParamTable ();
				ParamRow pRow = m_rowWriter.CreateParamRow (
					param.Attributes,
					0,
					0);

				pTable.Rows.Add (pRow);
				param.MetadataToken = new MetadataToken (TokenType.Param, (uint) pTable.Rows.Count);
			}

			VisitParameterDefinitionCollection (method.Parameters);
		}

		public override void VisitPInvokeInfo (PInvokeInfo pinvk)
		{
			ImplMapTable imTable = m_tableWriter.GetImplMapTable ();
			ImplMapRow imRow = m_rowWriter.CreateImplMapRow (
				pinvk.Attributes,
				new MetadataToken (TokenType.Method, GetRidFor (pinvk.Method)),
				m_mdWriter.AddString (pinvk.EntryPoint),
				GetRidFor (pinvk.Module));

			imTable.Rows.Add (imRow);
		}

		public override void VisitEventDefinitionCollection (EventDefinitionCollection events)
		{
			if (events.Count == 0)
				return;

			EventMapTable emTable = m_tableWriter.GetEventMapTable ();
			EventMapRow emRow = m_rowWriter.CreateEventMapRow (
				GetRidFor (events.Container),
				m_eventIndex);

			emTable.Rows.Add (emRow);
			VisitCollection (events);
		}

		public override void VisitEventDefinition (EventDefinition evt)
		{
			EventTable eTable = m_tableWriter.GetEventTable ();
			EventRow eRow = m_rowWriter.CreateEventRow (
				evt.Attributes,
				m_mdWriter.AddString (evt.Name),
				GetTypeDefOrRefToken (evt.EventType));

			eTable.Rows.Add (eRow);
			evt.MetadataToken = new MetadataToken (TokenType.Event, (uint) eTable.Rows.Count);

			if (evt.AddMethod != null)
				WriteSemantic (MethodSemanticsAttributes.AddOn, evt, evt.AddMethod);

			if (evt.InvokeMethod != null)
				WriteSemantic (MethodSemanticsAttributes.Fire, evt, evt.InvokeMethod);

			if (evt.RemoveMethod != null)
				WriteSemantic (MethodSemanticsAttributes.RemoveOn, evt, evt.RemoveMethod);

			m_eventIndex++;
		}

		public override void VisitFieldDefinition (FieldDefinition field)
		{
			FieldTable fTable = m_tableWriter.GetFieldTable ();
			FieldRow fRow = m_rowWriter.CreateFieldRow (
				field.Attributes,
				m_mdWriter.AddString (field.Name),
				m_sigWriter.AddFieldSig (GetFieldSig (field)));

			fTable.Rows.Add (fRow);
			field.MetadataToken = new MetadataToken (TokenType.Field, (uint) fTable.Rows.Count);
			m_fieldIndex++;

			if (field.HasConstant)
				WriteConstant (field, field.MetadataToken, field.FieldType);

			if (field.LayoutInfo.HasLayoutInfo)
				WriteLayout (field);

			m_fieldStack.Add (field);
		}

		public override void VisitPropertyDefinitionCollection (PropertyDefinitionCollection properties)
		{
			if (properties.Count == 0)
				return;

			PropertyMapTable pmTable = m_tableWriter.GetPropertyMapTable ();
			PropertyMapRow pmRow = m_rowWriter.CreatePropertyMapRow (
				GetRidFor (properties.Container),
				m_propertyIndex);

			pmTable.Rows.Add (pmRow);
			VisitCollection (properties);
		}

		public override void VisitPropertyDefinition (PropertyDefinition property)
		{
			PropertyTable pTable = m_tableWriter.GetPropertyTable ();
			PropertyRow pRow = m_rowWriter.CreatePropertyRow (
				property.Attributes,
				m_mdWriter.AddString (property.Name),
				m_sigWriter.AddPropertySig (GetPropertySig (property)));

			pTable.Rows.Add (pRow);
			property.MetadataToken = new MetadataToken (TokenType.Property, (uint) pTable.Rows.Count);

			if (property.GetMethod != null)
				WriteSemantic (MethodSemanticsAttributes.Getter, property, property.GetMethod);

			if (property.SetMethod != null)
				WriteSemantic (MethodSemanticsAttributes.Setter, property, property.SetMethod);

			m_propertyIndex++;
		}

		private MetadataToken GetMetadataToken (IHasSecurity container)
		{
			if (container is IAssemblyDefinition)
				return new MetadataToken (TokenType.Assembly, 1);

			IMetadataTokenProvider provider = (container as IMetadataTokenProvider);
			if (container == null)
				throw new ReflectionException ("Unknown Security Declaration parent");

			return provider.MetadataToken;
		}

		public override void VisitSecurityDeclarationCollection (SecurityDeclarationCollection secDecls)
		{
			if (secDecls.Count == 0)
				return;

			MetadataToken parent = GetMetadataToken (secDecls.Container);
			DeclSecurityTable dsTable = m_tableWriter.GetDeclSecurityTable ();
			foreach (SecurityDeclaration secDec in secDecls) {
				DeclSecurityRow dsRow = m_rowWriter.CreateDeclSecurityRow (
					secDec.Action,
					parent,
					m_mdWriter.AddBlob (
						m_mod.GetAsByteArray (secDec)));

				dsTable.Rows.Add (dsRow);
			}
		}

		public override void VisitCustomAttributeCollection (CustomAttributeCollection customAttrs)
		{
			if (customAttrs.Count == 0)
				return;

			CustomAttributeTable caTable = m_tableWriter.GetCustomAttributeTable ();
			foreach (CustomAttribute ca in customAttrs) {
				MetadataToken parent;
				if (customAttrs.Container is IAssemblyDefinition)
					parent = new MetadataToken (TokenType.Assembly, 1);
				else if (customAttrs.Container is IModuleDefinition)
					parent = new MetadataToken (TokenType.Module, 1);
				else if (customAttrs.Container is IMetadataTokenProvider)
					parent = (customAttrs.Container as IMetadataTokenProvider).MetadataToken;
				else
					throw new ReflectionException ("Unknown Custom Attribute parent");

				uint value = ca.IsReadable ?
					m_sigWriter.AddCustomAttribute (GetCustomAttributeSig (ca), ca.Constructor) :
					m_mdWriter.AddBlob (m_mod.GetAsByteArray (ca));
				CustomAttributeRow caRow = m_rowWriter.CreateCustomAttributeRow (
					parent,
					ca.Constructor.MetadataToken,
					value);

				caTable.Rows.Add (caRow);
			}
		}

		public override void VisitMarshalSpec (MarshalDesc marshalSpec)
		{
			FieldMarshalTable fmTable = m_tableWriter.GetFieldMarshalTable ();
			FieldMarshalRow fmRow = m_rowWriter.CreateFieldMarshalRow (
				((IMetadataTokenProvider) marshalSpec.Container).MetadataToken,
				m_sigWriter.AddMarshalSig (GetMarshalSig (marshalSpec)));

			fmTable.Rows.Add (fmRow);
		}

		void WriteConstant (IHasConstant hc, MetadataToken parent, TypeReference type)
		{
			ConstantTable cTable = m_tableWriter.GetConstantTable ();
			ElementType et;
			if (type is TypeDefinition && (type as TypeDefinition).IsEnum) {
				Type t = hc.Constant.GetType ();
				if (t.IsEnum)
					t = Enum.GetUnderlyingType (t);

				et = GetCorrespondingType (string.Concat (t.Namespace, '.', t.Name));
			} else
				et = GetCorrespondingType (type.FullName);

			ConstantRow cRow = m_rowWriter.CreateConstantRow (
				et,
				parent,
				m_mdWriter.AddBlob (EncodeConstant (et, hc.Constant)));

			cTable.Rows.Add (cRow);
		}

		void WriteLayout (FieldDefinition field)
		{
			FieldLayoutTable flTable = m_tableWriter.GetFieldLayoutTable ();
			FieldLayoutRow flRow = m_rowWriter.CreateFieldLayoutRow (
				field.LayoutInfo.Offset,
				GetRidFor (field));

			flTable.Rows.Add (flRow);
		}

		void WriteLayout (TypeDefinition type)
		{
			ClassLayoutTable clTable = m_tableWriter.GetClassLayoutTable ();
			ClassLayoutRow clRow = m_rowWriter.CreateClassLayoutRow (
				type.LayoutInfo.PackingSize,
				type.LayoutInfo.ClassSize,
				GetRidFor (type));

			clTable.Rows.Add (clRow);
		}

		void WriteSemantic (MethodSemanticsAttributes attrs,
			IMemberDefinition member, MethodDefinition meth)
		{
			MethodSemanticsTable msTable = m_tableWriter.GetMethodSemanticsTable ();
			MethodSemanticsRow msRow = m_rowWriter.CreateMethodSemanticsRow (
				attrs,
				GetRidFor (meth),
				member.MetadataToken);

			msTable.Rows.Add (msRow);
		}

		void SortTables ()
		{
			TablesHeap th = m_mdWriter.GetMetadataRoot ().Streams.TablesHeap;

			if (th.HasTable (typeof (NestedClassTable)))
				m_tableWriter.GetNestedClassTable ().Rows.Sort (
					TableComparers.NestedClass.Instance);

			if (th.HasTable (typeof (InterfaceImplTable)))
				m_tableWriter.GetInterfaceImplTable ().Rows.Sort (
					TableComparers.InterfaceImpl.Instance);

			if (th.HasTable (typeof (ConstantTable)))
				m_tableWriter.GetConstantTable ().Rows.Sort (
					TableComparers.Constant.Instance);

			if (th.HasTable (typeof (MethodSemanticsTable)))
				m_tableWriter.GetMethodSemanticsTable ().Rows.Sort (
					TableComparers.MethodSem.Instance);

			if (th.HasTable (typeof (FieldMarshalTable)))
				m_tableWriter.GetFieldMarshalTable ().Rows.Sort (
					TableComparers.FieldMarshal.Instance);

			if (th.HasTable (typeof (ClassLayoutTable)))
				m_tableWriter.GetClassLayoutTable ().Rows.Sort (
					TableComparers.TypeLayout.Instance);

			if (th.HasTable (typeof (FieldLayoutTable)))
				m_tableWriter.GetFieldLayoutTable ().Rows.Sort (
					TableComparers.FieldLayout.Instance);

			if (th.HasTable (typeof (ImplMapTable)))
				m_tableWriter.GetImplMapTable ().Rows.Sort (
					TableComparers.PInvoke.Instance);

			if (th.HasTable (typeof (FieldRVATable)))
				m_tableWriter.GetFieldRVATable ().Rows.Sort (
					TableComparers.FieldRVA.Instance);

			if (th.HasTable (typeof (MethodImplTable)))
				m_tableWriter.GetMethodImplTable ().Rows.Sort (
					TableComparers.Override.Instance);

			if (th.HasTable (typeof (CustomAttributeTable)))
				m_tableWriter.GetCustomAttributeTable ().Rows.Sort (
					TableComparers.CustomAttribute.Instance);

			if (th.HasTable (typeof (DeclSecurityTable)))
				m_tableWriter.GetDeclSecurityTable ().Rows.Sort (
					TableComparers.SecurityDeclaration.Instance);
		}

		public override void TerminateModuleDefinition (ModuleDefinition module)
		{
			VisitCustomAttributeCollection (module.Assembly.CustomAttributes);
			VisitSecurityDeclarationCollection (module.Assembly.SecurityDeclarations);
			VisitCustomAttributeCollection (module.CustomAttributes);

			SortTables ();

			MethodTable mTable = m_tableWriter.GetMethodTable ();
			for (int i = 0; i < m_methodStack.Count; i++) {
				MethodDefinition meth = (MethodDefinition) m_methodStack [i];
				if (!meth.IsAbstract && meth.PInvokeInfo == null)
					mTable [i].RVA = m_codeWriter.WriteMethodBody (meth);
			}

			if (m_fieldStack.Count > 0) {
				FieldRVATable frTable = null;
				foreach (IFieldDefinition field in m_fieldStack) {
					if (field.InitialValue != null && field.InitialValue.Length > 0) {
						if (frTable == null)
							frTable = m_tableWriter.GetFieldRVATable ();

						FieldRVARow frRow = m_rowWriter.CreateFieldRVARow (
							m_mdWriter.GetDataCursor (),
							field.MetadataToken.RID);

						m_mdWriter.AddData (field.InitialValue.Length + 3 & (~3));
						m_mdWriter.AddFieldInitData (field.InitialValue);

						frTable.Rows.Add (frRow);
					}
				}
			}

			if (m_mod.Assembly.EntryPoint != null)
				m_mdWriter.EntryPointToken =
					((uint) TokenType.Method) | GetRidFor (m_mod.Assembly.EntryPoint);

			m_mod.Image.MetadataRoot.Accept (m_mdWriter);
		}

		public ElementType GetCorrespondingType (string fullName)
		{
			switch (fullName) {
			case Constants.Boolean :
				return ElementType.Boolean;
			case Constants.Char :
				return ElementType.Char;
			case Constants.SByte :
				return ElementType.I1;
			case Constants.Int16 :
				return ElementType.I2;
			case Constants.Int32 :
				return ElementType.I4;
			case Constants.Int64 :
				return ElementType.I8;
			case Constants.Byte :
				return ElementType.U1;
			case Constants.UInt16 :
				return ElementType.U2;
			case Constants.UInt32 :
				return ElementType.U4;
			case Constants.UInt64 :
				return ElementType.U8;
			case Constants.Single :
				return ElementType.R4;
			case Constants.Double :
				return ElementType.R8;
			case Constants.String :
				return ElementType.String;
			case Constants.Type :
				return ElementType.Type;
			default:
				return ElementType.Class;
			}
		}

		byte [] EncodeConstant (ElementType et, object value)
		{
			m_constWriter.Empty ();

			switch (et) {
			case ElementType.Boolean :
				m_constWriter.Write ((byte) (((bool) value) ? 1 : 0));
				break;
			case ElementType.Char :
				m_constWriter.Write ((ushort) (char) value);
				break;
			case ElementType.I1 :
				m_constWriter.Write ((sbyte) value);
				break;
			case ElementType.I2 :
				m_constWriter.Write ((short) value);
				break;
			case ElementType.I4 :
				m_constWriter.Write ((int) value);
				break;
			case ElementType.I8 :
				m_constWriter.Write ((long) value);
				break;
			case ElementType.U1 :
				m_constWriter.Write ((byte) value);
				break;
			case ElementType.U2 :
				m_constWriter.Write ((ushort) value);
				break;
			case ElementType.U4 :
				m_constWriter.Write ((uint) value);
				break;
			case ElementType.U8 :
				m_constWriter.Write ((ulong) value);
				break;
			case ElementType.R4 :
				m_constWriter.Write ((float) value);
				break;
			case ElementType.R8 :
				m_constWriter.Write ((double) value);
				break;
			case ElementType.String :
				m_constWriter.Write (Encoding.Unicode.GetBytes ((string) value));
				break;
			case ElementType.Class :
				m_constWriter.Write (new byte [4]);
				break;
			default :
				throw new ReflectionException ("Non valid element for a constant");
			}

			return m_constWriter.ToArray ();
		}

		public SigType GetSigType (ITypeReference type)
		{
			string name = type.FullName;

			switch (name) {
			case Constants.Void :
				return new SigType (ElementType.Void);
			case Constants.Object :
				return new SigType (ElementType.Object);
			case Constants.Boolean :
				return new SigType (ElementType.Boolean);
			case Constants.String :
				return new SigType (ElementType.String);
			case Constants.Char :
				return new SigType (ElementType.Char);
			case Constants.SByte :
				return new SigType (ElementType.I1);
			case Constants.Byte :
				return new SigType (ElementType.U1);
			case Constants.Int16 :
				return new SigType (ElementType.I2);
			case Constants.UInt16 :
				return new SigType (ElementType.U2);
			case Constants.Int32 :
				return new SigType (ElementType.I4);
			case Constants.UInt32 :
				return new SigType (ElementType.U4);
			case Constants.Int64 :
				return new SigType (ElementType.I8);
			case Constants.UInt64 :
				return new SigType (ElementType.U8);
			case Constants.Single :
				return new SigType (ElementType.R4);
			case Constants.Double :
				return new SigType (ElementType.R8);
			case Constants.IntPtr :
				return new SigType (ElementType.I);
			case Constants.UIntPtr :
				return new SigType (ElementType.U);
			case Constants.TypedReference :
				return new SigType (ElementType.TypedByRef);
			}

			if (type is IGenericParameter) {
				GenericParameter gp = type as GenericParameter;
				int pos = gp.Owner.GenericParameters.IndexOf (gp);
				if (gp.Owner is TypeDefinition)
					return new VAR (pos);
				else if (gp.Owner is MethodDefinition)
					return new MVAR (pos);
				else
					throw new ReflectionException ("Unkown generic parameter type");
			} else if (type is GenericInstanceType) {
				GenericInstanceType git = type as GenericInstanceType;
				GENERICINST gi = new GENERICINST ();
				gi.ValueType = git.IsValueType;
				gi.Type = GetTypeDefOrRefToken (git.ElementType);
				gi.Signature = new GenericInstSignature ();
				gi.Signature.Arity = git.GenericArguments.Count;
				gi.Signature.Types = new SigType [gi.Signature.Arity];
				for (int i = 0; i < git.GenericArguments.Count; i++)
					gi.Signature.Types [i] = GetSigType (git.GenericArguments [i]);

				return gi;
			} else if (type is IArrayType) {
				IArrayType aryType = type as IArrayType;
				if (aryType.IsSizedArray) {
					SZARRAY szary = new SZARRAY ();
					szary.Type = GetSigType (aryType.ElementType);
					return szary;
				}

				throw new NotImplementedException ("Complex arrays are not implemented"); // TODO
			} else if (type is IPointerType) {
				PTR p = new PTR ();
				ITypeReference elementType = (type as IPointerType).ElementType;
				p.Void = elementType.FullName == Constants.Void;
				if (!p.Void) {
					p.CustomMods = GetCustomMods (elementType);
					p.PtrType = GetSigType (elementType);
				}
				return p;
			} else if (type is IFunctionPointerType) {
				throw new NotImplementedException ("Function pointer are not implemented"); // TODO
			} else if (type.IsValueType) {
				VALUETYPE vt = new VALUETYPE ();
				vt.Type = GetTypeDefOrRefToken (type);
				return vt;
			} else {
				CLASS c = new CLASS ();
				c.Type = GetTypeDefOrRefToken (type);
				return c;
			}
		}

		public CustomMod [] GetCustomMods (ITypeReference type)
		{
			if (!(type is IModifierType))
				return new CustomMod [0];

			ArrayList cmods = new ArrayList ();
			ITypeReference cur = type;
			while (cur is IModifierType) {
				if (cur is IModifierOptional) {
					CustomMod cmod = new CustomMod ();
					cmod.CMOD = CustomMod.CMODType.OPT;
					cmod.TypeDefOrRef = GetTypeDefOrRefToken ((cur as IModifierOptional).ModifierType);
					cmods.Add (cmod);
				} else if (cur is IModifierRequired) {
					CustomMod cmod = new CustomMod ();
					cmod.CMOD = CustomMod.CMODType.REQD;
					cmod.TypeDefOrRef = GetTypeDefOrRefToken ((cur as IModifierRequired).ModifierType);
					cmods.Add (cmod);
				}

				cur = (cur as IModifierType).ElementType;
			}

			return cmods.ToArray (typeof (CustomMod)) as CustomMod [];
		}

		public Signature GetMemberRefSig (IMemberReference member)
		{
			if (member is IFieldReference)
				return GetFieldSig (member as IFieldReference);
			else
				return GetMemberRefSig (member as IMethodReference);
		}

		public FieldSig GetFieldSig (IFieldReference field)
		{
			FieldSig sig = new FieldSig ();
			sig.CallingConvention |= 0x6;
			sig.Field = true;
			sig.CustomMods = GetCustomMods (field.FieldType);
			sig.Type = GetSigType (field.FieldType);
			return sig;
		}

		Param [] GetParametersSig (IParameterDefinitionCollection parameters)
		{
			Param [] ret = new Param [parameters.Count];
			for (int i = 0; i < ret.Length; i++) {
				IParameterDefinition pDef =parameters [i];
				Param p = new Param ();
				p.CustomMods = GetCustomMods (pDef.ParameterType);
				if (pDef.ParameterType.FullName == Constants.TypedReference)
					p.TypedByRef = true;
				else if (pDef.ParameterType is IReferenceType) {
					p.ByRef = true;
					p.Type = GetSigType (
						(pDef.ParameterType as IReferenceType).ElementType);
				} else
					p.Type = GetSigType (pDef.ParameterType);
				ret [i] = p;
			}
			return ret;
		}

		void CompleteMethodSig (IMethodReference meth, MethodSig sig)
		{
			sig.HasThis = meth.HasThis;
			sig.ExplicitThis = meth.ExplicitThis;
			if (sig.HasThis)
				sig.CallingConvention |= 0x20;
			if (sig.ExplicitThis)
				sig.CallingConvention |= 0x40;

			if ((meth.CallingConvention & MethodCallingConvention.VarArg) != 0)
				sig.CallingConvention |= 0x5;

			sig.ParamCount = meth.Parameters.Count;
			sig.Parameters = GetParametersSig (meth.Parameters);

			RetType rtSig = new RetType ();
			rtSig.CustomMods = GetCustomMods (meth.ReturnType.ReturnType);

			if (meth.ReturnType.ReturnType.FullName == Constants.Void)
				rtSig.Void = true;
			else if (meth.ReturnType.ReturnType.FullName == Constants.TypedReference)
				rtSig.TypedByRef = true;
			else if (meth.ReturnType.ReturnType is IReferenceType) {
				rtSig.ByRef = true;
				rtSig.Type = GetSigType (
					(meth.ReturnType.ReturnType as IReferenceType).ElementType);
			} else
				rtSig.Type = GetSigType (meth.ReturnType.ReturnType);

			sig.RetType = rtSig;
		}

		public MethodRefSig GetMethodRefSig (IMethodReference meth)
		{
			MethodRefSig methSig = new MethodRefSig ();
			CompleteMethodSig (meth, methSig);

			if ((meth.CallingConvention & MethodCallingConvention.C) != 0)
				methSig.CallingConvention |= 0x1;
			else if ((meth.CallingConvention & MethodCallingConvention.StdCall) != 0)
				methSig.CallingConvention |= 0x2;
			else if ((meth.CallingConvention & MethodCallingConvention.ThisCall) != 0)
				methSig.CallingConvention |= 0x3;
			else if ((meth.CallingConvention & MethodCallingConvention.FastCall) != 0)
				methSig.CallingConvention |= 0x4;

			return methSig;
		}

		public MethodDefSig GetMethodDefSig (IMethodDefinition meth)
		{
			MethodDefSig sig = new MethodDefSig ();
			if (meth.GenericParameters.Count > 0) {
				sig.CallingConvention |= 0x10;
				sig.GenericParameterCount = meth.GenericParameters.Count;
			}

			CompleteMethodSig (meth, sig);
			return sig;
		}

		public PropertySig GetPropertySig (IPropertyDefinition prop)
		{
			PropertySig ps = new PropertySig ();
			ps.CallingConvention |= 0x8;

			bool hasThis;
			bool explicitThis;
			MethodCallingConvention mcc;
			IParameterDefinitionCollection parameters = prop.Parameters;

			IMethodDefinition meth;
			if (prop.GetMethod != null)
				meth = prop.GetMethod;
			else if (prop.SetMethod != null)
				meth = prop.SetMethod;
			else
				meth = null;

			if (meth != null) {
				hasThis = meth.HasThis;
				explicitThis = meth.ExplicitThis;
				mcc = meth.CallingConvention;
			} else {
				hasThis = explicitThis = false;
				mcc = MethodCallingConvention.Default;
			}

			if (hasThis)
				ps.CallingConvention |= 0x20;
			if (explicitThis)
				ps.CallingConvention |= 0x40;

			if ((mcc & MethodCallingConvention.VarArg) != 0)
				ps.CallingConvention |= 0x5;

			int paramCount = parameters != null ? parameters.Count : 0;

			ps.ParamCount = paramCount;
			ps.Parameters = GetParametersSig (parameters);

			ps.Type = GetSigType (prop.PropertyType);

			return ps;
		}

		public TypeSpec GetTypeSpecSig (ITypeReference type)
		{
			TypeSpec ts = new TypeSpec (GetSigType (type));
			return ts;
		}

		public MethodSpec GetMethodSpecSig (GenericInstanceMethod gim)
		{
			GenericInstSignature gis = new GenericInstSignature ();
			gis.Arity = gim.Arity;
			gis.Types = new SigType [gis.Arity];
			for (int i = 0; i < gis.Arity; i++)
				gis.Types [i] = GetSigType (gim.GenericArguments [i]);

			return new MethodSpec (gis);
		}

		public CustomAttrib GetCustomAttributeSig (ICustomAttribute ca)
		{
			CustomAttrib cas = new CustomAttrib (ca.Constructor);
			cas.Prolog = CustomAttrib.StdProlog;

			cas.FixedArgs = new CustomAttrib.FixedArg [0];

			cas.FixedArgs = new CustomAttrib.FixedArg [ca.Constructor.Parameters.Count];

			for (int i = 0; i < cas.FixedArgs.Length; i++) {
				object o = ca.ConstructorParameters [i];
				CustomAttrib.FixedArg fa = new CustomAttrib.FixedArg ();
//				if (o is object []) {
//					object [] values = o as object [];
//					fa.Elems = new CustomAttrib.Elem [values.Length];
//					for (int j = 0; j < values.Length; j++) {
//						CustomAttrib.Elem elem = new CustomAttrib.Elem ();
//						elem.Value = values [j];
//						elem.FieldOrPropType = ElementType.Object;
//						elem.ElemType = ca.Constructor.Parameters [i].ParameterType;
//						fa.Elems [j] = elem;
//					}
//				} else {
					fa.Elems = new CustomAttrib.Elem [1];
					fa.Elems [0].Value = o;
					fa.Elems [0].ElemType = ca.Constructor.Parameters [i].ParameterType;
					fa.Elems [0].FieldOrPropType = GetCorrespondingType (fa.Elems [0].ElemType.FullName);
					if (fa.Elems [0].FieldOrPropType == ElementType.Class)
						fa.Elems [0].FieldOrPropType = ElementType.I4; // buggy
//				}

				cas.FixedArgs [i] = fa;
			}

			cas.NumNamed = 0;

			cas.NamedArgs = new CustomAttrib.NamedArg [0];

			return cas;
		}

		public MarshalSig GetMarshalSig (IMarshalSpec mSpec)
		{
			MarshalSig ms = new MarshalSig (mSpec.NativeIntrinsic);

			if (mSpec is ArrayMarshalDesc) {
				ArrayMarshalDesc amd = mSpec as ArrayMarshalDesc;
				MarshalSig.Array ar = new MarshalSig.Array ();
				ar.ArrayElemType = amd.ElemType;
				ar.NumElem = amd.NumElem;
				ar.ParamNum = amd.ParamNum;
				ar.ElemMult = amd.ElemMult;
				ms.Spec = ar;
			} else if (mSpec is CustomMarshalerDesc) {
				CustomMarshalerDesc cmd = mSpec as CustomMarshalerDesc;
				MarshalSig.CustomMarshaler cm = new MarshalSig.CustomMarshaler ();
				cm.Guid = cmd.Guid.ToString ();
				cm.UnmanagedType = cmd.UnmanagedType;
				cm.ManagedType = cmd.ManagedType.FullName;
				cm.Cookie = cmd.Cookie;
				ms.Spec = cm;
			} else if (mSpec is FixedArrayDesc) {
				FixedArrayDesc fad = mSpec as FixedArrayDesc;
				MarshalSig.FixedArray fa = new MarshalSig.FixedArray ();
				fa.ArrayElemType  = fad.ElemType;
				fa.NumElem = fad.NumElem;
				ms.Spec = fa;
			} else if (mSpec is FixedSysStringDesc) {
				MarshalSig.FixedSysString fss = new MarshalSig.FixedSysString ();
				fss.Size = (mSpec as FixedSysStringDesc).Size;
				ms.Spec = fss;
			} else if (mSpec is SafeArrayDesc) {
				MarshalSig.SafeArray sa = new MarshalSig.SafeArray ();
				sa.ArrayElemType = (mSpec as SafeArrayDesc).ElemType;
				ms.Spec = sa;
			}

			return ms;
		}
	}
}
