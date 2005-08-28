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
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class AggressiveReflectionReader : ReflectionReader {

		public AggressiveReflectionReader (ModuleDefinition module) : base (module)
		{
		}

		public override void VisitTypeDefinitionCollection (ITypeDefinitionCollection types)
		{
			base.VisitTypeDefinitionCollection (types);
			ReadClassLayoutInfos ();
			ReadFieldLayoutInfos ();
			ReadPInvokeInfos ();
			ReadProperties ();
			ReadEvents ();
			ReadSemantics ();
			ReadInterfaces ();
			ReadOverrides ();
			ReadSecurityDeclarations ();
			ReadCustomAttributes ();
			ReadConstants ();
			ReadExternTypes ();
			ReadMarshalSpecs ();
			ReadInitialValues ();

			m_events = null;
			m_properties = null;
			m_parameters = null;
		}

		private void ReadClassLayoutInfos ()
		{
			if (!m_tHeap.HasTable (typeof (ClassLayoutTable)))
				return;

			ClassLayoutTable clTable = MetadataRoot.Streams.TablesHeap [typeof (ClassLayoutTable)] as ClassLayoutTable;
			for (int i = 0; i < clTable.Rows.Count; i++) {
				ClassLayoutRow clRow = clTable [i];
				TypeDefinition type = GetTypeDefAt (clRow.Parent);
				type.PackingSize = clRow.PackingSize;
				type.ClassSize = clRow.ClassSize;
			}
		}

		private void ReadFieldLayoutInfos ()
		{
			if (!m_tHeap.HasTable (typeof (FieldLayoutTable)))
				return;

			FieldLayoutTable flTable = MetadataRoot.Streams.TablesHeap [typeof (FieldLayoutTable)] as FieldLayoutTable;
			for (int i = 0; i < flTable.Rows.Count; i++) {
				FieldLayoutRow flRow = flTable [i];
				FieldDefinition field = GetFieldDefAt (flRow.Field);
				field.Offset = flRow.Offset;
			}
		}

		private void ReadPInvokeInfos ()
		{
			if (!m_tHeap.HasTable (typeof (ImplMapTable)))
				return;

			ImplMapTable imTable = MetadataRoot.Streams.TablesHeap [typeof (ImplMapTable)] as ImplMapTable;
			for (int i = 0; i < imTable.Rows.Count; i++) {
				ImplMapRow imRow = imTable [i];
				if (imRow.MemberForwarded.TokenType == TokenType.Method) { // should always be true
					MethodDefinition meth = GetMethodDefAt (imRow.MemberForwarded.RID);
					meth.PInvokeInfo = new PInvokeInfo (
						meth, imRow.MappingFlags, MetadataRoot.Streams.StringsHeap [imRow.ImportName],
						Module.ModuleReferences [(int) imRow.ImportScope - 1]);
				}
			}
		}

		private void ReadProperties ()
		{
			if (!m_tHeap.HasTable (typeof (PropertyTable)))
				return;

			PropertyTable propsTable = m_tHeap [typeof (PropertyTable)] as PropertyTable;
			PropertyMapTable pmapTable = m_tHeap [typeof (PropertyMapTable)] as PropertyMapTable;
			m_properties = new PropertyDefinition [propsTable.Rows.Count];
			for (int i = 0; i < pmapTable.Rows.Count; i++) {
				PropertyMapRow pmapRow = pmapTable [i];
				TypeDefinition owner = GetTypeDefAt (pmapRow.Parent);
				int start = (int) pmapRow.PropertyList, end;
				if (i < pmapTable.Rows.Count - 1)
					end = (int) pmapTable [i + 1].PropertyList;
				else
					end = propsTable.Rows.Count + 1;

				for (int j = start; j < end; j++) {
					PropertyRow prow = propsTable [j - 1];
					PropertySig psig = m_sigReader.GetPropSig (prow.Type);
					PropertyDefinition pdef = new PropertyDefinition (
						m_root.Streams.StringsHeap [prow.Name],
						this.GetTypeRefFromSig (psig.Type), prow.Flags);
					pdef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Property, j - 1);

					owner.Properties.Add (pdef);
					m_properties [j - 1] = pdef;
				}
			}
		}

		private void ReadEvents ()
		{
			if (!m_tHeap.HasTable (typeof (EventTable)))
				return;

			EventTable evtTable = m_tHeap [typeof (EventTable)] as EventTable;
			EventMapTable emapTable = m_tHeap [typeof (EventMapTable)] as EventMapTable;
			m_events = new EventDefinition [evtTable.Rows.Count];
			for (int i = 0; i < emapTable.Rows.Count; i++) {
				EventMapRow emapRow = emapTable [i];
				TypeDefinition owner = GetTypeDefAt (emapRow.Parent);
				int start = (int) emapRow.EventList, end;
				if (i < (emapTable.Rows.Count - 1))
					end = (int) emapTable [i + 1].EventList;
				else
					end = evtTable.Rows.Count + 1;

				for (int j = start; j < end; j++) {
					EventRow erow = evtTable [j - 1];
					EventDefinition edef = new EventDefinition (
						m_root.Streams.StringsHeap [erow.Name],
						GetTypeDefOrRef (erow.EventType), erow.EventFlags);
					edef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Event, j - 1);

					owner.Events.Add (edef);
					m_events [j - 1] = edef;
				}
			}
		}

		private void ReadSemantics ()
		{
			if (!m_tHeap.HasTable (typeof (MethodSemanticsTable)))
				return;

			MethodSemanticsTable semTable = m_tHeap [typeof (MethodSemanticsTable)] as MethodSemanticsTable;
			for (int i = 0; i < semTable.Rows.Count; i++) {
				MethodSemanticsRow semRow = semTable [i];
				MethodDefinition semMeth = GetMethodDefAt (semRow.Method);
				semMeth.SemanticsAttributes = semRow.Semantics;
				switch (semRow.Association.TokenType) {
				case TokenType.Event :
					EventDefinition evt = m_events [semRow.Association.RID - 1];
					if ((semRow.Semantics & MethodSemanticsAttributes.AddOn) != 0)
						evt.AddMethod = semMeth;
					else if ((semRow.Semantics & MethodSemanticsAttributes.Fire) != 0)
						evt.InvokeMethod = semMeth;
					else if ((semRow.Semantics & MethodSemanticsAttributes.RemoveOn) != 0)
						evt.RemoveMethod = semMeth;
					break;
				case TokenType.Property :
					PropertyDefinition prop = m_properties [semRow.Association.RID - 1];
					if ((semRow.Semantics & MethodSemanticsAttributes.Getter) != 0)
						prop.GetMethod = semMeth;
					else if ((semRow.Semantics & MethodSemanticsAttributes.Setter) != 0)
						prop.SetMethod = semMeth;
					break;
				}
			}
		}

		private void ReadInterfaces ()
		{
			if (!m_tHeap.HasTable (typeof (InterfaceImplTable)))
				return;

			InterfaceImplTable intfsTable = m_tHeap [typeof (InterfaceImplTable)] as InterfaceImplTable;
			for (int i = 0; i < intfsTable.Rows.Count; i++) {
				InterfaceImplRow intfsRow = intfsTable [i];
				TypeDefinition owner = GetTypeDefAt (intfsRow.Class);
				owner.Interfaces.Add (GetTypeDefOrRef (intfsRow.Interface));
			}
		}

		private void ReadOverrides ()
		{
			if (!m_tHeap.HasTable (typeof (MethodImplTable)))
				return;

			MethodImplTable implTable = m_tHeap [typeof (MethodImplTable)] as MethodImplTable;
			for (int i = 0; i < implTable.Rows.Count; i++) {
				MethodImplRow implRow = implTable [i];
				if (implRow.MethodBody.TokenType == TokenType.Method) {
					MethodDefinition owner = GetMethodDefAt (implRow.MethodBody.RID);
					switch (implRow.MethodDeclaration.TokenType) {
					case TokenType.Method :
						owner.Overrides.Add (
							GetMethodDefAt (implRow.MethodDeclaration.RID));
						break;
					case TokenType.MemberRef :
						owner.Overrides.Add (
							GetMemberRefAt (implRow.MethodDeclaration.RID) as IMethodReference);
						break;
					}
				}
			}
		}

		private void ReadSecurityDeclarations ()
		{
			if (!m_tHeap.HasTable (typeof (DeclSecurityTable)))
				return;

			DeclSecurityTable dsTable = m_tHeap [typeof (DeclSecurityTable)] as DeclSecurityTable;
			for (int i = 0; i < dsTable.Rows.Count; i++) {
				DeclSecurityRow dsRow = dsTable [i];
				SecurityDeclaration dec = BuildSecurityDeclaration (dsRow);

				IHasSecurity owner = null;
				switch (dsRow.Parent.TokenType) {
				case TokenType.Assembly :
					owner = this.Module.Assembly;
					break;
				case TokenType.TypeDef :
					owner = GetTypeDefAt (dsRow.Parent.RID);
					break;
				case TokenType.Method :
					owner = GetMethodDefAt (dsRow.Parent.RID);
					break;
				}

				SecurityDeclarationCollection secDecls = owner.SecurityDeclarations as SecurityDeclarationCollection;

				secDecls.Add (dec);
			}
		}

		private void ReadCustomAttributes ()
		{
			if (!m_tHeap.HasTable (typeof (CustomAttributeTable)))
				return;

			CustomAttributeTable caTable = m_tHeap [typeof (CustomAttributeTable)] as CustomAttributeTable;
			for (int i = 0; i < caTable.Rows.Count; i++) {
				CustomAttributeRow caRow = caTable [i];
				IMethodReference ctor;
				if (caRow.Type.TokenType == TokenType.Method)
					ctor = GetMethodDefAt (caRow.Type.RID);
				else
					ctor = GetMemberRefAt (caRow.Type.RID) as IMethodReference;

				CustomAttrib ca = m_sigReader.GetCustomAttrib (caRow.Value, ctor);
				CustomAttribute cattr = BuildCustomAttribute (ctor, ca);

				ICustomAttributeCollection owner = null;
				switch (caRow.Parent.TokenType) {
				case TokenType.Assembly :
					owner = this.Module.Assembly.CustomAttributes;
					break;
				case TokenType.Module :
					owner = this.Module.CustomAttributes;
					break;
				case TokenType.TypeDef :
					owner = GetTypeDefAt (caRow.Parent.RID).CustomAttributes;
					break;
				case TokenType.TypeRef :
					owner = GetTypeRefAt (caRow.Parent.RID).CustomAttributes;
					break;
				case TokenType.Field :
					owner = GetFieldDefAt (caRow.Parent.RID).CustomAttributes;
					break;
				case TokenType.Method :
					owner = GetMethodDefAt (caRow.Parent.RID).CustomAttributes;
					break;
				case TokenType.Property :
					owner = GetPropertyDefAt (caRow.Parent.RID).CustomAttributes;
					break;
				case TokenType.Event :
					owner = GetEventDefAt (caRow.Parent.RID).CustomAttributes;
					break;
				case TokenType.Param :
					owner = GetParamDefAt (caRow.Parent.RID).CustomAttributes;
					break;
				default :
					//TODO: support other ?
					break;
				}
				if (owner != null)
					(owner as CustomAttributeCollection).Add (cattr);
			}
		}

		private void ReadConstants ()
		{
			if (!m_tHeap.HasTable (typeof (ConstantTable)))
				return;

			ConstantTable csTable = m_tHeap [typeof (ConstantTable)] as ConstantTable;
			for (int i = 0; i < csTable.Rows.Count; i++) {
				ConstantRow csRow = csTable [i];

				object constant = GetConstant (csRow.Value, csRow.Type);

				switch (csRow.Parent.TokenType) {
				case TokenType.Field :
					FieldDefinition field = GetFieldDefAt (csRow.Parent.RID);
					field.Constant = constant;
					break;
				case TokenType.Property :
					PropertyDefinition prop = GetPropertyDefAt (csRow.Parent.RID);
					prop.Constant = constant;
					break;
				case TokenType.Param :
					ParameterDefinition param = GetParamDefAt (csRow.Parent.RID);
					param.Constant = constant;
					break;
				}
			}
		}

		private void ReadExternTypes ()
		{
			base.VisitExternTypeCollection (Module.ExternTypes as ExternTypeCollection);
		}

		private void ReadMarshalSpecs ()
		{
			if (!m_tHeap.HasTable (typeof (FieldMarshalTable)))
				return;

			FieldMarshalTable fmTable = m_tHeap [typeof (FieldMarshalTable)] as FieldMarshalTable;
			for (int i = 0; i < fmTable.Rows.Count; i++) {
				FieldMarshalRow fmRow = fmTable [i];
				switch (fmRow.Parent.TokenType) {
				case TokenType.Field:
					FieldDefinition field = GetFieldDefAt (fmRow.Parent.RID);
					field.MarshalSpec = BuildMarshalDesc (
						m_sigReader.GetMarshalSig (fmRow.NativeType), field);
					break;
				case TokenType.Param:
					ParameterDefinition param = GetParamDefAt (fmRow.Parent.RID);
					param.MarshalSpec = BuildMarshalDesc (
						m_sigReader.GetMarshalSig (fmRow.NativeType), param);
					break;
				}
			}
		}

		private void ReadInitialValues ()
		{
			if (!m_tHeap.HasTable (typeof (FieldRVATable)))
				return;

			FieldRVATable frTable = m_tHeap [typeof (FieldRVATable)] as FieldRVATable;
			for (int i = 0; i < frTable.Rows.Count; i++) {
				FieldRVARow frRow = frTable [i];
				FieldDefinition field = GetFieldDefAt (frRow.Field);
				field.RVA = frRow.RVA;
				SetInitialValue (field);
			}
		}
	}
}
