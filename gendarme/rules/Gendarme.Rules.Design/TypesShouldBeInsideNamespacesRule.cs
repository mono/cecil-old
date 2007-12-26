// 
// Gendarme.Rules.Design.TypesShouldBeInsideNamespacesRule
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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

using Mono.Cecil;

using Gendarme.Framework;

namespace Gendarme.Rules.Design {

	public class TypesShouldBeInsideNamespacesRule: ITypeRule {

		public MessageCollection CheckType (TypeDefinition type, Runner runner)
		{
			// rule applies to public types
			if (!type.IsPublic && !(type.IsNestedPublic || type.IsNestedFamily))
				return runner.RuleSuccess;

			// rule applies!

			// if the type is nested then check our declaring type namespace
			string ns = type.IsNested ? (type.DeclaringType as TypeDefinition).Namespace : type.Namespace;

			// check if the type resides inside a namespace
			if (ns.Length > 0)
				return runner.RuleSuccess;

			Message msg = new Message ("Type is not declared inside a namespace.", new Location (type), MessageType.Warning);
			return new MessageCollection (msg);
		}
	}
}
