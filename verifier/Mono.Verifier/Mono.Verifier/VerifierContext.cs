//
// VerifierContext.cs
//
// Author:
//   Jb Evain (jb@nurv.fr)
//
// (C) 2007 Jb Evain
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

using Mono.Cecil;

namespace Mono.Verifier {

	public class VerifierContext {

		AssemblyDefinition _assembly;
		VerifierParameters _parameters;
		VerifierResult _result;

		public VerifierContext (VerifierParameters parameters, AssemblyDefinition assembly)
		{
			_parameters = parameters;
			_assembly = assembly;
			_result = new VerifierResult (parameters);
		}

		public AssemblyDefinition Assembly {
			get { return _assembly; }
			set { _assembly = value; }
		}

		public VerifierParameters Parameters {
			get { return _parameters; }
			set { _parameters = value; }
		}

		public VerifierResult Result {
			get { return _result; }
			set { _result = value; }
		}
	}
}
