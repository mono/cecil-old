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
    using System.Text;

    using Mono.Cecil;
    using Mono.Cecil.Metadata;

    internal class AssemblyName : IAssemblyName {

        private string m_name;
        private string m_culture;
        private Version m_version;
        private byte[] m_publicKey;
        private byte[] m_publicKeyToken;

        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        public string Culture {
            get { return m_culture; }
            set {
                if (!CultureUtils.IsValid(value)) {
                    throw new ArgumentException("Culture");
                }
                m_culture = value;
            }
        }

        public Version Version {
            get { return m_version; }
            set { m_version = value; }
        }

        public byte[] PublicKey {
            get { return m_publicKey; }
            set { m_publicKey = value; }
        }

        public byte[] PublicKeyToken {
            get { return m_publicKeyToken; }
            set { m_publicKeyToken = value; }
        }

        public string FullName {
            get {
                StringBuilder sb = new StringBuilder();
                string sep = ", ";
                sb.Append(m_name);
                if (m_version != null) {
                    sb.Append(sep);
                    sb.Append(m_version.ToString());
                }
                sb.Append(sep);
                sb.Append("Culture=");
                sb.Append(m_culture == string.Empty ? "neutral" : m_culture);
                sb.Append(sep);
                sb.Append("PublicKeyToken=");
                if (m_publicKeyToken != null && m_publicKeyToken.Length > 0) {
                    for (int i = 0 ; i < m_publicKeyToken.Length ; i++) {
                        sb.Append(m_publicKeyToken[i].ToString("x2"));
                    }
                } else {
                    sb.Append("null");
                }
                return sb.ToString();
            }
        }

        public AssemblyName() : this(string.Empty, string.Empty, null) {}

        public AssemblyName(string name, string culture, Version version) {
            if (name == null) {
                throw new ArgumentException("name");
            }
            if (culture == null || !CultureUtils.IsValid(culture)) {
                throw new ArgumentException("culture");
            }
            m_name = name;
            m_culture = culture;
            m_version = version;
        }

        public virtual void Accept(IReflectionStructureVisitor visitor) {
            visitor.Visit(this);
        }
    }

    internal class AssemblyNameReference : AssemblyName, IAssemblyNameReference {

        public AssemblyNameReference() : base() {}
        public AssemblyNameReference(string name, string culture, Version version) : base(name, culture, version) {}

        public override void Accept(IReflectionStructureVisitor visitor) {
            visitor.Visit(this);
        }
    }
}

