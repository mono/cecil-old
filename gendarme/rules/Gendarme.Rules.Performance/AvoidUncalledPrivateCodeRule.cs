//
// Gendarme.Rules.Performance.AvoidUncalledPrivateCodeRule
//
// Authors:
//	Nidhi Rawal <sonu2404@gmail.com>
//
// Copyright (c) <2007> Nidhi Rawal
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Gendarme.Framework;

namespace Gendarme.Rules.Performance
{
	public class AvoidUncalledPrivateCodeRule: IMethodRule
	{
		public MessageCollection CheckMethod (MethodDefinition method, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection ();
			TypeDefinition type = method.DeclaringType as TypeDefinition;

			if (MemberIsCallable (type, method) && !MemberIsCalled (type, method)) {
				Location location = new Location (method.DeclaringType.FullName, method.Name, 0);
				Message message = new Message ("The private or internal code is not called", location, MessageType.Error);
				messageCollection.Add (message);
			}

			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}

		private bool MemberIsCalled (TypeDefinition type, MethodDefinition method)
		{
			bool isCalled = false;

			if (TypeIsInternal (type) && TypeIsPrivate (type) && MemberIsInternal (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (TypeIsInternal (type) && TypeIsPrivate (type) && MemberIsPrivate (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (TypeIsInternal (type) && !TypeIsPrivate (type) && MemberIsInternal (method))
				isCalled = InternalMemberIsCalled (method);
			if (TypeIsInternal (type) && !TypeIsPrivate (type) && MemberIsPrivate (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (!TypeIsInternal (type) && TypeIsPrivate (type) && MemberIsInternal (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (!TypeIsInternal (type) && TypeIsPrivate (type) && MemberIsPrivate (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (!TypeIsInternal (type) && !TypeIsPrivate (type) && MemberIsInternal (method))
				isCalled = InternalMemberIsCalled (method);
			if (!TypeIsInternal (type) && !TypeIsPrivate (type) && MemberIsPrivate (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (TypeIsInternal (type) && TypeIsPrivate (type) && !MemberIsInternal (method) && !MemberIsPrivate (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (TypeIsInternal (type) && !TypeIsPrivate (type) && !MemberIsInternal (method) && !MemberIsPrivate (method))
				isCalled = InternalMemberIsCalled (method);
			if (!TypeIsInternal (type) && TypeIsPrivate (type) && !MemberIsInternal (method) && !MemberIsPrivate (method))
				isCalled = PrivateMemberIsCalled (type, method);
			if (!TypeIsInternal (type) && !TypeIsPrivate (type) && !MemberIsInternal (method) && !MemberIsPrivate (method))
				isCalled = true;

			return isCalled;
		}

		private bool PrivateMemberIsCalled (TypeDefinition type, MethodDefinition ToBeCalledMethod)
		{
			bool isCalled = false;

			foreach (MethodDefinition method in type.Methods)
				if (method.HasBody)
					foreach (Instruction instruction in method.Body.Instructions)
						if (instruction.Operand != null && instruction.Operand.ToString () == ToBeCalledMethod.ToString ())
							isCalled = true;

			if (type.NestedTypes.Count > 0)
				foreach (TypeDefinition nestedType in type.NestedTypes)
					isCalled = PrivateMemberIsCalled (nestedType, ToBeCalledMethod);

			return isCalled;
		}

		private bool InternalMemberIsCalled (MethodDefinition ToBeCalledMethod)
		{
			bool isCalled = false;

			foreach (ModuleDefinition module in ToBeCalledMethod.DeclaringType.Module.Assembly.Modules)
				foreach (TypeDefinition type in module.Types)
					foreach (MethodDefinition method in type.Methods)
						if (method.HasBody)
							foreach (Instruction instruction in method.Body.Instructions)
								if (instruction.Operand != null && instruction.Operand.ToString () == ToBeCalledMethod.ToString ())
									isCalled = true;

			return isCalled;
		}

		private bool TypeIsInternal (TypeDefinition type)
		{
			bool isInternalType = true;
			string [] modifier = type.Attributes.ToString ().Split (',');

			if (modifier [0] == "NestedAssembly" || modifier [0] == "NestedFamANDAssem" || modifier [0] == "NestedFamORAssem")
				isInternalType = true;
			for (int i = 0; i < modifier.Length; i++)
				if (modifier [i] == "Public" || modifier [i] == "Private" || modifier [i] == "NestedPublic" || modifier [i] == "NestedPrivate" || modifier [i] == "NestedFamily")
					isInternalType = false;

			if (isInternalType)
				return isInternalType;
			else if (type.DeclaringType != null)
					isInternalType = TypeIsInternal (type.DeclaringType as TypeDefinition);
			return isInternalType;
		}

		private bool TypeIsPrivate (TypeDefinition type)
		{
			bool isPrivateType = false;
			string [] modifier = type.Attributes.ToString ().Split (',');

			for (int i = 0; i < modifier.Length; i++)
				if (modifier [i] == "NestedPrivate")
					isPrivateType = true;

			if (isPrivateType)
				return isPrivateType;
			else if (type.DeclaringType != null)
					isPrivateType = TypeIsPrivate (type.DeclaringType as TypeDefinition);
			return isPrivateType;
		}

		private bool MemberIsInternal (MethodDefinition method)
		{
			bool isInternalMember = false;
			string [] modifier = method.Attributes.ToString ().Split (',');

			if (modifier [0] == "Assem")
				isInternalMember = true;

			return isInternalMember;
		}

		private bool MemberIsPrivate (MethodDefinition method)
		{
			bool isPrivateMember = false;
			string [] modifier = method.Attributes.ToString ().Split (',');

			if (modifier [0] == "Private")
				isPrivateMember = true;

			return isPrivateMember;
		}

		private bool MemberIsCallable (TypeDefinition type, MethodDefinition method)
		{
			bool isCallable = true;
			AssemblyDefinition assemblyDefinition = method.DeclaringType.Module.Assembly;

			foreach (CustomAttribute customAttribute in method.CustomAttributes)
				if (customAttribute.Constructor.DeclaringType.FullName == "System.Runtime.InteropServices.ComRegisterFunctionAttribute" || customAttribute.Constructor.DeclaringType.FullName == "System.Runtime.InteropServices.ComUnregisterFunctionAttribute")
					isCallable = false;
			if (type.IsInterface)
				isCallable = false;
			if (method.Attributes.ToString () == "Private, Final, Virtual, HideBySig, NewSlot")
				isCallable = false;
			if (method.IsStatic && method.IsConstructor)
				isCallable = false;
			if (method == assemblyDefinition.EntryPoint)
				isCallable = false;
			if (method.IsConstructor && method.Parameters.Count == 2)
				if (method.Parameters [0].ParameterType.Name == "System.Runtime.Serialization.SerializationInfo" && method.Parameters [0].ParameterType.Name == "System.Runtime.Serialization.StreamingContext")
					isCallable = false;
			if (method.IsVirtual && !method.IsNewSlot)
				isCallable = false;

			return isCallable;
		}
	}
}
			