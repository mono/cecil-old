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

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Metadata;
    using Mono.Cecil.Signatures;

    internal sealed class ReflectionReader : IReflectionVisitor, IMethodsReader {

        private ModuleDefinition m_module;
        private ImageReader m_reader;
        private SignatureReader m_sigReader;
        private MetadataRoot m_root;

        private TypeDefinition [] m_types;
        private TypeReference [] m_refs;
        private MethodDefinition [] m_meths;

        private bool m_isCorlib;

        public ReflectionReader (ModuleDefinition module)
        {
            m_module = module;
            m_reader = m_module.Reader;
            m_root = m_reader.Image.MetadataRoot;
            m_sigReader = new SignatureReader (m_root);
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

        public void Visit (ITypeDefinitionCollection types)
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
            for (int i = 0; i < nested.Rows.Count; i++) {
                NestedClassRow row = nested [i];

                TypeDefinition parent = GetTypeDefAt ((int) row.EnclosingClass);
                TypeDefinition child = GetTypeDefAt ((int) row.NestedClass);

                child.DeclaringType = parent;
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

                // ok, I've thought a lot before doing that
                // if I do not load the two primitives that are field and methods here
                // i'll run into big troubles as soon a lazy loaded stuff reference it
                // such as methods body or an override collection
                this.Visit (type.Fields);
                this.Visit (type.Methods);
            }

            tdc.Loaded = true;
        }

        public void Visit (ITypeDefinition type)
        {
        }

        public void Visit (ITypeReferenceCollection refs)
        {
        }

        public void Visit (ITypeReference type)
        {
        }

        public void Visit (IInterfaceCollection interfaces)
        {
            InterfaceCollection interfs = interfaces as InterfaceCollection;
            if (interfs != null && interfs.Loaded)
                return;

            TypeDefinition implementor = interfaces.Container as TypeDefinition;

            int rid = GetRidForTypeDef (implementor);

            InterfaceImplTable intfsTable = m_root.Streams.TablesHeap [typeof (InterfaceImplTable)] as InterfaceImplTable;
            for (int i = 0; i < intfsTable.Rows.Count; i++) {
                InterfaceImplRow intRow = intfsTable [i];
                if (intRow.Class == rid) {
                    ITypeReference interf = GetTypeDefOrRef (intRow.Interface);
                    interfaces [interf.FullName] = interf;
                }
            }

            interfs.Loaded = true;
        }

        public void Visit (IOverrideCollection meth)
        {
        }

        public void Visit (IParameterDefinitionCollection parameters)
        {
        }

        public void Visit (IParameterDefinition parameter)
        {
        }

        public void Visit (IMethodDefinitionCollection methods)
        {
            MethodDefinitionCollection meths = methods as MethodDefinitionCollection;
            if (meths.Loaded)
                return;

            TypeDefinition dec = meths.Container as TypeDefinition;
            int index = GetRidForTypeDef (dec) - 1, next;
            TypeDefTable tdefTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            MethodTable methTable = m_root.Streams.TablesHeap [typeof (MethodTable)] as MethodTable;
            ParamTable paramTable = m_root.Streams.TablesHeap [typeof (ParamTable)] as ParamTable;
            if (index == tdefTable.Rows.Count - 1)
                next = methTable.Rows.Count + 1;
            else
                next = (int) (tdefTable [index + 1]).MethodList;

            if (m_meths == null)
                m_meths = new MethodDefinition [methTable.Rows.Count];

            for (int i = (int) tdefTable [index].MethodList; i < next; i++) {
                MethodRow methRow = methTable [i - 1];
                MethodDefinition mdef = new MethodDefinition (m_root.Streams.StringsHeap [methRow.Name], dec,
                                                              methRow.RVA, methRow.Flags, methRow.ImplFlags);
                MethodSig msig = m_sigReader.GetMethodDefSig (methRow.Signature);

                for (int j = 0, k = (int) methRow.ParamList; j < msig.ParamCount; j++) {
                    ParamRow prow = paramTable [k - 1]; k++;
                    Param psig = msig.Parameters [j];

                    ParameterDefinition pdef = new ParameterDefinition (m_root.Streams.StringsHeap [prow.Name], prow.Sequence, prow.Flags, null);
                    pdef.ParameterType = psig.ByRef ? new Reference (GetTypeRefFromSig (psig.Type)) : GetTypeRefFromSig (psig.Type);
                    mdef.Parameters.Add (pdef);
                }

                mdef.ReturnType = GetMethodReturnType (msig);
                m_meths [i - 1] = mdef;
                meths [Utilities.MethodSignature (mdef)] = mdef;
            }

            meths.Loaded = true;
        }

        public void Visit (IMethodDefinition method)
        {
        }

        public void Visit (IEventDefinitionCollection events)
        {
            EventDefinitionCollection evts = events as EventDefinitionCollection;
            if (evts.Loaded)
                return;

            TypeDefinition dec = evts.Container as TypeDefinition;
            int rid = GetRidForTypeDef (dec), next;
            EventTable evtTable = m_root.Streams.TablesHeap [typeof (EventTable)] as EventTable;

            EventMapTable evtMapTable = m_root.Streams.TablesHeap [typeof (EventMapTable)] as EventMapTable;
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
                next = evtTable.Rows.Count;
            else
                next = (int) nextRow.EventList;

            for (int i = (int) thisRow.EventList; i < next; i++) {
                EventRow erow = evtTable [i - 1];
                EventDefinition edef = new EventDefinition (m_root.Streams.StringsHeap [erow.Name], dec,
                                                            GetTypeDefOrRef (erow.EventType), erow.EventFlags);
                events [edef.Name] = edef;
            }

            evts.Loaded = true;
        }

        public void Visit (IEventDefinition evt)
        {
        }

        public void Visit (IFieldDefinitionCollection fields)
        {
            FieldDefinitionCollection flds = fields as FieldDefinitionCollection;
            if (flds.Loaded)
                return;

            TypeDefinition dec = flds.Container as TypeDefinition;
            int index = GetRidForTypeDef (dec) - 1, next;
            TypeDefTable tdefTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            FieldTable fldTable = m_root.Streams.TablesHeap [typeof (FieldTable)] as FieldTable;
            if (index == tdefTable.Rows.Count - 1)
                next = fldTable.Rows.Count + 1;
            else
                next = (int) (tdefTable [index + 1]).FieldList;

            for (int i = (int) tdefTable [index].FieldList; i < next; i++) {
                FieldRow frow = fldTable [i - 1];
                FieldSig fsig = m_sigReader.GetFieldSig (frow.Signature);
                FieldDefinition fdef = new FieldDefinition (m_root.Streams.StringsHeap [frow.Name],
                                                            dec, this.GetTypeRefFromSig (fsig.Type), frow.Flags);
                fields [fdef.Name] = fdef;
            }

            flds.Loaded = true;
        }

        public void Visit (IFieldDefinition field)
        {
        }

        public void Visit (IPropertyDefinitionCollection properties)
        {
            PropertyDefinitionCollection props = properties as PropertyDefinitionCollection;
            if (props.Loaded)
                return;

            TypeDefinition dec = props.Container as TypeDefinition;
            int rid = GetRidForTypeDef (dec), next;
            PropertyTable propsTable = m_root.Streams.TablesHeap [typeof (PropertyTable)] as PropertyTable;

            PropertyMapTable pmapTable = m_root.Streams.TablesHeap [typeof (PropertyMapTable)] as PropertyMapTable;
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
                next = propsTable.Rows.Count;
            else
                next = (int) nextRow.PropertyList;

            for (int i = (int) thisRow.PropertyList; i < next; i++) {
                PropertyRow prow = propsTable [i - 1];
                PropertySig psig = m_sigReader.GetPropSig (prow.Type);
                PropertyDefinition pdef = new PropertyDefinition (m_root.Streams.StringsHeap [prow.Name],
                                                                  dec, this.GetTypeRefFromSig (psig.Type), prow.Flags);
                properties [pdef.Name] = pdef;
            }

            props.Loaded = true;
        }

        public void Visit (IPropertyDefinition property)
        {
        }

        public void ReadMethods (EventDefinition evt)
        {
            if (evt.Readed)
                return;

            // read semantic here

            evt.Readed = true;
        }

        public void ReadMethods (PropertyDefinition prop)
        {
            if (prop.Readed)
                return;

            // read semantic here

            prop.Readed = true;
        }

        private MethodReturnType GetMethodReturnType (MethodSig msig)
        {
            ITypeReference retType = null;
            if (msig.RetType.Void)
                retType = SearchCoreType ("System.Void");
            else if (msig.RetType.ByRef)
                retType = new Reference (GetTypeRefFromSig (msig.RetType.Type));
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
                return new Pointer (GetTypeRefFromSig (pointer.PtrType));
            case ElementType.FnPtr :
                // not very sure of this
                FNPTR funcptr = t as FNPTR;
                ParameterDefinitionCollection parameters = new ParameterDefinitionCollection (null);
                for (int i = 0; i < funcptr.Method.ParamCount; i++) {
                    Param p = funcptr.Method.Parameters [i];
                    ParameterDefinition pdef = new ParameterDefinition (string.Concat ("arg", i), i, new ParamAttributes (), null);
                    pdef.ParameterType = p.ByRef ? new Reference (GetTypeRefFromSig (p.Type)) : GetTypeRefFromSig (p.Type);
                    parameters.Add (pdef);
                }
                return new FunctionPointer (funcptr.Method.HasThis, funcptr.Method.ExplicitThis, funcptr.Method.MethCallConv,
                                            parameters, GetMethodReturnType (funcptr.Method));
            }
            return null;
        }
    }
}
