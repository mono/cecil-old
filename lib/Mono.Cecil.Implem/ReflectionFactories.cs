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

namespace Mono.Cecil.Implem {

	using System;
	using System.Collections;
	using SR = System.Reflection;
	using SS = System.Security;
	using SSP = System.Security.Permissions;
	using System.Text;

	using Mono.Cecil;
	using Mono.Cecil.Binary;
	using Mono.Cecil.Cil;
	using Mono.Xml;

	internal sealed class ReflectionFactories : IReflectionFactories, ITypeFactory, IFieldFactory, IMethodFactory,
		IParamaterFactory, IEventFactory, IPropertyFactory, ICustomAttributeFactory, ISecurityDeclarationFactory,
		ICilFactory {

		private ModuleDefinition m_module;
		private SecurityParser m_secParser;

		public ITypeFactory TypeFactory {
			get { return this; }
		}

		public IFieldFactory FieldFactory {
			get { return this; }
		}

		public IMethodFactory MethodFactory {
			get { return this; }
		}

		public ICilFactory CilFactory {
			get { return this; }
		}

		public IParamaterFactory ParameterFactory {
			get { return this; }
		}

		public IEventFactory EventFactory {
			get { return this; }
		}

		public IPropertyFactory PropertyFactory {
			get { return this; }
		}

		public ICustomAttributeFactory CustomAttributeFactory {
			get { return this; }
		}

		public ISecurityDeclarationFactory SecurityDeclarationFactory {
			get { return this; }
		}

		public ReflectionFactories (ModuleDefinition module)
		{
			m_module = module;
			m_secParser = new SecurityParser ();
		}

		public bool IsAttached (IMemberDefinition member)
		{
			return member.DeclaringType != null;
		}

		public ITypeDefinition CreateType (string name)
		{
			return CreateType (name, string.Empty);
		}

		public ITypeDefinition CreateType (string name, string ns)
		{
			return CreateType (name, ns, (TypeAttributes) 0);
		}

		public ITypeDefinition CreateType (string name, string ns, TypeAttributes attributes)
		{
			return CreateType (name, ns, attributes, typeof (object));
		}

		public ITypeDefinition CreateType (string name, string ns, TypeAttributes attributes, ITypeReference baseType)
		{
			TypeDefinition t = new TypeDefinition (name, ns, attributes);
			t.BaseType = m_module.Controller.Helper.CheckType (baseType);
			return t;
		}

		public ITypeDefinition CreateType (string name, string ns, TypeAttributes attributes, Type baseType)
		{
			TypeDefinition t = new TypeDefinition (name, ns, attributes);
			t.BaseType = m_module.Controller.Helper.CheckType (baseType);
			return t;
		}

		public ITypeDefinition CreateInterface (string name)
		{
			return CreateInterface (name, string.Empty);
		}

		public ITypeDefinition CreateInterface (string name, string ns)
		{
			return CreateInterface (name, ns, (TypeAttributes) 0);
		}

		public ITypeDefinition CreateInterface (string name, string ns, TypeAttributes attributes)
		{
			return CreateType (name, ns, attributes | TypeAttributes.Interface);
		}

		public ITypeDefinition CreateValueType (string name)
		{
			return CreateValueType (name, string.Empty);
		}

		public ITypeDefinition CreateValueType (string name, string ns)
		{
			return CreateValueType (name, ns, (TypeAttributes) 0);
		}

		public ITypeDefinition CreateValueType (string name, string ns, TypeAttributes attributes)
		{
			return CreateType (name, ns, attributes, typeof (ValueType));
		}

		public ITypeDefinition CreateEnum (string name)
		{
			return CreateEnum (name, string.Empty);
		}

		public ITypeDefinition CreateEnum (string name, string ns)
		{
			return CreateEnum (name, ns, (TypeAttributes) 0);
		}

		public ITypeDefinition CreateEnum (string name, string ns, TypeAttributes attributes)
		{
			return CreateType (name, ns, attributes, typeof (Enum));
		}

		public ITypeDefinition CreateNestedType (string name)
		{
			return CreateNestedType (name, (TypeAttributes) 0);
		}

		public ITypeDefinition CreateNestedType (string name, TypeAttributes attributes)
		{
			return CreateType (name, string.Empty, attributes);
		}

		public ITypeDefinition CreateNestedType (string name, TypeAttributes attributes, ITypeReference baseType)
		{
			return CreateType (name, string.Empty, attributes, baseType);
		}

		public ITypeDefinition CreateNestedType (string name, TypeAttributes attributes, Type baseType)
		{
			return CreateType (name, string.Empty, attributes, baseType);
		}

		public IArrayType CreateArray (ITypeReference type)
		{
			return new ArrayType (m_module.Controller.Helper.CheckType (type));
		}

		public IArrayType CreateArray (Type type)
		{
			return new ArrayType (m_module.Controller.Helper.CheckType (type));
		}

		public IArrayDimension CreateArrayDimension (int lowerBound, int upperBound)
		{
			return new ArrayDimension (lowerBound, upperBound);
		}

		public IPointerType CreatePointer (ITypeReference type)
		{
			return new PointerType (m_module.Controller.Helper.CheckType (type));
		}

		public IPointerType CreatePointer (Type type)
		{
			return CreatePointer (m_module.Controller.Helper.CheckType (type));
		}

		public IReferenceType CreateReferenceType (ITypeReference type)
		{
			return new ReferenceType (m_module.Controller.Helper.CheckType (type));
		}

		public IReferenceType CreateReferenceType (Type type)
		{
			return new ReferenceType (m_module.Controller.Helper.CheckType (type));
		}

		public ITypeDefinition CloneType (ITypeDefinition original)
		{
			TypeDefinition nt = new TypeDefinition (
				original.Name, original.Namespace, original.Attributes);
			nt.BaseType = m_module.Controller.Helper.CheckType (original.BaseType);
			foreach (FieldDefinition field in original.Fields)
				nt.Fields.Add (CloneField (field));
			foreach (IMethodDefinition ctor in original.Constructors)
				nt.Constructors.Add (CloneMethod (ctor));
			foreach (MethodDefinition meth in original.Methods)
				nt.Methods.Add (CloneMethod (meth));
			foreach (EventDefinition evt in original.Events)
				nt.Events.Add (CloneEvent (evt));
			foreach (PropertyDefinition prop in original.Properties)
				nt.Properties.Add (CloneProperty (prop));
			foreach (ITypeReference intf in original.Interfaces)
				nt.Interfaces.Add (m_module.Controller.Helper.CheckType (intf));
			foreach (ITypeDefinition nested in original.NestedTypes)
				nt.NestedTypes.Add (m_module.Controller.Helper.CheckType (nested));

			CloneCustomAttributes (original.CustomAttributes, nt.CustomAttributes);
			CloneSecurityDeclarations (original.SecurityDeclarations, nt.SecurityDeclarations);
			if (original.LayoutInfo.HasLayoutInfo) {
				nt.LayoutInfo.ClassSize = original.LayoutInfo.ClassSize;
				nt.LayoutInfo.PackingSize = original.LayoutInfo.PackingSize;
			}

			return nt;
		}

		public void MergeType (ITypeDefinition original, ITypeDefinition target)
		{
			throw new NotImplementedException ("TODO"); // TODO
		}

		public ITypeReference CreateTypeReference (string name, IMetadataScope scope, bool isValueType)
		{
			return CreateTypeReference (name, string.Empty, scope, isValueType);
		}

		public ITypeReference CreateTypeReference (string name, string ns, IMetadataScope scope, bool isValueType)
		{
			TypeReference t = new TypeReference (name, ns, scope);
			t.IsValueType = isValueType;
			return t;
		}

		public IFieldDefinition CreateField (string name, ITypeReference fieldType)
		{
			return CreateField (name, (FieldAttributes) 0, fieldType);
		}

		public IFieldDefinition CreateField (string name, Type fieldType)
		{
			return CreateField (name, m_module.Controller.Helper.CheckType (fieldType));
		}

		public IFieldDefinition CreateField (string name, FieldAttributes attributes, ITypeReference fieldType)
		{
			return new FieldDefinition (name, fieldType, attributes);
		}

		public IFieldDefinition CreateField (string name, FieldAttributes attributes, Type fieldType)
		{
			return CreateField (name, attributes, m_module.Controller.Helper.CheckType (fieldType));
		}

		public IFieldDefinition CloneField (IFieldDefinition field)
		{
			FieldDefinition nf = new FieldDefinition (
				field.Name, m_module.Controller.Helper.CheckType (field.FieldType), field.Attributes);
			if (field.HasConstant)
				nf.Constant = field.Constant;
			if (field.MarshalSpec != null)
				nf.MarshalSpec = field.MarshalSpec;
			if (field.RVA != RVA.Zero)
				nf.InitialValue = field.InitialValue;
			else
				nf.InitialValue = new byte [0];
			if (field.LayoutInfo.HasLayoutInfo)
				nf.LayoutInfo.Offset = field.LayoutInfo.Offset;
			CloneCustomAttributes (field.CustomAttributes, nf.CustomAttributes);
			return nf;
		}

		public IFieldReference CreateFieldReference (string name, ITypeReference decType, ITypeReference fType)
		{
			FieldReference fr = new FieldReference (name, fType);
			fr.DeclaringType = decType;
			return fr;
		}

		public IMethodDefinition CreateMethod (string name, MethodAttributes attributes)
		{
			return CreateMethod (name, attributes, typeof (void));
		}

		public IMethodDefinition CreateMethod (string name, MethodAttributes attributes, ITypeReference retType)
		{
			MethodDefinition meth = new MethodDefinition (name, attributes);
			meth.ReturnType.ReturnType = m_module.Controller.Helper.CheckType (retType);
			return meth;
		}

		public IMethodDefinition CreateMethod (string name, MethodAttributes attributes, Type retType)
		{
			return CreateMethod (name, attributes, m_module.Controller.Helper.CheckType (retType));
		}

		public IMethodDefinition CreateConstructor ()
		{
			return CreateConstructor ((MethodAttributes) 0);
		}

		public IMethodDefinition CreateConstructor (MethodAttributes attributes)
		{
			bool isStatic = (MethodAttributes.Static & attributes) != 0;
			return CreateMethod (isStatic ? MethodDefinition.Cctor : MethodDefinition.Ctor,
				attributes | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName);
		}

		public IMethodDefinition CreateStaticConstructor ()
		{
			return CreateStaticConstructor ((MethodAttributes) 0);
		}

		public IMethodDefinition CreateStaticConstructor (MethodAttributes attributes)
		{
			return CreateConstructor (attributes | MethodAttributes.Static);
		}

		public IMethodDefinition CreateGetMethod (IPropertyDefinition prop)
		{
			IMethodDefinition get = CreateMethod (
				string.Concat ("get_", prop.Name), (MethodAttributes) 0,
				m_module.Controller.Helper.CheckType (prop.PropertyType));
			prop.GetMethod = get;
			return get;
		}

		public IMethodDefinition CreateSetMethod (IPropertyDefinition prop)
		{
			IMethodDefinition set = CreateMethod (
				string.Concat ("set_", prop.Name), (MethodAttributes) 0,
				m_module.Controller.Helper.CheckType (prop.PropertyType));
			prop.SetMethod = set;
			return set;
		}

		public IMethodDefinition CreateAddMethod (IEventDefinition evt)
		{
			IMethodDefinition add = CreateMethod (
				string.Concat ("add_", evt.Name), (MethodAttributes) 0,
				m_module.Controller.Helper.CheckType (evt.EventType));
			evt.AddMethod = add;
			return add;
		}

		public IMethodDefinition CreateRemoveMethod (IEventDefinition evt)
		{
			IMethodDefinition remove = CreateMethod (
				string.Concat ("remove_", evt.Name), (MethodAttributes) 0,
				m_module.Controller.Helper.CheckType (evt.EventType));
			evt.RemoveMethod = remove;
			return remove;
		}

		public IMethodDefinition CreateInvokeMethod (IEventDefinition evt)
		{
			IMethodDefinition raise = CreateMethod (
				string.Concat ("raise_", evt.Name), (MethodAttributes) 0,
				m_module.Controller.Helper.CheckType (evt.EventType));
			evt.InvokeMethod = raise;
			return raise;
		}

		public IPInvokeInfo CreatePInvokeInfo (string entryPoint, string module, PInvokeAttributes attributes)
		{
			return CreatePInvokeInfo (entryPoint, new ModuleReference (module), attributes);
		}

		public IPInvokeInfo CreatePInvokeInfo (string entryPoint, IModuleReference module, PInvokeAttributes attributes)
		{
			return new PInvokeInfo (null, attributes, entryPoint, module);
		}

		public IMethodDefinition CloneMethod (IMethodDefinition original)
		{
			throw new NotImplementedException ("TODO"); // TODO
		}

		public IMethodReference CreateMethodReference (string name, ITypeReference decType, ITypeReference retType,
			ITypeReference [] pTypes, bool hasThis, bool explicitThis, MethodCallingConvention mcc)
		{
			MethodReference mr = new MethodReference (name, hasThis, explicitThis, mcc);
			mr.DeclaringType = decType;
			mr.ReturnType.ReturnType = retType;
			foreach (ITypeReference pt in pTypes)
				mr.Parameters.Add (new ParameterDefinition (pt));
			return mr;
		}

		public ICilWorker CreateCilWorker (IMethodBody body)
		{
			MethodBody bdy = body as MethodBody;
			CilWorker cw = new CilWorker (bdy, m_module);
			bdy.CilWorker = cw;
			return cw;
		}

		public IVariableDefinition CreateVariableDefinition (ITypeReference type)
		{
			return new VariableDefinition (type);
		}

		public IVariableDefinition CreateVariableDefinition (Type type)
		{
			return CreateVariableDefinition (m_module.Controller.Helper.CheckType (type));
		}

		public IExceptionHandler CreateExceptionHandler (ExceptionHandlerType kind)
		{
			return new ExceptionHandler (kind);
		}

		public IParameterDefinition CreateParameter (string name, ParamAttributes attributes, ITypeReference type)
		{
			return new ParameterDefinition (name, -1, attributes, m_module.Controller.Helper.CheckType (type));
		}

		public IParameterDefinition CreateParameter (string name, ParamAttributes attributes, Type type)
		{
			return CreateParameter (name, attributes, m_module.Controller.Helper.CheckType (type));
		}

		public IEventDefinition CreateEvent (string name, ITypeReference eventType)
		{
			return CreateEvent (name, (EventAttributes) 0, eventType);
		}

		public IEventDefinition CreateEvent (string name, Type eventType)
		{
			return CreateEvent (name, m_module.Controller.Helper.CheckType (eventType));
		}

		public IEventDefinition CreateEvent (string name, EventAttributes attributes, ITypeReference eventType)
		{
			return new EventDefinition (name, m_module.Controller.Helper.CheckType (eventType), attributes);
		}

		public IEventDefinition CreateEvent (string name, EventAttributes attributes, Type eventType)
		{
			return CreateEvent (name, attributes, m_module.Controller.Helper.CheckType (eventType));
		}

		public IEventDefinition CloneEvent (IEventDefinition evt)
		{
			EventDefinition ne = new EventDefinition (
				evt.Name, m_module.Controller.Helper.CheckType (evt.EventType), evt.Attributes);
			ne.AddMethod = evt.AddMethod;
			ne.InvokeMethod = evt.InvokeMethod;
			ne.RemoveMethod = evt.RemoveMethod;
			CloneCustomAttributes (evt.CustomAttributes, ne.CustomAttributes);
			return ne;
		}

		public IPropertyDefinition CreateProperty (string name, ITypeReference propType)
		{
			return CreateProperty (name, (PropertyAttributes) 0, propType);
		}

		public IPropertyDefinition CreateProperty (string name, Type propType)
		{
			return CreateProperty (name, m_module.Controller.Helper.CheckType (propType));
		}

		public IPropertyDefinition CreateProperty (string name, PropertyAttributes attributes, ITypeReference propType)
		{
			return new PropertyDefinition (name, m_module.Controller.Helper.CheckType (propType), attributes);
		}

		public IPropertyDefinition CreateProperty (string name, PropertyAttributes attributes, Type propType)
		{
			return CreateProperty (name, attributes, m_module.Controller.Helper.CheckType (propType));
		}

		public IPropertyDefinition CloneProperty (IPropertyDefinition prop)
		{
			PropertyDefinition np = new PropertyDefinition (
				prop.Name, m_module.Controller.Helper.CheckType (prop.PropertyType), prop.Attributes);
			if (prop.HasConstant)
				np.Constant = prop.Constant;
			np.GetMethod = prop.GetMethod;
			np.SetMethod = prop.SetMethod;
			CloneCustomAttributes (prop.CustomAttributes, np.CustomAttributes);
			return np;
		}

		public ICustomAttribute CreateCustomAttribute (IMethodReference ctor)
		{
			return new CustomAttribute (ctor);
		}

		public ICustomAttribute CreateCustomAttribute (SR.ConstructorInfo ctor)
		{
			return new CustomAttribute (m_module.Controller.Helper.CheckConstructor (ctor));
		}

		public ICustomAttribute CreateCustomAttribute (IMethodReference ctor, byte [] data)
		{
			return m_module.Controller.Reader.GetCustomAttribute (ctor, data);
		}

		public ICustomAttribute CreateCustomAttribute (SR.ConstructorInfo ctor, byte [] data)
		{
			return m_module.Controller.Reader.GetCustomAttribute (
				m_module.Controller.Helper.CheckConstructor (ctor), data);
		}

		public byte [] GetAsByteArray (ICustomAttribute ca)
		{
			CustomAttribute customAttr = ca as CustomAttribute;
			if (!ca.IsReadable)
				if (customAttr.Blob != null)
					return customAttr.Blob;
				else
					return new byte [0];

			return m_module.Controller.Writer.SignatureWriter.CompressCustomAttribute (
				m_module.Controller.Writer.GetCustomAttributeSig (ca), ca.Constructor);
		}

		public ISecurityDeclaration CreateSecurityDeclaration (SecurityAction action)
		{
			return new SecurityDeclaration (action);
		}

		public ISecurityDeclaration CreateSecurityDeclaration (SecurityAction action, SS.PermissionSet permSet)
		{
			SecurityDeclaration sec = new SecurityDeclaration (action);
			sec.PermissionSet = permSet;
			return sec;
		}

		public ISecurityDeclaration CreateSecurityDeclaration (SecurityAction action, byte [] declaration)
		{
			SecurityDeclaration dec = new SecurityDeclaration (action);
			SS.PermissionSet ps = new SS.PermissionSet (SSP.PermissionState.None);
			m_secParser.LoadXml (Encoding.Unicode.GetString (declaration));
			ps.FromXml (m_secParser.ToXml ());
			dec.PermissionSet = ps;
			return dec;
		}

		public byte [] GetAsByteArray (ISecurityDeclaration sec)
		{
			if (sec.PermissionSet != null)
				return Encoding.Unicode.GetBytes (sec.PermissionSet.ToXml ().ToString ());

			return new byte [0];
		}

		private void CloneCustomAttributes (ICustomAttributeCollection original,
			ICustomAttributeCollection target)
		{
			foreach (ICustomAttribute ori in original) {
				CustomAttribute ca = new CustomAttribute (ori.Constructor);
				foreach (object o in ori.ConstructorParameters)
					ca.ConstructorParameters.Add (o);
				foreach (DictionaryEntry de in ori.Fields)
					ca.Fields.Add (de.Key, de.Value);
				foreach (DictionaryEntry de in ori.Properties)
					ca.Properties.Add (de.Key, de.Value);
				target.Add (ca);
			}
		}

		private void CloneSecurityDeclarations (ISecurityDeclarationCollection original,
			ISecurityDeclarationCollection target)
		{
			foreach (ISecurityDeclaration ori in original) {
				SecurityDeclaration sd = new SecurityDeclaration (ori.Action);
				sd.PermissionSet = ori.PermissionSet;
				target.Add (sd);
			}
		}
	}
}
