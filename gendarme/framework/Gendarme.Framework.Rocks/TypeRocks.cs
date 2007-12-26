//
// Gendarme.Framework.Rocks.TypeRocks
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

namespace Gendarme.Framework.Rocks {

	// add Type[Definition|Reference] extensions methods here
	// only if:
	// * you supply minimal documentation for them (xml)
	// * you supply unit tests for them
	// * they are required somewhere to simplify, even indirectly, the rules
	//   (i.e. don't bloat the framework in case of x, y or z in the future)

	/// <summary>
	/// TypeRocks contains extensions methods for Type[Definition|Reference]
	/// and the related collection classes.
	/// 
	/// Note: whenever possible try to use TypeReference since it's extend the
	/// reach/usability of the code.
	/// </summary>
	public static class TypeRocks {

		/// <summary>
		/// Check if the type contains an attribute of a specified type.
		/// </summary>
		/// <param name="self">The TypeDefinition on which the extension method can be called.</param>
		/// <param name="attributeName">Full name of the attribute class</param>
		/// <returns>True if the type contains an attribute of the same name,
		/// False otherwise.</returns>
		public static bool HasAttribute (this TypeDefinition self, string attributeName)
		{
			return self.CustomAttributes.Contains (attributeName);
		}

		/// <summary>
		/// Check if the type implemented a specified interface. Note that it is possible that
		/// we might now be able to know all implementations since the assembly where 
		/// the information resides could be unavailable.
		/// </summary>
		/// <param name="self">The TypeDefinition on which the extension method can be called.</param>
		/// <param name="interfaceName">Full name of the interface</param>
		/// <returns>True if the type implements the interface, False otherwise.</returns>
		public static bool Implements (this TypeDefinition self, string interfaceName)
		{
			if (interfaceName == null)
				throw new ArgumentNullException ("interfaceName");

			// special case, check if we implement ourselves
			if (self.IsInterface && (self.FullName == interfaceName))
				return true;

			// does the type implements it itself
			foreach (TypeReference iface in self.Interfaces) {
				if (iface.FullName == interfaceName)
					return true;
			}
			
			// if not, then maybe it's parent does
			// FIXME: right now we "ignore" case were a TypeReference is given
			TypeDefinition parent = (self.BaseType as TypeDefinition);
			if (parent != null)
				return parent.Implements (interfaceName);

			return false;
		}

		/// <summary>
		/// Check if the type is a enumeration flags.
		/// </summary>
		/// <param name="self">The TypeDefinition on which the extension method can be called.</param>
		/// <returns>True if the type as the [Flags] attribute, false otherwise.</returns>
		public static bool IsFlags (this TypeDefinition self)
		{
			if (!self.IsEnum)
				return false;

			return self.HasAttribute ("System.FlagsAttribute");
		}
	}
}
