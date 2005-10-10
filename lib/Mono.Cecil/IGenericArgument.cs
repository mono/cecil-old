namespace Mono.Cecil {

	public interface IGenericArgument : ITypeReference
	{
		int Position { get; }
	}
}
