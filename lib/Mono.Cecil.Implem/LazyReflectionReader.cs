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
    using Mono.Cecil.Signatures;

    internal sealed class LazyReflectionReader : ReflectionReader {

        public LazyReflectionReader (ModuleDefinition module) : base (module)
        {
        }

        public override void Visit (IInterfaceCollection interfaces)
        {
            InterfaceCollection interfs = interfaces as InterfaceCollection;
            if (interfs != null && interfs.Loaded)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (InterfaceImplTable))) {
                interfs.Loaded = true;
                return;
            }

            TypeDefinition implementor = interfaces.Container as TypeDefinition;

            int rid = GetRidForTypeDef (implementor);

            InterfaceImplTable intfsTable = m_root.Streams.TablesHeap [typeof (InterfaceImplTable)] as InterfaceImplTable;
            for (int i = 0; i < intfsTable.Rows.Count; i++) {
                InterfaceImplRow intRow = intfsTable [i];
                if (intRow.Class == rid) {
                    ITypeReference interf = GetTypeDefOrRef (intRow.Interface);
                    interfaces.Add (interf);
                }
            }

            interfs.Loaded = true;
        }

        public override void Visit (IOverrideCollection meths)
        {
            OverrideCollection methods = meths as OverrideCollection;
            if (methods.Loaded)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (MethodImplTable))) {
                methods.Loaded = true;
                return;
            }

            MethodImplTable implTable = MetadataRoot.Streams.TablesHeap [typeof (MethodImplTable)] as MethodImplTable;

            int index = GetRidForMethodDef (meths.Container as MethodDefinition);
            for (int i = 0; i < implTable.Rows.Count; i++) {
                MethodImplRow implRow = implTable [i];
                if (implRow.MethodBody.TokenType == TokenType.Method && implRow.MethodBody.RID == index) {
                    if (implRow.MethodDeclaration.TokenType == TokenType.Method)
                        meths.Add (GetMethodDefAt ((int) implRow.MethodDeclaration.RID));
                    //else if (implRow.MethodDeclaration.TokenType == TokenType.MemberRef)
                    //TODO: handle this case
                }
            }
        }

        public override void Visit (ISecurityDeclarationCollection secDecls)
        {
            SecurityDeclarationCollection secDeclarations = secDecls as SecurityDeclarationCollection;
            if (secDeclarations.Loaded)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (DeclSecurityTable))) {
                secDeclarations.Loaded = true;
                return;
            }

            DeclSecurityTable dsTable = m_root.Streams.TablesHeap [typeof (DeclSecurityTable)] as DeclSecurityTable;
            int rid = 0;
            TokenType target;
            if (secDecls.Container is AssemblyDefinition)
                target = TokenType.Assembly;
            else if (secDecls.Container is TypeDefinition) {
                target = TokenType.TypeDef;
                rid = GetRidForTypeDef (secDecls.Container as TypeDefinition);
            } else { // MethodDefinition
                target = TokenType.Method;
                rid = GetRidForMethodDef (secDecls.Container as MethodDefinition);
            }

            for (int i = 0; i < dsTable.Rows.Count; i++) {
                DeclSecurityRow dsRow = dsTable [i];

                switch (dsRow.Parent.TokenType) {
                case TokenType.Assembly :
                    if (target == TokenType.Assembly)
                        secDecls.Add (BuildSecurityDeclaration (dsRow));
                    break;
                case TokenType.Method :
                case TokenType.TypeDef :
                    if (dsRow.Parent.TokenType == target && dsRow.Parent.RID == rid)
                        secDecls.Add (BuildSecurityDeclaration (dsRow));
                    break;
                }
            }

            secDeclarations.Loaded = true;
        }

        public override void Visit (IEventDefinitionCollection events)
        {
            EventDefinitionCollection evts = events as EventDefinitionCollection;
            if (evts.Loaded)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (EventTable))) {
                m_events = new EventDefinition [0];
                evts.Loaded = true;
                return;
            }

            TypeDefinition dec = evts.Container as TypeDefinition;
            int rid = GetRidForTypeDef (dec), next;
            EventTable evtTable = m_root.Streams.TablesHeap [typeof (EventTable)] as EventTable;

            if (m_events == null)
                m_events = new EventDefinition [evtTable.Rows.Count];

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
                next = evtTable.Rows.Count + 1;
            else
                next = (int) nextRow.EventList;

            for (int i = (int) thisRow.EventList; i < next; i++) {
                EventRow erow = evtTable [i - 1];
                EventDefinition edef = new EventDefinition (m_root.Streams.StringsHeap [erow.Name], dec,
                                                            GetTypeDefOrRef (erow.EventType), erow.EventFlags);
                events [edef.Name] = edef;
                m_events [i - 1] = edef;
            }

            evts.Loaded = true;
        }

        public override void Visit (IPInvokeInfo pinvk)
        {
            //TODO: that
        }

        public override void Visit (IPropertyDefinitionCollection properties)
        {
            PropertyDefinitionCollection props = properties as PropertyDefinitionCollection;
            if (props.Loaded)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (PropertyTable))) {
                m_properties = new PropertyDefinition [0];
                props.Loaded = true;
                return;
            }

            TypeDefinition dec = props.Container as TypeDefinition;
            int rid = GetRidForTypeDef (dec), next;
            PropertyTable propsTable = m_root.Streams.TablesHeap [typeof (PropertyTable)] as PropertyTable;
            if (m_properties == null)
                m_properties = new PropertyDefinition [propsTable.Rows.Count];

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
                PropertyDefinition pdef = new PropertyDefinition (MetadataRoot.Streams.StringsHeap [prow.Name],
                                                                  dec, this.GetTypeRefFromSig (psig.Type), prow.Flags);
                properties [pdef.Name] = pdef;
                m_properties [i - 1] = pdef;
            }

            props.Loaded = true;
        }

        public override void ReadMethods (EventDefinition evt)
        {
            if (evt.Readed)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (MethodSemanticsTable))) {
                evt.Readed = true;
                return;
            }

            int index = Array.IndexOf (m_events, evt) + 1;

            MethodSemanticsTable semTable = m_root.Streams.TablesHeap [typeof (MethodSemanticsTable)] as MethodSemanticsTable;
            for (int i = 0; i < semTable.Rows.Count; i++) {
                MethodSemanticsRow semRow = semTable [i];
                MethodDefinition semMeth = GetMethodDefAt ((int) semRow.Method);
                if (semRow.Association.TokenType == TokenType.Event && semRow.Association.RID == index) {
                    semMeth.SemanticsAttributes = semRow.Semantics;
                    if ((semRow.Semantics & MethodSemanticsAttributes.AddOn) != 0)
                        evt.AddMethod = semMeth;
                    else if ((semRow.Semantics & MethodSemanticsAttributes.Fire) != 0)
                        evt.InvokeMethod = semMeth;
                    else if ((semRow.Semantics & MethodSemanticsAttributes.RemoveOn) != 0)
                        evt.RemoveMethod = semMeth;
                }
            }

            evt.Readed = true;
        }

        public override void ReadMethods (PropertyDefinition prop)
        {
            if (prop.Readed)
                return;

            if (!m_root.Streams.TablesHeap.HasTable (typeof (MethodSemanticsTable))) {
                prop.Readed = true;
                return;
            }

            int index = Array.IndexOf (m_properties, prop) + 1;

            MethodSemanticsTable semTable = m_root.Streams.TablesHeap [typeof (MethodSemanticsTable)] as MethodSemanticsTable;
            for (int i = 0; i < semTable.Rows.Count; i++) {
                MethodSemanticsRow semRow = semTable [i];
                MethodDefinition semMeth = GetMethodDefAt ((int) semRow.Method);
                if (semRow.Association.TokenType == TokenType.Property && semRow.Association.RID == index) {
                    semMeth.SemanticsAttributes = semRow.Semantics;
                    if ((semRow.Semantics & MethodSemanticsAttributes.Getter) != 0)
                        prop.GetMethod = semMeth;
                    else if ((semRow.Semantics & MethodSemanticsAttributes.Setter) != 0)
                        prop.SetMethod = semMeth;
                }
            }

            prop.Readed = true;
        }
    }
}
