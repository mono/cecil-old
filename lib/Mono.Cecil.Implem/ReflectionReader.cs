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

                TypeDefinition parent = m_types [row.EnclosingClass - 2];
                TypeDefinition child = m_types [row.NestedClass - 2];

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
            TypeSpecTable tsTable = m_root.Streams.TablesHeap [typeof (TypeSpecTable)] as TypeSpecTable;
            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable [i];
                TypeDefinition child = m_types [i - 1];

                if (type.Extends.RID != 0) {
                    switch (type.Extends.TokenType) {
                    case TokenType.TypeDef :
                        child.BaseType = m_types [type.Extends.RID - 2];
                        break;
                    case TokenType.TypeRef :
                        child.BaseType = m_refs [type.Extends.RID - 1];
                        break;
                    case TokenType.TypeSpec :
                        TypeSpecRow tsRow = tsTable [(int) type.Extends.RID];
                        TypeSpec ts = m_sigReader.GetTypeSpec (tsRow.Signature);
                        child.BaseType = this.GetTypeRefFromSig (ts.Type);
                        break;
                    }
                }
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

            int index = Array.IndexOf (m_types, implementor);

            InterfaceImplTable intfsTable = m_root.Streams.TablesHeap [typeof (InterfaceImplTable)] as InterfaceImplTable;
            TypeSpecTable tsTable = m_root.Streams.TablesHeap [typeof (TypeSpecTable)] as TypeSpecTable;
            for (int i = 0; i < intfsTable.Rows.Count; i++) {
                InterfaceImplRow intRow = intfsTable [i];
                if ((intRow.Class - 2) == index) {
                    ITypeReference interf = null;
                    switch (intRow.Interface.TokenType) {
                    case TokenType.TypeDef :
                        interf = m_types [intRow.Interface.RID - 2];
                        break;
                    case TokenType.TypeRef :
                        interf = m_refs [intRow.Interface.RID - 1];
                        break;
                    case TokenType.TypeSpec :
                        TypeSpecRow tsRow = tsTable [(int) intRow.Interface.RID];
                        TypeSpec ts = m_sigReader.GetTypeSpec (tsRow.Signature);
                        interf = this.GetTypeRefFromSig (ts.Type);
                        break;
                    }
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
        }

        public void Visit (IMethodDefinition method)
        {
        }

        public void Visit (IEventDefinitionCollection events)
        {
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
            int index = Array.IndexOf (m_types, dec), next;
            TypeDefTable tdefTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            FieldTable fldTable = m_root.Streams.TablesHeap [typeof (FieldTable)] as FieldTable;
            if ((index + 2) == (tdefTable.Rows.Count))
                next = fldTable.Rows.Count + 1;
            else
                next = (int) (tdefTable [index + 2]).FieldList;

            for (int i = (int) tdefTable [index + 1].FieldList; i < next; i++) {
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
            int index = Array.IndexOf (m_types, dec), next;
            PropertyTable propsTable = m_root.Streams.TablesHeap [typeof (PropertyTable)] as PropertyTable;

            PropertyMapTable pmapTable = m_root.Streams.TablesHeap [typeof (PropertyMapTable)] as PropertyMapTable;
            PropertyMapRow thisRow = null, nextRow = null;
            for (int i = 0; i < pmapTable.Rows.Count; i++) {
                if (pmapTable [i].Parent == index + 2) {
                    thisRow = pmapTable [i];
                    continue;
                } else if (pmapTable [i].Parent == index + 3) {
                    nextRow = pmapTable [i];
                }
            }

            if (thisRow == null)
                return;

            if (nextRow == null)
                next = propsTable.Rows.Count;
            else
                next = (int) nextRow.PropertyList;

            for (int i = (int) thisRow.PropertyList; i < next; i++) {
                PropertyRow prow = propsTable [i];

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
                    ret = m_types [c.Type.RID - 2];
                    break;
                case TokenType.TypeRef :
                    ret = m_refs [c.Type.RID - 1];
                    break;
                }
                break;
            case ElementType.ValueType :
                VALUETYPE vt = t as VALUETYPE;
                switch (vt.Type.TokenType) {
                case TokenType.TypeDef :
                    ret = m_types [vt.Type.RID - 2];
                    break;
                case TokenType.TypeRef :
                    ret = m_refs [vt.Type.RID - 1];
                    break;
                }
                break;
            }
            //TODO: continue this with all element types
            return ret;
        }
    }
}
