// 
// Unit tests for TypeRocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//      Daniel Abramov <ex@vingrad.ru>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
// (C) 2007 Daniel Abramov
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
using System.Reflection;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

using Mono.Cecil;
using NUnit.Framework;

namespace Test.Framework.Rocks {

	[TestFixture]
	public class TypeRocksTest {

		public enum Enum {
			Value
		}

		[Flags]
		public enum Flags {
			Mask
		}

		interface IDeepCloneable : ICloneable {
		}

		class Deep : IDeepCloneable {
			public object Clone ()
			{
				throw new NotImplementedException ();
			}
		}

		class NotAttribute {
		}

		class AnAttribute : Attribute {
		}

		class ClassInheritsNotAttribute : NotAttribute {
		}

		class AttributeInheritsAnAttribute : AnAttribute {
		}

		class AttributeInheritsOuterAttribute : ContextStaticAttribute {
		}

		class AttributeInheritsOuterAttributeDerivingAttribute : AttributeInheritsOuterAttribute {
		}

		private byte [] array_of_bytes;
		private Enum [] array_of_enum;
		private Flags [] array_of_flags;
		private string [] array_of_strings;
		private Deep [] array_of_classes;
		private ICloneable [] array_of_interfaces;


		private AssemblyDefinition assembly;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
		}

		private TypeDefinition GetType (string name)
		{
			return assembly.MainModule.Types ["Test.Framework.Rocks.TypeRocksTest" + name];
		}

		private TypeReference GetFieldType (string name)
		{
			TypeDefinition type = assembly.MainModule.Types ["Test.Framework.Rocks.TypeRocksTest"];
			foreach (FieldDefinition field in type.Fields) {
				if (name == field.Name)
					return field.FieldType;
			}
			Assert.Fail (name);
			return null;
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void HasAttribute_Null ()
		{
			GetType (String.Empty).HasAttribute (null);
		}

		[Test]
		public void HasAttribute ()
		{
			Assert.IsTrue (GetType (String.Empty).HasAttribute ("NUnit.Framework.TestFixtureAttribute"), "TypeRocksTest");
			Assert.IsFalse (GetType ("/Enum").HasAttribute ("System.FlagsAttribute"), "Enum/System.FlagsAttribute");
			Assert.IsTrue (GetType ("/Flags").HasAttribute ("System.FlagsAttribute"), "Flags/System.FlagsAttribute");
			// fullname is required
			Assert.IsFalse (GetType ("/Flags").HasAttribute ("System.Flags"), "Flags/System.Flags");
			Assert.IsFalse (GetType ("/Flags").HasAttribute ("FlagsAttribute"), "Flags/FlagsAttribute");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Implements_Null ()
		{
			GetType (String.Empty).Implements (null);
		}

		[Test]
		public void Implements ()
		{
			Assert.IsFalse (GetType (String.Empty).Implements ("System.ICloneable"), "ICloneable");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements ("Test.Framework.Rocks.TypeRocksTest/IDeepCloneable"), "itself");
			Assert.IsTrue (GetType ("/IDeepCloneable").Implements ("System.ICloneable"), "interface inheritance");
			Assert.IsTrue (GetType ("/Deep").Implements ("Test.Framework.Rocks.TypeRocksTest/IDeepCloneable"), "IDeepCloneable");
			Assert.IsTrue (GetType ("/Deep").Implements ("System.ICloneable"), "second-level ICloneable");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Inherits_Null ()
		{
			GetType (String.Empty).Inherits (null);
		}

		[Test]
		public void Inherits ()
		{
			Assert.IsFalse (GetType ("/NotAttribute").Inherits ("System.Attribute"), "NotAttribute");
			Assert.IsTrue (GetType ("/AnAttribute").Inherits ("System.Attribute"), "AnAttribute");
			Assert.IsFalse (GetType ("/ClassInheritsNotAttribute").Inherits ("System.Attribute"), "ClassInheritsNotAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsAnAttribute").Inherits ("System.Attribute"), "AttributeInheritsAnAttribute");
		}

		[Test]
		[Ignore ("cannot be sure without the AssemblyResolver")]
		public void Inherits_FromAnotherAssembly ()
		{
			// we can't be sure here so to avoid false positives return false
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttribute").Inherits ("System.Attribute"), "AttributeInheritsOuterAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttributeDerivingAttribute").Inherits ("System.Attribute"), "AttributeInheritsOuterAttributeDerivingAttribute");
		}

		[Test]
		public void Inherits_Itself ()
		{
			TypeDefinition type = GetType (String.Empty);
			Assert.IsTrue (type.Inherits (type.FullName), "itself");
		}

		[Test]
		public void IsArray ()
		{
			Assert.IsTrue (GetFieldType ("array_of_bytes").IsArray (), "array_of_bytes");
			Assert.IsTrue (GetFieldType ("array_of_strings").IsArray (), "array_of_strings");
			Assert.IsTrue (GetFieldType ("array_of_interfaces").IsArray (), "array_of_interfaces");

			Assert.IsFalse (GetType ("/AnAttribute").IsArray (), "AnAttribute");
			Assert.IsTrue (GetFieldType ("array_of_classes").IsArray (), "array_of_classes");

			Assert.IsFalse (GetType ("/Enum").IsArray (), "Enum");
			Assert.IsTrue (GetFieldType ("array_of_enum").IsArray (), "array_of_enum");

			Assert.IsFalse (GetType ("/Flags").IsArray (), "Flags");
			Assert.IsTrue (GetFieldType ("array_of_flags").IsArray (), "array_of_flags");
		}

		[Test]
		public void IsAttribute ()
		{
			Assert.IsFalse (GetType ("/NotAttribute").IsAttribute (), "NotAttribute");
			Assert.IsTrue (GetType ("/AnAttribute").IsAttribute (), "AnAttribute");
			Assert.IsFalse (GetType ("/ClassInheritsNotAttribute").IsAttribute (), "ClassInheritsNotAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsAnAttribute").IsAttribute (), "AttributeInheritsAnAttribute");
		}

		[Test]
		[Ignore ("cannot be sure without the AssemblyResolver")]
		public void IsAttribute_InheritsFromAnotherAssembly ()
		{
			// we can't be sure here so to avoid false positives return false
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttribute").IsAttribute (), "AttributeInheritsOuterAttribute");
			Assert.IsTrue (GetType ("/AttributeInheritsOuterAttributeDerivingAttribute").IsAttribute (), "AttributeInheritsOuterAttributeDerivingAttribute");
		}

		[Test]
		public void IsFlags ()
		{
			Assert.IsFalse (GetType (String.Empty).IsFlags (), "Type.IsFlags");
			Assert.IsFalse (GetType ("/Enum").IsFlags (), "Enum.IsFlags");
			Assert.IsTrue (GetType ("/Flags").IsFlags (), "Flags.IsFlags");
		}
	}
}
