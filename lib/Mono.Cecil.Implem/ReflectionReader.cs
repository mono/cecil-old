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

    internal sealed class ReflectionReader : IReflectionVisitor {

        private ImageReader m_reader;
        private MetadataRoot m_root;
        private TypeDefinition [] m_types;

        public ReflectionReader (ImageReader sr)
        {
            m_reader = sr;
            m_root = sr.Image.MetadataRoot;
        }

        public void Visit (ITypeDefinitionCollection types)
        {
            //TODO: optimize this
            TypeDefinitionCollection tdc = types as TypeDefinitionCollection;
            if (tdc != null && tdc.Loaded)
                return;

            ModuleDefinition def = tdc.Container as ModuleDefinition;

            TypeDefTable typesTable = m_root.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;

            m_types = new TypeDefinition [typesTable.Rows.Count - 1];

            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable.Rows [i] as TypeDefRow;
                TypeDefinition t = new TypeDefinition (
                    m_root.Streams.StringsHeap [type.Name],
                    m_root.Streams.StringsHeap [type.Namespace],
                    type.Flags, def);

                m_types [i - 1] = t;
            }

            NestedClassTable nested = m_root.Streams.TablesHeap [typeof (NestedClassTable)] as NestedClassTable;

            for (int i = 0; i < nested.Rows.Count; i++) {
                NestedClassRow row = nested.Rows [i] as NestedClassRow;

                TypeDefinition parent = m_types [row.EnclosingClass - 2];
                TypeDefinition child = m_types [row.NestedClass - 2];

                child.DeclaringType = parent;
            }

            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable.Rows [i] as TypeDefRow;
                TypeDefinition child = m_types [i - 1];

                if (type.Extends.RID != 0) {
                    switch (type.Extends.TokenType) {
                    case TokenType.TypeDef :
                        child.BaseType = m_types [type.Extends.RID - 2];
                        break;
                    case TokenType.TypeRef :
                        //TODO: implement type ref reading
                        break;
                    case TokenType.TypeSpec :
                        //TODO: implement this...
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
            for (int i = 0; i < intfsTable.Rows.Count; i++) {
                InterfaceImplRow intRow = intfsTable.Rows [i] as InterfaceImplRow;
                if ((intRow.Class - 2) == index) {
                    switch (intRow.Interface.TokenType) {
                    case TokenType.TypeDef :
                        TypeDefinition interf = m_types [intRow.Interface.RID - 2];
                        interfaces [interf.FullName] = interf;
                        break;
                    case TokenType.TypeRef :
                        //TODO: implement type ref reading
                        break;
                    case TokenType.TypeSpec :
                        //TODO: implement this...
                        break;
                    }
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
    }
}
