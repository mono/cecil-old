//
// SymbolStoreFactory.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2006 Jb Evain
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

namespace Mono.CompilerServices.SymbolStore {

	using System;
	using System.IO;

	public class SymbolStoreFactory {

		SymbolStoreFactory ()
		{
		}

		public static SymbolStore GetSymbolStore (string file)
		{
			using (FileStream fs = new FileStream (file, FileMode.Open, FileAccess.Read, FileShare.Read))
				return GetSymbolStore (fs);
		}

		public static SymbolStore GetSymbolStore (byte [] store)
		{
			return GetSymbolStore (new MemoryStream (store));
		}

		public static SymbolStore GetSymbolStore (Stream stream)
		{
			SymbolStoreReader reader = new SymbolStoreReader (new BinaryReader (stream));
			SymbolStore ss = new SymbolStore ();
			reader.ReadSymbolStore (ss);
			return ss;
		}

		public static void SaveSymbolStore (SymbolStore store, string file)
		{
			throw new NotImplementedException ();
		}

		public static void SaveSymbolStore (SymbolStore store, out byte [] symbolstore)
		{
			throw new NotImplementedException ();
		}

		public static void SaveSymbolStore (SymbolStore store, Stream stream)
		{
			throw new NotImplementedException ();
		}
	}
}
