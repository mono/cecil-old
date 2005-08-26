/*
 * Copyright (c) 2004, 2005 DotNetGuru and the individuals listed
 * on the ChangeLog entries.
 *
 * Authors :
 *   Jb Evain   (jbevain@gmail.com)
 *
 * This is a free software distributed under a MIT/X11 license
 * See LICENSE.MIT file for more details
 *
 *****************************************************************************/

namespace Mono.Cecil {

	using System;
	using SR = System.Reflection;
	using System.Security;

	using Mono.Cecil.Cil;

	public interface IReflectionFactories {

		ITypeFactory TypeFactory { get; }
		IFieldFactory FieldFactory { get; }
		IMethodFactory MethodFactory { get; }
		ICilFactory CilFactory { get; }
		IParamaterFactory ParameterFactory { get; }
		IEventFactory EventFactory { get; }
		IPropertyFactory PropertyFactory { get; }
		ICustomAttributeFactory CustomAttributeFactory { get; }
		ISecurityDeclarationFactory SecurityDeclarationFactory { get; }
	}

	public interface IMemberFactory {

		bool IsAttached (IMemberDefinition member);
	}

	public interface ITypeFactory : IMemberFactory {

		ITypeDefinition CreateType (string name);
		ITypeDefinition CreateType (string name, string ns);
		ITypeDefinition CreateType (string name, string ns, TypeAttributes attributes);
		ITypeDefinition CreateType (string name, string ns,
			TypeAttributes attributes, ITypeReference baseType);
		ITypeDefinition CreateType (string name, string ns,
			TypeAttributes attributes, Type baseType);

		ITypeDefinition CreateInterface (string name);
		ITypeDefinition CreateInterface (string name, string ns);
		ITypeDefinition CreateInterface (string name, string ns, TypeAttributes attributes);

		ITypeDefinition CreateValueType (string name);
		ITypeDefinition CreateValueType (string name, string ns);
		ITypeDefinition CreateValueType (string name, string ns, TypeAttributes attributes);

		ITypeDefinition CreateEnum (string name);
		ITypeDefinition CreateEnum (string name, string ns);
		ITypeDefinition CreateEnum (string name, string ns, TypeAttributes attributes);

		ITypeDefinition CreateNestedType (string name);
		ITypeDefinition CreateNestedType (string name, TypeAttributes attributes);
		ITypeDefinition CreateNestedType (string name,
			TypeAttributes attributes, ITypeReference baseType);
		ITypeDefinition CreateNestedType (string name,
			TypeAttributes attributes, Type baseType);

		IArrayType CreateArray (ITypeReference type);
		IArrayType CreateArray (Type type);

		IArrayDimension CreateArrayDimension (int lowerBound, int upperBound);

		IPointerType CreatePointer (ITypeReference type);
		IPointerType CreatePointer (Type type);

		ITypeDefinition CloneType (ITypeDefinition original);

		void MergeType (ITypeDefinition original, ITypeDefinition target);

		ITypeReference CreateTypeReference (string name, IMetadataScope scope, bool isValueType);
		ITypeReference CreateTypeReference (string name, string ns, IMetadataScope scope, bool isValueType);
	}

	public interface IFieldFactory : IMemberFactory {

		IFieldDefinition CreateField (string name, ITypeReference fieldType);
		IFieldDefinition CreateField (string name, Type fieldType);
		IFieldDefinition CreateField (string name, FieldAttributes attributes, ITypeReference fieldType);
		IFieldDefinition CreateField (string name, FieldAttributes attributes, Type fieldType);

		IFieldDefinition CloneField (IFieldDefinition original);

		IFieldReference CreateFieldReference (string name, ITypeReference declaringType, ITypeReference fieldType);
	}

	public interface IMethodFactory : IMemberFactory {

		IMethodDefinition CreateMethod (string name, MethodAttributes attributes);
		IMethodDefinition CreateMethod (string name, MethodAttributes attributes, ITypeReference retType);
		IMethodDefinition CreateMethod (string name, MethodAttributes attributes, Type retType);

		IMethodDefinition CreateConstructor ();
		IMethodDefinition CreateConstructor (MethodAttributes attributes);
		IMethodDefinition CreateStaticConstructor ();
		IMethodDefinition CreateStaticConstructor (MethodAttributes attributes);

		IMethodDefinition CreateGetMethod (IPropertyDefinition prop);
		IMethodDefinition CreateSetMethod (IPropertyDefinition prop);

		IMethodDefinition CreateAddMethod (IEventDefinition evt);
		IMethodDefinition CreateRemoveMethod (IEventDefinition evt);
		IMethodDefinition CreateInvokeMethod (IEventDefinition evt);

		IPInvokeInfo CreatePInvokeInfo (string entryPoint, string module, PInvokeAttributes attributes);
		IPInvokeInfo CreatePInvokeInfo (string entryPoint, IModuleReference module, PInvokeAttributes attributes);

		IMethodDefinition CloneMethod (IMethodDefinition original);

		IMethodReference CreateMethodReference (string name, ITypeReference declaringType, ITypeReference retType,
			ITypeReference [] parametersTypes, bool hasThis, bool explicitThis, MethodCallingConvention mcc);
	}

	public interface ICilFactory {

		ICilWorker CreateCilWorker (IMethodBody body);

		IVariableDefinition CreateVariableDefinition (ITypeReference type);
		IVariableDefinition CreateVariableDefinition (Type type);

		IExceptionHandler CreateExceptionHandler (ExceptionHandlerType kind);
	}

	public interface IParamaterFactory {

		IParameterDefinition CreateParameter (string name, ParamAttributes attributes, ITypeReference type);
		IParameterDefinition CreateParameter (string name, ParamAttributes attributes, Type type);
		IParameterDefinition CreateParameter (string name, ParamAttributes attributes,
			ITypeReference type, bool byRef);
		IParameterDefinition CreateParameter (string name, ParamAttributes attributes,
			Type type, bool byRef);
	}

	public interface IEventFactory : IMemberFactory {

		IEventDefinition CreateEvent (string name, ITypeReference eventType);
		IEventDefinition CreateEvent (string name, Type eventType);
		IEventDefinition CreateEvent (string name, EventAttributes attributes, ITypeReference eventType);
		IEventDefinition CreateEvent (string name, EventAttributes attributes, Type eventType);

		IEventDefinition CloneEvent (IEventDefinition original);
	}

	public interface IPropertyFactory : IMemberFactory {

		IPropertyDefinition CreateProperty (string name, ITypeReference propType);
		IPropertyDefinition CreateProperty (string name, Type propType);
		IPropertyDefinition CreateProperty (string name, PropertyAttributes attributes, ITypeReference propType);
		IPropertyDefinition CreateProperty (string name, PropertyAttributes attributes, Type propType);

		IPropertyDefinition CloneProperty (IPropertyDefinition prop);
	}

	public interface ICustomAttributeFactory {

		ICustomAttribute CreateCustomAttribute (IMethodReference ctor);
		ICustomAttribute CreateCustomAttribute (SR.ConstructorInfo ctor);
		ICustomAttribute CreateCustomAttribute (IMethodReference ctor, byte [] data);
		ICustomAttribute CreateCustomAttribute (SR.ConstructorInfo ctor, byte [] data);

		byte [] GetAsByteArray (ICustomAttribute ca);
	}

	public interface ISecurityDeclarationFactory {

		ISecurityDeclaration CreateSecurityDeclaration (SecurityAction action);
		ISecurityDeclaration CreateSecurityDeclaration (SecurityAction action, PermissionSet permSet);
		ISecurityDeclaration CreateSecurityDeclaration (SecurityAction action, byte [] declaration);

		byte [] GetAsByteArray (ISecurityDeclaration sec);
	}
}
