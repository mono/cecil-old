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

	internal sealed class LazyReflectionReader : ReflectionReader {

		public LazyReflectionReader (ModuleDefinition module) : base (module)
		{
		}

		public override void VisitInterfaceCollection (IInterfaceCollection interfaces)
		{
			InterfaceCollection interfs = interfaces as InterfaceCollection;
			if (interfs != null && interfs.Loaded)
				return;

			if (!m_tHeap.HasTable (typeof (InterfaceImplTable))) {
				interfs.Loaded = true;
				return;
			}

			TypeDefinition implementor = interfaces.Container as TypeDefinition;

			int rid = GetRidForTypeDef (implementor);

			InterfaceImplTable intfsTable = m_tHeap [typeof (InterfaceImplTable)] as InterfaceImplTable;
			for (int i = 0; i < intfsTable.Rows.Count; i++) {
				InterfaceImplRow intRow = intfsTable [i];
				if (intRow.Class == rid) {
					ITypeReference interf = GetTypeDefOrRef (intRow.Interface);
					(interfaces as InterfaceCollection).Add (interf);
				}
			}

			interfs.Loaded = true;
		}

		public override void VisitExternTypeCollection (IExternTypeCollection externs)
		{
			ExternTypeCollection ext = externs as ExternTypeCollection;
			if (ext.Loaded)
				return;

			base.VisitExternTypeCollection (externs);

			ext.Loaded = true;
		}

		public override void VisitOverrideCollection (IOverrideCollection meths)
		{
			OverrideCollection methods = meths as OverrideCollection;
			if (methods.Loaded)
				return;

			if (!m_tHeap.HasTable (typeof (MethodImplTable))) {
				methods.Loaded = true;
				return;
			}

			MethodImplTable implTable = MetadataRoot.Streams.TablesHeap [typeof (MethodImplTable)] as MethodImplTable;

			int index = GetRidForMethodDef (meths.Container as MethodDefinition);
			for (int i = 0; i < implTable.Rows.Count; i++) {
				MethodImplRow implRow = implTable [i];
				if (implRow.MethodBody.TokenType == TokenType.Method && implRow.MethodBody.RID == index) {
					if (implRow.MethodDeclaration.TokenType == TokenType.Method)
						methods.Add (GetMethodDefAt (implRow.MethodDeclaration.RID));
					else if (implRow.MethodDeclaration.TokenType == TokenType.MemberRef)
						methods.Add (GetMemberRefAt (implRow.MethodDeclaration.RID) as IMethodReference);
				}
			}
		}

		public override void VisitSecurityDeclarationCollection (ISecurityDeclarationCollection secDecls)
		{
			SecurityDeclarationCollection secDeclarations = secDecls as SecurityDeclarationCollection;
			if (secDeclarations.Loaded)
				return;

			if (!m_tHeap.HasTable (typeof (DeclSecurityTable))) {
				secDeclarations.Loaded = true;
				return;
			}

			DeclSecurityTable dsTable = m_tHeap [typeof (DeclSecurityTable)] as DeclSecurityTable;
			int rid = 0;
			TokenType target;
			if (secDecls.Container is AssemblyDefinition)
				target = TokenType.Assembly;
			else if (secDecls.Container is TypeDefinition) {
				target = TokenType.TypeDef;
				rid = GetRidForTypeDef (secDecls.Container as TypeDefinition);
			} else { // MethodDefinition
				target = TokenType.Method;
				rid = GetRidForMethodDef (secDecls.Container as MethodDefinition);
			}

			for (int i = 0; i < dsTable.Rows.Count; i++) {
				DeclSecurityRow dsRow = dsTable [i];

				switch (dsRow.Parent.TokenType) {
				case TokenType.Assembly :
					if (target == TokenType.Assembly)
						secDeclarations.Add (BuildSecurityDeclaration (dsRow));
					break;
				case TokenType.Method :
				case TokenType.TypeDef :
					if (dsRow.Parent.TokenType == target && dsRow.Parent.RID == rid)
						secDeclarations.Add (BuildSecurityDeclaration (dsRow));
					break;
				}
			}

			secDeclarations.Loaded = true;
		}

		public override void VisitCustomAttributeCollection (ICustomAttributeCollection customAttrs)
		{
			CustomAttributeCollection customAttributes = customAttrs as CustomAttributeCollection;
			if (customAttributes.Loaded)
				return;

			if (!m_tHeap.HasTable (typeof (CustomAttributeTable))) {
				customAttributes.Loaded = true;
				return;
			}

			CustomAttributeTable caTable = m_tHeap [typeof (CustomAttributeTable)] as CustomAttributeTable;
			int rid = 0;
			TokenType target;
			if (customAttrs.Container is AssemblyDefinition) {
				rid =1;
				target = TokenType.Assembly;
			} else if (customAttrs.Container is ModuleDefinition) {
				rid = 1;
				target = TokenType.Module;
			} else if (customAttrs.Container is TypeDefinition) {
				rid = GetRidForTypeDef (customAttrs.Container as TypeDefinition);
				target = TokenType.TypeDef;
			} else if (customAttrs.Container is TypeReference) {
				rid = Array.IndexOf (m_typeRefs, customAttrs.Container) + 1;
				target = TokenType.TypeRef;
			} else if (customAttrs.Container is FieldDefinition) {
				rid = Array.IndexOf (m_fields, customAttrs.Container) + 1;
				target = TokenType.Field;
			} else if (customAttrs.Container is MethodDefinition) {
				rid = GetRidForMethodDef (customAttrs.Container as MethodDefinition);
				target = TokenType.Method;
			} else if (customAttrs.Container is PropertyDefinition) {
				rid = Array.IndexOf (m_properties, customAttrs.Container) + 1;
				target = TokenType.Property;
			} else if (customAttrs.Container is EventDefinition) {
				rid = Array.IndexOf (m_events, customAttrs.Container) + 1;
				target = TokenType.Event;
			} else if (customAttrs.Container is ParameterDefinition) {
				rid = Array.IndexOf (m_parameters, customAttrs.Container) + 1;
				target = TokenType.Param;
			} else {
				//TODO: support other ?
				customAttributes.Loaded = true;
				return;
			}

			for (int i = 0; i < caTable.Rows.Count; i++) {
				CustomAttributeRow caRow = caTable [i];

				CustomAttrib sig = null;
				CustomAttribute ca = null;
				IMethodReference ctor = null;

				switch (caRow.Parent.TokenType) {
				case TokenType.Assembly :
				case TokenType.Module :
				case TokenType.TypeDef :
				case TokenType.Field :
				case TokenType.Method :
				case TokenType.Event :
				case TokenType.Property :
				case TokenType.Param :
					if (caRow.Parent.TokenType == target && caRow.Parent.RID == rid) {
						if (caRow.Type.TokenType == TokenType.Method)
							ctor = GetMethodDefAt (caRow.Type.RID);
						else
							ctor = GetMemberRefAt (caRow.Type.RID) as IMethodReference;

						sig = m_sigReader.GetCustomAttrib (caRow.Value, ctor);
						ca = BuildCustomAttribute (ctor, sig);
					}
					break;
				}
				if (ca != null) {
					customAttributes.Add (ca);
				}
			}

			customAttributes.Loaded = true;
		}

		public override void VisitEventDefinitionCollection (IEventDefinitionCollection events)
		{
			EventDefinitionCollection evts = events as EventDefinitionCollection;
			if (evts.Loaded)
				return;

			if (!m_tHeap.HasTable (typeof (EventTable))) {
				m_events = new EventDefinition [0];
				evts.Loaded = true;
				return;
			}

			TypeDefinition dec = evts.Container as TypeDefinition;
			int rid = GetRidForTypeDef (dec), next;
			EventTable evtTable = m_tHeap [typeof (EventTable)] as EventTable;

			if (m_events == null)
				m_events = new EventDefinition [evtTable.Rows.Count];

			EventMapTable evtMapTable = m_tHeap [typeof (EventMapTable)] as EventMapTable;
			EventMapRow thisRow = null, nextRow = null;
			for (int i = 0; i < evtMapTable.Rows.Count; i++) {
				if (evtMapTable [i].Parent == rid) {
					thisRow = evtMapTable [i];
					if (i < evtMapTable.Rows.Count - 1)
						nextRow = evtMapTable [i + 1];
					break;
				}
			}

			if (thisRow == null)
				return;

			if (nextRow == null)
				next = evtTable.Rows.Count + 1;
			else
				next = (int) nextRow.EventList;

			for (int i = (int) thisRow.EventList; i < next; i++) {
				EventRow erow = evtTable [i - 1];
				EventDefinition edef = new EventDefinition (
					m_root.Streams.StringsHeap [erow.Name], dec,
					GetTypeDefOrRef (erow.EventType), erow.EventFlags);
				edef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Event, i - 1);
				evts.Add (edef);
				m_events [i - 1] = edef;
			}

			evts.Loaded = true;
		}

		public override void VisitPInvokeInfo (IPInvokeInfo pinvk)
		{
			MethodDefinition meth = pinvk.Method as MethodDefinition;
			int index = GetRidForMethodDef (meth);

			ImplMapTable imTable = m_tHeap [typeof (ImplMapTable)] as ImplMapTable;
			for (int i = 0; i < imTable.Rows.Count; i++) {
				ImplMapRow imRow = imTable [i];
				if (imRow.MemberForwarded.RID == index) {
					meth.PInvokeInfo = new PInvokeInfo (
						meth, imRow.MappingFlags, MetadataRoot.Streams.StringsHeap [imRow.ImportName],
						Module.ModuleReferences [(int) imRow.ImportScope - 1]);
					break;
				}
			}
		}

		public override void VisitPropertyDefinitionCollection (IPropertyDefinitionCollection properties)
		{
			PropertyDefinitionCollection props = properties as PropertyDefinitionCollection;
			if (props.Loaded)
				return;

			if (!m_tHeap.HasTable (typeof (PropertyTable))) {
				m_properties = new PropertyDefinition [0];
				props.Loaded = true;
				return;
			}

			TypeDefinition dec = props.Container as TypeDefinition;
			int rid = GetRidForTypeDef (dec), next;
			PropertyTable propsTable = m_tHeap [typeof (PropertyTable)] as PropertyTable;
			if (m_properties == null)
				m_properties = new PropertyDefinition [propsTable.Rows.Count];

			PropertyMapTable pmapTable = m_tHeap [typeof (PropertyMapTable)] as PropertyMapTable;
			PropertyMapRow thisRow = null, nextRow = null;
			for (int i = 0; i < pmapTable.Rows.Count; i++) {
				if (pmapTable [i].Parent == rid) {
					thisRow = pmapTable [i];
					if (i < pmapTable.Rows.Count - 1)
						nextRow = pmapTable [i + 1];
					break;
				}
			}

			if (thisRow == null)
				return;

			if (nextRow == null)
				next = propsTable.Rows.Count + 1;
			else
				next = (int) nextRow.PropertyList;

			for (int i = (int) thisRow.PropertyList; i < next; i++) {
				PropertyRow prow = propsTable [i - 1];
				PropertySig psig = m_sigReader.GetPropSig (prow.Type);
				PropertyDefinition pdef = new PropertyDefinition (
					MetadataRoot.Streams.StringsHeap [prow.Name],
					dec, this.GetTypeRefFromSig (psig.Type), prow.Flags);
				pdef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Property, i - 1);
				props.Add (pdef);
				m_properties [i - 1] = pdef;
			}

			props.Loaded = true;
		}

		public override void ReadSemantic (EventDefinition evt)
		{
			if (!m_tHeap.HasTable (typeof (MethodSemanticsTable))) {
				evt.SemanticLoaded = true;
				return;
			}

			int index = Array.IndexOf (m_events, evt) + 1;

			MethodSemanticsTable semTable = m_tHeap [typeof (MethodSemanticsTable)] as MethodSemanticsTable;
			for (int i = 0; i < semTable.Rows.Count; i++) {
				MethodSemanticsRow semRow = semTable [i];
				MethodDefinition semMeth = GetMethodDefAt (semRow.Method);
				if (semRow.Association.TokenType == TokenType.Event && semRow.Association.RID == index) {
					semMeth.SemanticsAttributes = semRow.Semantics;
					if ((semRow.Semantics & MethodSemanticsAttributes.AddOn) != 0)
						evt.AddMethod = semMeth;
					else if ((semRow.Semantics & MethodSemanticsAttributes.Fire) != 0)
						evt.InvokeMethod = semMeth;
					else if ((semRow.Semantics & MethodSemanticsAttributes.RemoveOn) != 0)
						evt.RemoveMethod = semMeth;
				}
			}

			evt.SemanticLoaded = true;
		}

		public override void ReadSemantic (PropertyDefinition prop)
		{
			if (!m_tHeap.HasTable (typeof (MethodSemanticsTable))) {
				prop.SemanticLoaded = true;
				return;
			}

			int index = Array.IndexOf (m_properties, prop) + 1;

			MethodSemanticsTable semTable = m_tHeap [typeof (MethodSemanticsTable)] as MethodSemanticsTable;
			for (int i = 0; i < semTable.Rows.Count; i++) {
				MethodSemanticsRow semRow = semTable [i];
				MethodDefinition semMeth = GetMethodDefAt (semRow.Method);
				if (semRow.Association.TokenType == TokenType.Property && semRow.Association.RID == index) {
					semMeth.SemanticsAttributes = semRow.Semantics;
					if ((semRow.Semantics & MethodSemanticsAttributes.Getter) != 0)
						prop.GetMethod = semMeth;
					else if ((semRow.Semantics & MethodSemanticsAttributes.Setter) != 0)
						prop.SetMethod = semMeth;
				}
			}

			prop.SemanticLoaded = true;
		}

		public override void ReadMarshalSpec (ParameterDefinition param)
		{
			param.MarshalSpec = GetMarshalDesc (
				Array.IndexOf (m_parameters, param) + 1, TokenType.Param, param);
			param.MarshalSpecLoaded = true;
		}

		public override void ReadMarshalSpec (FieldDefinition field)
		{
			field.MarshalSpec = GetMarshalDesc (
				Array.IndexOf (m_fields, field) + 1, TokenType.Field, field);
			field.MarshalSpecLoaded = true;
		}

		private MarshalDesc GetMarshalDesc (int rid, TokenType target, IHasMarshalSpec container)
		{
			if (!m_tHeap.HasTable (typeof (FieldMarshalTable)))
				return null;

			FieldMarshalTable fmTable = m_tHeap [typeof (FieldMarshalTable)] as FieldMarshalTable;
			for (int i = 0; i < fmTable.Rows.Count; i++) {
				FieldMarshalRow fmRow = fmTable [i];
				if (fmRow.Parent.TokenType == target && fmRow.Parent.RID == rid)
					return BuildMarshalDesc (m_sigReader.GetMarshalSig (fmRow.NativeType), container);
			}

			return null;
		}

		public override void ReadLayout (TypeDefinition type)
		{
			if (!m_tHeap.HasTable (typeof (ClassLayoutTable))) {
				type.LayoutLoaded = true;
				return;
			}

			uint rid = (uint) GetRidForTypeDef (type);

			ClassLayoutTable clTable = MetadataRoot.Streams.TablesHeap [typeof (ClassLayoutTable)] as ClassLayoutTable;
			for (int i = 0; i < clTable.Rows.Count; i++) {
				ClassLayoutRow clRow = clTable [i];
				if (clRow.Parent == rid) {
					type.ClassSize = clRow.ClassSize;
					type.PackingSize = clRow.PackingSize;
					break;
				}
			}

			type.LayoutLoaded = true;
		}

		public override void ReadLayout (FieldDefinition field)
		{
			if (!m_tHeap.HasTable (typeof (FieldLayoutTable))) {
				field.LayoutLoaded = true;
				return;
			}

			uint rid = (uint) Array.IndexOf (m_fields, field) + 1;

			FieldLayoutTable flTable = MetadataRoot.Streams.TablesHeap [typeof (FieldLayoutTable)] as FieldLayoutTable;
			for (int i = 0; i < flTable.Rows.Count; i++) {
				FieldLayoutRow flRow = flTable [i];
				if (flRow.Field == rid) {
					field.Offset = flRow.Offset;
					break;
				}
			}

			field.LayoutLoaded = true;
		}

		public override void ReadConstant (FieldDefinition field)
		{
			bool hasConst;
			object constant = GetConstant (
				Array.IndexOf (m_fields, field) + 1, TokenType.Field, out hasConst);
			if (hasConst)
				field.Constant = constant;
			field.ConstantLoaded = true;
		}

		public override void ReadConstant (PropertyDefinition prop)
		{
			bool hasConst;
			object constant = GetConstant (
				Array.IndexOf (m_properties, prop) + 1, TokenType.Property, out hasConst);
			if (hasConst)
				prop.Constant = constant;
			prop.ConstantLoaded = true;
		}

		public override void ReadConstant (ParameterDefinition param)
		{
			bool hasConst;
			object constant = GetConstant (
				Array.IndexOf (m_parameters, param) + 1, TokenType.Param, out hasConst);
			if (hasConst)
				param.Constant = constant;
			param.ConstantLoaded = true;
		}

		public object GetConstant (int rid, TokenType target, out bool hasConstant)
		{
			hasConstant = false;

			if (!m_tHeap.HasTable (typeof (ConstantTable)))
				return null;

			ConstantTable csTable = m_tHeap [typeof (ConstantTable)] as ConstantTable;
			for (int i = 0; i < csTable.Rows.Count; i++) {
				ConstantRow csRow = csTable [i];
				if (csRow.Parent.TokenType == target && csRow.Parent.RID == rid) {
					hasConstant = true;
					return GetConstant (csRow.Value, csRow.Type);
				}
			}

			return null;
		}
	}
}
