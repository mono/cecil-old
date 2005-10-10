namespace Mono.Cecil.Signatures {

	using Mono.Cecil.Metadata;

	internal sealed class VAR : SigType {

		public int Index;

		public VAR () : base (ElementType.Var)
		{
		}
	}
}
