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
	using System.Collections;
	using System.IO;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class ReflectionWriter : BaseReflectionVisitor {

		private StructureWriter m_structureWriter;
		private ModuleDefinition m_mod;
		private SignatureWriter m_sigWriter;
		private CodeWriter m_codeWriter;
		private MetadataWriter m_mdWriter;
		private MetadataTableWriter m_tableWriter;
		private MetadataRowWriter m_rowWriter;

		private IList m_membersRefContainer;
		private IList m_methodStack;
		private IList m_fieldDataStack;

		private uint m_methodIndex;
		private uint m_fieldIndex;
		private uint m_paramIndex;
		private uint m_eventIndex;
		private uint m_propertyIndex;

		private MemoryBinaryWriter m_constWriter;

		public StructureWriter StructureWriter {
			get { return m_structureWriter; }
			set {
				m_structureWriter = value;
				m_mdWriter = new MetadataWriter (
					m_mod.Image.MetadataRoot,
					m_structureWriter.AssemblyKind,
					m_mod.Assembly.Runtime,
					m_structureWriter.GetWriter ());
				m_tableWriter = m_mdWriter.GetTableVisitor ();
				m_rowWriter = m_tableWriter.GetRowVisitor () as MetadataRowWriter;
				m_sigWriter = new SignatureWriter (m_mdWriter, this);
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

			m_membersRefContainer = new ArrayList ();
			m_methodStack = new ArrayList ();
			m_fieldDataStack = new ArrayList ();

			m_methodIndex = 1;
			m_fieldIndex = 1;
			m_paramIndex = 1;
			m_eventIndex = 1;
			m_propertyIndex = 1;

			m_constWriter = new MemoryBinaryWriter ();
		}

		public void AddMemberRef (IMemberReference member)
		{
			m_membersRefContainer.Add (member);
		}

		private void ImportMemberRefFromReader ()
		{
			ReflectionReader reader = m_mod.Controller.Reader;
			if (reader.MemberReferences != null && reader.MemberReferences.Length > 0)
				foreach (MemberReference member in m_mod.Controller.Reader.MemberReferences)
					AddMemberRef (member);
		}

		public ITypeReference GetCoreType (string name)
		{
			return m_mod.Controller.Reader.SearchCoreType (name);
		}

		public uint GetRidFor (IMetadataTokenProvider tp)
		{
			return tp.MetadataToken.RID;
		}

		public uint GetRidFor (IAssemblyNameReference asmName)
		{
			return (uint) m_mod.AssemblyReferences.IndexOf (asmName) + 1;
		}

		public uint GetRidFor (IModuleDefinition mod)
		{
			return (uint) m_mod.Assembly.Modules.IndexOf (mod) + 1;
		}

		public uint GetRidFor (IModuleReference modRef)
		{
			return (uint) m_mod.ModuleReferences.IndexOf (modRef) + 1;
		}

		public MetadataToken GetTypeDefOrRefToken (ITypeReference type)
		{
			if (type is IArrayType || type is IFunctionPointerType || type is IPointerType) {
				TypeSpecTable tsTable = m_tableWriter.GetTypeSpecTable ();
				TypeSpecRow tsRow = m_rowWriter.CreateTypeSpecRow (
					m_sigWriter.AddTypeSpec (GetTypeSpecSig (type)));
				tsTable.Rows.Add (tsRow);
				type.MetadataToken = new MetadataToken (TokenType.TypeSpec, (uint) tsTable.Rows.Count);
				return type.MetadataToken;
			} else if (type != null) {
				return type.MetadataToken;
			} else { // <Module> and interfaces
				return new MetadataToken (TokenType.TypeRef, 0);
			}
		}

		public override void VisitTypeDefinitionCollection (ITypeDefinitionCollection types)
		{
			ArrayList orderedTypes = new ArrayList (types.Count);
			TypeDefTable tdTable = m_tableWriter.GetTypeDefTable ();

			if (types [Constants.ModuleType] == null)
				m_mod.DefineType (
					Constants.ModuleType, string.Empty, (TypeAttributes) 0);

			foreach (ITypeDefinition t in types)
				orderedTypes.Add (t);

			orderedTypes.Sort (TableComparers.TypeDef.Instance);

			for (int i = 0; i < orderedTypes.Count; i++) {
				ITypeDefinition t = orderedTypes [i] as ITypeDefinition;
				t.MetadataToken = new MetadataToken (TokenType.TypeDef, (uint) (i + 1));
			}

			foreach (ITypeDefinition t in orderedTypes) {
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
				ITypeDefinition t = orderedTypes [i] as ITypeDefinition;
				tdRow.FieldList = m_fieldIndex;
				tdRow.MethodList = m_methodIndex;
				t.Accept (this);
			}
		}

		public override void VisitTypeReferenceCollection (ITypeReferenceCollection refs)
		{
			ImportMemberRefFromReader ();

			ArrayList orderedTypeRefs = new ArrayList (refs.Count);
			foreach (TypeReference tr in refs)
				orderedTypeRefs.Add (tr);

			orderedTypeRefs.Sort (TableComparers.TypeRef.Instance);

			TypeRefTable trTable = m_tableWriter.GetTypeRefTable ();
			foreach (TypeReference t in orderedTypeRefs) {
				MetadataToken scope;

				if (t.Scope == null)
					continue;

				if (t.DeclaringType != null)
					scope = new MetadataToken (TokenType.TypeRef, GetRidFor (t.DeclaringType));
				if (t.Scope is IAssemblyNameReference)
					scope = new MetadataToken (TokenType.AssemblyRef,
						GetRidFor (t.Scope as IAssemblyNameReference));
				else if (t.Scope is IModuleDefinition)
					scope = new MetadataToken (TokenType.Module,
						GetRidFor (t.Scope as IModuleDefinition));
				else if (t.Scope is IModuleReference)
					scope = new MetadataToken (TokenType.ModuleRef,
						GetRidFor (t.Scope as IModuleReference));
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

		public override void VisitInterfaceCollection (IInterfaceCollection interfaces)
		{
			InterfaceImplTable iiTable = m_tableWriter.GetInterfaceImplTable ();
			foreach (ITypeReference interf in interfaces) {
				InterfaceImplRow iiRow = m_rowWriter.CreateInterfaceImplRow (
					GetRidFor (interfaces.Container),
					GetTypeDefOrRefToken (interf));

				iiTable.Rows.Add (iiRow);
			}
		}

		public override void VisitExternTypeCollection (IExternTypeCollection externs)
		{
			VisitCollection (externs);
		}

		public override void VisitExternType (ITypeReference externType)
		{
			// TODO
		}

		public override void VisitOverrideCollection (IOverrideCollection meths)
		{
			VisitCollection (meths);

			MethodImplTable miTable = m_tableWriter.GetMethodImplTable ();
			foreach (IMethodReference ov in meths) {
				MethodImplRow miRow = m_rowWriter.CreateMethodImplRow (
					GetRidFor (meths.Container.DeclaringType as ITypeDefinition),
					new MetadataToken (TokenType.Method, GetRidFor (meths.Container)),
					ov.MetadataToken);

				miTable.Rows.Add (miRow);
			}
		}

		public override void VisitNestedTypeCollection (INestedTypeCollection nestedTypes)
		{
			NestedClassTable ncTable = m_tableWriter.GetNestedClassTable ();
			foreach (ITypeDefinition nested in nestedTypes) {
				NestedClassRow ncRow = m_rowWriter.CreateNestedClassRow (
					nested.MetadataToken.RID,
					GetRidFor (nestedTypes.Container));

				ncTable.Rows.Add (ncRow);
			}
		}

		public override void VisitParameterDefinitionCollection (IParameterDefinitionCollection parameters)
		{
			ushort seq = 1;
			ParamTable pTable = m_tableWriter.GetParamTable ();
			foreach (IParameterDefinition param in parameters) {
				ParamRow pRow = m_rowWriter.CreateParamRow (
					param.Attributes,
					seq++,
					m_mdWriter.AddString (param.Name));

				pTable.Rows.Add (pRow);
				param.MetadataToken = new MetadataToken (TokenType.Param, (uint) pTable.Rows.Count);
				m_paramIndex++;
			}
		}

		public override void VisitMethodDefinitionCollection (IMethodDefinitionCollection methods)
		{
			VisitCollection (methods);
		}

		public override void VisitMethodDefinition (IMethodDefinition method)
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
		}

		public override void VisitPInvokeInfo (IPInvokeInfo pinvk)
		{
			ImplMapTable imTable = m_tableWriter.GetImplMapTable ();
			ImplMapRow imRow = m_rowWriter.CreateImplMapRow (
				pinvk.Attributes,
				new MetadataToken (TokenType.Method, GetRidFor (pinvk.Method)),
				m_mdWriter.AddString (pinvk.EntryPoint),
				GetRidFor (pinvk.Module));

			imTable.Rows.Add (imRow);
		}

		public override void VisitEventDefinitionCollection (IEventDefinitionCollection events)
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

		public override void VisitEventDefinition (IEventDefinition evt)
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

		public override void VisitFieldDefinitionCollection (IFieldDefinitionCollection fields)
		{
			VisitCollection (fields);
		}

		public override void VisitFieldDefinition (IFieldDefinition field)
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

			if (field.InitialValue != null && field.InitialValue.Length > 0)
				m_fieldDataStack.Add (field);
		}

		public override void VisitPropertyDefinitionCollection (IPropertyDefinitionCollection properties)
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

		public override void VisitPropertyDefinition (IPropertyDefinition property)
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

		public override void VisitSecurityDeclarationCollection (ISecurityDeclarationCollection secDecls)
		{
			DeclSecurityTable dsTable = m_tableWriter.GetDeclSecurityTable ();
			foreach (ISecurityDeclaration secDec in secDecls) {
				MetadataToken parent;
				if (secDecls.Container is IAssemblyDefinition)
					parent = new MetadataToken (TokenType.Assembly, 1);
				else if (secDecls.Container is IMetadataTokenProvider)
					parent = (secDecls.Container as IMetadataTokenProvider).MetadataToken;
				else
					throw new ReflectionException ("Unknown Security Declaration parent");

				DeclSecurityRow dsRow = m_rowWriter.CreateDeclSecurityRow (
					secDec.Action,
					parent,
					m_mdWriter.AddBlob (secDec.GetAsByteArray ()));

				dsTable.Rows.Add (dsRow);
			}
		}

		public override void VisitCustomAttributeCollection (ICustomAttributeCollection customAttrs)
		{
			CustomAttributeTable caTable = m_tableWriter.GetCustomAttributeTable ();
			foreach (ICustomAttribute ca in customAttrs) {
				MetadataToken parent;
				if (customAttrs.Container is IAssemblyDefinition)
					parent = new MetadataToken (TokenType.Assembly, 1);
				else if (customAttrs.Container is IModuleDefinition)
					parent = new MetadataToken (TokenType.Module, 1);
				else if (customAttrs.Container is IMetadataTokenProvider)
					parent = (customAttrs.Container as IMetadataTokenProvider).MetadataToken;
				else
					throw new ReflectionException ("Unknown Custom Attribute parent");
				/*CustomAttributeRow caRow = m_rowWriter.CreateCustomAttributeRow (
					parent,
					ca.Constructor.MetadataToken,
					m_sigWriter.AddCustomAttribute (GetCustomAttributeSig (ca), ca.Constructor));

				caTable.Rows.Add (caRow);*/
			}
		}

		public override void VisitMarshalSpec (IMarshalSpec marshalSpec)
		{
			FieldMarshalTable fmTable = m_tableWriter.GetFieldMarshalTable ();
			FieldMarshalRow fmRow = m_rowWriter.CreateFieldMarshalRow (
				(marshalSpec as IMetadataTokenProvider).MetadataToken,
				m_sigWriter.AddMarshalSig (GetMarshalSig (marshalSpec)));

			fmTable.Rows.Add (fmRow);
		}

		private void WriteConstant (IHasConstant hc, MetadataToken parent, ITypeReference type)
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

		private void WriteLayout (IFieldDefinition field)
		{
			FieldLayoutTable flTable = m_tableWriter.GetFieldLayoutTable ();
			FieldLayoutRow flRow = m_rowWriter.CreateFieldLayoutRow (
				field.LayoutInfo.Offset,
				GetRidFor (field));

			flTable.Rows.Add (flRow);
		}

		private void WriteLayout (ITypeDefinition type)
		{
			ClassLayoutTable clTable = m_tableWriter.GetClassLayoutTable ();
			ClassLayoutRow clRow = m_rowWriter.CreateClassLayoutRow (
				type.LayoutInfo.PackingSize,
				type.LayoutInfo.ClassSize,
				GetRidFor (type));

			clTable.Rows.Add (clRow);
		}

		private void WriteSemantic (MethodSemanticsAttributes attrs,
			IMemberDefinition member, IMethodDefinition meth)
		{
			MethodSemanticsTable msTable = m_tableWriter.GetMethodSemanticsTable ();
			MethodSemanticsRow msRow = m_rowWriter.CreateMethodSemanticsRow (
				attrs,
				GetRidFor (meth),
				member.MetadataToken);

			msTable.Rows.Add (msRow);
		}

		private void SortTables ()
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

			// TODO continue;
		}

		public override void TerminateModuleDefinition (IModuleDefinition module)
		{
			if (m_membersRefContainer.Count > 0) {
				MemberRefTable mrTable = m_tableWriter.GetMemberRefTable ();
				foreach (IMemberReference member in m_membersRefContainer) {
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

			SortTables ();

			MethodTable mTable = m_tableWriter.GetMethodTable ();
			for (int i = 0; i < m_methodStack.Count; i++)
				mTable [i].RVA = m_codeWriter.WriteMethodBody (
					m_methodStack [i] as IMethodDefinition);

			if (m_fieldDataStack.Count > 0) {
				FieldRVATable frTable = m_tableWriter.GetFieldRVATable ();
				foreach (FieldDefinition field in m_fieldDataStack) {
					FieldRVARow frRow = m_rowWriter.CreateFieldRVARow (
						m_mdWriter.GetDataCursor (),
						field.MetadataToken.RID);

					m_mdWriter.AddData (field.InitialValue.Length + 3 & (~3));
					m_mdWriter.AddFieldInitData (field.InitialValue);

					frTable.Rows.Add (frRow);
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

		private byte [] EncodeConstant (ElementType et, object value)
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
				m_constWriter.Write (Encoding.UTF8.GetBytes ((string) value));
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

			if (type is IArrayType) {
				IArrayType aryType = type as IArrayType;
				if (aryType.IsSizedArray) {
					SZARRAY szary = new SZARRAY ();
					szary.Type = GetSigType (aryType.ElementType);
					return szary;
				}

				ARRAY ary = new ARRAY ();
				return ary; // TODO
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
				FNPTR fp = new FNPTR (); // TODO
				return fp;
			} else if (type is ITypeDefinition && (type as ITypeDefinition).IsValueType) {
				/*
				Potential bug here ?
				If the type is a type reference, we can't know
				if it is a ValueType or a class. So by default
				we use a class signature.
				 */
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

		private Param [] GetParametersSig (IParameterDefinitionCollection parameters)
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

		private void CompleteMethodSig (IMethodReference meth, MethodSig sig)
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
