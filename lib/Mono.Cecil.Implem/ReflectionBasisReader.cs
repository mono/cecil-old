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
            TypeDefinitionCollection tdc = types as TypeDefinitionCollection;
            if (tdc != null && tdc.Loaded)
                return;

            ModuleDefinition def = tdc.Container as ModuleDefinition;
            StructureReader sr = def.Assembly.StructureReader;

            MetadataRoot mr = sr.Image.MetadataRoot;

            TypeDefTable tdt = mr.Streams.TablesHeap [typeof (TypeDefTable)] as TypeDefTable;
            foreach (TypeDefRow type in tdt.Rows) {
                TypeDefinition t = new TypeDefinition (
                    mr.Streams.StringsHeap [type.Name],
                    mr.Streams.StringsHeap [type.Namespace],
                    type.Flags);
                types [t.FullName] = t;
            }

            //TODO: read nested types

            tdc.Loaded = true;
        }

        public void Visit (ITypeDefinition type)
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
