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

	using Mono.Cecil;
	using Mono.Cecil.Signatures;

	internal sealed class FunctionPointerType : TypeReference, IFunctionPointerType {

		private MethodReference m_function;

		public bool HasThis {
			get { return m_function.HasThis; }
			set { m_function.HasThis = value; }
		}

		public bool ExplicitThis {
			get { return m_function.ExplicitThis; }
			set { m_function.ExplicitThis = value; }
		}

		public MethodCallingConvention CallingConvention {
			get { return m_function.CallingConvention; }
			set { m_function.CallingConvention = value; }
		}

		public IParameterDefinitionCollection Parameters {
			get { return m_function.Parameters; }
		}

		public IMethodReturnType ReturnType {
			get { return m_function.ReturnType; }
			set { m_function.ReturnType = value; }
		}

		public override string Name {
			get { return m_function.Name; }
			set { throw new InvalidOperationException (); }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override IMetadataScope Scope {
			get { return m_function.DeclaringType.Scope; }
		}

		public override string FullName {
			get { return m_function.ToString (); }
		}

		public FunctionPointerType (bool hasThis, bool explicitThis, MethodCallingConvention callConv,
								ParameterDefinitionCollection parameters, MethodReturnType retType) : base (string.Empty, string.Empty)
		{
			m_function = new MethodReference ("function", this, hasThis, explicitThis, callConv);
			m_function.ReturnType = retType;
			foreach (ParameterDefinition param in parameters)
				(m_function.Parameters as ParameterDefinitionCollection).Add (param);
		}
	}
}
