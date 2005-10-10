//
// ReflectionReader.cs
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
	using System.Security;
	using System.Security.Permissions;
	using System.Text;

	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Cecil.Metadata;
	using Mono.Cecil.Signatures;

	using Mono.Xml;

	internal abstract class ReflectionReader : BaseReflectionReader {

		ModuleDefinition m_module;
		ImageReader m_reader;
		protected SecurityParser m_secParser;
		protected MetadataRoot m_root;
		protected TablesHeap m_tHeap;

		protected TypeDefinition [] m_typeDefs;
		protected TypeReference [] m_typeRefs;
		protected TypeReference [] m_typeSpecs;
		protected MethodDefinition [] m_meths;
		protected FieldDefinition [] m_fields;
		protected EventDefinition [] m_events;
		protected PropertyDefinition [] m_properties;
		protected MemberReference [] m_memberRefs;
		protected ParameterDefinition [] m_parameters;

		IAssemblyNameReference m_corlib;

		protected SignatureReader m_sigReader;
		protected CodeReader m_codeReader;

		bool m_isCorlib;

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

		public MemberReference [] MemberReferences {
			get { return m_memberRefs; }
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
			m_isCorlib = module.Assembly.Name.Name == Constants.Corlib;
		}

		public TypeDefinition GetTypeDefAt (uint rid)
		{
			return m_typeDefs [rid - 1];
		}

		public TypeReference GetTypeRefAt (uint rid)
		{
			return m_typeRefs [rid - 1];
		}

		public TypeReference GetTypeSpecAt (uint rid)
		{
			return m_typeSpecs [rid - 1];
		}

		public FieldDefinition GetFieldDefAt (uint rid)
		{
			return m_fields [rid - 1];
		}

		public MethodDefinition GetMethodDefAt (uint rid)
		{
			return m_meths [rid - 1];
		}

		public MemberReference GetMemberRefAt (uint rid)
		{
			return m_memberRefs [rid - 1];
		}

		public PropertyDefinition GetPropertyDefAt (uint rid)
		{
			return m_properties [rid - 1];
		}

		public EventDefinition GetEventDefAt (uint rid)
		{
			return m_events [rid - 1];
		}

		public ParameterDefinition GetParamDefAt (uint rid)
		{
			return m_parameters [rid - 1];
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

		public TypeReference GetTypeDefOrRef (MetadataToken token)
		{
			if (token.RID == 0)
				return null;

			switch (token.TokenType) {
			case TokenType.TypeDef :
				return GetTypeDefAt (token.RID);
			case TokenType.TypeRef :
				return GetTypeRefAt (token.RID);
			case TokenType.TypeSpec :
				return GetTypeSpecAt (token.RID);
			default :
				return null;
			}
		}

		public TypeReference SearchCoreType (string fullName)
		{
			if (m_isCorlib)
				return m_module.Types [fullName] as TypeReference;

			TypeReference coreType =  m_module.TypeReferences [fullName] as TypeReference;
			if (coreType == null) {
				if (m_corlib == null) {
					foreach (IAssemblyNameReference ar in m_module.AssemblyReferences) {
						if (ar.Name == Constants.Corlib)
							m_corlib = ar;
						break;
					}
				}

				string [] parts = fullName.Split ('.');
				if (parts.Length != 2)
					throw new ReflectionException ("Unvalid core type name");
				coreType = new TypeReference (parts [1], parts [0], m_corlib);
				m_module.TypeReferences.Add (coreType);
			}
			return coreType;
		}

		public IMetadataTokenProvider LookupByToken (MetadataToken token)
		{
			switch (token.TokenType) {
			case TokenType.TypeDef :
				return GetTypeDefAt (token.RID);
			case TokenType.TypeRef :
				return GetTypeRefAt (token.RID);
			case TokenType.TypeSpec :
				return GetTypeSpecAt (token.RID);
			case TokenType.Method :
				return GetMethodDefAt (token.RID);
			case TokenType.Field :
				return GetFieldDefAt (token.RID);
			case TokenType.Event :
				return GetEventDefAt (token.RID);
			case TokenType.Property :
				return GetPropertyDefAt (token.RID);
			case TokenType.Param :
				return GetParamDefAt (token.RID);
			case TokenType.MemberRef :
				return GetMemberRefAt (token.RID);
			default :
				throw new NotSupportedException ("Lookup is not allowed on this kind of token");
			}
		}

		public CustomAttribute GetCustomAttribute (MethodReference ctor, byte [] data)
		{
			CustomAttrib sig = m_sigReader.GetCustomAttrib (data, ctor);
			return BuildCustomAttribute (ctor, sig);
		}

		public override void VisitModuleDefinition (ModuleDefinition mod)
		{
			VisitTypeDefinitionCollection (mod.Types);
		}

		public override void VisitTypeDefinitionCollection (TypeDefinitionCollection types)
		{
			// type def reading
			TypeDefTable typesTable = m_tHeap [typeof (TypeDefTable)] as TypeDefTable;
			m_typeDefs = new TypeDefinition [typesTable.Rows.Count];
			for (int i = 0; i < typesTable.Rows.Count; i++) {
				TypeDefRow type = typesTable [i];
				TypeDefinition t = new TypeDefinition (
					m_root.Streams.StringsHeap [type.Name],
					m_root.Streams.StringsHeap [type.Namespace],
					type.Flags);
				t.MetadataToken = MetadataToken.FromMetadataRow (TokenType.TypeDef, i);

				m_typeDefs [i] = t;
			}

			// nested types
			NestedClassTable nested = m_tHeap [typeof (NestedClassTable)] as NestedClassTable;
			if (m_tHeap.HasTable (typeof(NestedClassTable))) {
				for (int i = 0; i < nested.Rows.Count; i++) {
					NestedClassRow row = nested [i];

					TypeDefinition parent = GetTypeDefAt (row.EnclosingClass);
					TypeDefinition child = GetTypeDefAt (row.NestedClass);

					parent.NestedTypes.Add (child);
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
					case TokenType.Module :
						scope = m_module.Assembly.Modules [(int) type.ResolutionScope.RID - 1];
						break;
					case TokenType.TypeRef :
						parent = GetTypeRefAt (type.ResolutionScope.RID);
						scope = parent.Scope;
						break;
					}

					TypeReference t = new TypeReference (
						m_root.Streams.StringsHeap [type.Name],
						m_root.Streams.StringsHeap [type.Namespace],
						scope);
					t.MetadataToken = MetadataToken.FromMetadataRow (TokenType.TypeRef, i);

					if (parent != null)
						t.DeclaringType = parent;

					m_typeRefs [i] = t;
					m_module.TypeReferences.Add (t);
				}

			} else
				m_typeRefs = new TypeReference [0];

			for (int i = 0; i < m_typeDefs.Length; i++) {
				TypeDefinition type = m_typeDefs [i];
				types.Add (type);
			}

			for (int i = 0; i < m_typeRefs.Length; i++) {
				TypeReference type = m_typeRefs [i];
				m_module.TypeReferences.Add (type);
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

		void ReadTypeSpecs ()
		{
			if (!m_tHeap.HasTable (typeof (TypeSpecTable)))
				return;

			TypeSpecTable tsTable = m_tHeap [typeof (TypeSpecTable)] as TypeSpecTable;
			m_typeSpecs = new TypeReference [tsTable.Rows.Count];
			for (int i = 0; i < tsTable.Rows.Count; i++) {
				TypeSpecRow tsRow = tsTable [i];
				TypeSpec ts = m_sigReader.GetTypeSpec (tsRow.Signature);
				TypeReference tspec = GetTypeRefFromSig (ts.Type);
				tspec.MetadataToken = MetadataToken.FromMetadataRow (TokenType.TypeSpec, i);
				m_typeSpecs [i] = tspec;
			}
		}

		void ReadAllFields ()
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
					FieldDefinition fdef = new FieldDefinition (
						m_root.Streams.StringsHeap [frow.Name],
						this.GetTypeRefFromSig (fsig.Type), frow.Flags);
					fdef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Field, j - 1);

					if (fsig.CustomMods.Length > 0)
						fdef.FieldType = GetModifierType (fsig.CustomMods, fdef.FieldType);

					dec.Fields.Add (fdef);
					m_fields [j - 1] = fdef;
				}
			}
		}

		void ReadAllMethods ()
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

					MethodDefinition mdef = new MethodDefinition (
						m_root.Streams.StringsHeap [methRow.Name],
						methRow.RVA, methRow.Flags, methRow.ImplFlags,
						msig.HasThis, msig.ExplicitThis, msig.MethCallConv);
					mdef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Method, j - 1);

					int prms;
					if (j == methTable.Rows.Count)
						prms = m_parameters.Length + 1;
					else
						prms = (int) (methTable [j]).ParamList;

					ParameterDefinition retparam = null;

					//TODO: optimize this
					ParamRow pRow = null;
					int start = (int) methRow.ParamList - 1;

					if (paramTable != null && start < prms - 1)
						pRow = paramTable [start];

					if (pRow != null && pRow.Sequence == 0) { // ret type
						retparam = new ParameterDefinition (string.Empty, 0, (ParamAttributes) 0, null);
						retparam.Method = mdef;
						m_parameters [start] = retparam;
						start++;
					}

					for (int k = 0; k < msig.ParamCount; k++) {

						int pointer = start + k;

						if (paramTable != null && pointer < prms - 1)
							pRow = paramTable [pointer];

						Param psig = msig.Parameters [k];

						ParameterDefinition pdef;
						if (pRow != null) {
							pdef = BuildParameterDefinition (
								m_root.Streams.StringsHeap [pRow.Name],
								pRow.Sequence, pRow.Flags, psig);
							pdef.MetadataToken = MetadataToken.FromMetadataRow (TokenType.Param, pointer);
							m_parameters [pointer] = pdef;
						} else
							pdef = BuildParameterDefinition (
								string.Concat ("A_", mdef.IsStatic ? k : k + 1),
								k + 1, (ParamAttributes) 0, psig);

						pdef.Method = mdef;
						mdef.Parameters.Add (pdef);

					}

					mdef.ReturnType = GetMethodReturnType (msig);
					MethodReturnType mrt = mdef.ReturnType as MethodReturnType;
					mrt.Method = mdef;
					if (retparam != null) {
						mrt.Parameter = retparam;
						mrt.Parameter.ParameterType = mrt.ReturnType;
					}

					m_meths [j - 1] = mdef;

					if (mdef.IsConstructor)
						dec.Constructors.Add (mdef);
					else
						dec.Methods.Add (mdef);
				}
			}

			uint eprid = m_reader.Image.CLIHeader.EntryPointToken & 0x00ffffff;
			if (eprid != 0)
				m_module.Assembly.EntryPoint = GetMethodDefAt (eprid);
		}

		void ReadMemberReferences ()
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
						member = new FieldReference (
							m_root.Streams.StringsHeap [mrefRow.Name], GetTypeRefFromSig (fs.Type));
						member.DeclaringType = GetTypeDefOrRef (mrefRow.Class);
					} else {
						MethodSig ms = sig as MethodSig;
						MethodReference methref = new MethodReference (
							m_root.Streams.StringsHeap [mrefRow.Name],
							ms.HasThis, ms.ExplicitThis, ms.MethCallConv);
						methref.DeclaringType = GetTypeDefOrRef (mrefRow.Class);
						methref.ReturnType = GetMethodReturnType (ms);
						(methref.ReturnType as MethodReturnType).Method = methref;
						for (int j = 0; j < ms.ParamCount; j++) {
							Param p = ms.Parameters [j];
							ParameterDefinition pdef = BuildParameterDefinition (
								string.Concat ("arg", i), i, new ParamAttributes (), p);
							pdef.Method = methref;
							methref.Parameters.Add (pdef);
						}
						member = methref;
					}
					break;
				case TokenType.Method :
					// really not sure about this
					MethodDefinition methdef = GetMethodDefAt (mrefRow.Class.RID);
					member = new MethodReference (
						methdef.Name, methdef.HasThis,
						methdef.ExplicitThis, methdef.CallingConvention);
					member.DeclaringType = methdef.DeclaringType;
					break;
				case TokenType.ModuleRef :
					break; // TODO, implement that, or not
				}

				member.MetadataToken = MetadataToken.FromMetadataRow (TokenType.MemberRef, i);
				m_module.MemberReferences.Add (member);
				m_memberRefs [i] = member;
			}
		}

		public override void VisitExternTypeCollection (ExternTypeCollection externs)
		{
			ExternTypeCollection ext = externs as ExternTypeCollection;

			if (!m_tHeap.HasTable (typeof (ExportedTypeTable)))
				return;

			ExportedTypeTable etTable = m_tHeap [typeof (ExportedTypeTable)] as ExportedTypeTable;
			TypeReference [] buffer = new TypeReference [etTable.Rows.Count];

			for (int i = 0; i < etTable.Rows.Count; i++) {
				ExportedTypeRow etRow = etTable [i];
				if (etRow.Implementation.TokenType != TokenType.File)
					continue;

				string name = m_root.Streams.StringsHeap [etRow.TypeName];
				string ns = m_root.Streams.StringsHeap [etRow.TypeNamespace];
				if (ns.Length == 0)
					buffer [i] = m_module.TypeReferences [name];
				else
					buffer [i] = m_module.TypeReferences [string.Concat (ns, '.', name)];
			}

			for (int i = 0; i < etTable.Rows.Count; i++) {
				ExportedTypeRow etRow = etTable [i];
				if (etRow.Implementation.TokenType != TokenType.ExportedType)
					continue;

				ITypeReference owner = buffer [etRow.Implementation.RID - 1];
				string name = m_root.Streams.StringsHeap [etRow.TypeName];
				buffer [i] = m_module.TypeReferences [string.Concat (owner.FullName, '/', name)];
			}

			for (int i = 0; i < buffer.Length; i++) {
				TypeReference curs = buffer [i];
				if (curs != null)
					ext.Add (curs);
			}
		}

		object GetFixedArgValue (CustomAttrib.FixedArg fa)
		{
			if (fa.SzArray) {
				object [] vals = new object [fa.NumElem];
				for (int j = 0; j < vals.Length; j++)
					vals [j] = fa.Elems [j].Value;
				return vals;
			} else
				return fa.Elems [0].Value;
		}

		TypeReference GetFixedArgType (CustomAttrib.FixedArg fa)
		{
			if (fa.SzArray) {
				if (fa.NumElem == 0)
					return new ArrayType (SearchCoreType (Constants.Object));
				else
					return new ArrayType (fa.Elems [0].ElemType);
			} else
				return fa.Elems [0].ElemType;
		}

		protected CustomAttribute BuildCustomAttribute (MethodReference ctor, CustomAttrib sig)
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

		protected ParameterDefinition BuildParameterDefinition (string name, int sequence,
			ParamAttributes attrs, Param psig)
		{
			ParameterDefinition ret = new ParameterDefinition (name, sequence, attrs, null);
			TypeReference paramType = null;

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
			m_secParser.LoadXml (Encoding.Unicode.GetString (permset));
			try {
				ps.FromXml (m_secParser.ToXml ());
			} catch {
			}
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
				cmd.Guid = cmsig.Guid.Length > 0 ? new Guid (cmsig.Guid) : new Guid ();
				cmd.UnmanagedType = cmsig.UnmanagedType;
				cmd.ManagedType = m_module.Types [cmsig.ManagedType];
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

		protected TypeReference GetModifierType (CustomMod [] cmods, TypeReference type)
		{
			TypeReference ret = type;
			for (int i = cmods.Length - 1; i >= 0; i--) {
				CustomMod cmod = cmods [i];
				TypeReference modType = null;

				if (cmod.TypeDefOrRef.TokenType == TokenType.TypeDef)
					modType = GetTypeDefAt (cmod.TypeDefOrRef.RID);
				else
					modType = GetTypeRefAt (cmod.TypeDefOrRef.RID);

				if (cmod.CMOD == CustomMod.CMODType.OPT)
					ret = new ModifierOptional (ret, modType);
				else if (cmod.CMOD == CustomMod.CMODType.REQD)
					ret = new ModifierRequired (ret, modType);
			}
			return ret;
		}

		MethodReturnType GetMethodReturnType (MethodSig msig)
		{
			TypeReference retType = null;
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

		public TypeReference GetTypeRefFromSig (SigType t)
		{
			switch (t.ElementType) {
			case ElementType.Class :
				CLASS c = t as CLASS;
				return GetTypeDefOrRef (c.Type);
			case ElementType.ValueType :
				VALUETYPE vt = t as VALUETYPE;
				TypeReference vtr = GetTypeDefOrRef (vt.Type);
				vtr.IsValueType = true;
				return vtr;
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
					ParameterDefinition pdef = new ParameterDefinition (p.ByRef ? new ReferenceType (
							GetTypeRefFromSig (p.Type)) : GetTypeRefFromSig (p.Type));
					parameters.Add (pdef);
				}
				return new FunctionPointerType (funcptr.Method.HasThis, funcptr.Method.ExplicitThis,
					funcptr.Method.MethCallConv,
					parameters, GetMethodReturnType (funcptr.Method));
			case ElementType.Var:
				VAR var = t as VAR;
				return new TypeParameterType (var.Index);
			case ElementType.MVar:
				MVAR mvar = t as MVAR;
				return new MethodTypeParameterType (mvar.Index);
			case ElementType.GenericInst:
				GENERICINST ginst = t as GENERICINST;
				TypeReference[] ginst_argv = new TypeReference [ginst.Signature.Arity];
				for (int i = 0; i < ginst.Signature.Arity; i++)
					ginst_argv [i] = GetTypeRefFromSig (ginst.Signature.Types [i]);
				return new GenericInstType (GetTypeRefFromSig (ginst.Type), ginst_argv);
			default:
				break;
			}
			return null;
		}

		protected object GetConstant (uint pos, ElementType elemType)
		{
			byte [] constant = m_root.Streams.BlobHeap.Read (pos);
			BinaryReader br = new BinaryReader (new MemoryStream (constant));

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
				string str = Encoding.Unicode.GetString (br.ReadBytes (constant.Length));
				return str;
			case ElementType.Class :
				return null;
			default :
				throw new ReflectionException ("Non valid element in constant table");
			}
		}

		protected void SetInitialValue (FieldDefinition field)
		{
			if (field.RVA != RVA.Zero) {
				BinaryReader br = this.Module.ImageReader.MetadataReader.GetDataReader (field.RVA);
				field.InitialValue = br.ReadBytes (
					(int) (field.FieldType as ITypeDefinition).LayoutInfo.ClassSize);
			} else
				field.InitialValue = new byte [0];
		}
	}
}
