/*
 * Copyright (c) 2004 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jb.evain@dotnetguru.org)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Implem {

    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Metadata;
    using Mono.Cecil.Signatures;
    using Mono.Xml;

    internal abstract class ReflectionReader : IReflectionVisitor, IMethodsReader {

        private ModuleDefinition m_module;
        private ImageReader m_reader;
        protected SignatureReader m_sigReader;
        protected SecurityParser m_secParser;
        protected MetadataRoot m_root;

        protected TypeDefinition [] m_types;
        protected TypeReference [] m_refs;
        protected MethodDefinition [] m_meths;
        protected FieldDefinition [] m_fields;
        protected EventDefinition [] m_events;
        protected PropertyDefinition [] m_properties;

        private bool m_isCorlib;

        public ModuleDefinition Module {
            get { return m_module; }
        }

        public SignatureReader SigReader {
            get { return m_sigReader; }
        }

        public MetadataRoot MetadataRoot {
            get { return m_root; }
        }

        public ReflectionReader (ModuleDefinition module)
        {
            m_module = module;
            m_reader = m_module.Reader;
            m_root = m_reader.Image.MetadataRoot;
            m_sigReader = new SignatureReader (m_root);
            m_secParser = new SecurityParser ();
            m_isCorlib = m_reader.Image.FileInformation.Name == "mscorlib.dll";
        }

        public TypeDefinition GetTypeDefAt (int rid)
        {
            // 0 based - <Module> type
            return m_types [rid - 2];
        }

        public TypeReference GetTypeRefAt (int rid)
        {
            // 0 based
            return m_refs [rid - 1];
        }

        public FieldDefinition GetFieldDefAt (int rid)
        {
            return m_fields [rid - 1];
        }

        public MethodDefinition GetMethodDefAt (int rid)
        {
            return m_meths [rid - 1];
        }

        public int GetRidForMethodDef (MethodDefinition meth)
        {
            int index = Array.IndexOf (m_meths, meth);
            if (index == -1)
                return 0;

            return index + 1;
        }

        public int GetRidForTypeDef (TypeDefinition typeDef)
        {
            int index = Array.IndexOf (m_types, typeDef);
            if (index == -1)
                return 0;

            return index + 2;
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
                TypeSpecTable tsTable = m_root.Streams.TablesHeap [typeof (TypeSpecTable)] as TypeSpecTable;
                TypeSpecRow tsRow = tsTable [(int) token.RID];
                TypeSpec ts = m_sigReader.GetTypeSpec (tsRow.Signature);
                return this.GetTypeRefFromSig (ts.Type);
            default :
                return null;
            }
        }

        private ITypeReference SearchCoreType (string fullName)
        {
            if (m_isCorlib)
                return m_module.Types [fullName];

            return m_module.TypeReferences [fullName];
        }

        public virtual void Visit (ITypeDefinitionCollection types)
        {
            TypeDefinitionCollection tdc = types as TypeDefinitionCollection;
            if (tdc != null && tdc.Loaded)
                return;

            ModuleDefinition def = tdc.Container as ModuleDefinition;

            // type def reading
            TypeDefTable typesTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            m_types = new TypeDefinition [typesTable.Rows.Count - 1];
            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable [i];
                TypeDefinition t = new TypeDefinition (
                    m_root.Streams.StringsHeap [type.Name],
                    m_root.Streams.StringsHeap [type.Namespace],
                    type.Flags, def);

                m_types [i - 1] = t;
            }

            // nested types
            NestedClassTable nested = m_root.Streams.TablesHeap [typeof (NestedClassTable)] as NestedClassTable;
            if (m_root.Streams.TablesHeap.HasTable (typeof(NestedClassTable))) {
                for (int i = 0; i < nested.Rows.Count; i++) {
                    NestedClassRow row = nested [i];

                    TypeDefinition parent = GetTypeDefAt ((int) row.EnclosingClass);
                    TypeDefinition child = GetTypeDefAt ((int) row.NestedClass);

                    child.DeclaringType = parent;
                }
            }

            // type ref reading
            if (m_root.Streams.TablesHeap.HasTable(typeof (TypeRefTable))) {
                TypeRefTable typesRef = m_root.Streams.TablesHeap [typeof (TypeRefTable)] as TypeRefTable;

                m_refs = new TypeReference [typesRef.Rows.Count];

                for (int i = 0; i < typesRef.Rows.Count; i++) {
                    TypeRefRow type = typesRef [i];
                    TypeReference t = new TypeReference (
                        m_root.Streams.StringsHeap [type.Name],
                        m_root.Streams.StringsHeap [type.Namespace]);

                    m_refs [i] = t;
                    ((ModuleDefinition)tdc.Container).TypeReferences [t.FullName] = t;
                }
            } else
                m_refs = new TypeReference [0];

            // set base types
            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable [i];
                TypeDefinition child = m_types [i - 1];
                child.BaseType = GetTypeDefOrRef (type.Extends);
            }

            for (int i = 0; i < m_types.Length; i++) {
                TypeDefinition type = m_types [i];
                tdc [type.FullName] = type;
            }

            tdc.Loaded = true;

            // ok, I've thought a lot before doing that
            // if I do not load the two primitives that are field and methods here
            // i'll run into big troubles as soon a lazy loaded stuff reference it
            // such as methods body or an override collection
            ReadAllFields ();
            ReadAllMethods ();
        }

        private void ReadAllFields ()
        {
            TypeDefTable tdefTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            if (!m_root.Streams.TablesHeap.HasTable(typeof (FieldTable))) {
                m_fields = new FieldDefinition [0];
                return;
            }

            FieldTable fldTable = m_root.Streams.TablesHeap [typeof (FieldTable)] as FieldTable;
            m_fields = new FieldDefinition [fldTable.Rows.Count];

            for (int i = 0; i < m_types.Length; i++) {
                TypeDefinition dec = m_types [i];

                int index = i + 1, next; // avoid a call to GetRidForTypeDef

                if (index == tdefTable.Rows.Count - 1)
                    next = fldTable.Rows.Count + 1;
                else
                    next = (int) (tdefTable [index + 1]).FieldList;

                for (int j = (int) tdefTable [index].FieldList; j < next; j++) {
                    FieldRow frow = fldTable [j - 1];
                    FieldSig fsig = m_sigReader.GetFieldSig (frow.Signature);
                    FieldDefinition fdef = new FieldDefinition (m_root.Streams.StringsHeap [frow.Name],
                                                                dec, this.GetTypeRefFromSig (fsig.Type), frow.Flags);
                    dec.Fields [fdef.Name] = fdef;
                    ((FieldDefinitionCollection)dec.Fields).Loaded = true;
                    m_fields [j - 1] = fdef;
                }
            }
        }

        private void ReadAllMethods ()
        {
            TypeDefTable tdefTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;

            if (!m_root.Streams.TablesHeap.HasTable(typeof (MethodTable))) {
                m_meths = new MethodDefinition [0];
                return;
            }

            MethodTable methTable = m_root.Streams.TablesHeap [typeof (MethodTable)] as MethodTable;
            ParamTable paramTable = m_root.Streams.TablesHeap [typeof (ParamTable)] as ParamTable;
            m_meths = new MethodDefinition [methTable.Rows.Count];

            for (int i = 0; i < m_types.Length; i++) {
                TypeDefinition dec = m_types [i];

                int index = i + 1, next; // avoid a call to GetRidForTypeDef

                if (index == tdefTable.Rows.Count - 1)
                    next = methTable.Rows.Count + 1;
                else
                    next = (int) (tdefTable [index + 1]).MethodList;

                for (int j = (int) tdefTable [index].MethodList; j < next; j++) {
                    MethodRow methRow = methTable [j - 1];
                    MethodSig msig = m_sigReader.GetMethodDefSig (methRow.Signature);
                    MethodDefinition mdef = new MethodDefinition (m_root.Streams.StringsHeap [methRow.Name], dec,
                                                                  methRow.RVA, methRow.Flags, methRow.ImplFlags,
                                                                  msig.HasThis, msig.ExplicitThis, msig.MethCallConv);

                    for (int k = 0, l = (int) methRow.ParamList; k < msig.ParamCount; k++) {
                        ParamRow prow = paramTable [l - 1]; l++;
                        Param psig = msig.Parameters [k];

                        ParameterDefinition pdef = new ParameterDefinition (m_root.Streams.StringsHeap [prow.Name], prow.Sequence, prow.Flags, null);
                        pdef.ParameterType = psig.ByRef ? new ReferenceType (GetTypeRefFromSig (psig.Type)) : GetTypeRefFromSig (psig.Type);
                        mdef.Parameters.Add (pdef);
                    }

                    mdef.ReturnType = GetMethodReturnType (msig);
                    m_meths [j - 1] = mdef;
                    ((MethodDefinitionCollection)dec.Methods).Loaded = true;
                    dec.Methods.Add (mdef);
                }
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

        public virtual void Visit (IOverrideCollection meth)
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

        public virtual void ReadMethods (EventDefinition evt)
        {
        }

        public virtual void ReadMethods (PropertyDefinition prop)
        {
        }

        protected SecurityDeclaration BuildSecurityDeclaration (DeclSecurityRow dsRow)
        {
            SecurityDeclaration dec = new SecurityDeclaration (dsRow.Action);
            if (m_root.Header.MajorVersion < 2) {
                PermissionSet permset = new PermissionSet (PermissionState.None);
                m_secParser.Reset ();
                m_secParser.LoadXml (Encoding.Unicode.GetString (m_root.Streams.BlobHeap.Read (dsRow.PermissionSet)));
                permset.FromXml (m_secParser.ToXml ());
                dec.PermissionSet = permset;
            } //TODO: else
            return dec;
        }

        private MethodReturnType GetMethodReturnType (MethodSig msig)
        {
            ITypeReference retType = null;
            if (msig.RetType.Void)
                retType = SearchCoreType ("System.Void");
            else if (msig.RetType.ByRef)
                retType = new ReferenceType (GetTypeRefFromSig (msig.RetType.Type));
            else
                retType = GetTypeRefFromSig (msig.RetType.Type);
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
                return SearchCoreType ("System.String");
            case ElementType.Object :
                return SearchCoreType ("System.Object");
            case ElementType.Void :
                return SearchCoreType ("System.Void");
            case ElementType.Boolean :
                return SearchCoreType ("System.Boolean");
            case ElementType.Char :
                return SearchCoreType ("System.Char");
            case ElementType.I1 :
                return SearchCoreType ("System.SByte");
            case ElementType.U1 :
                return SearchCoreType ("System.Byte");
            case ElementType.I2 :
                return SearchCoreType ("System.Int16");
            case ElementType.U2 :
                return SearchCoreType ("System.UInt16");
            case ElementType.I4 :
                return SearchCoreType ("System.Int32");
            case ElementType.U4 :
                return SearchCoreType ("System.UInt32");
            case ElementType.I8 :
                return SearchCoreType ("System.Int64");
            case ElementType.U8 :
                return SearchCoreType ("System.UInt64");
            case ElementType.R4 :
                return SearchCoreType ("System.Single");
            case ElementType.R8 :
                return SearchCoreType ("System.Double");
            case ElementType.I :
                return SearchCoreType ("System.IntPtr");
            case ElementType.U :
                return SearchCoreType ("System.UIntPtr");
            case ElementType.Array :
                ARRAY ary = t as ARRAY;
                return new ArrayType (GetTypeRefFromSig (ary.Type), ary.Shape);
            case ElementType.SzArray :
                SZARRAY szary = t as SZARRAY;
                ArrayType at = new ArrayType (GetTypeRefFromSig (szary.Type));
                at.Dimensions.Add (new ArrayDimension (0, 0));
                return at;
            case ElementType.Ptr :
                PTR pointer = t as PTR;
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
    }
}
