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
	using System.IO;
	using System.Security;
	using System.Security.Permissions;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	using Mono.Xml;

	internal abstract class ReflectionReader : IReflectionVisitor, IDetailReader {

		private ModuleDefinition m_module;
		private ImageReader m_reader;
		protected SecurityParser m_secParser;
		protected MetadataRoot m_root;
		protected TablesHeap m_tHeap;

		protected TypeDefinition [] m_typeDefs;
		protected TypeReference [] m_typeRefs;
		protected ITypeReference [] m_typeSpecs;
		protected MethodDefinition [] m_meths;
		protected FieldDefinition [] m_fields;
		protected EventDefinition [] m_events;
		protected PropertyDefinition [] m_properties;
		protected MemberReference [] m_memberRefs;
		protected ParameterDefinition [] m_parameters;

		protected SignatureReader m_sigReader;
		protected CodeReader m_codeReader;

		private bool m_isCorlib;

		public ModuleDefinition Module {
			get { return m_module; }
		}

		public SignatureReader SigReader {
			get { return m_sigReader; }
		}

		public CodeReader Code {
			get { return m_codeReader; }
		}

		public MetadataRoot MetadataRoot {
			get { return m_root; }
		}

		public ReflectionReader (ModuleDefinition module)
		{
			m_module = module;
			m_reader = m_module.ImageReader;
			m_root = m_module.Image.MetadataRoot;
			m_tHeap = m_root.Streams.TablesHeap;
			m_codeReader = new CodeReader (this);
			m_sigReader = new SignatureReader (m_root, this);
			m_secParser = new SecurityParser ();
			m_isCorlib = m_module.Image.FileInformation != null && m_module.Image.FileInformation.Name == Constants.Corlib;
		}

		public TypeDefinition GetTypeDefAt (int rid)
		{
			return m_typeDefs [rid - 1];
		}

		public TypeReference GetTypeRefAt (int rid)
		{
			// 0 based
			return m_typeRefs [rid - 1];
		}

		public ITypeReference GetTypeSpecAt (int rid)
		{
			return m_typeSpecs [rid - 1];
		}

		public FieldDefinition GetFieldDefAt (int rid)
		{
			return m_fields [rid - 1];
		}

		public MethodDefinition GetMethodDefAt (int rid)
		{
			return m_meths [rid - 1];
		}

		public MemberReference GetMemberRefAt (int rid)
		{
			return m_memberRefs [rid - 1];
		}

		public int GetRidForMethodDef (MethodDefinition meth)
		{
			int index = Array.IndexOf (m_meths, meth);
			return index == - 1 ? 0 : index + 1;
		}

		public int GetRidForTypeDef (TypeDefinition typeDef)
		{
			int index = Array.IndexOf (m_typeDefs, typeDef);
			return index == -1 ? 0 : index + 1;
		}

		public ITypeReference GetTypeDefOrRef (MetadataToken token)
		{
			if (token.RID == 0)
				return null;

			switch (token.TokenType) {
			case TokenType.TypeDef :
				return GetTypeDefAt ((int) token.RID);
			case TokenType.TypeRef :
				return GetTypeRefAt ((int) token.RID);
			case TokenType.TypeSpec :
				return GetTypeSpecAt ((int) token.RID);
			default :
				return null;
			}
		}

		public ITypeReference SearchCoreType (string fullName)
		{
			if (m_isCorlib)
				return m_module.Types [fullName];

			ITypeReference coreType =  m_module.TypeReferences [fullName];
			if (coreType == null) {
				string [] parts = fullName.Split ('.');
				if (parts.Length != 2)
					throw new ReflectionException ("Unvalid core type name");
				coreType = new TypeReference (parts [1], parts [0]);
				(m_module.TypeReferences as TypeReferenceCollection) [coreType.FullName] = coreType;
			}
			return coreType;
		}

		public CustomAttribute GetCustomAttribute (IMethodReference ctor, byte [] data)
		{
			CustomAttrib sig = m_sigReader.GetCustomAttrib (data, ctor);
			return BuildCustomAttribute (ctor, sig);
		}

		public virtual void Visit (ITypeDefinitionCollection types)
		{
			TypeDefinitionCollection tdc = types as TypeDefinitionCollection;
			if (tdc != null && tdc.Loaded)
				return;

			ModuleDefinition def = tdc.Container as ModuleDefinition;

			// type def reading
			TypeDefTable typesTable = m_tHeap [typeof (TypeDefTable)] as TypeDefTable;
			m_typeDefs = new TypeDefinition [typesTable.Rows.Count];
			for (int i = 0; i < typesTable.Rows.Count; i++) {
				TypeDefRow type = typesTable [i];
				TypeDefinition t = new TypeDefinition (
					m_root.Streams.StringsHeap [type.Name],
					m_root.Streams.StringsHeap [type.Namespace],
					type.Flags, def);

				m_typeDefs [i] = t;
			}

			// nested types
			NestedClassTable nested = m_tHeap [typeof (NestedClassTable)] as NestedClassTable;
			if (m_tHeap.HasTable (typeof(NestedClassTable))) {
				for (int i = 0; i < nested.Rows.Count; i++) {
					NestedClassRow row = nested [i];

					TypeDefinition parent = GetTypeDefAt ((int) row.EnclosingClass);
					TypeDefinition child = GetTypeDefAt ((int) row.NestedClass);

					child.DeclaringType = parent;
					(parent.NestedTypes as NestedTypeCollection) [child.Name] = child;
				}
			}

			// type ref reading
			if (m_tHeap.HasTable(typeof (TypeRefTable))) {
				TypeRefTable typesRef = m_tHeap [typeof (TypeRefTable)] as TypeRefTable;

				m_typeRefs = new TypeReference [typesRef.Rows.Count];

				for (int i = 0; i < typesRef.Rows.Count; i++) {
					TypeRefRow type = typesRef [i];
					IMetadataScope scope = null;
					TypeReference parent = null;
					switch (type.ResolutionScope.TokenType) {
					case TokenType.AssemblyRef :
						scope = m_module.AssemblyReferences [(int) type.ResolutionScope.RID - 1];
						break;
					case TokenType.ModuleRef :
						scope = m_module.ModuleReferences [(int) type.ResolutionScope.RID - 1];
						break;
					case TokenType.TypeRef :
						parent = GetTypeRefAt ((int) type.ResolutionScope.RID);
						scope = parent.Scope;
						break;
					}

					TypeReference t = new TypeReference (
						m_root.Streams.StringsHeap [type.Name],
						m_root.Streams.StringsHeap [type.Namespace],
						m_module, scope);

					if (parent != null)
						t.DeclaringType = parent;

					m_typeRefs [i] = t;
					(m_module.TypeReferences as TypeReferenceCollection) [t.FullName] = t;
				}

			} else
				m_typeRefs = new TypeReference [0];

			tdc.Loaded = true;

			for (int i = 0; i < m_typeDefs.Length; i++) {
				TypeDefinition type = m_typeDefs [i];
				tdc [type.FullName] = type;
			}

			for (int i = 0; i < m_typeRefs.Length; i++) {
				TypeReference type = m_typeRefs [i];
				(m_module.TypeReferences as TypeReferenceCollection) [type.FullName] = type;
			}

			ReadTypeSpecs ();

			// set base types
			for (int i = 0; i < typesTable.Rows.Count; i++) {
				TypeDefRow type = typesTable [i];
				TypeDefinition child = m_typeDefs [i];
				child.BaseType = GetTypeDefOrRef (type.Extends);
			}

			// ok, I've thought a lot before doing that
			// if I do not load the two primitives that are field and methods here
			// i'll run into big troubles as soon a lazy loaded stuff reference it
			// such as methods body or an override collection

			ReadAllFields ();
			ReadAllMethods ();
			ReadMemberReferences ();
		}

		private void ReadTypeSpecs ()
		{
			if (!m_tHeap.HasTable (typeof (TypeSpecTable)))
				return;

			TypeSpecTable tsTable = m_tHeap [typeof (TypeSpecTable)] as TypeSpecTable;
			m_typeSpecs = new ITypeReference [tsTable.Rows.Count];
			for (int i = 0; i < tsTable.Rows.Count; i++) {
				TypeSpecRow tsRow = tsTable [i];
				TypeSpec ts = m_sigReader.GetTypeSpec (tsRow.Signature);
				ITypeReference tspec = GetTypeRefFromSig (ts.Type);
				m_typeSpecs [i] = tspec;
			}
		}

		private void ReadAllFields ()
		{
			TypeDefTable tdefTable = m_tHeap [typeof (TypeDefTable)] as TypeDefTable;
			if (!m_tHeap.HasTable(typeof (FieldTable))) {
				m_fields = new FieldDefinition [0];
				return;
			}

			FieldTable fldTable = m_tHeap [typeof (FieldTable)] as FieldTable;
			m_fields = new FieldDefinition [fldTable.Rows.Count];

			for (int i = 0; i < m_typeDefs.Length; i++) {
				TypeDefinition dec = m_typeDefs [i];

				int index = i, next;

				if (index == tdefTable.Rows.Count - 1)
					next = fldTable.Rows.Count + 1;
				else
					next = (int) (tdefTable [index + 1]).FieldList;

				for (int j = (int) tdefTable [index].FieldList; j < next; j++) {
					FieldRow frow = fldTable [j - 1];
					FieldSig fsig = m_sigReader.GetFieldSig (frow.Signature);
					FieldDefinition fdef = new FieldDefinition (m_root.Streams.StringsHeap [frow.Name],
																dec, this.GetTypeRefFromSig (fsig.Type), frow.Flags);

					if (fsig.CustomMods.Length > 0)
						fdef.FieldType = GetModifierType (fsig.CustomMods, fdef.FieldType);

					FieldDefinitionCollection flds = dec.Fields as FieldDefinitionCollection;

					flds.Loaded = true;
					flds [fdef.Name] = fdef;
					m_fields [j - 1] = fdef;
				}
			}
		}

		private void ReadAllMethods ()
		{
			TypeDefTable tdefTable = m_tHeap [typeof (TypeDefTable)] as TypeDefTable;

			if (!m_tHeap.HasTable (typeof (MethodTable))) {
				m_meths = new MethodDefinition [0];
				return;
			}

			MethodTable methTable = m_tHeap [typeof (MethodTable)] as MethodTable;
			ParamTable paramTable = m_tHeap [typeof (ParamTable)] as ParamTable;
			m_meths = new MethodDefinition [methTable.Rows.Count];
			if (!m_tHeap.HasTable (typeof (ParamTable)))
				m_parameters = new ParameterDefinition [0];
			else
				m_parameters = new ParameterDefinition [paramTable.Rows.Count];

			for (int i = 0; i < m_typeDefs.Length; i++) {
				TypeDefinition dec = m_typeDefs [i];

				int index = i, next;

				if (index == tdefTable.Rows.Count - 1)
					next = methTable.Rows.Count + 1;
				else
					next = (int) (tdefTable [index + 1]).MethodList;

				for (int j = (int) tdefTable [index].MethodList; j < next; j++) {
					MethodRow methRow = methTable [j - 1];
					MethodDefSig msig = m_sigReader.GetMethodDefSig (methRow.Signature);

					MethodDefinition mdef = new MethodDefinition (m_root.Streams.StringsHeap [methRow.Name], dec,
																  methRow.RVA, methRow.Flags, methRow.ImplFlags,
																  msig.HasThis, msig.ExplicitThis, msig.MethCallConv);
					int prms;
					if (j == methTable.Rows.Count)
						prms = m_parameters.Length + 1;
					else
						prms = (int) (methTable [j]).ParamList;

					ParameterDefinition retparam = null;

					for (int k = (int) methRow.ParamList, l = 0; k < prms; k++) {
						ParamRow prow = paramTable [k - 1];
						if (prow.Sequence == 0) { // this is a fake parameter meaning a return type
							retparam = new ParameterDefinition (string.Empty, 0, (ParamAttributes) 0, null);
							retparam.Method = mdef;
							m_parameters [k - 1] = retparam;
							continue;
						}
						Param psig = msig.Parameters [l];
						ParameterDefinition pdef = BuildParameterDefinition (m_root.Streams.StringsHeap [prow.Name],
																			 prow.Sequence, prow.Flags, psig);
						pdef.Method = mdef;
						(mdef.Parameters as ParameterDefinitionCollection).Add (pdef);
						m_parameters [k - 1] = pdef; l++;
					}

					mdef.ReturnType = GetMethodReturnType (msig);
					MethodReturnType mrt = mdef.ReturnType as MethodReturnType;
					mrt.Method = mdef;
					if (retparam != null) {
						mrt.CustomAttributeOwner = retparam;
						mrt.CustomAttributeOwner.ParameterType = mrt.ReturnType;
					}

					m_meths [j - 1] = mdef;

					MethodDefinitionCollection methDefs = dec.Methods as MethodDefinitionCollection;

					methDefs.Loaded = true;
					methDefs.Add (mdef);
				}
			}

			int eprid = (int) m_reader.Image.CLIHeader.EntryPointToken & 0x00ffffff;
			if (eprid != 0)
				m_module.Assembly.EntryPoint = GetMethodDefAt (eprid);
		}

		private void ReadMemberReferences ()
		{
			if (!m_tHeap.HasTable (typeof (MemberRefTable)))
				return;

			MemberRefTable mrefTable = m_tHeap [typeof (MemberRefTable)] as MemberRefTable;
			m_memberRefs = new MemberReference [mrefTable.Rows.Count];
			for (int i = 0; i < mrefTable.Rows.Count; i++) {
				MemberRefRow mrefRow = mrefTable [i];

				MemberReference member = null;
				Signature sig = m_sigReader.GetMemberRefSig (mrefRow.Class.TokenType, mrefRow.Signature);
				switch (mrefRow.Class.TokenType) {
				case TokenType.TypeDef :
				case TokenType.TypeRef :
				case TokenType.TypeSpec :
					if (sig is FieldSig) {
						FieldSig fs = sig as FieldSig;
						member = new FieldReference (m_root.Streams.StringsHeap [mrefRow.Name], GetTypeDefOrRef (mrefRow.Class),
													 GetTypeRefFromSig (fs.Type));
					} else {
						MethodSig ms = sig as MethodSig;
						MethodReference methref = new MethodReference (m_root.Streams.StringsHeap [mrefRow.Name], GetTypeDefOrRef (mrefRow.Class),
													  ms.HasThis, ms.ExplicitThis, ms.MethCallConv);
						methref.ReturnType = GetMethodReturnType (ms);
						(methref.ReturnType as MethodReturnType).Method = methref;
						for (int j = 0; j < ms.ParamCount; j++) {
							Param p = ms.Parameters [j];
							ParameterDefinition pdef = BuildParameterDefinition (string.Concat ("arg", i), i, new ParamAttributes (), p);
							pdef.Method = methref;
							(methref.Parameters as ParameterDefinitionCollection).Add (pdef);
						}
						member = methref;
					}
					break;
				case TokenType.Method :
					// really not sure about this
					MethodDefinition methdef = GetMethodDefAt ((int) mrefRow.Class.RID);
					member = new MethodReference (methdef.Name, methdef.DeclaringType, methdef.HasThis,
																methdef.ExplicitThis, methdef.CallingConvention);
					break;
				case TokenType.ModuleRef :
					break; // implement that, or not
				}

				m_memberRefs [i] = member;
			}
		}

		public virtual void Visit (ITypeDefinition type)
		{
		}

		public virtual void Visit (ITypeReferenceCollection refs)
		{
		}

		public virtual void Visit (ITypeReference type)
		{
		}

		public virtual void Visit (IInterfaceCollection interfaces)
		{
		}

		public virtual void Visit (IExternTypeCollection externs)
		{
			ExternTypeCollection ext = externs as ExternTypeCollection;

			if (!m_tHeap.HasTable (typeof (ExportedTypeTable))) {
				ext.Loaded = true;
				return;
			}

			ExportedTypeTable etTable = m_tHeap [typeof (ExportedTypeTable)] as ExportedTypeTable;
			ITypeReference [] buffer = new ITypeReference [etTable.Rows.Count];

			for (int i = 0; i < etTable.Rows.Count; i++) {
				ExportedTypeRow etRow = etTable [i];
				if (etRow.Implementation.TokenType != TokenType.File)
					continue;

				string name = m_root.Streams.StringsHeap [etRow.TypeName];
				string ns = m_root.Streams.StringsHeap [etRow.TypeNamespace];
				if (ns.Length == 0)
					buffer [i] = Module.TypeReferences [name];
				else
					buffer [i] = Module.TypeReferences [string.Concat (ns, '.', name)];
			}

			for (int i = 0; i < etTable.Rows.Count; i++) {
				ExportedTypeRow etRow = etTable [i];
				if (etRow.Implementation.TokenType != TokenType.ExportedType)
					continue;

				ITypeReference owner = buffer [etRow.Implementation.RID - 1];
				string name = m_root.Streams.StringsHeap [etRow.TypeName];
				buffer [i] = Module.TypeReferences [string.Concat (owner.FullName, '/', name)];
			}

			for (int i = 0; i < buffer.Length; i++) {
				ITypeReference curs = buffer [i];
				if (curs != null)
					ext [curs.FullName] = curs;
			}
		}

		public virtual void Visit (IOverrideCollection meth)
		{
		}

		public virtual void Visit (INestedTypeCollection nestedTypes)
		{
		}

		public virtual void Visit (IParameterDefinitionCollection parameters)
		{
		}

		public virtual void Visit (IParameterDefinition parameter)
		{
		}

		public virtual void Visit (IMethodDefinitionCollection methods)
		{
		}

		public virtual void Visit (IMethodDefinition method)
		{
		}

		public virtual void Visit (IPInvokeInfo pinvk)
		{
		}

		public virtual void Visit (IEventDefinitionCollection events)
		{
		}

		public virtual void Visit (IEventDefinition evt)
		{
		}

		public virtual void Visit (IFieldDefinitionCollection fields)
		{
		}

		public virtual void Visit (IFieldDefinition field)
		{
		}

		public virtual void Visit (IPropertyDefinitionCollection properties)
		{
		}

		public virtual void Visit (IPropertyDefinition property)
		{
		}

		public virtual void Visit (ISecurityDeclarationCollection secDecls)
		{
		}

		public virtual void Visit (ISecurityDeclaration secDecl)
		{
		}

		public virtual void Visit (ICustomAttributeCollection customAttrs)
		{
		}

		public virtual void Visit (ICustomAttribute customAttr)
		{
		}

		public virtual void Visit (IMarshalSpec marshalSpec)
		{
		}

		public virtual void ReadSemantic (EventDefinition evt)
		{
		}

		public virtual void ReadSemantic (PropertyDefinition prop)
		{
		}

		public virtual void ReadMarshalSpec (ParameterDefinition param)
		{
		}

		public virtual void ReadMarshalSpec (FieldDefinition field)
		{
		}

		public virtual void ReadLayout (TypeDefinition type)
		{
		}

		public virtual void ReadLayout (FieldDefinition field)
		{
		}

		public virtual void ReadConstant (FieldDefinition field)
		{
		}

		public virtual void ReadConstant (PropertyDefinition prop)
		{
		}

		public virtual void ReadConstant (ParameterDefinition param)
		{
		}

		private object GetFixedArgValue (CustomAttrib.FixedArg fa)
		{
			if (fa.SzArray) {
				object [] vals = new object [fa.NumElem];
				for (int j = 0; j < vals.Length; j++)
					vals [j] = fa.Elems [j].Value;
				return vals;
			} else
				return fa.Elems [0].Value;
		}

		private ITypeReference GetFixedArgType (CustomAttrib.FixedArg fa)
		{
			if (fa.SzArray) {
				if (fa.NumElem == 0)
					return new ArrayType (SearchCoreType (Constants.Object));
				else
					return new ArrayType (fa.Elems [0].ElemType);
			} else
				return fa.Elems [0].ElemType;
		}

		protected CustomAttribute BuildCustomAttribute (IMethodReference ctor, CustomAttrib sig)
		{
			CustomAttribute cattr = new CustomAttribute (ctor);

			foreach (CustomAttrib.FixedArg fa in sig.FixedArgs)
				cattr.ConstructorParameters.Add (GetFixedArgValue (fa));

			foreach (CustomAttrib.NamedArg na in sig.NamedArgs) {
				object value = GetFixedArgValue (na.FixedArg);
				if (na.Field) {
					cattr.Fields [na.FieldOrPropName] = value;
					cattr.SetFieldType (na.FieldOrPropName, GetFixedArgType (na.FixedArg));
				} else if (na.Property) {
					cattr.Properties [na.FieldOrPropName] = value;
					cattr.SetPropertyType (na.FieldOrPropName, GetFixedArgType (na.FixedArg));
				} else
					throw new ReflectionException ("Non valid named arg");
			}

			return cattr;
		}

		protected ParameterDefinition BuildParameterDefinition (string name, int sequence, ParamAttributes attrs, Param psig)
		{
			ParameterDefinition ret = new ParameterDefinition (name, sequence, attrs, null);
			ITypeReference paramType = null;

			if (psig.ByRef)
				paramType = new ReferenceType (GetTypeRefFromSig (psig.Type));
			else if (psig.TypedByRef)
				paramType = SearchCoreType (Constants.TypedReference);
			else
				paramType = GetTypeRefFromSig (psig.Type);

			if (psig.CustomMods.Length > 0)
				paramType = GetModifierType (psig.CustomMods, paramType);

			ret.ParameterType = paramType;

			return ret;
		}

		protected SecurityDeclaration BuildSecurityDeclaration (DeclSecurityRow dsRow)
		{
			if (m_root.Header.MajorVersion < 2) {
				return BuildSecurityDeclaration (dsRow.Action, m_root.Streams.BlobHeap.Read (dsRow.PermissionSet));
			} //TODO: else

			return null;
		}

		public SecurityDeclaration BuildSecurityDeclaration (Mono.Cecil.SecurityAction action, byte [] permset)
		{
			SecurityDeclaration dec = new SecurityDeclaration (action);
			PermissionSet ps = new PermissionSet (PermissionState.None);
			m_secParser.Reset ();
			m_secParser.LoadXml (Encoding.Unicode.GetString (permset));
			ps.FromXml (m_secParser.ToXml ());
			dec.PermissionSet = ps;
			return dec;
		}

		protected MarshalDesc BuildMarshalDesc (MarshalSig ms, IHasMarshalSpec container)
		{
			if (ms.Spec is MarshalSig.Array) {
				ArrayMarshalDesc amd = new ArrayMarshalDesc (container);
				MarshalSig.Array ar = (MarshalSig.Array) ms.Spec;
				amd.ElemType = ar.ArrayElemType;
				amd.NumElem = ar.NumElem;
				amd.ParamNum = ar.ParamNum;
				amd.ElemMult = ar.ElemMult;
				return amd;
			} else if (ms.Spec is MarshalSig.CustomMarshaler) {
				CustomMarshalerDesc cmd = new CustomMarshalerDesc (container);
				MarshalSig.CustomMarshaler cmsig = (MarshalSig.CustomMarshaler) ms.Spec;
				cmd.Guid = new Guid (cmsig.Guid);
				cmd.UnmanagedType = cmsig.UnmanagedType;
				cmd.ManagedType = this.Module.Types [cmsig.ManagedType];
				cmd.Cookie = cmsig.Cookie;
				return cmd;
			} else if (ms.Spec is MarshalSig.FixedArray) {
				FixedArrayDesc fad = new FixedArrayDesc (container);
				MarshalSig.FixedArray fasig = (MarshalSig.FixedArray) ms.Spec;
				fad.ElemType = fasig.ArrayElemType;
				fad.NumElem = fasig.NumElem;
				return fad;
			} else if (ms.Spec is MarshalSig.FixedSysString) {
				FixedSysStringDesc fssc = new FixedSysStringDesc (container);
				fssc.Size = ((MarshalSig.FixedSysString) ms.Spec).Size;
				return fssc;
			} else if (ms.Spec is MarshalSig.SafeArray) {
				SafeArrayDesc sad = new SafeArrayDesc (container);
				sad.ElemType = ((MarshalSig.SafeArray) ms.Spec).ArrayElemType;
				return sad;
			} else {
				return new MarshalDesc (ms.NativeInstrinsic, container);
			}
		}

		protected ITypeReference GetModifierType (CustomMod [] cmods, ITypeReference type)
		{
			ITypeReference ret = type;
			for (int i = cmods.Length - 1; i >= 0; i--) {
				CustomMod cmod = cmods [i];
				ITypeReference modType = null;

				if (cmod.TypeDefOrRef.TokenType == TokenType.TypeDef)
					modType = GetTypeDefAt ((int) cmod.TypeDefOrRef.RID);
				else
					modType = GetTypeRefAt ((int) cmod.TypeDefOrRef.RID);

				if (cmod.CMOD == CustomMod.CMODType.OPT)
					ret = new ModifierOptional (ret, modType);
				else if (cmod.CMOD == CustomMod.CMODType.REQD)
					ret = new ModifierRequired (ret, modType);
			}
			return ret;
		}

		private MethodReturnType GetMethodReturnType (MethodSig msig)
		{
			ITypeReference retType = null;
			if (msig.RetType.Void)
				retType = SearchCoreType (Constants.Void);
			else if (msig.RetType.ByRef)
				retType = new ReferenceType (GetTypeRefFromSig (msig.RetType.Type));
			else if (msig.RetType.TypedByRef)
				retType = SearchCoreType (Constants.TypedReference);
			else
				retType = GetTypeRefFromSig (msig.RetType.Type);

			if (msig.RetType.CustomMods.Length > 0)
				retType = GetModifierType (msig.RetType.CustomMods, retType);

			return new MethodReturnType (retType);
		}

		public ITypeReference GetTypeRefFromSig (SigType t)
		{
			switch (t.ElementType) {
			case ElementType.Class :
				CLASS c = t as CLASS;
				return GetTypeDefOrRef (c.Type);
			case ElementType.ValueType :
				VALUETYPE vt = t as VALUETYPE;
				return GetTypeDefOrRef (vt.Type);
			case ElementType.String :
				return SearchCoreType (Constants.String);
			case ElementType.Object :
				return SearchCoreType (Constants.Object);
			case ElementType.Void :
				return SearchCoreType (Constants.Void);
			case ElementType.Boolean :
				return SearchCoreType (Constants.Boolean);
			case ElementType.Char :
				return SearchCoreType (Constants.Char);
			case ElementType.I1 :
				return SearchCoreType (Constants.SByte);
			case ElementType.U1 :
				return SearchCoreType (Constants.Byte);
			case ElementType.I2 :
				return SearchCoreType (Constants.Int16);
			case ElementType.U2 :
				return SearchCoreType (Constants.UInt16);
			case ElementType.I4 :
				return SearchCoreType (Constants.Int32);
			case ElementType.U4 :
				return SearchCoreType (Constants.UInt32);
			case ElementType.I8 :
				return SearchCoreType (Constants.Int64);
			case ElementType.U8 :
				return SearchCoreType (Constants.UInt64);
			case ElementType.R4 :
				return SearchCoreType (Constants.Single);
			case ElementType.R8 :
				return SearchCoreType (Constants.Double);
			case ElementType.I :
				return SearchCoreType (Constants.IntPtr);
			case ElementType.U :
				return SearchCoreType (Constants.UIntPtr);
			case ElementType.TypedByRef :
				return SearchCoreType (Constants.TypedReference);
			case ElementType.Array :
				ARRAY ary = t as ARRAY;
				return new ArrayType (GetTypeRefFromSig (ary.Type), ary.Shape);
			case ElementType.SzArray :
				SZARRAY szary = t as SZARRAY;
				ArrayType at = new ArrayType (GetTypeRefFromSig (szary.Type));
				(at.Dimensions as ArrayDimensionCollection).Add (new ArrayDimension (0, 0));
				return at;
			case ElementType.Ptr :
				PTR pointer = t as PTR;
				if (pointer.Void)
					return new PointerType (SearchCoreType (Constants.Void));
				return new PointerType (GetTypeRefFromSig (pointer.PtrType));
			case ElementType.FnPtr :
				// not very sure of this
				FNPTR funcptr = t as FNPTR;
				ParameterDefinitionCollection parameters = new ParameterDefinitionCollection (null);
				for (int i = 0; i < funcptr.Method.ParamCount; i++) {
					Param p = funcptr.Method.Parameters [i];
					ParameterDefinition pdef = new ParameterDefinition (string.Concat ("arg", i), i, new ParamAttributes (), null);
					pdef.ParameterType = p.ByRef ? new ReferenceType (GetTypeRefFromSig (p.Type)) : GetTypeRefFromSig (p.Type);
					parameters.Add (pdef);
				}
				return new FunctionPointerType (funcptr.Method.HasThis, funcptr.Method.ExplicitThis, funcptr.Method.MethCallConv,
											parameters, GetMethodReturnType (funcptr.Method));
			}
			return null;
		}

		protected object GetConstant (uint pos, ElementType elemType)
		{
			BinaryReader br = m_root.Streams.BlobHeap.GetReader (pos);

			switch (elemType) {
			case ElementType.Boolean :
				return br.ReadByte () == 1;
			case ElementType.Char :
				return (char) br.ReadUInt16 ();
			case ElementType.I1 :
				return br.ReadSByte ();
			case ElementType.I2 :
				return br.ReadInt16 ();
			case ElementType.I4 :
				return br.ReadInt32 ();
			case ElementType.I8 :
				return br.ReadInt64 ();
			case ElementType.U1 :
				return br.ReadByte ();
			case ElementType.U2 :
				return br.ReadUInt16 ();
			case ElementType.U4 :
				return br.ReadUInt32 ();
			case ElementType.U8 :
				return br.ReadUInt64 ();
			case ElementType.R4 :
				return br.ReadSingle ();
			case ElementType.R8 :
				return br.ReadDouble ();
			case ElementType.String :
				int next, length = Utilities.ReadCompressedInteger (m_root.Streams.BlobHeap.Data, (int) br.BaseStream.Position, out next);
				br.BaseStream.Position = next;
				return Encoding.UTF8.GetString (br.ReadBytes (length));
			case ElementType.Class :
				return null;
			default :
				throw new ReflectionException ("Non valid element in constant table");
			}
		}
	}
}
