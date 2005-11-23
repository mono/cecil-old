namespace Mono.Cecil
{
	public interface IGenericArgumentProvider
	{
		GenericArgumentCollection GenericArguments { get; }
	}
}
