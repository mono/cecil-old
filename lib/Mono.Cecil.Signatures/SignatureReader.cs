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

namespace Mono.Cecil.Signatures {

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;

    using Mono.Cecil;
    using Mono.Cecil.Metadata;

    internal sealed class SignatureReader : ISignatureVisitor {

        private MetadataRoot m_root;
        private byte [] m_blobData;

        // flyweights
        private IDictionary m_fieldSigs;
        private IDictionary m_propSigs;
        private IDictionary m_typeSpecs;
        private IDictionary m_methodsDefSigs;
        private IDictionary m_methodsRefSigs;
        private IDictionary m_localVars;
        private IDictionary m_customAttribs;

        public SignatureReader (MetadataRoot root)
        {
            m_root = root;
            m_blobData = m_root.Streams.BlobHeap.Data;

            m_fieldSigs = new HybridDictionary ();
            m_propSigs = new HybridDictionary ();
            m_customAttribs = new HybridDictionary ();
            m_methodsDefSigs = new HybridDictionary ();
            m_methodsRefSigs = new HybridDictionary ();
            m_localVars = new HybridDictionary ();
        }

        public FieldSig GetFieldSig (uint index)
        {
            FieldSig f = m_fieldSigs [index] as FieldSig;
            if (f == null) {
                f = new FieldSig (index);
                f.Accept (this);
                m_fieldSigs [index] = f;
            }
            return f;
        }

        public PropertySig GetPropSig (uint index)
        {
            PropertySig p = m_propSigs [index] as PropertySig;
            if (p == null) {
                p = new PropertySig (index);
                p.Accept (this);
                m_propSigs [index] = p;
            }
            return p;
        }

        public MethodSig GetMethodDefSig (uint index)
        {
            MethodDefSig m = m_methodsDefSigs [index] as MethodDefSig;
            if (m == null) {
                m = new MethodDefSig (index);
                m.Accept (this);
                m_methodsDefSigs [index] = m;
            }
            return m;
        }

        public MethodSig GetMethodRefSig (uint index)
        {
            MethodRefSig m = m_methodsRefSigs [index] as MethodRefSig;
            if (m == null) {
                m = new MethodRefSig (index);
                m.Accept (this);
                m_methodsRefSigs [index] = m;
            }
            return m;
        }

        public TypeSpec GetTypeSpec (uint index)
        {
            TypeSpec ts = null;
            if (m_typeSpecs == null)
                m_typeSpecs = new HybridDictionary ();
            else
                ts = m_typeSpecs [index] as TypeSpec;

            if (ts == null) {
                ts = ReadTypeSpec (m_blobData, (int) index);
                m_typeSpecs [index] = ts;
            }
            return ts;
        }

        public LocalVarSig GetLocalVarSig (uint index)
        {
            LocalVarSig lv = m_localVars [index] as LocalVarSig;
            if (lv == null) {
                lv = new LocalVarSig (index);
                lv.Accept (this);
                m_localVars [index] = lv;
            }
            return lv;
        }

        public CustomAttrib GetCustomAttrib (uint index, IMethodReference ctor)
        {
            CustomAttrib ca = m_customAttribs [index] as CustomAttrib;
            if (ca == null) {
                ca = ReadCustomAttrib (m_blobData, (int) index, ctor);
                m_customAttribs [index] = ca;
            }
            return ca;
        }

        public Signature GetMemberRefSig (TokenType tt, uint index)
        {
            int start, callconv;
            Utilities.ReadCompressedInteger (m_blobData, (int) index, out start);
            callconv = m_blobData [start];
            bool field = (callconv & 0x7) != 0;
            if (field)
                return GetFieldSig (index);
            switch (tt) {
            case TokenType.TypeDef :
            case TokenType.TypeRef :
            case TokenType.TypeSpec :
                return GetMethodRefSig (index);
            case TokenType.ModuleRef :
            case TokenType.Method :
                return GetMethodDefSig (index);
            }
            return null;
        }

        public void Visit (MethodDefSig methodDef)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) methodDef.BlobIndex, out start);
            methodDef.CallingConvention = m_blobData [start];
            methodDef.HasThis = (methodDef.CallingConvention & 0x20) != 0;
            methodDef.ExplicitThis = (methodDef.CallingConvention & 0x40) != 0;
            if ((methodDef.CallingConvention & 0x0) != 0)
                methodDef.MethCallConv |= MethodCallingConvention.Default;
            if ((methodDef.CallingConvention & 0x5) != 0)
                methodDef.MethCallConv |= MethodCallingConvention.VarArg;
            methodDef.ParamCount = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            methodDef.RetType = this.ReadRetType (m_blobData, start, out start);
            methodDef.Parameters = this.ReadParameters (methodDef.ParamCount, m_blobData, start, out start);
        }

        public void Visit (MethodRefSig methodRef)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) methodRef.BlobIndex, out start);
            methodRef.CallingConvention = m_blobData [start];
            methodRef.HasThis = (methodRef.CallingConvention & 0x20) != 0;
            methodRef.ExplicitThis = (methodRef.CallingConvention & 0x40) != 0;
            if ((methodRef.CallingConvention & 0x0) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.Default;
            if ((methodRef.CallingConvention & 0x1) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.C;
            if ((methodRef.CallingConvention & 0x2) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.StdCall;
            if ((methodRef.CallingConvention & 0x3) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.ThisCall;
            if ((methodRef.CallingConvention & 0x4) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.FastCall;
            if ((methodRef.CallingConvention & 0x5) != 0)
                methodRef.MethCallConv |= MethodCallingConvention.VarArg;
            methodRef.ParamCount = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            methodRef.RetType = this.ReadRetType (m_blobData, start, out start);
            int sentpos;
            methodRef.Parameters = this.ReadParameters (methodRef.ParamCount, m_blobData, start, out sentpos);
            methodRef.Sentinel = sentpos;
        }

        public void Visit (FieldSig field)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) field.BlobIndex, out start);
            field.CallingConvention = m_blobData [start];
            field.Field = (field.CallingConvention & 0x6) != 0;
            field.CustomMods = this.ReadCustomMods (m_blobData, start + 1, out start);
            field.Type = this.ReadType (m_blobData, start, out start);
        }

        public void Visit (PropertySig property)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) property.BlobIndex, out start);
            property.CallingConvention = m_blobData [start];
            property.Property = (property.CallingConvention & 0x8) != 0;
            property.ParamCount = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            property.Type = this.ReadType (m_blobData, start, out start);
            property.Parameters = this.ReadParameters (property.ParamCount, m_blobData, start);
        }

        public void Visit (LocalVarSig localvar)
        {
            int start;
            Utilities.ReadCompressedInteger (m_blobData, (int) localvar.BlobIndex, out start);
            localvar.CallingConvention = m_blobData [start];
            localvar.Local = (localvar.CallingConvention & 0x7) != 0;
            localvar.Count = Utilities.ReadCompressedInteger (m_blobData, start + 1, out start);
            localvar.LocalVariables = this.ReadLocalVariables (localvar.Count, m_blobData, start);
        }

        private LocalVarSig.LocalVariable [] ReadLocalVariables (int length, byte [] data, int pos)
        {
            int start = pos;
            LocalVarSig.LocalVariable [] types = new LocalVarSig.LocalVariable [length];
            for (int i = 0; i < length; i++)
                types [i] = this.ReadLocalVariable (data, start, out start);
            return types;
        }

        private LocalVarSig.LocalVariable ReadLocalVariable (byte [] data, int pos, out int start)
        {
            start = pos;
            LocalVarSig.LocalVariable lv = new LocalVarSig.LocalVariable ();
            lv.ByRef = false;
            int cursor;
            while (true) {
                cursor = start;
                int current = Utilities.ReadCompressedInteger (data, start, out start);
                if (current == (int) ElementType.Pinned) // the only possible constraint
                    lv.Constraint |= Constraint.Pinned;
                else if (current == (int) ElementType.ByRef)
                    lv.ByRef = true;
                else {
                    lv.Type = this.ReadType (data, cursor, out start);
                    break;
                }
            }
            return lv;
        }

        private TypeSpec ReadTypeSpec (byte [] data, int pos)
        {
            int start = pos;
            Utilities.ReadCompressedInteger (data, start, out start);
            SigType t = this.ReadType (data, start, out start);
            return new TypeSpec (t);
        }

        private RetType ReadRetType (byte [] data, int pos, out int start)
        {
            RetType rt = new RetType ();
            start = pos;
            rt.CustomMods = this.ReadCustomMods (data, start, out start);
            int curs = start;
            ElementType flag = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            switch (flag) {
            case ElementType.Void :
                rt.ByRef = rt.TypedByRef = false;
                rt.Void = true;
                break;
            case ElementType.TypedByRef :
                rt.ByRef = rt.Void = false;
                rt.TypedByRef = true;
                break;
            case ElementType.ByRef :
                rt.TypedByRef = rt.Void = false;
                rt.ByRef = true;
                rt.Type = this.ReadType (data, start, out start);
                break;
            default :
                rt.TypedByRef = rt.Void = rt.ByRef = false;
                rt.Type = this.ReadType (data, curs, out start);
                break;
            }
            return rt;
        }

        private Param [] ReadParameters (int length, byte [] data, int pos)
        {
            Param [] ret = new Param [length];
            int start = pos;
            for (int i = 0; i < length; i++)
                ret [i] = this.ReadParameter (data, start, out start);
            return ret;
        }

        private Param [] ReadParameters (int length, byte [] data, int pos, out int sentinelpos)
        {
            Param [] ret = new Param [length];
            int start = pos;
            sentinelpos = -1;

            for (int i = 0; i < length; i++) {
                int buf = start;
                Utilities.ReadCompressedInteger (data, start, out start);
                start = buf;

                if ((start & (int) ElementType.Sentinel) != 0)
                    sentinelpos = i;

                ret [i] = this.ReadParameter (data, start, out start);
            }

            return ret;
        }

        private Param ReadParameter (byte [] data, int pos, out int start)
        {
            Param p = new Param ();
            start = pos;
            p.CustomMods = this.ReadCustomMods (data, start, out start);
            int curs = start;
            ElementType flag = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            switch (flag) {
            case ElementType.TypedByRef :
                p.TypedByRef = true;
                p.ByRef = false;
                break;
            case ElementType.ByRef :
                p.TypedByRef = false;
                p.ByRef = true;
                p.Type = this.ReadType (data, start, out start);
                break;
            default :
                p.TypedByRef = false;
                p.ByRef = false;
                p.Type = this.ReadType (data, curs, out start);
                break;
            }
            return p;
        }

        private SigType ReadType (byte [] data, int pos, out int start)
        {
            start = pos;
            ElementType element = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            switch (element) {
            case ElementType.ValueType :
                VALUETYPE vt = new VALUETYPE ();
                vt.Type = Utilities.GetMetadataToken(CodedIndex.TypeDefOrRef,
                                                     (uint) Utilities.ReadCompressedInteger (data, start, out start));
                return vt;
            case ElementType.Class :
                CLASS c = new CLASS ();
                c.Type = Utilities.GetMetadataToken (CodedIndex.TypeDefOrRef,
                                                     (uint) Utilities.ReadCompressedInteger (data, start, out start));
                return c;
            case ElementType.Ptr :
                PTR p = new PTR ();
                int buf = start;
                int flag = Utilities.ReadCompressedInteger (data, start, out start);
                p.Void = (flag & (int) ElementType.Void) != 0;
                if (!p.Void)
                    start = buf;
                p.CustomMods = this.ReadCustomMods (data, start, out start);
                p.PtrType = this.ReadType (data, start, out start);
                return p;
            case ElementType.FnPtr :
                FNPTR fp = new FNPTR ();
                if ((data [pos] & 0x5) != 0) {
                    MethodRefSig mr = new MethodRefSig ((uint) pos);
                    mr.Accept (this);
                    fp.Method = mr;
                } else {
                    MethodDefSig md = new MethodDefSig ((uint) pos);
                    md.Accept (this);
                    fp.Method = md;
                }
                return fp;
            case ElementType.Array :
                ARRAY ary = new ARRAY ();
                ArrayShape shape = new ArrayShape ();
                ary.Type = this.ReadType (data, start, out start);
                shape.Rank = Utilities.ReadCompressedInteger (data, start, out start);
                shape.NumSizes = Utilities.ReadCompressedInteger (data, start, out start);
                shape.Sizes = new int [shape.NumSizes];
                for (int i = 0; i < shape.NumSizes; i++)
                    shape.Sizes [i] = Utilities.ReadCompressedInteger (data, start, out start);
                shape.NumLoBounds = Utilities.ReadCompressedInteger (data, start, out start);
                shape.LoBounds = new int [shape.NumLoBounds];
                for (int i = 0; i < shape.NumLoBounds; i++)
                    shape.LoBounds [i] = Utilities.ReadCompressedInteger (data, start, out start);
                ary.Shape = shape;
                return ary;
            case ElementType.SzArray :
                SZARRAY sa = new SZARRAY ();
                sa.Type = this.ReadType (data, start, out start);
                return sa;
            default :
                return new SigType (element);
            }
        }

        private CustomMod [] ReadCustomMods (byte [] data, int pos, out int start)
        {
            ArrayList cmods = new ArrayList ();
            start = pos;
            while (true) {
                int buf = start;
                ElementType flag = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
                start = buf;
                if (!((flag == ElementType.CModOpt) || (flag == ElementType.CModReqD)))
                    break;
                cmods.Add (this.ReadCustomMod (data, start, out start));
            }
            return cmods.ToArray (typeof (CustomMod)) as CustomMod [];
        }

        private CustomMod ReadCustomMod (byte [] data, int pos, out int start)
        {
            CustomMod cm = new CustomMod ();
            start = pos;
            ElementType cmod = (ElementType) Utilities.ReadCompressedInteger (data, start, out start);
            if (cmod == ElementType.CModOpt)
                cm.CMOD = CustomMod.CMODType.OPT;
            else if (cmod == ElementType.CModReqD)
                cm.CMOD = CustomMod.CMODType.REQD;
            else
                cm.CMOD = CustomMod.CMODType.None;
            cm.TypeDefOrRef = Utilities.GetMetadataToken (CodedIndex.TypeDefOrRef,
                                                          (uint) Utilities.ReadCompressedInteger (data, start, out start));
            return cm;
        }

        private CustomAttrib ReadCustomAttrib (byte [] data, int pos, IMethodReference ctor)
        {
            BinaryReader br = new BinaryReader (new MemoryStream (data, pos, data.Length - pos));
            CustomAttrib ca = new CustomAttrib (ctor);
            ca.Prolog = br.ReadUInt16 ();
            if (ca.Prolog != CustomAttrib.StdProlog)
                throw new MetadataFormatException ("Non standard prolog for custom attribute");

            ca.FixedArgs = new CustomAttrib.FixedArg [ca.Constructor.Parameters.Count];
            for (int i = 0; i < ca.FixedArgs.Length; i++)
                ca.FixedArgs [i] = this.ReadFixedArg (br);

            ca.NumNamed = br.ReadUInt16 ();
            ca.NamedArgs = new CustomAttrib.NamedArg [ca.NumNamed];
            for (int i = 0; i < ca.NumNamed; i++)
                ca.NamedArgs [i] = this.ReadNamedArg (br);

            return ca;
        }

        private CustomAttrib.FixedArg ReadFixedArg (BinaryReader br)
        {
            return null;
        }

        private CustomAttrib.NamedArg ReadNamedArg (BinaryReader br)
        {
            return null;
        }
    }
}
