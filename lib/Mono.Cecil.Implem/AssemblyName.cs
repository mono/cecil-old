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

namespace Mono.Cecil.Implem {

	using System;
	using System.Security.Cryptography;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Metadata;

	internal class AssemblyNameReference : IAssemblyNameReference {

		private string m_name;
		private string m_culture;
		private Version m_version;
		private AssemblyFlags m_flags;
		private byte [] m_publicKey;
		private byte [] m_publicKeyToken;
		private AssemblyHashAlgorithm m_hashAlgo;
		private byte [] m_hash;

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public string Culture {
			get { return m_culture; }
			set {
				if (!CultureUtils.IsValid (value)) {
					throw new ArgumentException ("Culture");
				}
				m_culture = value;
			}
		}

		public Version Version {
			get { return m_version; }
			set { m_version = value; }
		}

		public AssemblyFlags Flags {
			get { return m_flags; }
			set { m_flags = value; }
		}

		public byte [] PublicKey {
			get { return m_publicKey; }
			set {
				m_publicKey = value;
				m_publicKeyToken = null;
			}
		}

		public byte [] PublicKeyToken {
			get {
				if ((m_publicKeyToken == null) && (m_publicKey != null)) {
					HashAlgorithm ha = null;
					switch (m_hashAlgo) {
					case AssemblyHashAlgorithm.Reserved:
						ha = MD5.Create (); break;
					default:
						// None default to SHA1
						ha = SHA1.Create (); break;
					}
					byte[] hash = ha.ComputeHash (m_publicKey);
					// we need the last 8 bytes in reverse order
					m_publicKeyToken = new byte [8];
					Array.Copy (hash, (hash.Length - 8), m_publicKeyToken, 0, 8);
					Array.Reverse (m_publicKeyToken, 0, 8);
				}
				return m_publicKeyToken;
			}
			set { m_publicKeyToken = value; }
		}

		public string FullName {
			get {
				StringBuilder sb = new StringBuilder ();
				string sep = ", ";
				sb.Append (m_name);
				if (m_version != null) {
					sb.Append (sep);
					sb.Append (m_version.ToString ());
				}
				sb.Append (sep);
				sb.Append ("Culture=");
				sb.Append (m_culture == string.Empty ? "neutral" : m_culture);
				sb.Append (sep);
				sb.Append ("PublicKeyToken=");
				if (this.PublicKeyToken != null && m_publicKeyToken.Length > 0) {
					for (int i = 0 ; i < m_publicKeyToken.Length ; i++) {
						sb.Append (m_publicKeyToken [i].ToString ("x2"));
					}
				} else {
					sb.Append ("null");
				}
				return sb.ToString ();
			}
		}

		public AssemblyHashAlgorithm HashAlgorithm {
			get { return m_hashAlgo; }
			set { m_hashAlgo = value; }
		}

		public virtual byte [] Hash {
			get { return m_hash; }
			set { m_hash = value; }
		}

		public AssemblyNameReference () : this(string.Empty, string.Empty, null)
		{
		}

		public AssemblyNameReference (string name, string culture, Version version)
		{
			if (name == null)
				throw new ArgumentException ("name");
			if (culture == null || !CultureUtils.IsValid (culture))
				throw new ArgumentException ("culture");
			m_name = name;
			m_culture = culture;
			m_version = version;
			m_hashAlgo = AssemblyHashAlgorithm.None;
		}

		public virtual void Accept (IReflectionStructureVisitor visitor)
		{
			visitor.Visit (this);
		}
	}

	internal class AssemblyNameDefinition : AssemblyNameReference, IAssemblyNameDefinition {

		public override byte [] Hash {
			get { return new byte [0]; }
		}

		public AssemblyNameDefinition () : base()
		{
		}

		public AssemblyNameDefinition (string name, string culture, Version version) : base(name, culture, version)
		{
		}

		public override void Accept (IReflectionStructureVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

