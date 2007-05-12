//
// VerifierResult.cs
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

using System.Collections.Generic;

namespace Mono.Verifier {

	public class VerifierResult {

		IList<ResultItem> _errors = new List<ResultItem> ();
		IList<ResultItem> _warnings = new List<ResultItem> ();
		VerifierParameters _parameters;

		internal VerifierResult (VerifierParameters parameters)
		{
			_parameters = parameters;
		}

		public IEnumerable<ResultItem> GetItems ()
		{
			foreach (ResultItem warning in _warnings)
				yield return warning;

			foreach (ResultItem error in _errors)
				yield return error;
		}

		public void AddError (ResultItem error)
		{
			if (!ShouldAdd (error, _errors))
				return;

			_errors.Add (error);
		}

		public void AddWarning (ResultItem warning)
		{
			if (!ShouldAdd (warning, _warnings))
				return;

			_warnings.Add (warning);
		}

		bool ShouldAdd (ResultItem resultItem, IList<ResultItem> collection)
		{
			if (!_parameters.UniqueErrors)
				return true;

			return !collection.Contains (resultItem);
		}
	}
}
