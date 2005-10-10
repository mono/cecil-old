namespace Mono.Cecil {

	using System;

	using Mono.Cecil;
	using Mono.Cecil.Signatures;

	public sealed class MethodTypeParameterType : TypeReference, IMethodTypeParameterType {

		private int index;

		public int Index {
			get { return index; }
			set { index = value; }
		}

		public override string Name {
			get { return "!!" + index; }
			set { throw new InvalidOperationException (); }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override string FullName {
			get { return Name; }
		}

		public MethodTypeParameterType (int index)
			: base (string.Empty, string.Empty)
		{
			this.index = index;
		}
	}
}
