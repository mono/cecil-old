//
// BaseMergeReflectionVisitor.cs
//
// Author:
//   Alex Prudkiy (prudkiy@gmail.com)
//
// (C) 2006 Alex Prudkiy
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace Mono.Merge {

	/// <summary>
	/// Base classes for visitor
	/// </summary>
	public class BaseMergeReflectionVisitor : BaseReflectionVisitor {

		MergeContext context;
		AssemblyDefinition target;
		AssemblyDefinition source;
		ModuleDefinition module;

		public MergeContext Context
		{
			get { return context; }
		}

		public AssemblyDefinition Target
		{
			get { return target; }
		}

		public AssemblyDefinition Source
		{
			get { return source; }
		}

		public BaseMergeReflectionVisitor (MergeContext context, AssemblyDefinition target, AssemblyDefinition source)
		{
			this.context = context;
			this.target = target;
			this.source = source;
			module = target.MainModule;
		}

		public TypeReference ResolveType (string FullName, bool throwError)
		{
			if (module.Types.Contains (FullName))
				return module.Types [FullName];
			else if (module.TypeReferences.Contains (FullName))
				return module.TypeReferences [FullName];
			else if (throwError)
				throw new ArgumentException ("Can't found type : " + FullName);
			return null;
		}

		public TypeReference GetTypeReference (TypeReference type)
		{
			return GetTypeReference (type, true);
		}

		public TypeReference GetTypeDefinition (TypeDefinitionCollection members, TypeReference member)
		{
			foreach (TypeReference mr in members) {
				if ((mr.FullName == member.FullName)
				    && ((mr.DeclaringType == null && member.DeclaringType == null)
				        || (mr.DeclaringType != null && member.DeclaringType != null
				            && mr.DeclaringType.FullName == member.DeclaringType.FullName
				           )
				       )
					)
					return mr;
			}
			return null;
		}

		public TypeReference GetTypeReference (TypeReferenceCollection members, TypeReference member)
		{
			foreach (TypeReference mr in members) {
				if ((mr.FullName == member.FullName) &&
					((mr.DeclaringType == null && member.DeclaringType == null)
					||
					(mr.DeclaringType != null && member.DeclaringType != null
					&& mr.DeclaringType.FullName == member.DeclaringType.FullName)))

					return mr;
			}
			return null;
		}

		SortedDictionary<string, GenericInstanceType> generictypes_cache =
			new SortedDictionary<string, GenericInstanceType> ();

		public SortedDictionary<string, GenericInstanceType> Generics
		{
			get { return generictypes_cache; }
		}

		public TypeReference GetTypeReference (TypeReference type, bool throwError)
		{
			if (type is GenericInstanceType) {
				GenericInstanceType res;
				if (!Generics.TryGetValue (type.FullName, out res)) {
					res = type as GenericInstanceType;
					for (int i = 0; i < res.GenericArguments.Count; i++) {
						TypeReference tr = GetTypeReference (res.GenericArguments [i], false);
						if (tr != null)
							res.GenericArguments [i] = tr;
					}

					if (res.DeclaringType != null)
						res.DeclaringType = GetTypeReference (res.ElementType);

					if (res.ElementType != null)
						res.ElementType = GetTypeReference (res.ElementType);

					Generics [res.FullName] = res;
				}
				return res;
			} else if (type is ArrayType) {
				(type as ArrayType).ElementType = GetTypeReference ((type as ArrayType).ElementType);
				return type;
			} else if (type is ReferenceType) {
				(type as ReferenceType).ElementType = GetTypeReference ((type as ReferenceType).ElementType);
				return type;
			} else if (type is GenericParameter)
				return type; //TODO: needed to be checked
			else if (type == null && !throwError)
				return null;
			else if (type.DeclaringType != null) {
				TypeReference result = GetTypeDefinition (target.MainModule.Types, type);
				if (result == null)
					result = GetTypeReference (target.MainModule.TypeReferences, type);
				if (result == null && throwError)
					throw new ArgumentException ("Can't found type : " + type.DeclaringType.FullName);
				return result;
			} else
				return ResolveType (type.FullName, throwError);
		}

		public MemberReference GetMemberReference (MemberReferenceCollection members, MemberReference member)
		{
			foreach (MemberReference mr in members) {
				if (mr.ToString () == member.ToString ())
					return mr;
			}
			return null;
		}
	}
}
