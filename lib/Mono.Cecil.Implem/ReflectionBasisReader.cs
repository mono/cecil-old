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
    using Mono.Cecil.Metadata;

    internal sealed class ReflectionBasisReader : IReflectionVisitor {

        public void Visit (ITypeDefinitionCollection types)
        {
            //TODO: optimize this
            TypeDefinitionCollection tdc = types as TypeDefinitionCollection;
            if (tdc != null && tdc.Loaded)
                return;

            ModuleDefinition def = tdc.Container as ModuleDefinition;
            StructureReader sr = def.Assembly.StructureReader;

            MetadataRoot mr = sr.Image.MetadataRoot;

            TypeDefTable typesTable = mr.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;

            TypeDefinition [] typeDefs = new TypeDefinition [typesTable.Rows.Count - 1];

            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable.Rows [i] as TypeDefRow;
                TypeDefinition t = new TypeDefinition (
                    mr.Streams.StringsHeap [type.Name],
                    mr.Streams.StringsHeap [type.Namespace],
                    type.Flags);

                typeDefs [i - 1] = t;
            }

            NestedClassTable nested = mr.Streams.TablesHeap [typeof (NestedClassTable)] as NestedClassTable;

            for (int i = 0; i < nested.Rows.Count; i++) {
                NestedClassRow row = nested.Rows [i] as NestedClassRow;

                TypeDefinition parent = typeDefs [row.EnclosingClass - 2];
                TypeDefinition child = typeDefs [row.NestedClass - 2];

                child.DeclaringType = parent;
                parent.NestedTypes [child.Name] = child;
            }

            for (int i = 1; i < typesTable.Rows.Count; i++) {
                TypeDefRow type = typesTable.Rows [i] as TypeDefRow;
                TypeDefinition child = typeDefs [i - 1];

                if (type.Extends.RID != 0) {
                    switch (type.Extends.TokenType) {
                    case TokenType.TypeDef :
                        child.BaseType = typeDefs [type.Extends.RID - 2];
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

            for (int i = 0; i < typeDefs.Length; i++) {
                TypeDefinition type = typeDefs [i];
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

        public void Visit (INestedTypesCollection nestedTypes)
        {
        }

        public void Visit (IInterfaceCollection interfaces)
        {
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
    }
}
