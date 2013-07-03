#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
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
#endregion

using System;
using Mono.Cecil;

namespace Cecil.Decompiler.Languages {

    public interface IFormatter {
        // Raw write.
        void WriteRaw(string value);

        // Pygments bc/bo
        void WriteBlockStart(string value);
        void WriteBlockEnd(string value);

        // Pygments c1, cm, cp, cs
        void WriteComment(string comment);
        void WritePrepocComment(string comment);
        void WriteMultilineComment(string comment);
        void WriteDocComment(string comment);

        // Pygments k, kt
        void WriteKeyword(string keyword);
        void WriteAliasTypeKeyword(string keyword, TypeReference type);

        // Pygments m, s
        void WriteLiteralNumber(string value);
        void WriteLiteralString(string value);
        void WriteLiteralChar(string value);
        void WriteNamedLiteral(string value);

        // Pygments names
        void WriteNameReference(string name, MemberReference member);
        void WriteLabelReference(string name);
        void WriteVariableReference(string name);

        // Pygments Tokens
        void WriteOperator(string name);
        void WriteOperatorWord(string name);
        void WriteGenericToken(string name);

        // Pygments method decl.
        void WriteParenthesisOpen(string value);
        void WriteParenthesisClose(string value);
        void WriteParameterName(string value);

        // Operations.
        void Indent();
        void Outdent();
        void WriteLine();
        void WriteSpace();
    }
}
