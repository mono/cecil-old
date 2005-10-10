namespace Mono.Cecil.Signatures {

	using Mono.Cecil.Metadata;

	internal sealed class GENERICINST : SigType {

		public SigType Type;
		public GenericInstSignature Signature;

		public GENERICINST () : base (ElementType.GenericInst)
		{
		}
	}
}
