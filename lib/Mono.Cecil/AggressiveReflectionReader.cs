//
// AggressiveRefletionReader.cs
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

	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	internal sealed class AggressiveReflectionReader : ReflectionReader {

		public AggressiveReflectionReader (ModuleDefinition module) : base (module)
		{
		}

		public override void VisitTypeDefinitionCollection (TypeDefinitionCollection types)
		{
			base.VisitTypeDefinitionCollection (types);
			if (types.Container.Assembly.Runtime >= TargetRuntime.NET_2_0) {
				ReadGenericParameters ();
				ReadGenericParameterConstraints ();
				ReadMethodSpec ();
			}
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

		void ReadGenericParameters ()
		{
			if (!m_tHeap.HasTable (typeof (GenericParamTable)))
				return;

			GenericParamTable gpTable = m_tHeap [typeof (GenericParamTable)] as GenericParamTable;
			m_genericParameters = new GenericParameter [gpTable.Rows.Count];
			for (int i = 0; i < gpTable.Rows.Count; i++) {
				GenericParamRow gpRow = gpTable [i];
				IGenericParameterProvider owner;
				if (gpRow.Owner.TokenType == TokenType.Method)
					owner = GetMethodDefAt (gpRow.Owner.RID);
				else if (gpRow.Owner.TokenType == TokenType.TypeDef)
					owner = GetTypeDefAt (gpRow.Owner.RID);
				else
					throw new ReflectionException ("Unknown owner type for generic parameter");

				GenericParameter gp = new GenericParameter ((int) gpRow.Number, owner);
				gp.Attributes = gpRow.Flags;
				gp.Name = MetadataRoot.Streams.StringsHeap [gpRow.Name];

				owner.GenericParameters.Add (gp);
				m_genericParameters [i] = gp;
			}
		}

		void ReadGenericParameterConstraints ()
		{
			if (!m_tHeap.HasTable (typeof (GenericParamConstraintTable)))
				return;

			GenericParamConstraintTable gpcTable = MetadataRoot.Streams.TablesHeap [
				typeof (GenericParamConstraintTable)] as GenericParamConstraintTable;
			for (int i = 0; i < gpcTable.Rows.Count; i++) {
				GenericParamConstraintRow gpcRow = gpcTable [i];
				GenericParameter gp = GetGenericParameterAt (gpcRow.Owner);

				TypeReference constraint = GetTypeDefOrRef (gpcRow.Constraint);
				// TODO: find how to know if constraint is an interface
			}
		}

		void ReadMethodSpec ()
		{
			if (!m_tHeap.HasTable (typeof (MethodSpecTable)))
				return;

			MethodSpecTable msTable = m_tHeap [typeof (MethodSpecTable)] as MethodSpecTable;
			m_methodSpecs = new GenericInstanceMethod [msTable.Rows.Count];
			for (int i = 0; i < msTable.Rows.Count; i++) {
				MethodSpecRow msRow = msTable [i];

				MethodReference meth;
				if (msRow.Method.TokenType == TokenType.Method)
					meth = GetMethodDefAt (msRow.Method.RID);
				else if (msRow.Method.TokenType == TokenType.MemberRef)
					meth = (MethodReference) GetMemberRefAt (msRow.Method.RID);
				else
					throw new ReflectionException ("Unknown method type for method spec");

				GenericInstanceMethod gim = new GenericInstanceMethod (meth);
				MethodSpec sig = m_sigReader.GetMethodSpec (msRow.Instantiation);
				foreach (SigType st in sig.Signature.Types)
					gim.Arguments.Add (GetTypeRefFromSig (st));

				m_methodSpecs [i] = gim;
			}
		}

		void ReadClassLayoutInfos ()
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

		void ReadFieldLayoutInfos ()
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

		void ReadPInvokeInfos ()
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

		void ReadProperties ()
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

		void ReadEvents ()
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

		void ReadSemantics ()
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

		void ReadInterfaces ()
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

		void ReadOverrides ()
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
							GetMemberRefAt (implRow.MethodDeclaration.RID) as MethodReference);
						break;
					}
				}
			}
		}

		void ReadSecurityDeclarations ()
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

		void ReadCustomAttributes ()
		{
			if (!m_tHeap.HasTable (typeof (CustomAttributeTable)))
				return;

			CustomAttributeTable caTable = m_tHeap [typeof (CustomAttributeTable)] as CustomAttributeTable;
			for (int i = 0; i < caTable.Rows.Count; i++) {
				CustomAttributeRow caRow = caTable [i];
				MethodReference ctor;
				if (caRow.Type.TokenType == TokenType.Method)
					ctor = GetMethodDefAt (caRow.Type.RID);
				else
					ctor = GetMemberRefAt (caRow.Type.RID) as MethodReference;

				CustomAttrib ca = m_sigReader.GetCustomAttrib (caRow.Value, ctor);
				CustomAttribute cattr;
				if (!ca.Read) {
					cattr = new CustomAttribute (ctor);
					cattr.IsReadable = false;
					cattr.Blob = m_root.Streams.BlobHeap.Read (caRow.Value);
				} else
					cattr = BuildCustomAttribute (ctor, ca);

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

		void ReadConstants ()
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

		void ReadExternTypes ()
		{
			base.VisitExternTypeCollection (Module.ExternTypes as ExternTypeCollection);
		}

		void ReadMarshalSpecs ()
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

		void ReadInitialValues ()
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
