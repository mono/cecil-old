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
    using System.IO;

    using Mono.Cecil;
    using Mono.Cecil.Binary;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Metadata;
    using Mono.Cecil.Signatures;

    internal sealed class CodeReader : ICodeVisitor {

        private ReflectionReader m_reflectReader;
        private MetadataRoot m_root;

        public CodeReader (ReflectionReader reflectReader)
        {
            m_reflectReader = reflectReader;
            m_root = m_reflectReader.MetadataRoot;
        }

        public void Visit (IMethodBody body)
        {
            MethodDefinition meth = body.Method as MethodDefinition;
            MethodBody methBody = body as MethodBody;
            BinaryReader br = m_reflectReader.Module.Reader.GetReader ();
            br.BaseStream.Position = m_reflectReader.Module.Reader.Image.ResolveVirtualAddress (meth.RVA);

            // lets read the method
            int flags = br.ReadByte ();
            switch (flags & 0x7) {
            case (int) MethodHeaders.TinyFormat :
                methBody.CodeSize = flags >> 2;
                methBody.MaxStack = 8;
                ReadCilBody (methBody, br.ReadBytes (body.CodeSize));
                return;
            case (int) MethodHeaders.FatFormat :
                br.BaseStream.Position--;
                int fatflags = br.ReadUInt16 ();
                //int headersize = (fatflags >> 12) & 0xf;
                methBody.MaxStack = br.ReadUInt16 ();
                methBody.CodeSize = br.ReadInt32 ();
                methBody.LocalVarToken = br.ReadInt32 ();
                body.InitLocals = (fatflags & (int) MethodHeaders.InitLocals) != 0;
                ReadLocalVars (methBody);
                ReadCilBody (methBody, br.ReadBytes (methBody.CodeSize));
                if ((fatflags & (int) MethodHeaders.MoreSects) != 0)
                    ReadExceptionHandlers (methBody, br);
                return;
            }
        }

        private void ReadLocalVars (MethodBody body)
        {
            MethodDefinition meth = body.Method as MethodDefinition;
            StandAloneSigTable sasTable = m_root.Streams.TablesHeap [typeof (StandAloneSigTable)] as StandAloneSigTable;
            StandAloneSigRow sasRow = sasTable [(body.LocalVarToken & 0xffffff) - 1];
            LocalVarSig sig = m_reflectReader.SigReader.GetLocalVarSig (sasRow.Signature);
            for (int i = 0; i < sig.Count; i++) {
                LocalVarSig.LocalVariable lv = sig.LocalVariables [i];
                ITypeReference varType = m_reflectReader.GetTypeRefFromSig(lv.Type);
                if (lv.ByRef)
                    varType = new ReferenceType (varType);
                if ((lv.Constraint & Constraint.Pinned) != 0)
                    varType = new PinnedType (varType);
                body.Variables.Add (new VariableDefinition (string.Concat ("V_", i), meth, varType));
            }
        }

        private void ReadCilBody (MethodBody body, byte [] data)
        {

        }

        private void ReadExceptionHandlers (MethodBody body, BinaryReader br)
        {

        }

        public void Visit (IInstructionCollection instructions)
        {
        }

        public void Visit (IInstruction instr)
        {
        }

        public void Visit (IExceptionHandlerCollection seh)
        {
        }

        public void Visit (IExceptionHandler eh)
        {
        }

        public void Visit (IVariableDefinitionCollection variables)
        {
        }

        public void Visit (IVariableDefinition var)
        {
        }
    }
}
