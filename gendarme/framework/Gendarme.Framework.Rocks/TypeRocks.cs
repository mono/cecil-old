//
// Gendarme.Framework.Rocks.TypeRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//      Daniel Abramov <ex@vingrad.ru>
//	Adrian Tsai <adrian_tsai@hotmail.com>
//	Andreas Noever <andreas.noever@gmail.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
// (C) 2007 Daniel Abramov
// Copyright (c) 2007 Adrian Tsai
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
		/// Return the type finalizer (aka destructor).
		/// </summary>
		/// <param name="self">The TypeDefinition on which the extension method can be called.</param>
		/// <returns>The MethodDefinition of the finalizer or null if the type has no finalizer.</returns>
		public static MethodDefinition GetFinalizer (this TypeDefinition self)
		{
			foreach (MethodDefinition method in self.Methods) {
				if (method.Name != "Finalize")
					continue;
				if (method.Parameters.Count != 0)
					continue;
				if (method.ReturnType.ReturnType.ToString () == "System.Void")
					return method;
			}
			return null;
		}

		/// <summary>
		/// Check if the type contains an attribute of a specified type.
		/// </summary>
		/// <param name="self">The TypeReference on which the extension method can be called.</param>
		/// <param name="attributeName">Full name of the attribute class</param>
		/// <returns>True if the type contains an attribute of the same name,
		/// False otherwise.</returns>
		public static bool HasAttribute (this TypeReference self, string attributeName)
		{
			return self.CustomAttributes.ContainsType (attributeName);
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
		/// Check if the type inherits from the specified type. Note that it is possible that
		/// we might now be able to know the complete inheritance chain since the assembly 
		/// where the information resides could be unavailable.
		/// </summary>
		/// <param name="self">The TypeReference on which the extension method can be called.</param>
		/// <param name="className">Full name of the base class</param>
		/// <returns>True if the type inherits from specified class, False otherwise</returns>
		public static bool Inherits (this TypeReference self, string className)
		{
			if (className == null)
				throw new ArgumentNullException ("className");

			TypeReference current = self;
			while ((current != null) && (current.FullName != "System.Object")) {
				if (current.FullName == className)
					return true;

				// FIXME: plugin AssemblyResolver when ready
				TypeDefinition type = (current as TypeDefinition);
				current = (type == null) ? null : type.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Check if the type represent an array (of any other type).
		/// </summary>
		/// <param name="self">The TypeReference on which the extension method can be called.</param>
		/// <returns>True if the type is an array, False otherwise</returns>
		public static bool IsArray (this TypeReference self)
		{
			return (self is ArrayType);
		}

		/// <summary>
		/// Checks if type is attribute. Note that it is possible that
		/// we might now be able to know all inheritance since the assembly where 
		/// the information resides could be unavailable.
		/// </summary>
		/// <param name="self">The TypeDefinition on which the extension method can be called.</param>
		/// <returns>True if the type inherits from <c>System.Attribute</c>, 
		/// False otherwise.</returns>
		public static bool IsAttribute (this TypeDefinition self)
		{
			return self.Inherits ("System.Attribute");
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

		/// <summary>
		/// Check if the type represent a floating-point type.
		/// </summary>
		/// <param name="self">The TypeReference on which the extension method can be called.</param>
		/// <returns>True if the type is System.Single (C# float) or System.Double (C3 double), False otherwise.</returns>
		public static bool IsFloatingPoint (this TypeReference self)
		{
			return ((self.FullName == Mono.Cecil.Constants.Single) ||
				(self.FullName == Mono.Cecil.Constants.Double));
		}

		/// <summary>
		/// Check if the type is generated code, either by the compiler or by a tool.
		/// </summary>
		/// <param name="self">The TypeReference on which the extension method can be called.</param>
		/// <returns>True if the code is not generated directly by the developer, 
		/// False otherwise (e.g. compiler or tool generated)</returns>
		public static bool IsGeneratedCode (this TypeReference self)
		{
			if (self.Module.Assembly.Runtime >= TargetRuntime.NET_2_0) {
				if (self.CustomAttributes.ContainsAnyType (CustomAttributeRocks.GeneratedCodeAttributes))
					return true;

				TypeReference type = self;
				while (type.IsNested) {
					if (type.CustomAttributes.ContainsAnyType (CustomAttributeRocks.GeneratedCodeAttributes))
						return true;
					type = type.DeclaringType;
				}
				return false;
			} else {
				switch (self.Name [0]) {
				case '<': // e.g. <PrivateImplementationDetails>
				case '$': // e.g. $ArrayType$1 nested inside <PrivateImplementationDetails>
					return true;
				default:
					return false;
				}
			}
		}

		/// <summary>
		/// Check if the type refers to native code.
		/// </summary>
		/// <param name="self">The TypeReference on which the extension method can be called.</param>
		/// <returns>True if the type refers to native code, False otherwise</returns>
		public static bool IsNative (this TypeReference self)
		{
			switch (self.FullName) {
			case "System.IntPtr":
			case "System.UIntPtr":
			case "System.Runtime.InteropServices.HandleRef":
				return true;
			default:
				return false;
			}
		}
	}
}
