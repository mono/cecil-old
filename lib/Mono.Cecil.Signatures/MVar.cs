namespace Mono.Cecil.Signatures {

	using Mono.Cecil.Metadata;

	internal sealed class MVAR : SigType {

		public int Index;

		public MVAR () : base (ElementType.MVar)
		{
		}
	}
}
