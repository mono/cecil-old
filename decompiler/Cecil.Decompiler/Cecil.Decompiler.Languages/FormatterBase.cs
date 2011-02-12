using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cecil.Decompiler.Languages
{
    /// <summary>
    /// Represents a formatter.
    /// </summary>
    public abstract class FormatterBase : IFormatter
    {
        private bool _writeIndent; // Whether to write an indent next time WriteIndent is called.
        private int _indent; // The current indent level.

        /// <summary>
        /// Writes the indent if required.
        /// </summary>
        private void WriteIndent()
        {
            if (_writeIndent)
                _writeIndent = false;
            OnWriteIndent(_indent);
        }

        protected abstract void OnWriteIndent(int indent);
        protected abstract void OnWriteRaw(string value);
        protected abstract void OnWriteLine();
        protected abstract void OnWriteSpace();

        #region Interface Members
        public void WriteRaw(string value)
        {
            WriteIndent();
            OnWriteRaw(value);
        }

        public void WriteBlockStart(string value)
        {
            WriteIndent();
            OnWriteBlockStart(value);
        }

        protected virtual void OnWriteBlockStart(string value)
        {
            WriteRaw(value);
        }

        public void WriteBlockEnd(string value)
        {
            WriteIndent();
            OnWriteBlockEnd(value);
        }

        protected virtual void OnWriteBlockEnd(string value)
        {
            WriteRaw(value);
        }

        public void WriteComment(string comment)
        {
            WriteIndent();
            OnWriteComment(comment);
        }

        protected virtual void OnWriteComment(string comment)
        {
            WriteRaw(comment);
        }

        public void WritePrepocComment(string comment)
        {
            WriteIndent();
            OnWritePrepocComment(comment);
        }

        protected virtual void OnWritePrepocComment(string comment)
        {
            WriteRaw(comment);
        }

        public void WriteMultilineComment(string comment)
        {
            WriteIndent();
            OnWriteMultilineComment(comment);
        }

        protected virtual void OnWriteMultilineComment(string comment)
        {
            WriteRaw(comment);
        }

        public void WriteDocComment(string comment)
        {
            WriteIndent();
            OnWriteDocComment(comment);
        }

        protected virtual void OnWriteDocComment(string comment)
        {
            WriteRaw(comment);
        }

        public void WriteKeyword(string keyword)
        {
            WriteIndent();
            OnWriteKeyword(keyword);
        }

        protected virtual void OnWriteKeyword(string keyword)
        {
            WriteRaw(keyword);
        }

        public void WriteAliasTypeKeyword(string keyword, Mono.Cecil.TypeReference type)
        {
            WriteIndent();
            OnWriteAliasTypeKeyword(keyword, type);
        }

        protected virtual void OnWriteAliasTypeKeyword(string keyword, Mono.Cecil.TypeReference type)
        {
            WriteRaw(keyword);
        }

        public void WriteLiteralNumber(string value)
        {
            WriteIndent();
            OnWriteLiteralNumber(value);
        }

        protected virtual void OnWriteLiteralNumber(string value)
        {
            WriteRaw(value);
        }

        public void WriteLiteralString(string value)
        {
            WriteIndent();
            OnWriteLiteralString(value);
        }

        protected virtual void OnWriteLiteralString(string value)
        {
            WriteRaw(value);
        }

        public void WriteLiteralChar(string value)
        {
            WriteIndent();
            OnWriteLiteralChar(value);
        }

        protected virtual void OnWriteLiteralChar(string value)
        {
            WriteRaw(value);
        }

        public void WriteNamedLiteral(string value)
        {
            WriteIndent();
            OnWriteNamedLiteral(value);
        }

        private void OnWriteNamedLiteral(string value)
        {
            WriteRaw(value);
        }

        public void WriteNameReference(string name, Mono.Cecil.MemberReference member)
        {
            WriteIndent();
            OnWriteNameReference(name, member);
        }

        protected virtual void OnWriteNameReference(string name, Mono.Cecil.MemberReference member)
        {
            WriteRaw(name);
        }

        public void WriteLabelReference(string name)
        {
            WriteIndent();
            OnWriteLabelReference(name);
        }

        protected virtual void OnWriteLabelReference(string name)
        {
            WriteRaw(name);
        }

        public void WriteVariableReference(string name)
        {
            WriteIndent();
            OnWriteVariableReference(name);
        }

        protected virtual void OnWriteVariableReference(string name)
        {
            WriteRaw(name);
        }

        public void WriteOperator(string name)
        {
            WriteIndent();
            OnWriteOperator(name);
        }

        protected virtual void OnWriteOperator(string name)
        {
            WriteRaw(name);
        }

        public void WriteOperatorWord(string name)
        {
            WriteIndent();
            OnWriteOperatorWord(name);
        }

        protected virtual void OnWriteOperatorWord(string name)
        {
            WriteRaw(name);
        }

        public void WriteGenericToken(string name)
        {
            WriteIndent();
            OnWriteGenericToken(name);
        }

        protected virtual void OnWriteGenericToken(string name)
        {
            WriteRaw(name);
        }

        public void WriteParenthesisOpen(string value)
        {
            WriteIndent();
            OnWriteParenthesisOpen(value);
        }

        protected virtual void OnWriteParenthesisOpen(string value)
        {
            WriteRaw(value);
        }

        public void WriteParenthesisClose(string value)
        {
            WriteIndent();
            OnWriteParenthesisClose(value);
        }

        protected virtual void OnWriteParenthesisClose(string value)
        {
            WriteRaw(value);
        }

        public void WriteParameterName(string value)
        {
            WriteIndent();
            OnWriteParameterName(value);
        }

        protected virtual void OnWriteParameterName(string value)
        {
            WriteRaw(value);
        }

        public void WriteLine()
        {
            OnWriteLine();
            _writeIndent = true;
        }

        public void WriteSpace()
        {
            WriteIndent();
            OnWriteSpace();
        }
        
        public void Indent()
        {
            _indent++;
            OnIndent();
        }

        protected virtual void OnIndent()
        {

        }

        public void Outdent()
        {
            _indent--;
            OnOutdent();
        }

        protected virtual void OnOutdent()
        {

        }
        #endregion
    }
}
