namespace Mono.Cecil {

	public interface IGenericInstance : ITypeReference {
		ITypeReference GenericType { get; }
		int Arity { get; }
		ITypeReference[] TypeArguments { get; }
	}
}
