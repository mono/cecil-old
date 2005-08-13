/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil.Signatures {

	using System;
	using System.Collections;
	using System.Text;

	using Mono.Cecil.Binary;
	using Mono.Cecil.Implem;
	using Mono.Cecil.Metadata;

	internal sealed class SignatureWriter : ISignatureVisitor {

		private MetadataWriter m_mdWriter;
		private ReflectionWriter m_reflectWriter;

		private MemoryBinaryWriter m_sigWriter;

		private IDictionary m_sigCache;

		public SignatureWriter (MetadataWriter mdWriter, ReflectionWriter reflectWriter)
		{
			m_mdWriter = mdWriter;
			m_reflectWriter = reflectWriter;

			m_sigWriter = new MemoryBinaryWriter ();

			m_sigCache = new Hashtable ();
		}

		void DebugSig (string msg)
		{
			Console.Write (msg);
			Console.Write (" [ ");
			byte [] sig = m_sigWriter.ToArray ();
			for (int i = 0; i < sig.Length; i++) {
				if (i > 0)
					Console.Write (", ");
				Console.Write (sig [i].ToString ("x2"));
			}
			Console.WriteLine (" ]");
		}

		private uint GetCachedPointer (bool withSize)
		{
			byte [] signature = m_sigWriter.ToArray ();
			string sig = Encoding.ASCII.GetString (signature);
			if (m_sigCache.Contains (sig))
				return (uint) m_sigCache [sig];

			uint p = m_mdWriter.AddBlob (signature, withSize);
			m_sigCache.Add (sig, p);
			return p;
		}

		public uint AddMethodDefSig (MethodDefSig methSig)
		{
			m_sigWriter.Empty ();
			Visit (methSig);
			return GetCachedPointer (true);
		}

		public uint AddMethodRefSig (MethodRefSig methSig)
		{
			m_sigWriter.Empty ();
			Visit (methSig);
			return GetCachedPointer (true);
		}

		public uint AddFieldSig (FieldSig fSig)
		{
			m_sigWriter.Empty ();
			Visit (fSig);
			return GetCachedPointer (true);
		}

		public void Visit (MethodDefSig methodDef)
		{
			m_sigWriter.Write ((byte) methodDef.CallingConvention);
			Utilities.WriteCompressedInteger (m_sigWriter, methodDef.ParamCount);
			Write (methodDef.RetType);
			Write (methodDef.Parameters);
		}

		public void Visit (MethodRefSig methodRef)
		{
			m_sigWriter.Write ((byte) methodRef.CallingConvention);
			Utilities.WriteCompressedInteger (m_sigWriter, methodRef.ParamCount);
			Write (methodRef.RetType);
			Write (methodRef.Parameters);
		}

		public void Visit (FieldSig field)
		{
			m_sigWriter.Write ((byte) field.CallingConvention);
			Write (field.CustomMods);
			Write (field.Type);
		}

		public void Visit (PropertySig property)
		{
			// TODO
		}

		public void Visit (LocalVarSig localvar)
		{
			// TODO
		}

		private void Write (RetType retType)
		{
			Write (retType.CustomMods);
			if (retType.Void)
				Write (ElementType.Void);
			else if (retType.TypedByRef)
				Write (ElementType.TypedByRef);
			else if (retType.ByRef) {
				Write (ElementType.ByRef);
				Write (retType.Type);
			} else
				Write (retType.Type);
		}

		private void Write (Param [] parameters)
		{
			foreach (Param p in parameters)
				Write (p);
		}

		private void Write (ElementType et)
		{
			Utilities.WriteCompressedInteger (m_sigWriter, (int) et);
		}

		private void Write (SigType t)
		{
			if (t is CLASS) {
				Write (ElementType.Class);
				CLASS k = t as CLASS;
				Utilities.WriteCompressedInteger (m_sigWriter,
					(int) Utilities.CompressMetadataToken (CodedIndex.TypeDefOrRef, k.Type));
			} else if (t is VALUETYPE) {
				// TODO and so on
			} else
				Write (t.ElementType);
		}

		private void Write (Param p)
		{
			Write (p.CustomMods);
			if (p.TypedByRef)
				Write (ElementType.TypedByRef);
			else if (p.ByRef) {
				Write (ElementType.ByRef);
				Write (p.Type);
			} else
				Write (p.Type);
		}

		private void Write (CustomMod [] customMods)
		{
			foreach (CustomMod cm in customMods)
				Write (cm);
		}

		private void Write (CustomMod cm)
		{
			switch (cm.CMOD) {
			case CustomMod.CMODType.OPT :
				Write (ElementType.CModOpt);
				break;
			case CustomMod.CMODType.REQD :
				Write (ElementType.CModReqD);
				break;
			}

			Utilities.WriteCompressedInteger (m_sigWriter,
				(int) Utilities.CompressMetadataToken (
					CodedIndex.TypeDefOrRef, cm.TypeDefOrRef));
		}
	}
}
