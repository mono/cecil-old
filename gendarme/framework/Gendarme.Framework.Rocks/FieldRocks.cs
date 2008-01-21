//
// Gendarme.Framework.Rocks.FieldRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//	Andreas Noever <andreas.noever@gmail.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
// (C) 2008 Andreas Noever
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

namespace Gendarme.Framework.Rocks {

	public static class FieldRocks {

		/// <summary>
		/// Check if the field contains an attribute of a specified type.
		/// </summary>
		/// <param name="self">The FieldDefinition on which the extension method can be called.</param>
		/// <param name="attributeName">Full name of the attribute class</param>
		/// <returns>True if the field contains an attribute of the same name,
		/// False otherwise.</returns>
		public static bool HasAttribute (this FieldDefinition self, string attributeName)
		{
			return self.CustomAttributes.ContainsType (attributeName);
		}

		/// <summary>
		/// Check if the field was generated by the compiler or a tool (i.e. not by the developer).
		/// This can occurs (compiler) with auto-implemented properties (C# 3)
		/// </summary>
		/// <param name="self">The FieldDefinition on which the extension method can be called.</param>
		/// <returns>True if the field was not added directly by the developer, False otherwise</returns>
		public static bool IsGeneratedCode (this FieldDefinition self)
		{
			return self.CustomAttributes.ContainsAnyType (CustomAttributeRocks.GeneratedCodeAttributes);
		}

		/// <summary>
		/// Check if the field is visible outside of the assembly.
		/// </summary>
		/// <param name="self">The FieldDefinition on which the extension method can be called.</param>
		/// <returns>True if the field can be used from outside of the assembly, false otherwise.</returns>
		public static bool IsVisible (this FieldDefinition self)
		{
			if (self.IsPrivate || self.IsAssembly)
				return false;
			return ((TypeDefinition) self.DeclaringType).IsVisible ();
		}
	}
}
