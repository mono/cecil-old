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

    internal sealed class ReflectionReader : IReflectionVisitor {

        private ImageReader m_reader;
        private SignatureReader m_sigReader;
        private MetadataRoot m_root;

        private TypeDefinition [] m_types;
        private TypeReference [] m_refs;

        public ReflectionReader (ImageReader sr)
        {
            m_reader = sr;
            m_root = sr.Image.MetadataRoot;
            m_sigReader = new SignatureReader (m_root);
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
        }

        public void Visit (ITypeDefinition type)
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

        public void Visit (IMethodDefinitionCollection methods)
        {
            MethodDefinitionCollection meths = methods as MethodDefinitionCollection;
            if (meths.Loaded)
                return;

            TypeDefinition dec = meths.Container as TypeDefinition;
            int index = GetRidForTypeDef (dec) - 1, next;
            TypeDefTable tdefTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            MethodTable methTable = m_root.Streams.TablesHeap [typeof (MethodTable)] as MethodTable;
            if (index == tdefTable.Rows.Count - 1)
                next = methTable.Rows.Count + 1;
            else
                next = (int) (tdefTable [index + 1]).MethodList;

            for (int i = (int) tdefTable [index].FieldList; i < next; i++) {
                MethodRow methRow = methTable [i - 1];

                // write here :)
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

                // read invoke, add & remove methods
                // should they be lazy loaded to avoid loading of methods ?
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

                // read set & get method
                // should they be lazy loaded to avoid loading of methods ?

                properties [pdef.Name] = pdef;
            }

            props.Loaded = true;
        }

        public void Visit (IPropertyDefinition property)
        {
        }

        public ITypeReference GetTypeRefFromSig (SigType t)
        {
            ITypeReference ret = null;
            switch (t.ElementType) {
            case ElementType.Class :
                CLASS c = t as CLASS;
                switch (c.Type.TokenType) {
                case TokenType.TypeDef :
                    ret = GetTypeDefAt ((int) c.Type.RID);
                    break;
                case TokenType.TypeRef :
                    ret = GetTypeRefAt ((int) c.Type.RID);
                    break;
                }
                break;
            case ElementType.ValueType :
                VALUETYPE vt = t as VALUETYPE;
                switch (vt.Type.TokenType) {
                case TokenType.TypeDef :
                    ret = GetTypeDefAt ((int) vt.Type.RID);
                    break;
                case TokenType.TypeRef :
                    ret = GetTypeRefAt ((int) vt.Type.RID);
                    break;
                }
                break;
            }
            //TODO: continue this with all element types
            return ret;
        }
    }
}
