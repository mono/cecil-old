//
// CilWriter.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// (C) 2005 Jb Evain
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

namespace Mono.Disassembler {

	using System;
	using System.IO;
	using System.Text;

	using Mono.Cecil;

	class CilWriter : TextWriter {

		TextWriter m_writer;
		int m_indentLevel = 0;

		public TextWriter BaseWriter {
			get { return m_writer; }
		}

		public override Encoding Encoding {
			get { return m_writer.Encoding; }
		}

		public CilWriter (TextWriter writer)
		{
			m_writer = writer;
		}

		public void Indent ()
		{
			m_indentLevel++;
		}

		public void Unindent ()
		{
			m_indentLevel--;
		}

		void WriteIndent ()
		{
			for (int i = 0; i < m_indentLevel; i++)
				m_writer.Write ("  ");
		}

		public override void Write (ulong value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (int value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (string value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (string format, object arg0, object arg1, object arg2)
		{
			WriteIndent ();
			m_writer.Write (format, arg0, arg1, arg2);
		}

		public override void Write (double value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (string format, object arg0, object arg1)
		{
			WriteIndent ();
			m_writer.Write (format, arg0, arg1);
		}

		public override void Write (object value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (char [] buffer, int index, int count)
		{
			WriteIndent ();
			m_writer.Write (buffer, index, count);
		}

		public override void Write (char value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (bool value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (long value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (char [] value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (float value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (decimal value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (string format, object [] arg)
		{
			WriteIndent ();
			m_writer.Write (format, arg);
		}

		public override void Write (uint value)
		{
			WriteIndent ();
			m_writer.Write (value);
		}

		public override void Write (string format, object arg0)
		{
			WriteIndent ();
			m_writer.Write (format, arg0);
		}

		public override void WriteLine (object value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (char [] buffer, int index, int count)
		{
			WriteIndent ();
			m_writer.WriteLine (buffer, index, count);
		}

		public override void WriteLine (ulong value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (char value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (double value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (int value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (float value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (uint value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (string format, object arg0, object arg1, object arg2)
		{
			WriteIndent ();
			m_writer.WriteLine (format, arg0, arg1, arg2);
		}

		public override void WriteLine (long value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (string format, object arg0)
		{
			WriteIndent ();
			m_writer.WriteLine (format, arg0);
		}

		public override void WriteLine (string format, object [] arg)
		{
			WriteIndent ();
			m_writer.WriteLine (format, arg);
		}

		public override void WriteLine (bool value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (string format, object arg0, object arg1)
		{
			WriteIndent ();
			m_writer.WriteLine (format, arg0, arg1);
		}

		public override void WriteLine (decimal value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (char [] value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine (string value)
		{
			WriteIndent ();
			m_writer.WriteLine (value);
		}

		public override void WriteLine ()
		{
			WriteIndent ();
			m_writer.WriteLine ();
		}

		public void Write (byte [] ary)
		{
			m_writer.WriteLine ("(");
			Indent ();

			WriteIndent ();
			for (int i = 0; i < ary.Length; i++) {
				if (i > 0 && i % 16 == 0) {
					m_writer.WriteLine ();
					WriteIndent ();
				} else
					m_writer.Write (" ");

				m_writer.Write (ary [i].ToString ("x2"));
			}

			m_writer.WriteLine ();
			Unindent ();
			WriteLine (")");
		}

		public void OpenBlock ()
		{
			WriteLine ("{");
			Indent ();
		}

		public void CloseBlock ()
		{
			Unindent ();
			WriteLine ("}");
		}

		public void WriteFlags (int value, int [] vals, string [] map)
		{
			for (int i = 0; i < vals.Length; i++)
				if ((value & vals [i]) > 0) {
					m_writer.Write (map [i]);
					m_writer.Write (" ");
				}
		}

		public void WriteFlags (int value, int mask, int [] vals, string [] map)
		{
			int v = value & mask;
			for (int i = 0; i < vals.Length; i++)
				if (v == vals [i]) {
					m_writer.Write (map [i]);
					m_writer.Write (" ");
					return;
				}
		}

		public override void Close ()
		{
			m_writer.Close ();
		}

		public override string ToString ()
		{
			return m_writer.ToString ();
		}
	}
}
