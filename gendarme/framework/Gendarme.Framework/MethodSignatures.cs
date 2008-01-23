//
// Gendarme.Framework.MethodSignatures
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
//  (C) 2008 Andreas Noever
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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
//

using System;

using Mono.Cecil;

namespace Gendarme.Framework {

	/// <summary>
	/// Defines commonly used MethodSignatures
	/// </summary>
	/// <see cref="Gendarme.Framework.MethodSignature"/>
	public static class MethodSignatures {
		private static readonly string [] NoParameter = new string [0];
		private static readonly string [] OneParameter = new string [1];
		private static readonly string [] TwoParameters = new string [2];

		//System.Object
		public static readonly new MethodSignature Equals = new MethodSignature () { Name = "Equals", ReturnType = "System.Boolean", Parameters = new string [] { "System.Object" }, Attributes = MethodAttributes.Public | MethodAttributes.Virtual };
		public static readonly MethodSignature Finalize = new MethodSignature () { Name = "Finalize", ReturnType = "System.Void", Parameters = NoParameter, Attributes = MethodAttributes.Family };

		// IDisposable
		public static readonly MethodSignature Dispose = new MethodSignature () { Name = "Dispose", ReturnType = "System.Void", Parameters = NoParameter };
		public static readonly MethodSignature DisposeExplicit = new MethodSignature () { Name = "System.IDisposable.Dispose", ReturnType = "System.Void", Parameters = NoParameter };

		//operators
		public static readonly MethodSignature op = new MethodSignature () { Attributes = MethodAttributes.Static | MethodAttributes.SpecialName };

		//unary
		public static readonly MethodSignature op_UnaryPlus = new MethodSignature (op) { Name = "op_UnaryPlus", Parameters = OneParameter };			// +5
		public static readonly MethodSignature op_UnaryNegation = new MethodSignature (op) { Name = "op_UnaryNegation", Parameters = OneParameter };	// -5
		public static readonly MethodSignature op_LogicalNot = new MethodSignature (op) { Name = "op_LogicalNot", Parameters = OneParameter };			// !true
		public static readonly MethodSignature op_OnesComplement = new MethodSignature (op) { Name = "op_OnesComplement", Parameters = OneParameter }; // ~5

		public static readonly MethodSignature op_Increment = new MethodSignature (op) { Name = "op_Increment", Parameters = OneParameter };			// 5++
		public static readonly MethodSignature op_Decrement = new MethodSignature (op) { Name = "op_Decrement", Parameters = OneParameter };			// 5--
		public static readonly MethodSignature op_True = new MethodSignature (op) { Name = "op_True", Parameters = OneParameter, ReturnType = "System.Boolean" }; // if (object)		
		public static readonly MethodSignature op_False = new MethodSignature (op) { Name = "op_False", Parameters = OneParameter, ReturnType = "System.Boolean" };	// if (object)

		//binary
		public static readonly MethodSignature op_Addition = new MethodSignature (op) { Name = "op_Addition", Parameters = TwoParameters };				// 5 + 5
		public static readonly MethodSignature op_Subtraction = new MethodSignature (op) { Name = "op_Subtraction", Parameters = TwoParameters };		// 5 - 5 
		public static readonly MethodSignature op_Multiply = new MethodSignature (op) { Name = "op_Multiply", Parameters = TwoParameters };				// 5 * 5
		public static readonly MethodSignature op_Division = new MethodSignature (op) { Name = "op_Division", Parameters = TwoParameters };				// 5 / 5
		public static readonly MethodSignature op_Modulus = new MethodSignature (op) { Name = "op_Modulus", Parameters = TwoParameters };				// 5 % 5

		public static readonly MethodSignature op_BitwiseAnd = new MethodSignature (op) { Name = "op_BitwiseAnd", Parameters = TwoParameters };			// 5 & 5
		public static readonly MethodSignature op_BitwiseOr = new MethodSignature (op) { Name = "op_BitwiseOr", Parameters = TwoParameters };			// 5 | 5
		public static readonly MethodSignature op_ExclusiveOr = new MethodSignature (op) { Name = "op_ExclusiveOr", Parameters = TwoParameters };		// 5 ^ 5

		public static readonly MethodSignature op_LeftShift = new MethodSignature (op) { Name = "op_LeftShift", Parameters = TwoParameters };			// 5 << 5
		public static readonly MethodSignature op_RightShift = new MethodSignature (op) { Name = "op_RightShift", Parameters = TwoParameters };			// 5 >> 5

		//comparison
		public static readonly MethodSignature op_Equality = new MethodSignature (op) { Name = "op_Equality", Parameters = TwoParameters };				// 5 == 5
		public static readonly MethodSignature op_Inequality = new MethodSignature (op) { Name = "op_Inequality", Parameters = TwoParameters };			// 5 != 5
		public static readonly MethodSignature op_GreaterThan = new MethodSignature (op) { Name = "op_GreaterThan", Parameters = TwoParameters };		// 5 > 5
		public static readonly MethodSignature op_LessThan = new MethodSignature (op) { Name = "op_LessThan", Parameters = TwoParameters };				// 5 < 5
		public static readonly MethodSignature op_GreaterThanOrEqual = new MethodSignature (op) { Name = "op_GreaterThanOrEqual", Parameters = TwoParameters }; // 5 >= 5
		public static readonly MethodSignature op_LessThanOrEqual = new MethodSignature (op) { Name = "op_LessThanOrEqual", Parameters = TwoParameters };// 5 <= 5
	}
}
