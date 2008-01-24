//
// Gendarme.Framework.MethodSignature
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//
//  (C) 2008 Andreas Noever
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
using System.Text;

using Mono.Cecil;

namespace Gendarme.Framework {

	/// <summary>
	/// Used to match methods. Properties that are set to null are ignored
	/// </summary>
	/// <example>
	/// <code>
	/// MethodDefinition method
	/// MethodSignature sig = new MethodSignature () { Name = "Dispose" };
	/// if (sig.Match (method)) { 
	///     //matches any method named "Dispose" with any (or no) return value and any number of parameters
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="Gendarme.Framework.MethodSignatures"/>
	public class MethodSignature {

		/// <summary>
		/// The name of the method to match. Ignored if null.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The FullName (Namespace.Type) of the return type. Ignored if null.
		/// </summary>
		public string ReturnType { get; set; }

		/// <summary>
		/// An array of FullNames (Namespace.Type) of parameter types. Ignored if null. Null entries act as wildcards.
		/// </summary>
		public string [] Parameters { get; set; }

		/// <summary>
		/// An attribute mask matched against the attributes of the method.
		/// </summary>
		public MethodAttributes Attributes { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MethodSignature ()
		{
		}

		/// <summary>
		/// Creates a new MethodSignature from another.
		/// </summary>
		/// <param name="signature">The MethodSignature to copy.</param>
		public MethodSignature (MethodSignature signature)
		{
			this.Name = signature.Name;
			this.ReturnType = signature.ReturnType;
			if (signature.Parameters != null)
				this.Parameters = (string []) signature.Parameters.Clone ();
			this.Attributes = signature.Attributes;
		}

		/// <summary>
		/// Checks a MethodDefinition.
		/// </summary>
		/// <param name="method">The method to check.</param>
		/// <returns>True if the MethodDefinition matches all aspects of the MethodSignature.</returns>
		public bool Matches (MethodDefinition method)
		{
			if (Name != null && method.Name != Name)
				return false;
			if ((method.Attributes & Attributes) != Attributes)
				return false;
			if (ReturnType != null && method.ReturnType.ReturnType.FullName != ReturnType)
				return false;
			if (Parameters != null) {
				if (Parameters.Length != method.Parameters.Count)
					return false;
				for (int i = 0; i < Parameters.Length; i++) {
					if (Parameters [i] == null)
						continue;//ignore parameter
					if (Parameters [i] != method.Parameters [i].ParameterType.FullName) {
						return false;
					}
				}
			}
			return true;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			if (ReturnType != null) {
				sb.Append (ReturnType);
				sb.Append (' ');
			}
			if (Name != null) {
				sb.Append (Name);
				sb.Append ('(');
				if (Parameters != null) {
					for (int i = 0; i < Parameters.Length; i++) {
						sb.Append (Parameters [i]);
						if (i < Parameters.Length - 1)
							sb.Append (',');
					}
				}
				sb.Append (')');
			}
			return sb.ToString ();
		}
	}
}
