namespace Mono.Cecil {

	public interface IGenericParameterConstraints {
		ITypeReference[] InterfaceConstraints { get; }
		ITypeReference ClassConstraint { get; }
		bool HasValueTypeConstraint { get; }
		bool HasReferenceTypeConstraint { get; }
		bool HasConstructorConstraint { get; }
	}
}
