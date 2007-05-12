//
// ResultItem.cs
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

using System;

namespace Mono.Verifier {

	public class ResultItem {

		ItemKind _kind;
		ItemSource _source;
		int _code;
		string _message;

		public ItemKind Kind {
			get { return _kind; }
			set { _kind = value; }
		}

		public ItemSource Source {
			get { return _source; }
			set { _source = value; }
		}

		public int Code {
			get { return _code; }
			set { _code = value; }
		}

		public string Message {
			get { return _message; }
			set { _message = value; }
		}

		public ResultItem (ItemKind kind, int code, string message)
		{
			_kind = kind;
			_code = code;
			_message = message;
		}

		public override bool Equals(object obj)
		{
			ResultItem i = obj as ResultItem;
			if (i == null)
				return false;

			return i._kind == _kind && i._code == _code && i._source == _source;
		}

		public override int GetHashCode()
		{
			return _code ^ (int) _kind ^ (int) _source;
		}

		public override string ToString()
		{
			return string.Format ("[{0}] (0x{1:X08}) {2}: {3}", Format (_source), _code, _kind, _message);
		}

		static string Format (ItemSource s)
		{
			switch (s) {
			case ItemSource.Metadata:
				return "MD";
			case ItemSource.CIL:
				return "IL";
			default:
				throw new ArgumentException ("Unknown source");
			}
		}
	}
}
