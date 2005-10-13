namespace Mono.Cecil {

	using System;

	using Mono.Cecil;
	using Mono.Cecil.Signatures;

	public sealed class TypeParameterType : TypeReference, ITypeParameterType, IGenericArgument {

		private int position;

		public int Position {
			get { return position; }
			set { position = value; }
		}

		public override string Name {
			get { return "!" + position; }
			set { throw new InvalidOperationException (); }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override string FullName {
			get { return Name; }
		}

		public TypeParameterType (int position)
			: base (string.Empty, string.Empty)
		{
			this.position = position;
		}
	}
}
