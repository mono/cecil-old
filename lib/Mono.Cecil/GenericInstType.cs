namespace Mono.Cecil {

	using System;

	using Mono.Cecil;
	using Mono.Cecil.Signatures;

	public sealed class GenericInstType : TypeReference, IGenericInstance {

		private TypeReference type;
		private TypeReference[] type_argv;

		public ITypeReference GenericType {
			get { return type; }
		}

		public int Arity {
			get { return type_argv.Length; }
		}

		public ITypeReference[] TypeArguments {
			get { return type_argv; }
		}

		public override string Name {
			get { return type.Name + "`" + Arity; }
			set { throw new InvalidOperationException (); }
		}

		public override string Namespace {
			get { return string.Empty; }
			set { throw new InvalidOperationException (); }
		}

		public override string FullName {
			get { return Name; }
		}

		public GenericInstType (TypeReference type, TypeReference[] type_argv)
			: base (string.Empty, string.Empty)
		{
			this.type = type;
			this.type_argv = type_argv;
		}
	}
}
